using MealPlannerAPI.Users.Dtos;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MealPlannerAPI.Users.Services
{
    public interface IUserProfileAppService : IApplicationService
    {
        /// <summary>Get the full profile of the currently authenticated user.</summary>
        Task<ProfileDto> GetCurrentUserProfileAsync();

        /// <summary>Get a public-facing community view of any user by their id.</summary>
        Task<CommunityUserDto> GetCommunityProfileAsync(Guid userId);

        /// <summary>Get a paged list of community users.</summary>
        Task<PagedResultDto<CommunityUserDto>> GetCommunityListAsync(PagedAndSortedResultRequestDto input);

        /// <summary>Update the current user's dietary and cuisine preferences.</summary>
        Task<ProfileDto> UpdatePreferencesAsync(CreateUpdateUserPreferencesDto input);

        /// <summary>Update the current user's privacy and notification settings.</summary>
        Task<ProfileDto> UpdateSettingsAsync(CreateUpdateUserSettingsDto input);

        /// <summary>Update the current user's avatar URL.</summary>
        Task<ProfileDto> UpdateAvatarAsync(string avatarUrl);

        /// <summary>Follow a user by their id.</summary>
        Task FollowAsync(Guid targetUserId);

        /// <summary>Unfollow a user by their id.</summary>
        Task UnfollowAsync(Guid targetUserId);
    }
}
