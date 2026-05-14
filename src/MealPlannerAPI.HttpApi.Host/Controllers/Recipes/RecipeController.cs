using MealPlannerAPI.Models;
using MealPlannerAPI.Recipes;
using MealPlannerAPI.Recipes.Dtos;
using MealPlannerAPI.Recipes.Services;
using MealPlannerAPI.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Domain.Entities;

namespace MealPlannerAPI.Controllers.Recipes
{
    [Route(APIRoute.APIApp + "[controller]")]
    [ApiController]
    [Authorize]
    public class RecipeController : AbpControllerBase
    {
        private readonly IRecipeAppService _recipeAppService;

        public RecipeController(IRecipeAppService recipeAppService)
        {
            _recipeAppService = recipeAppService;
        }

        /// <summary>Get a recipe by ID.</summary>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<RecipeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRecipeAsync(Guid id)
        {
            try
            {
                var recipe = await _recipeAppService.GetAsync(id);
                return Ok(new ApiResponse<RecipeDto>(true, "Success", recipe));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Recipe not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Get a paged list of recipes.</summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<RecipeSummaryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRecipesAsync([FromQuery] GetRecipesInput input)
        {
            try
            {
                var recipes = await _recipeAppService.GetListAsync(input);
                return Ok(new ApiResponse<PagedResultDto<RecipeSummaryDto>>(true, "Success", recipes));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Get all recipes by a specific author.</summary>
        [HttpGet("by-author/{authorId:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ListResultDto<RecipeSummaryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByAuthorAsync(Guid authorId)
        {
            try
            {
                var result = await _recipeAppService.GetByAuthorAsync(authorId);
                return Ok(new ApiResponse<ListResultDto<RecipeSummaryDto>>(true, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Get the top-rated recipes.</summary>
        [HttpGet("top-rated")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ListResultDto<RecipeSummaryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopRatedAsync([FromQuery] int count = 10)
        {
            try
            {
                var result = await _recipeAppService.GetTopRatedAsync(count);
                return Ok(new ApiResponse<ListResultDto<RecipeSummaryDto>>(true, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Get all recipes for a specific cuisine.</summary>
        [HttpGet("by-cuisine/{cuisine}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ListResultDto<RecipeSummaryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByCuisineAsync(string cuisine)
        {
            try
            {
                var result = await _recipeAppService.GetByCuisineAsync(cuisine);
                return Ok(new ApiResponse<ListResultDto<RecipeSummaryDto>>(true, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Create a new recipe. The current user becomes the author.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<RecipeDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUpdateRecipeDto input)
        {
            try
            {
                var result = await _recipeAppService.CreateAsync(input);
                return StatusCode(StatusCodes.Status201Created,
                    new ApiResponse<RecipeDto>(true, "Recipe created successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Update an existing recipe.</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<RecipeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] CreateUpdateRecipeDto input)
        {
            try
            {
                var result = await _recipeAppService.UpdateAsync(id, input);
                return Ok(new ApiResponse<RecipeDto>(true, "Recipe updated successfully", result));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Recipe not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Soft-delete a recipe.</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                await _recipeAppService.DeleteAsync(id);
                return Ok(new ApiResponse(true, "Recipe deleted successfully"));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Recipe not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }
    }
}
