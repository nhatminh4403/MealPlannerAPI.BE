using MealPlannerAPI.Users.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MealPlannerAPI.Users.Services
{
    public interface IUserFollowAnalyticsAppService : IApplicationService
    {
        Task<List<FollowStatPointDto>> GetMyStatsAsync(GetFollowStatsInput input);
    }
}
