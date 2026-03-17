using MealPlannerAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace MealPlannerAPI.Recipes;

public class Recipe : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; } = null!;
    public string Cuisine { get; set; } = null!;
    public DifficultyLevel Difficulty { get; set; }
    public int CookingTimeMinutes { get; set; }
    public int PrepTimeMinutes { get; set; }
    public int Servings { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public string? ImageUrl { get; set; }
    public string Description { get; set; } = null!;
    /// <summary>Comma-separated tags, e.g. "pasta,italian,dinner"</summary>
    public string? Tags { get; set; }
    /// <summary>JSON-serialised list of step strings</summary>
    public string InstructionsJson { get; set; } = "[]";
    public Guid AuthorId { get; set; }
    public ICollection<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();
    protected Recipe() { }
    public Recipe(
        Guid id,
        string name,
        string cuisine,
        DifficultyLevel difficulty,
        int cookingTimeMinutes,
        int prepTimeMinutes,
        int servings,
        string description,
        Guid authorId)
        : base(id)
    {
        Name = name;
        Cuisine = cuisine;
        Difficulty = difficulty;
        CookingTimeMinutes = cookingTimeMinutes;
        PrepTimeMinutes = prepTimeMinutes;
        Servings = servings;
        Description = description;
        AuthorId = authorId;
    }
    public List<string> GetTags()
    {
        return string.IsNullOrWhiteSpace(Tags) ? new List<string>() : [.. Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)];
    }
    public void SetTags(IEnumerable<string> tags)
    {
        var list = new List<string>(tags);
        if (list.Count > RecipeConsts.MaxTags)
            throw new BusinessException(MealPlannerAPIDomainErrorCodes.TooManyTags).WithData("max", RecipeConsts.MaxTags);
        Tags = list.Count > 0 ? string.Join(',', list) : null;
    }
    public List<string> GetInstructions()
    {
        return JsonSerializer.Deserialize<List<string>>(InstructionsJson) ?? new List<string>();
    }
    public void SetInstructions(IEnumerable<string> steps)
    {
        InstructionsJson = JsonSerializer.Serialize(new List<string>(steps));
    }
    public RecipeIngredient AddIngredient(Guid id, string name, decimal quantity, string unit)
    {
        if (Ingredients.Any(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new BusinessException(MealPlannerAPIDomainErrorCodes.DuplicateIngredient);
        if (Ingredients.Count >= RecipeConsts.MaxIngredients)
            throw new BusinessException(MealPlannerAPIDomainErrorCodes.TooManyIngredients)
                                        .WithData("max", RecipeConsts.MaxIngredients);
        var ingredient = new RecipeIngredient(id, Id, name, (float)quantity, unit);
        Ingredients.Add(ingredient);
        return ingredient;
    }
    public void ReplaceIngredients(IEnumerable<(Guid Id, string Name, decimal Quantity, string Unit)> ingredients)
    {
        Ingredients.Clear();
        foreach (var (id, name, quantity, unit) in ingredients)
        {
            AddIngredient(id, name, quantity, unit);
        }
    }
    public int GetTotalTimeMinutes() => CookingTimeMinutes + PrepTimeMinutes;
    public double CalculateTrendingScore()
        => Math.Round(Rating * (1 + ReviewCount * 0.001), 1);

    public static Recipe CreateSeed(Guid id,
                                    string name,
                                    string description,
                                    int servings,
                                    int prepMinutes,
                                    int cookMinutes,
                                    IEnumerable<(string Name, float Grams, string Display, Guid? NutritionId)> ingredients)
    {
        var recipe = new Recipe
        {
            Id = id,
            Name = name,
            Description = description,
            Servings = servings,
            PrepTimeMinutes = prepMinutes,
            CookingTimeMinutes = cookMinutes,
        };

        foreach (var (iName, grams, display, nutritionId) in ingredients)
        {
            recipe.Ingredients.Add(new RecipeIngredient(
               id: Guid.NewGuid(),
               recipeId: id,
               name: iName,
               quantityGrams: grams,
               displayQuantity: display,
               ingredientNutritionId: nutritionId));
        }

        return recipe;
    }
}
