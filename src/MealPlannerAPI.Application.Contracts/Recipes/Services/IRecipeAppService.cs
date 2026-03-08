using MealPlannerAPI.Recipes.Dtos;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MealPlannerAPI.Recipes.Services
{
    public interface IRecipeAppService : ICrudAppService<
        RecipeDto, RecipeSummaryDto,
        Guid,
        GetRecipesInput,
        CreateUpdateRecipeDto, CreateUpdateRecipeDto>
    {
        Task<ListResultDto<RecipeSummaryDto>> GetByAuthorAsync(Guid authorId);
        Task<ListResultDto<RecipeSummaryDto>> GetTopRatedAsync(int count);
        Task<ListResultDto<RecipeSummaryDto>> GetByCuisineAsync(string cuisine);
    }
}
