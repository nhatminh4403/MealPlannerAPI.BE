namespace MealPlannerAPI.Users.Dtos
{
    public class UserSettingsDto
    {
        public UserPrivacyDto Privacy { get; set; } = null!;
        public UserNotificationPreferencesDto Notifications { get; set; } = null!;
    }
}
