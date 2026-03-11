using MealPlannerAPI.Dashboard;
using MealPlannerAPI.Recipes;
using MealPlannerAPI.Recipes.Dtos;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Guids;
using Volo.Abp.Mapperly;

namespace MealPlannerAPI.Mappings.Recipes;


[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RecipeToRecipeDtoMapper : MapperBase<Recipe, RecipeDto>
{
    [MapperIgnoreTarget(nameof(RecipeDto.Author))]
    [MapperIgnoreTarget(nameof(RecipeDto.Tags))]
    [MapperIgnoreTarget(nameof(RecipeDto.Instructions))]
    [MapperIgnoreTarget(nameof(RecipeDto.Ingredients))]
    [MapperIgnoreTarget(nameof(RecipeDto.TotalTimeMinutes))]
    public override partial RecipeDto Map(Recipe source);

    [MapperIgnoreTarget(nameof(RecipeDto.Author))]
    [MapperIgnoreTarget(nameof(RecipeDto.Tags))]
    [MapperIgnoreTarget(nameof(RecipeDto.Instructions))]
    [MapperIgnoreTarget(nameof(RecipeDto.Ingredients))]
    [MapperIgnoreTarget(nameof(RecipeDto.TotalTimeMinutes))]
    public override partial void Map(Recipe source, RecipeDto destination);
}

// ── Recipe → RecipeSummaryDto ─────────────────────────────────────────────────

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RecipeToRecipeSummaryDtoMapper : MapperBase<Recipe, RecipeSummaryDto>
{
    [MapperIgnoreTarget(nameof(RecipeSummaryDto.Tags))]
    [MapperIgnoreTarget(nameof(RecipeSummaryDto.TotalTimeMinutes))]
    public override partial RecipeSummaryDto Map(Recipe source);

    [MapperIgnoreTarget(nameof(RecipeSummaryDto.Tags))]
    [MapperIgnoreTarget(nameof(RecipeSummaryDto.TotalTimeMinutes))]
    public override partial void Map(Recipe source, RecipeSummaryDto destination);
}


// ── Recipe → TrendingRecipeDto ────────────────────────────────────────────────
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RecipeToTrendingRecipeDtoMapper : MapperBase<Recipe, TrendingRecipeDto>
{
    [MapperIgnoreTarget(nameof(TrendingRecipeDto.TrendingScore))]
    [MapperIgnoreTarget(nameof(TrendingRecipeDto.TrendingSince))]
    public override partial TrendingRecipeDto Map(Recipe source);

    [MapperIgnoreTarget(nameof(TrendingRecipeDto.TrendingScore))]
    [MapperIgnoreTarget(nameof(TrendingRecipeDto.TrendingSince))]
    public override partial void Map(Recipe source, TrendingRecipeDto destination);
}

// ── RecipeIngredient → RecipeIngredientDto ────────────────────────────────────

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RecipeIngredientToRecipeIngredientDtoMapper
    : MapperBase<RecipeIngredient, RecipeIngredientDto>
{
    public override partial RecipeIngredientDto Map(RecipeIngredient source);

    public override partial void Map(RecipeIngredient source, RecipeIngredientDto destination);

    public List<RecipeIngredientDto> MapList(ICollection<RecipeIngredient> source)
        => source.Select(Map).ToList();
}

// ── CreateUpdateRecipeDto → Recipe ────────────────────────────────────────────

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateRecipeDtoToRecipeMapper
    : MapperBase<CreateUpdateRecipeDto, Recipe>
{
    private readonly IGuidGenerator _guidGenerator;

    public CreateUpdateRecipeDtoToRecipeMapper(IGuidGenerator guidGenerator)
    {
        _guidGenerator = guidGenerator;
    }

    [ObjectFactory]
    private Recipe CreateRecipe()
         => new Recipe(_guidGenerator.Create(), string.Empty, string.Empty, default, 0, 0, 1, string.Empty, Guid.Empty);


    [MapperIgnoreTarget(nameof(Recipe.Id))]
    [MapperIgnoreTarget(nameof(Recipe.AuthorId))]
    [MapperIgnoreTarget(nameof(Recipe.Rating))]
    [MapperIgnoreTarget(nameof(Recipe.ReviewCount))]
    [MapperIgnoreTarget(nameof(Recipe.Ingredients))]
    [MapperIgnoreTarget(nameof(Recipe.InstructionsJson))]
    [MapperIgnoreTarget(nameof(Recipe.Tags))]
    [MapperIgnoreTarget(nameof(Recipe.CreationTime))]
    [MapperIgnoreTarget(nameof(Recipe.CreatorId))]
    [MapperIgnoreTarget(nameof(Recipe.LastModificationTime))]
    [MapperIgnoreTarget(nameof(Recipe.LastModifierId))]
    [MapperIgnoreTarget(nameof(Recipe.IsDeleted))]
    [MapperIgnoreTarget(nameof(Recipe.DeletionTime))]
    [MapperIgnoreTarget(nameof(Recipe.DeleterId))]
    [MapperIgnoreTarget(nameof(Recipe.ConcurrencyStamp))]
    public override partial Recipe Map(CreateUpdateRecipeDto source);

    [MapperIgnoreTarget(nameof(Recipe.Id))]
    [MapperIgnoreTarget(nameof(Recipe.AuthorId))]
    [MapperIgnoreTarget(nameof(Recipe.Rating))]
    [MapperIgnoreTarget(nameof(Recipe.ReviewCount))]
    [MapperIgnoreTarget(nameof(Recipe.Ingredients))]
    [MapperIgnoreTarget(nameof(Recipe.InstructionsJson))]
    [MapperIgnoreTarget(nameof(Recipe.Tags))]
    [MapperIgnoreTarget(nameof(Recipe.CreationTime))]
    [MapperIgnoreTarget(nameof(Recipe.CreatorId))]
    [MapperIgnoreTarget(nameof(Recipe.LastModificationTime))]
    [MapperIgnoreTarget(nameof(Recipe.LastModifierId))]
    [MapperIgnoreTarget(nameof(Recipe.IsDeleted))]
    [MapperIgnoreTarget(nameof(Recipe.DeletionTime))]
    [MapperIgnoreTarget(nameof(Recipe.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(Recipe.DeleterId))]
    public override partial void Map(CreateUpdateRecipeDto source, Recipe destination);
}

