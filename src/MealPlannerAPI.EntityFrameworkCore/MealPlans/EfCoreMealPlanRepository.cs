using MealPlannerAPI.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MealPlannerAPI.MealPlans
{
    public class EfCoreMealPlanRepository : EfCoreRepository<MealPlannerAPIDbContext, MealPlan, Guid>,
          IMealPlanRepository
    {
        public EfCoreMealPlanRepository(IDbContextProvider<MealPlannerAPIDbContext> dbContextProvider)
            : base(dbContextProvider) { }

        public override async Task<MealPlan> GetAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet?
                .Include(r => r.Entries)?
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken: cancellationToken);
        }
    }
}
