using MealPlannerAPI.Users.Dtos;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
//using Volo.Abp.Identity;

namespace MealPlannerAPI.Users.Services
{
    public interface IUserProfileAppService : IApplicationService
    {
        Task<MealPlannerAPI.Users.Dtos.ProfileDto> GetCurrentUserProfileAsync(CancellationToken cancellationToken = default);

        /// <param name="input">Extends ABP's UpdateProfileDto (UserName, Name, Surname, PhoneNumber).</param>
        Task<MealPlannerAPI.Users.Dtos.ProfileDto> UpdateProfileInfoAsync(UpdateProfileInfoDto input, CancellationToken cancellationToken = default);

        Task<MealPlannerAPI.Users.Dtos.ProfileDto> UpdatePreferencesAsync(CreateUpdateUserPreferencesDto input, CancellationToken cancellationToken = default);
        Task<MealPlannerAPI.Users.Dtos.ProfileDto> UpdateSettingsAsync(CreateUpdateUserSettingsDto input, CancellationToken cancellationToken = default);
        Task<MealPlannerAPI.Users.Dtos.ProfileDto> UpdateAvatarAsync(string avatarUrl);

        Task ChangePasswordAsync(ChangePasswordInput input, CancellationToken cancellationToken = default);

        Task ChangeEmailAsync(ChangeEmailDto input, CancellationToken cancellationToken = default);

        // ── Community ─────────────────────────────────────────────────────────────
        Task<CommunityUserDto> GetCommunityProfileAsync(Guid userId);
        Task<PagedResultDto<CommunityUserDto>> GetCommunityListAsync(PagedAndSortedResultRequestDto input, CancellationToken cancellationToken = default);

        // ── Social ────────────────────────────────────────────────────────────────
        Task FollowAsync(Guid targetUserId);
        Task UnfollowAsync(Guid targetUserId);
    }
}
