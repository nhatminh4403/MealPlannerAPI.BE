using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories;

namespace MealPlannerAPI.Users.Interfaces
{
    public interface IUserEventFollowRepository : IRepository<UserFollowEvent, Guid>
    {
    }
}
