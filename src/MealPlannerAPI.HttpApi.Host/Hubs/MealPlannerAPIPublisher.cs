using MealPlannerAPI.Dashboard;
using MealPlannerAPI.MealPlans.Dtos;
using MealPlannerAPI.ShoppingLists.Dtos;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.AspNetCore.SignalR;
namespace MealPlannerAPI.Hubs
{
    public class MealPlannerAPIPublisher : IMealPlannerHubPublisher, ITransientDependency
    {
        private readonly IHubContext<MealPlannerAPIHub, IMealPlannerAPIHubClient> _hubContext;

        public MealPlannerAPIPublisher(IHubContext<MealPlannerAPIHub, IMealPlannerAPIHubClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyTrendingUpdatedAsync()
        {
            return _hubContext.Clients
                .Group(MealPlannerAPIHub.TrendingGroup)
                .TrendingUpdated();
        }

        // ── Dashboard stats ───────────────────────────────────────────────────────

        /// <summary>Push updated stats to a specific user.</summary>
        public Task NotifyStatsUpdatedAsync(Guid userId, DashboardStatsDto stats)
        {
            return _hubContext.Clients
                        .Group(MealPlannerAPIHub.UserGroup(userId))
                        .StatsUpdated(stats);
        }

        // ── Meal plans ────────────────────────────────────────────────────────────

        /// <summary>Push the updated meal plan to its owner.</summary>
        public Task NotifyMealPlanUpdatedAsync(Guid userId, MealPlanDto mealPlan)
        {
            return _hubContext.Clients
                        .Group(MealPlannerAPIHub.UserGroup(userId))
                        .MealPlanUpdated(mealPlan);
        }

        // ── Shopping lists ────────────────────────────────────────────────────────

        /// <summary>Push the full updated shopping list to all subscribers.</summary>
        public Task NotifyShoppingListUpdatedAsync(Guid shoppingListId, ShoppingListDto shoppingList)
        {
            return _hubContext.Clients
                        .Group(MealPlannerAPIHub.ShoppingListGroup(shoppingListId))
                        .ShoppingListUpdated(shoppingList);
        }

        /// <summary>Push a single toggled item — cheaper than sending the full list.</summary>
        public Task NotifyShoppingItemToggledAsync(Guid shoppingListId, ShoppingListItemDto item)
        {
            return _hubContext.Clients
                        .Group(MealPlannerAPIHub.ShoppingListGroup(shoppingListId))
                        .ShoppingItemToggled(shoppingListId, item);
        }
    }
}
