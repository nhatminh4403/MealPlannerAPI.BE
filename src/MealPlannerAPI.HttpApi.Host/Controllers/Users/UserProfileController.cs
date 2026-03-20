using MealPlannerAPI.Routes;
using MealPlannerAPI.Users.Dtos;
using MealPlannerAPI.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace MealPlannerAPI.Controllers.Users
{
    [ApiController]
    [Route(APIRoute.APIApp + "user")]
    [Authorize]
    public class UserProfileController : AbpControllerBase
    {
        private readonly IUserProfileAppService _userProfileAppService;

        public UserProfileController(IUserProfileAppService userProfileAppService)
        {
            _userProfileAppService = userProfileAppService;
        }

        // ── Own profile ───────────────────────────────────────────────────────────

        /// <summary>Get the current user's full profile.</summary>
        [HttpGet("me")]
        public Task<MealPlannerAPI.Users.Dtos.ProfileDto> GetCurrentUserProfileAsync()
            => _userProfileAppService.GetCurrentUserProfileAsync();

        /// <summary>Update basic identity info — name, surname, phone number.</summary>
        [HttpPatch("me")]
        public Task<MealPlannerAPI.Users.Dtos.ProfileDto> UpdateProfileInfoAsync([FromBody] UpdateProfileInfoDto input)
            => _userProfileAppService.UpdateProfileInfoAsync(input);

        /// <summary>Update dietary, cuisine and serving preferences.</summary>
        [HttpPatch("me/preferences")]
        public Task<MealPlannerAPI.Users.Dtos.ProfileDto> UpdatePreferencesAsync([FromBody] CreateUpdateUserPreferencesDto input)
            => _userProfileAppService.UpdatePreferencesAsync(input);

        /// <summary>Update privacy and notification settings.</summary>
        [HttpPatch("me/settings")]
        public Task<MealPlannerAPI.Users.Dtos.ProfileDto> UpdateSettingsAsync([FromBody] CreateUpdateUserSettingsDto input)
            => _userProfileAppService.UpdateSettingsAsync(input);

        /// <summary>Update avatar URL.</summary>
        [HttpPatch("me/avatar")]
        public Task<MealPlannerAPI.Users.Dtos.ProfileDto> UpdateAvatarAsync([FromBody] UpdateAvatarDto input)
            => _userProfileAppService.UpdateAvatarAsync(input.AvatarUrl);

        /// <summary>
        /// Change password. Uses ABP's ChangePasswordInput — runs full password
        /// policy validation including complexity rules and history checks.
        /// </summary>
        [HttpPatch("me/password")]
        public Task ChangePasswordAsync([FromBody] ChangePasswordInput input)
            => _userProfileAppService.ChangePasswordAsync(input);

        /// <summary>
        /// Change email address. Runs ABP's uniqueness and normalisation pipeline.
        /// </summary>
        [HttpPatch("me/email")]
        public Task ChangeEmailAsync([FromBody] ChangeEmailDto input)
            => _userProfileAppService.ChangeEmailAsync(input);

        // ── Community ─────────────────────────────────────────────────────────────

        /// <summary>Get the public community profile for any user.</summary>
        [HttpGet("{userId:guid}")]
        [AllowAnonymous]
        public Task<CommunityUserDto> GetCommunityProfileAsync(Guid userId)
            => _userProfileAppService.GetCommunityProfileAsync(userId);

        /// <summary>Get a paged list of community users.</summary>
        [HttpGet]
        [AllowAnonymous]
        public Task<PagedResultDto<CommunityUserDto>> GetCommunityListAsync(
            [FromQuery] PagedAndSortedResultRequestDto input)
            => _userProfileAppService.GetCommunityListAsync(input);

        // ── Social ────────────────────────────────────────────────────────────────

        /// <summary>Follow a user.</summary>
        [HttpPost("{targetUserId:guid}/follow")]
        public Task FollowAsync(Guid targetUserId)
            => _userProfileAppService.FollowAsync(targetUserId);

        /// <summary>Unfollow a user.</summary>
        [HttpDelete("{targetUserId:guid}/follow")]
        public Task UnfollowAsync(Guid targetUserId)
            => _userProfileAppService.UnfollowAsync(targetUserId);
    }
}
