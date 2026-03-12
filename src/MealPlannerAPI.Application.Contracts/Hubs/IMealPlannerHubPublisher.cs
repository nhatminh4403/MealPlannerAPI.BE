using MealPlannerAPI.Dashboard;
using MealPlannerAPI.MealPlans.Dtos;
using MealPlannerAPI.ShoppingLists.Dtos;
using System;
using System.Threading.Tasks;

namespace MealPlannerAPI.Hubs
{
    public interface IMealPlannerHubPublisher
    {
        Task NotifyTrendingUpdatedAsync();
        Task NotifyStatsUpdatedAsync(Guid userId, DashboardStatsDto stats);
        Task NotifyMealPlanUpdatedAsync(Guid userId, MealPlanDto mealPlan);
        Task NotifyShoppingListUpdatedAsync(Guid shoppingListId, ShoppingListDto shoppingList);
        Task NotifyShoppingItemToggledAsync(Guid shoppingListId, ShoppingListItemDto item);
    }
}
