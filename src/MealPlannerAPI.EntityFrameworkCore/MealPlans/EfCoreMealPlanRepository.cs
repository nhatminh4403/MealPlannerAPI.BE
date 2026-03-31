using MealPlannerAPI.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
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
            var query = await GetQueryableAsync();

            if (!includeDetails)
            {
                query = (await GetDbSetAsync()).AsQueryable();
            }


            return await query.FirstOrDefaultAsync(r => r.Id == id, cancellationToken: cancellationToken) ?? throw new EntityNotFoundException(typeof(MealPlan), id);
        }

        public async override Task<IQueryable<MealPlan>> GetQueryableAsync()
        {
            var dbSet = await GetDbSetAsync();

            return dbSet.Include(r => r.Entries).AsQueryable();
        }
    }
}
