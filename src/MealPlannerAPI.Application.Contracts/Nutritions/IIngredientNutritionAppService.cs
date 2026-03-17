using MealPlannerAPI.Nutritions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MealPlannerAPI.Nutritions
{
    public interface IIngredientNutritionAppService : IApplicationService
    {
        Task<IngredientNutritionSearchResultDto> SearchAsync(string query,
                                                             bool includeOff = false);
        Task<PagedResultDto<IngredientNutritionDto>> GetListAsync(PagedAndSortedResultRequestDto input);
        Task<IngredientNutritionDto> CreateAsync(CreateIngredientNutritionDto input);
    }
}
