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
        Task<List<Recipe>> GetListByAuthorAsync(Guid authorId, CancellationToken cancellationToken = default);
        Task<List<Recipe>> GetTopRatedAsync(int count, CancellationToken cancellationToken = default);
    }
}
