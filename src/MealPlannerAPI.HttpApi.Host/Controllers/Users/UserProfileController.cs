using MealPlannerAPI.Users.Dtos;
using MealPlannerAPI.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace MealPlannerAPI.Controllers.Users
{
    [ApiController]
    [Route("api/app/users")]
    [Authorize]
    public class UserProfileController : AbpControllerBase
    {
        private readonly IUserProfileAppService _userProfileAppService;

        public UserProfileController(IUserProfileAppService userProfileAppService)
        {
            _userProfileAppService = userProfileAppService;
        }

        /// <summary>Get the current user's full profile including preferences, stats and settings.</summary>
        [HttpGet("me")]
        public Task<ProfileDto> GetCurrentUserProfileAsync()
            => _userProfileAppService.GetCurrentUserProfileAsync();

        /// <summary>Get a community (public-facing) profile for any user.</summary>
        [HttpGet("{userId:guid}")]
        public Task<CommunityUserDto> GetCommunityProfileAsync(Guid userId)
            => _userProfileAppService.GetCommunityProfileAsync(userId);

        /// <summary>Get a paged list of community users.</summary>
        [HttpGet]
        public Task<PagedResultDto<CommunityUserDto>> GetCommunityListAsync(
            [FromQuery] PagedAndSortedResultRequestDto input)
            => _userProfileAppService.GetCommunityListAsync(input);

        /// <summary>Update the current user's dietary, cuisine and serving size preferences.</summary>
        [HttpPatch("me/preferences")]
        public Task<ProfileDto> UpdatePreferencesAsync([FromBody] CreateUpdateUserPreferencesDto input)
            => _userProfileAppService.UpdatePreferencesAsync(input);

        /// <summary>Update the current user's privacy and notification settings.</summary>
        [HttpPatch("me/settings")]
        public Task<ProfileDto> UpdateSettingsAsync([FromBody] CreateUpdateUserSettingsDto input)
            => _userProfileAppService.UpdateSettingsAsync(input);

        /// <summary>Update the current user's avatar URL.</summary>
        [HttpPatch("me/avatar")]
        public Task<ProfileDto> UpdateAvatarAsync([FromBody] UpdateAvatarDto input)
            => _userProfileAppService.UpdateAvatarAsync(input.AvatarUrl);

        /// <summary>Follow another user.</summary>
        [HttpPost("{targetUserId:guid}/follow")]
        public Task FollowAsync(Guid targetUserId)
            => _userProfileAppService.FollowAsync(targetUserId);

        /// <summary>Unfollow a user you are currently following.</summary>
        [HttpDelete("{targetUserId:guid}/follow")]
        public Task UnfollowAsync(Guid targetUserId)
            => _userProfileAppService.UnfollowAsync(targetUserId);
    }
}
