using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace MealPlannerAPI.Users.Interfaces
{
    public interface IUserFollowRepository : IRepository<UserFollow, Guid>
    {
        Task<int> AdjustCountAsync(Guid userId, int followersDelta, int followingDelta);
    }
}
