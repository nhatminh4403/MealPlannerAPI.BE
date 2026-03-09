using MealPlannerAPI.Mappings.Recipes;
using MealPlannerAPI.Recipes.Dtos;
using MealPlannerAPI.Recipes.Services;
using MealPlannerAPI.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace MealPlannerAPI.Recipes
{
    [RemoteService(false)]
    public class RecipeAppService :
        CrudAppService<Recipe, RecipeDto, RecipeSummaryDto, Guid, GetRecipesInput, CreateUpdateRecipeDto, CreateUpdateRecipeDto>,
        IRecipeAppService
    {

        private readonly IRecipeRepository _recipeRepository;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly RecipeToRecipeDtoMapper _toRecipeDtoMapper;
        private readonly RecipeToRecipeSummaryDtoMapper _toSummaryDtoMapper;
        private readonly RecipeIngredientToRecipeIngredientDtoMapper _toIngredientDtoMapper;
        private readonly CreateUpdateRecipeDtoToRecipeMapper _toRecipeMapper;

        public RecipeAppService(IRecipeRepository recipeRepository,
                                IIdentityUserRepository identityUserRepository,
                                RecipeToRecipeDtoMapper toRecipeDtoMapper,
                                RecipeToRecipeSummaryDtoMapper toSummaryDtoMapper,
                                RecipeIngredientToRecipeIngredientDtoMapper toIngredientDtoMapper,
                                CreateUpdateRecipeDtoToRecipeMapper toRecipeMapper) : base(recipeRepository)
        {
            _recipeRepository = recipeRepository;
            _identityUserRepository = identityUserRepository;
            _toRecipeDtoMapper = toRecipeDtoMapper;
            _toSummaryDtoMapper = toSummaryDtoMapper;
            _toIngredientDtoMapper = toIngredientDtoMapper;
            _toRecipeMapper = toRecipeMapper;
        }

        public async override Task<RecipeDto> CreateAsync(CreateUpdateRecipeDto input)
        {
            var recipe = new Recipe(
            GuidGenerator.Create(),
            input.Name,
            input.Cuisine,
            input.Difficulty,
            input.CookingTimeMinutes,
            input.PrepTimeMinutes,
            input.Servings,
            input.Description,
            CurrentUser.GetId());

            recipe.ImageUrl = input.ImageUrl;
            recipe.SetTags(input.Tags);
            recipe.SetInstructions(input.Instructions);

            foreach (var i in input.Ingredients)
                recipe.AddIngredient(GuidGenerator.Create(), i.Name, i.Quantity, i.Unit);

            await Repository.InsertAsync(recipe, autoSave: true);
            return await MapToRecipeDtoAsync(recipe);

        }

        public async override Task DeleteAsync(Guid id)
        {
            var recipe = await _recipeRepository.GetAsync(id);
            if (recipe != null)
            {
                await Repository.DeleteAsync(recipe, autoSave: true);
            }
            else
            {
                throw new EntityNotFoundException(typeof(Recipe), id);
            }
        }

        public async override Task<RecipeDto> GetAsync(Guid id)
        {
            var recipe = await _recipeRepository.GetAsync(id);
            if (recipe == null)
            {
                throw new EntityNotFoundException(typeof(Recipe), id);

            }
            return await MapToRecipeDtoAsync(recipe);
        }

        public async override Task<PagedResultDto<RecipeSummaryDto>> GetListAsync(GetRecipesInput input)
        {
            var query = await _recipeRepository.GetQueryableAsync();

            if (!string.IsNullOrWhiteSpace(input.SearchTerm))
                query = query.Where(r =>
                    r.Name.Contains(input.SearchTerm) ||
                    r.Description.Contains(input.SearchTerm));

            if (!string.IsNullOrWhiteSpace(input.Cuisine))
                query = query.Where(r => r.Cuisine == input.Cuisine);

            if (input.Difficulty.HasValue)
                query = query.Where(r => r.Difficulty == input.Difficulty.Value);

            if (input.MaxTotalTimeMinutes.HasValue)
                query = query.Where(r =>
                    r.CookingTimeMinutes + r.PrepTimeMinutes <= input.MaxTotalTimeMinutes.Value);

            if (input.Vegetarian == true)
                query = query.Where(r => r.Tags != null && r.Tags.Contains("vegetarian"));

            var totalCount = await AsyncExecuter.CountAsync(query);

            var recipes = await AsyncExecuter.ToListAsync(
                query.OrderByDescending(r => r.Rating)
                     .Skip(input.SkipCount)
                     .Take(input.MaxResultCount));

            return new PagedResultDto<RecipeSummaryDto>(
                totalCount,
                recipes.Select(MapToSummaryDto).ToList());
        }


        public async override Task<RecipeDto> UpdateAsync(Guid id, CreateUpdateRecipeDto input)
        {
            var recipe = await _recipeRepository.GetAsync(id);

            _toRecipeMapper.Map(input, recipe);

            recipe.SetTags(input.Tags);
            recipe.SetInstructions(input.Instructions);
            recipe.ReplaceIngredients(
                input.Ingredients.Select(i => (GuidGenerator.Create(), i.Name, i.Quantity, i.Unit)));

            await Repository.UpdateAsync(recipe, autoSave: true);
            return await MapToRecipeDtoAsync(recipe);
        }
        public async Task<ListResultDto<RecipeSummaryDto>> GetByAuthorAsync(Guid authorId)
        {
            var recipe = await _recipeRepository.GetListByAuthorAsync(authorId);
            return new ListResultDto<RecipeSummaryDto>(recipe.Select(MapToSummaryDto).ToList());

        }

        public async Task<ListResultDto<RecipeSummaryDto>> GetByCuisineAsync(string cuisine)
        {
            var recipe = await _recipeRepository.GetListByCuisineAsync(cuisine);
            return new ListResultDto<RecipeSummaryDto>(recipe.Select(MapToSummaryDto).ToList());
        }


        public async Task<ListResultDto<RecipeSummaryDto>> GetTopRatedAsync(int count)
        {
            var recipe = await _recipeRepository.GetTopRatedAsync(count);
            return new ListResultDto<RecipeSummaryDto>(recipe.Select(MapToSummaryDto).ToList());
        }

        private async Task<RecipeDto> MapToRecipeDtoAsync(Recipe recipe)
        {
             var dto = _toRecipeDtoMapper.Map(recipe);

        dto.Tags = recipe.GetTags();
        dto.Instructions = recipe.GetInstructions();
        dto.Ingredients = _toIngredientDtoMapper.MapList(recipe.Ingredients);

        var author = await _identityUserRepository.FindAsync(recipe.AuthorId);
        dto.Author = new RecipeAuthorDto
        {
            Id = recipe.AuthorId,
            Name = author?.Name ?? "Unknown",
            AvatarUrl = (author as UserProfile)?.AvatarUrl
        };

        return dto;
        }

        private RecipeSummaryDto MapToSummaryDto(Recipe recipe)
        {
            var dto = _toSummaryDtoMapper.Map(recipe);
            dto.TotalTimeMinutes = recipe.GetTotalTimeMinutes();
            dto.Tags = recipe.GetTags();
            return dto;
        }
    }
}
