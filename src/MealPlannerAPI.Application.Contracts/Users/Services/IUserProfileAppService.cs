using MealPlannerAPI.Users.Dtos;
using System;
using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
//using Volo.Abp.Identity;

namespace MealPlannerAPI.Users.Services
{
    public interface IUserProfileAppService : IApplicationService
    {
        Task<MealPlannerAPI.Users.Dtos.ProfileDto> GetCurrentUserProfileAsync();

        /// <param name="input">Extends ABP's UpdateProfileDto (UserName, Name, Surname, PhoneNumber).</param>
        Task<MealPlannerAPI.Users.Dtos.ProfileDto> UpdateProfileInfoAsync(UpdateProfileInfoDto input);

        Task<MealPlannerAPI.Users.Dtos.ProfileDto> UpdatePreferencesAsync(CreateUpdateUserPreferencesDto input);
        Task<MealPlannerAPI.Users.Dtos.ProfileDto> UpdateSettingsAsync(CreateUpdateUserSettingsDto input);
        Task<MealPlannerAPI.Users.Dtos.ProfileDto> UpdateAvatarAsync(string avatarUrl);

        /// <param name="input">ABP's ChangePasswordInput — CurrentPassword + NewPassword.</param>
        Task ChangePasswordAsync(ChangePasswordInput input);

        Task ChangeEmailAsync(ChangeEmailDto input);

        // ── Community ─────────────────────────────────────────────────────────────
        Task<CommunityUserDto> GetCommunityProfileAsync(Guid userId);
        Task<PagedResultDto<CommunityUserDto>> GetCommunityListAsync(PagedAndSortedResultRequestDto input);

        // ── Social ────────────────────────────────────────────────────────────────
        Task FollowAsync(Guid targetUserId);
        Task UnfollowAsync(Guid targetUserId);
    }
}
