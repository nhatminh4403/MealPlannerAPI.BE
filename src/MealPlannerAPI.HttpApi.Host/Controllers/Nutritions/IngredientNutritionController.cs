using MealPlannerAPI.Models;
using MealPlannerAPI.Nutritions;
using MealPlannerAPI.Nutritions.Dtos;
using MealPlannerAPI.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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

        /// <summary>Search ingredient nutrition data.</summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IngredientNutritionSearchResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchAsync(
            [FromQuery] string query,
            [FromQuery] bool includeExternal = false)
        {
            try
            {
                var result = await _ingredientNutritionAppService.SearchAsync(query, includeExternal);
                return Ok(new ApiResponse<IngredientNutritionSearchResultDto>(true, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Create a new ingredient nutrition entry.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<IngredientNutritionDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateIngredientNutritionDto input)
        {
            try
            {
                var result = await _ingredientNutritionAppService.CreateAsync(input);
                return StatusCode(StatusCodes.Status201Created,
                    new ApiResponse<IngredientNutritionDto>(true, "Ingredient nutrition created successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>
        /// Paged list.
        /// GET /api/app/ingredient-nutritions
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<IngredientNutritionDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
        {
            try
            {
                var result = await _ingredientNutritionAppService.GetListAsync(input);
                return Ok(new ApiResponse<PagedResultDto<IngredientNutritionDto>>(true, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }
    }
}
