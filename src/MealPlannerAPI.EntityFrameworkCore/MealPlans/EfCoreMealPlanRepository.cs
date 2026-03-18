using MealPlannerAPI.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MealPlannerAPI.MealPlans
{
    public class EfCoreMealPlanRepository : EfCoreRepository<MealPlannerAPIDbContext, MealPlan, Guid>,
          IMealPlanRepository
    {
        public EfCoreMealPlanRepository(IDbContextProvider<MealPlannerAPIDbContext> dbContextProvider)
            : base(dbContextProvider) { }

        // your custom methods...
    }
}
