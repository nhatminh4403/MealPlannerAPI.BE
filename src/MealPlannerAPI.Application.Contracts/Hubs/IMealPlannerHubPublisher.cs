using MealPlannerAPI.Dashboard;
using MealPlannerAPI.MealPlans.Dtos;
using MealPlannerAPI.Notifications;
using MealPlannerAPI.ShoppingLists.Dtos;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MealPlannerAPI.Hubs
{
    public interface IMealPlannerHubPublisher : ITransientDependency
    {
        Task NotifyTrendingUpdatedAsync();
        Task NotifyStatsUpdatedAsync(Guid userId, DashboardStatsDto stats);
        Task NotifyMealPlanUpdatedAsync(Guid userId, MealPlanDto mealPlan);
        Task NotifyShoppingListUpdatedAsync(Guid shoppingListId, ShoppingListDto shoppingList);
        Task NotifyShoppingItemToggledAsync(Guid shoppingListId, ShoppingListItemDto item); 
        Task NotifyNotificationReceivedAsync(Guid userId, UserNotificationDto notification);
        Task NotifyUnreadCountChangedAsync(Guid userId, int count);
    }
}
