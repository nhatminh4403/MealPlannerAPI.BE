using MealPlannerAPI.Models;
using MealPlannerAPI.Routes;
using MealPlannerAPI.Users.Dtos;
using MealPlannerAPI.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Domain.Entities;

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
        [ProducesResponseType(typeof(ApiResponse<MealPlannerAPI.Users.Dtos.ProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUserProfileAsync()
        {
            try
            {
                var result = await _userProfileAppService.GetCurrentUserProfileAsync();
                return Ok(new ApiResponse<MealPlannerAPI.Users.Dtos.ProfileDto>(true, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Update basic identity info — name, surname, phone number.</summary>
        [HttpPatch("me")]
        [ProducesResponseType(typeof(ApiResponse<MealPlannerAPI.Users.Dtos.ProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProfileInfoAsync([FromBody] UpdateProfileInfoDto input)
        {
            try
            {
                var result = await _userProfileAppService.UpdateProfileInfoAsync(input);
                return Ok(new ApiResponse<MealPlannerAPI.Users.Dtos.ProfileDto>(true, "Profile updated successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Update dietary, cuisine and serving preferences.</summary>
        [HttpPatch("me/preferences")]
        [ProducesResponseType(typeof(ApiResponse<MealPlannerAPI.Users.Dtos.ProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePreferencesAsync([FromBody] CreateUpdateUserPreferencesDto input)
        {
            try
            {
                var result = await _userProfileAppService.UpdatePreferencesAsync(input);
                return Ok(new ApiResponse<MealPlannerAPI.Users.Dtos.ProfileDto>(true, "Preferences updated successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Update privacy and notification settings.</summary>
        [HttpPatch("me/settings")]
        [ProducesResponseType(typeof(ApiResponse<MealPlannerAPI.Users.Dtos.ProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateSettingsAsync([FromBody] CreateUpdateUserSettingsDto input)
        {
            try
            {
                var result = await _userProfileAppService.UpdateSettingsAsync(input);
                return Ok(new ApiResponse<MealPlannerAPI.Users.Dtos.ProfileDto>(true, "Settings updated successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Update avatar URL.</summary>
        [HttpPatch("me/avatar")]
        [ProducesResponseType(typeof(ApiResponse<MealPlannerAPI.Users.Dtos.ProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAvatarAsync([FromBody] UpdateAvatarDto input)
        {
            try
            {
                var result = await _userProfileAppService.UpdateAvatarAsync(input.AvatarUrl);
                return Ok(new ApiResponse<MealPlannerAPI.Users.Dtos.ProfileDto>(true, "Avatar updated successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>
        /// Change password. Uses ABP's ChangePasswordInput — runs full password
        /// policy validation including complexity rules and history checks.
        /// </summary>
        [HttpPatch("me/password")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordInput input)
        {
            try
            {
                await _userProfileAppService.ChangePasswordAsync(input);
                return Ok(new ApiResponse(true, "Password changed successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>
        /// Change email address. Runs ABP's uniqueness and normalisation pipeline.
        /// </summary>
        [HttpPatch("me/email")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeEmailAsync([FromBody] ChangeEmailDto input)
        {
            try
            {
                await _userProfileAppService.ChangeEmailAsync(input);
                return Ok(new ApiResponse(true, "Email changed successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        // ── Community ─────────────────────────────────────────────────────────────

        /// <summary>Get the public community profile for any user.</summary>
        [HttpGet("{userId:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<CommunityUserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCommunityProfileAsync(Guid userId)
        {
            try
            {
                var result = await _userProfileAppService.GetCommunityProfileAsync(userId);
                return Ok(new ApiResponse<CommunityUserDto>(true, "Success", result));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "User not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Get a paged list of community users.</summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<CommunityUserDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCommunityListAsync([FromQuery] PagedAndSortedResultRequestDto input)
        {
            try
            {
                var result = await _userProfileAppService.GetCommunityListAsync(input);
                return Ok(new ApiResponse<PagedResultDto<CommunityUserDto>>(true, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        // ── Social ────────────────────────────────────────────────────────────────

        /// <summary>Follow a user.</summary>
        [HttpPost("{targetUserId:guid}/follow")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FollowAsync(Guid targetUserId)
        {
            try
            {
                await _userProfileAppService.FollowAsync(targetUserId);
                return Ok(new ApiResponse(true, "User followed successfully"));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "User not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Unfollow a user.</summary>
        [HttpDelete("{targetUserId:guid}/follow")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UnfollowAsync(Guid targetUserId)
        {
            try
            {
                await _userProfileAppService.UnfollowAsync(targetUserId);
                return Ok(new ApiResponse(true, "User unfollowed successfully"));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "User not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }
    }
}
