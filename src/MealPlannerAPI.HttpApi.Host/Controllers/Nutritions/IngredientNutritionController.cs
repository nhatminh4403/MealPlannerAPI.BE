using MealPlannerAPI.Nutritions;
using MealPlannerAPI.Nutritions.Dtos;
using MealPlannerAPI.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.Controllers.Nutritions
{
    [Route(APIRoute.APIApp + "nutritions")]
    [ApiController]
    [Authorize]
    public class IngredientNutritionController : ControllerBase
    {
        private readonly IIngredientNutritionAppService _ingredientNutritionAppService;

        public IngredientNutritionController(IIngredientNutritionAppService ingredientNutritionAppService)
        {
            _ingredientNutritionAppService = ingredientNutritionAppService;
        }
        [HttpGet("search")]
        public Task<IngredientNutritionSearchResultDto> SearchAsync(
        [FromQuery] string query,
        [FromQuery] bool includeOff = false)
        => _ingredientNutritionAppService.SearchAsync(query, includeOff);

        /// <summary>
        /// Save a new ingredient (manual entry or confirmed OFF result).
        /// POST /api/app/ingredient-nutritions
        /// </summary>
        [HttpPost]
        public Task<IngredientNutritionDto> CreateAsync(
            [FromBody] CreateIngredientNutritionDto input)
            => _ingredientNutritionAppService.CreateAsync(input);

        /// <summary>
        /// Paged list.
        /// GET /api/app/ingredient-nutritions
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public Task<PagedResultDto<IngredientNutritionDto>> GetListAsync(
            [FromQuery] PagedAndSortedResultRequestDto input)
            => _ingredientNutritionAppService.GetListAsync(input);
    }
}
