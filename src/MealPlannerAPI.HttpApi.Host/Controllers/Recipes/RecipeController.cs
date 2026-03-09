using MealPlannerAPI.Recipes;
using MealPlannerAPI.Recipes.Dtos;
using MealPlannerAPI.Recipes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace MealPlannerAPI.Controllers.Recipes
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecipeController : AbpControllerBase
    {
        private readonly IRecipeAppService _recipeAppService;

        public RecipeController(IRecipeAppService recipeAppService)
        {
            _recipeAppService = recipeAppService;
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRecipeAsync(Guid id)
        {
            var recipe = await _recipeAppService.GetAsync(id);
            return Ok(recipe);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetRecipesAsync([FromQuery] GetRecipesInput input)
        {
            var recipes = await _recipeAppService.GetListAsync(input);
            return Ok(recipes);
        }
        [HttpGet("by-author/{authorId:guid}")]
        [AllowAnonymous]
        public async Task<ListResultDto<RecipeSummaryDto>> GetByAuthorAsync(Guid authorId)
        {
            return await _recipeAppService.GetByAuthorAsync(authorId);
        }
        [HttpGet("top-rated")]
        [AllowAnonymous]
        public Task<ListResultDto<RecipeSummaryDto>> GetTopRatedAsync([FromQuery] int count = 10)
       => _recipeAppService.GetTopRatedAsync(count);

        /// <summary>Get all recipes for a specific cuisine.</summary>
        [HttpGet("by-cuisine/{cuisine}")]
        [AllowAnonymous]
        public Task<ListResultDto<RecipeSummaryDto>> GetByCuisineAsync(string cuisine)
            => _recipeAppService.GetByCuisineAsync(cuisine);

        /// <summary>Create a new recipe. The current user becomes the author.</summary>
        [HttpPost]
        public Task<RecipeDto> CreateAsync([FromBody] CreateUpdateRecipeDto input)
            => _recipeAppService.CreateAsync(input);

        /// <summary>Update an existing recipe.</summary>
        [HttpPut("{id:guid}")]
        public Task<RecipeDto> UpdateAsync(Guid id, [FromBody] CreateUpdateRecipeDto input)
            => _recipeAppService.UpdateAsync(id, input);

        /// <summary>Soft-delete a recipe.</summary>
        [HttpDelete("{id:guid}")]
        public Task DeleteAsync(Guid id)
            => _recipeAppService.DeleteAsync(id);
    }
}
