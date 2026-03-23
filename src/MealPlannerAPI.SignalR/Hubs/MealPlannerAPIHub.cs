//.SignalR project
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Microsoft.AspNetCore.RateLimiting;

namespace MealPlannerAPI.Hubs
{
    [Authorize]
    [HubRoute("/signalr-hubs/meal-planner")]
    [EnableRateLimiting("SignalR")]
    public class MealPlannerAPIHub : AbpHub<IMealPlannerAPIHubClient>
    {
        public static string UserGroup(Guid userId) => $"user:{userId}";

        public static string ShoppingListGroup(Guid shoppingListId) => $"shopping-list:{shoppingListId}";

        public const string TrendingGroup = "trending";
        private readonly ICurrentPrincipalAccessor _principalAccessor;

        public MealPlannerAPIHub(ICurrentPrincipalAccessor principalAccessor)
        {
            _principalAccessor = principalAccessor;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                Console.WriteLine($"=== OnConnectedAsync ===");

                using (_principalAccessor.Change(Context.User))
                {
                    Console.WriteLine($"CurrentUser.Id: {CurrentUser.Id}");

                    await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(CurrentUser.GetId()));

                    await Groups.AddToGroupAsync(Context.ConnectionId, TrendingGroup);

                    await base.OnConnectedAsync();
                    Console.WriteLine("base.OnConnectedAsync done");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnConnectedAsync EXCEPTION: {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw; // still throw so ABP knows the connection failed
            }
        
        }

        public async Task SubscribeShoppingList(Guid shoppingListId, bool subscribe = true)
        {
            using (_principalAccessor.Change(Context.User))
            {
                var group = ShoppingListGroup(shoppingListId);
                if (subscribe)
                    await Groups.AddToGroupAsync(Context.ConnectionId, group);
                else
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
            }
        }

    }
}
