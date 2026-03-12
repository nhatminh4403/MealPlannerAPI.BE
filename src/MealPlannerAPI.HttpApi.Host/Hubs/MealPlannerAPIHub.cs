using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Users;

namespace MealPlannerAPI.Hubs
{
    [Authorize]
    [HubRoute("/signalr-hubs/meal-planner")]
    public class MealPlannerAPIHub : AbpHub<IMealPlannerAPIHubClient>
    {
        public static string UserGroup(Guid userId) => $"user:{userId}";

        public static string ShoppingListGroup(Guid shoppingListId) => $"shopping-list:{shoppingListId}";

        public const string TrendingGroup = "trending";


        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(CurrentUser.GetId()));
            await Groups.AddToGroupAsync(Context.ConnectionId, TrendingGroup);
            await base.OnConnectedAsync();
        }

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
