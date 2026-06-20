using MealPlannerAPI.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MealPlannerAPI.DataSeeder.Helpers;

/// <summary>
/// Pure data record describing a seed recipe.
/// Add new recipes by adding entries to <see cref="RecipeBuilders.All"/> —
/// no other file needs to change.
/// </summary>
public sealed record RecipeDefinition(
    string Name,
    string Cuisine,
    string Description,
    int Servings,
    int PrepMinutes,
    int CookMinutes,
    DifficultyLevel Difficulty,
    IReadOnlyList<IngredientDef> Ingredients,
    IReadOnlyList<string> Instructions
);

/// <summary>
/// Ingredient line inside a <see cref="RecipeDefinition"/>.
/// <paramref name="IngredientName"/> must match a key in the nutrition lookup.
/// </summary>
public sealed record IngredientDef(
    string IngredientName,
    float Grams,
    string Display
);
