using MealPlannerAPI.Enums;

namespace MealPlannerAPI.Users.Dtos
{
    public class CreateUpdateUserSettingsDto
    {
        public VisibilityLevel ProfileVisibility { get; set; }
        public VisibilityLevel RecipesVisibility { get; set; }
        public VisibilityLevel ShoppingListVisibility { get; set; }

        public bool NotifyMealReminders { get; set; }
        public bool NotifyRecipeUpdates { get; set; }
        public bool NotifyCommunityActivity { get; set; }
        public bool NotifyShoppingListAlerts { get; set; }
    }
}
