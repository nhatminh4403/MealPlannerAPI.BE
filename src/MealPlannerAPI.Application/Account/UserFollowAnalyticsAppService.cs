using MealPlannerAPI.Enums;
using MealPlannerAPI.Users.Dtos;
using MealPlannerAPI.Users.Interfaces;
using MealPlannerAPI.Users.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace MealPlannerAPI.Account
{
    internal class UserFollowAnalyticsAppService : UserAppServiceBase, IUserFollowAnalyticsAppService
    {
        private readonly IUserEventFollowRepository _userEventFollowRepository;
        public UserFollowAnalyticsAppService(IIdentityUserRepository identityUserRepository, IUserEventFollowRepository userEventFollowRepository) : base(identityUserRepository)
        {
            _userEventFollowRepository = userEventFollowRepository;
        }

        public async Task<List<FollowStatPointDto>> GetMyStatsAsync(GetFollowStatsInput input)
        {
            var me = CurrentUser.GetId();
            var user = await GetUserProfileAsync(me);
            var now = Clock.Now;
            var monthly = input.Granularity == StatGranularity.Monthly;

            var years = Math.Clamp(input.Years, 1, 20);
            var start = monthly
                ? new DateTime(input.Year ?? now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                : new DateTime(now.Year - years + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Everything from start up to NOW (not just the selected year), so running totals can walk back from today
            var events = (await _userEventFollowRepository.GetQueryableAsync()).Where(e => e.CreationTime >= start);

            var followerRows = await AsyncExecuter.ToListAsync(
                events.Where(e => e.FolloweeId == me)
                      .GroupBy(e => new { e.CreationTime.Year, e.CreationTime.Month })
                      .Select(g => new
                      {
                          g.Key.Year,
                          g.Key.Month,
                          Gained = g.Count(e => e.Type == FollowEventType.Followed),
                          Lost = g.Count(e => e.Type == FollowEventType.Unfollowed)
                      }));

            var followingRows = await AsyncExecuter.ToListAsync(
                events.Where(e => e.FollowerId == me)
                      .GroupBy(e => new { e.CreationTime.Year, e.CreationTime.Month })
                      .Select(g => new
                      {
                          g.Key.Year,
                          g.Key.Month,
                          Gained = g.Count(e => e.Type == FollowEventType.Followed),
                          Lost = g.Count(e => e.Type == FollowEventType.Unfollowed)
                      }));

            // Build every bucket (zeros included) from start to now
            var points = new List<FollowStatPointDto>();
            for (var d = start; d <= now; d = monthly ? d.AddMonths(1) : d.AddYears(1))
            {
                bool In(int y, int m) => y == d.Year && (!monthly || m == d.Month);
                if (monthly && d.Year != start.Year) break; // monthly view = one calendar year

                points.Add(new FollowStatPointDto
                {
                    PeriodStart = d,
                    FollowersGained = followerRows.Where(r => In(r.Year, r.Month)).Sum(r => r.Gained),
                    FollowersLost = followerRows.Where(r => In(r.Year, r.Month)).Sum(r => r.Lost),
                    FollowingGained = followingRows.Where(r => In(r.Year, r.Month)).Sum(r => r.Gained),
                    FollowingLost = followingRows.Where(r => In(r.Year, r.Month)).Sum(r => r.Lost),
                });
            }

            // Totals: start from today's counters and walk backwards
            // (events after the last bucket, e.g. when viewing a past year, are subtracted first)
            var followers = user.Followers;
            var following = user.Following;

            var lastBucketEnd = monthly ? start.AddYears(1) : start.AddYears(years);
            followers -= followerRows.Where(r => new DateTime(r.Year, r.Month, 1) >= lastBucketEnd).Sum(r => r.Gained - r.Lost);
            following -= followingRows.Where(r => new DateTime(r.Year, r.Month, 1) >= lastBucketEnd).Sum(r => r.Gained - r.Lost);

            for (var i = points.Count - 1; i >= 0; i--)
            {
                points[i].FollowersTotal = Math.Max(0, followers);
                points[i].FollowingTotal = Math.Max(0, following);
                followers -= points[i].FollowersGained - points[i].FollowersLost;
                following -= points[i].FollowingGained - points[i].FollowingLost;
            }

            return points;
        }
    }
}
