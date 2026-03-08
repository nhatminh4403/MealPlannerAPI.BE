namespace MealPlannerAPI.Users.Dtos
{
    public class UserNotificationPreferencesDto
    {
        public bool MealReminders { get; set; }
        public bool RecipeUpdates { get; set; }
        public bool CommunityActivity { get; set; }
        public bool ShoppingListAlerts { get; set; }
    }
}
