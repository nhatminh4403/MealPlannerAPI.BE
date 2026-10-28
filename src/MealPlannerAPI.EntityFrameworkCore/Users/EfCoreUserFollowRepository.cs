using MealPlannerAPI.EntityFrameworkCore;
using MealPlannerAPI.Users.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MealPlannerAPI.Users
{
    public class EfCoreUserFollowRepository 
        : EfCoreRepository<MealPlannerAPIDbContext, UserFollow, Guid>, IUserFollowRepository
    {
        public EfCoreUserFollowRepository(IDbContextProvider<MealPlannerAPIDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<int> AdjustCountAsync(Guid userId, int followersDelta, int followingDelta)
        {
            var db = await GetDbContextAsync();
            return await db.Set<UserProfile>()
                .Where(u => u.Id == userId
                         && u.Followers + followersDelta >= 0
                         && u.Following + followingDelta >= 0)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.Followers, u => u.Followers + followersDelta)
                    .SetProperty(u => u.Following, u => u.Following + followingDelta));
        }
    }
}
