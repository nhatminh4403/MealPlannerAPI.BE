using MealPlannerAPI.Enums;

namespace MealPlannerAPI.Users.Dtos;

public class UserSettingsDto
{
    public UserPrivacyDto Privacy { get; set; } = null!;
    public UserNotificationPreferencesDto Notifications { get; set; } = null!;
}

public class UserPrivacyDto
{
    public VisibilityLevel ProfileVisibility { get; set; }
    public VisibilityLevel RecipesVisibility { get; set; }
    public VisibilityLevel ShoppingListVisibility { get; set; }
}
public class UserNotificationPreferencesDto
{
    public bool MealReminders { get; set; }
    public bool RecipeUpdates { get; set; }
    public bool CommunityActivity { get; set; }
    public bool ShoppingListAlerts { get; set; }
}
