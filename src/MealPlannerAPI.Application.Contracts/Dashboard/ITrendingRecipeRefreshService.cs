using MealPlannerAPI.Recipes.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MealPlannerAPI.Dashboard
{
    public interface ITrendingRecipeRefreshService : ITransientDependency
    {
        Task<List<TrendingRecipeDto>> FetchFromDbAsync();
        Task RefreshAsync();
    }
}
