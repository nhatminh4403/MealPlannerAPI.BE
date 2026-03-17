using MealPlannerAPI.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MealPlannerAPI.Nutritions
{
    public class EfCoreIngredientNutritionRepository :
                    EfCoreRepository<MealPlannerAPIDbContext, IngredientNutrition, Guid>, IIngredientNutritionRepository
    {
        public EfCoreIngredientNutritionRepository(IDbContextProvider<MealPlannerAPIDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<IngredientNutrition?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var db = await GetDbContextAsync();
            var normalized = name.ToLowerInvariant();
            return await db.IngredientNutritions
                .FirstOrDefaultAsync(x => x.NormalizedName == normalized, cancellationToken);
        }

        public async Task<List<IngredientNutrition>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            var db = await GetDbContextAsync();
            return await db.IngredientNutritions
                .Where(x => ids.Contains(x.Id))
                .ToListAsync(cancellationToken);
        }
    }
}
