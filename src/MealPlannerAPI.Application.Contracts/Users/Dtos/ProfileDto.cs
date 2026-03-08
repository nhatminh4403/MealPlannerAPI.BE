using System;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.Users.Dtos
{
    public class ProfileDto : FullAuditedEntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public string? AvatarUrl { get; set; }
        public UserPreferencesDto Preferences { get; set; } = null!;
        public UserStatsDto Stats { get; set; } = null!;
        public UserSettingsDto Settings { get; set; } = null!;
    }
}
