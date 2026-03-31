using MealPlannerAPI.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MealPlannerAPI.Notifications
{
    public class NotificationFactory
    {
        public static Task NewFollower(IUserNotificationAppService svc, Guid targetUserId, 
            string followerUsername, string? avatarUrl = null)
        => svc.SendAsync(targetUserId, NotificationType.NewFollower,
            title: "New Follower",
            message: $"{followerUsername} started following you.",
            avatarUrl: avatarUrl);

        public static Task RecipeShared(IUserNotificationAppService svc, Guid targetUserId, string sharerUsername, string recipeName)
            => svc.SendAsync(targetUserId, NotificationType.RecipeShared,
                title: "Recipe Shared With You",
                message: $"{sharerUsername} shared \"{recipeName}\" with you.");

        public static Task RecipeLiked(IUserNotificationAppService svc, Guid targetUserId, string likerUsername, string recipeName)
            => svc.SendAsync(targetUserId, NotificationType.RecipeLiked,
                title: "Someone Liked Your Recipe",
                message: $"{likerUsername} liked your recipe \"{recipeName}\".");

        public static Task RecipeCommented(IUserNotificationAppService svc, Guid targetUserId, string commenterUsername, string recipeName)
            => svc.SendAsync(targetUserId, NotificationType.RecipeCommented,
                title: "New Comment on Your Recipe",
                message: $"{commenterUsername} commented on \"{recipeName}\".");

        public static Task ShoppingListAlert(IUserNotificationAppService svc, Guid targetUserId, string listName)
            => svc.SendAsync(targetUserId, NotificationType.ShoppingListAlert,
                title: "Shopping List Updated",
                message: $"Your shopping list \"{listName}\" was updated.");

        public static Task MealReminder(IUserNotificationAppService svc, Guid targetUserId, string mealName)
            => svc.SendAsync(targetUserId, NotificationType.MealReminder,
                title: "Meal Reminder",preventDuplicateToday: true,
                message: $"Don't forget to prepare \"{mealName}\" today.");
    }
}
