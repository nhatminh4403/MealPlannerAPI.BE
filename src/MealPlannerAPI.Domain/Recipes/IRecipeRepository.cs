using MealPlannerAPI.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace MealPlannerAPI.Recipes
{
    public interface IRecipeRepository : IRepository<Recipe, Guid>
    {
        Task<List<Recipe>> GetListByCuisineAsync(string cuisine, CancellationToken cancellationToken = default);
        Task<List<Recipe>> GetTopRatedAsync(int count, CancellationToken cancellationToken = default);


        Task<long> GetCountByAuthorAsync(Guid authorId,
                                         string? searchTerm = null,
                                         string? cuisine = null,
                                         DifficultyLevel? difficulty = null,
                                         int? maxTotalTimeMinutes = null,
                                         bool? vegetarian = null,
                                         CancellationToken cancellationToken = default);

        Task<List<Recipe>> GetListByAuthorAsync(Guid authorId,
                                                string? sorting = null,
                                                int skipCount = 0,
                                                int maxResultCount = int.MaxValue,
                                                string? searchTerm = null,
                                                string? cuisine = null,
                                                DifficultyLevel? difficulty = null,
                                                int? maxTotalTimeMinutes = null,
                                                bool? vegetarian = null,
                                                CancellationToken cancellationToken = default);
    }
}
