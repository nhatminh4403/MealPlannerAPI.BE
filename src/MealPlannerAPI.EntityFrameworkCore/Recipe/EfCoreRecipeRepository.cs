using MealPlannerAPI.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MealPlannerAPI.Recipes
{
    public class EfCoreRecipeRepository : EfCoreRepository<MealPlannerAPIDbContext, Recipe, Guid>, IRecipeRepository
    {
        public EfCoreRecipeRepository(IDbContextProvider<MealPlannerAPIDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async override Task<Recipe> GetAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet?
                .Include(r => r.Ingredients)?
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken: cancellationToken);
        }

        public async Task<List<Recipe>> GetListByAuthorAsync(Guid authorId, CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            var query = dbSet.AsQueryable().AsNoTracking();

            return await query.Where(r => r.AuthorId == authorId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Recipe>> GetListByCuisineAsync(string cuisine, CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();
            var query = dbSet.AsQueryable();
            return await query.Where(r => r.Cuisine == cuisine).AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<List<Recipe>> GetTopRatedAsync(int count, CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();
            var query = dbSet.AsQueryable().AsNoTracking();
            
            return await query.OrderByDescending(r => r.Rating).Take(count).ToListAsync(cancellationToken);
        }

    }
}
