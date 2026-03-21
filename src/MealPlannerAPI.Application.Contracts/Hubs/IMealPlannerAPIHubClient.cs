using MealPlannerAPI.Dashboard;
using MealPlannerAPI.MealPlans.Dtos;
using MealPlannerAPI.Notifications;
using MealPlannerAPI.ShoppingLists.Dtos;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MealPlannerAPI.Hubs
{
    public interface IMealPlannerAPIHubClient 
    {
        Task TrendingUpdated();

        Task StatsUpdated(DashboardStatsDto stats);

        // ── Meal Plans ────────────────────────────────────────────────────────────
        Task MealPlanUpdated(MealPlanDto mealPlan);

        // ── Shopping Lists ────────────────────────────────────────────────────────
        Task ShoppingListUpdated(ShoppingListDto shoppingList);

        Task ShoppingItemToggled(Guid shoppingListId, ShoppingListItemDto item);
        Task NotificationReceived(UserNotificationDto notification);
        Task UnreadCountChanged(int count);
    }
}
