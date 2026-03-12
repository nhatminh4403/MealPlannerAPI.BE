using Volo.Abp.Identity;

namespace MealPlannerAPI.Users.Dtos
{
    public class ProfileDto : IdentityUserDto
    {
        public string? AvatarUrl { get; set; }
        public UserPreferencesDto Preferences { get; set; } = null!;
        public UserStatsDto Stats { get; set; } = null!;
        public UserSettingsDto Settings { get; set; } = null!;
    }
}
