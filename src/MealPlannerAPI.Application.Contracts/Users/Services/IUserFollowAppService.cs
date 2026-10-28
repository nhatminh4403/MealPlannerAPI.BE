using MealPlannerAPI.Users.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MealPlannerAPI.Users.Services
{
    public interface IUserFollowAppService : IApplicationService
    {
        Task FollowAsync(Guid targetUserId);
        Task UnfollowAsync(Guid targetUserId);
        Task<bool> IsFollowingAsync(Guid targetUserId);
        Task<PagedResultDto<FollowUserDto>> GetFollowersAsync(GetFollowListInput input);
        Task<PagedResultDto<FollowUserDto>> GetFollowingAsync(GetFollowListInput input);
        Task FillFollowStateAsync(IReadOnlyCollection<CommunityUserDto> dtos);
    }
}
