using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Users;

namespace MealPlannerAPI.Hubs
{
    [Authorize]
    public class MealPlannerAPIHub : AbpHub<IMealPlannerAPIHubClient>
    {
        public static string UserGroup(Guid userId) => $"user:{userId}";

        /// <summary>Group for a specific shopping list — receives item-level pushes.</summary>
        public static string ShoppingListGroup(Guid shoppingListId) => $"shopping-list:{shoppingListId}";

        /// <summary>Broadcast group — all connected clients receive trending updates.</summary>
        public const string TrendingGroup = "trending";

        // ── Connection lifecycle ──────────────────────────────────────────────────

        public override async Task OnConnectedAsync()
        {
            // Every authenticated user joins their own group and the trending group
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(CurrentUser.GetId()));
            await Groups.AddToGroupAsync(Context.ConnectionId, TrendingGroup);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserGroup(CurrentUser.GetId()));
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, TrendingGroup);
            await base.OnDisconnectedAsync(exception);
        }

        // ── Client-invokable methods ──────────────────────────────────────────────

        /// <summary>
        /// Client calls this to subscribe to a specific shopping list's real-time updates.
        /// Call again with subscribe=false to leave.
        /// </summary>
        public async Task SubscribeShoppingList(Guid shoppingListId, bool subscribe = true)
        {
            var group = ShoppingListGroup(shoppingListId);
            if (subscribe)
                await Groups.AddToGroupAsync(Context.ConnectionId, group);
            else
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        }
    }
}
