using MealPlannerAPI.EntityFrameworkCore;
using MealPlannerAPI.Enums;
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
                .Include(r => r.Ingredients).ThenInclude(i => i.Nutrition)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken: cancellationToken);
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
        private static IQueryable<Recipe> ApplyAuthorFilters(IQueryable<Recipe> query,
                                                             Guid authorId,
                                                             string? searchTerm,
                                                             string? cuisine,
                                                             DifficultyLevel? difficulty,
                                                             int? maxTotalTimeMinutes,
                                                             bool? vegetarian)
        {
            query = query.Where(r => r.AuthorId == authorId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(r => r.Name.Contains(searchTerm) ||
                                         r.Description.Contains(searchTerm));

            if (!string.IsNullOrWhiteSpace(cuisine))
                query = query.Where(r => r.Cuisine == cuisine);

            if (difficulty.HasValue)
                query = query.Where(r => r.Difficulty == difficulty.Value);

            if (maxTotalTimeMinutes.HasValue)
                query = query.Where(r => r.CookingTimeMinutes + r.PrepTimeMinutes <= maxTotalTimeMinutes.Value);

            if (vegetarian == true)
                query = query.Where(r => r.Tags != null && r.Tags.Contains("vegetarian"));

            return query;
        }

        public async Task<long> GetCountByAuthorAsync(Guid authorId,
                                                      string? searchTerm = null,
                                                      string? cuisine = null,
                                                      DifficultyLevel? difficulty = null,
                                                      int? maxTotalTimeMinutes = null,
                                                      bool? vegetarian = null,
                                                      CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();
            var query = ApplyAuthorFilters(dbSet.AsNoTracking(),
                                           authorId,
                                           searchTerm,
                                           cuisine,
                                           difficulty,
                                           maxTotalTimeMinutes,
                                           vegetarian);

            return await query.LongCountAsync(cancellationToken);
        }

        public async Task<List<Recipe>> GetListByAuthorAsync(Guid authorId,
                                                             string? sorting = null,
                                                             int skipCount = 0,
                                                             int maxResultCount = int.MaxValue,
                                                             string? searchTerm = null,
                                                             string? cuisine = null,
                                                             DifficultyLevel? difficulty = null,
                                                             int? maxTotalTimeMinutes = null,
                                                             bool? vegetarian = null,
                                                             CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();
            var query = ApplyAuthorFilters(dbSet.AsNoTracking(),
                                           authorId,
                                           searchTerm,
                                           cuisine,
                                           difficulty,
                                           maxTotalTimeMinutes,
                                           vegetarian);

            query = !string.IsNullOrWhiteSpace(sorting) ? query.OrderBy(sorting) : query.OrderByDescending(r => r.Rating);

            return await query.Skip(skipCount).Take(maxResultCount).ToListAsync(cancellationToken);
        }
    }
}
