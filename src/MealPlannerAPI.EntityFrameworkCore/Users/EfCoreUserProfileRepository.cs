using MealPlannerAPI.EntityFrameworkCore;
using MealPlannerAPI.Users.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MealPlannerAPI.Users
{
    public class EfCoreUserProfileRepository :
        EfCoreRepository<MealPlannerAPIDbContext, UserProfile, Guid>, IUserProfileRepository

    {
        public EfCoreUserProfileRepository(IDbContextProvider<MealPlannerAPIDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
