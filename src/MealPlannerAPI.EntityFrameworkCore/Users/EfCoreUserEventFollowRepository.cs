using MealPlannerAPI.EntityFrameworkCore;
using MealPlannerAPI.Users.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MealPlannerAPI.Users
{
    public class EfCoreUserEventFollowRepository
        : EfCoreRepository<MealPlannerAPIDbContext, UserFollowEvent, Guid>, IUserEventFollowRepository

    {
        public EfCoreUserEventFollowRepository(IDbContextProvider<MealPlannerAPIDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
