using System;
using System.Collections.Generic;
using System.Text;

namespace MealPlannerAPI.Nutritions.Dtos;

public class IngredientNutritionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public float CaloriesPer100g { get; set; }
    public float ProteinPer100g { get; set; }
    public float CarbsPer100g { get; set; }
    public float FatPer100g { get; set; }
    public float FiberPer100g { get; set; }

    /// <summary>True = came from our DB. False = OFF candidate not yet saved.</summary>
    public bool IsVerified { get; set; }
}
public class IngredientNutritionSearchResultDto
{
    /// <summary>Matched entries already in our database.</summary>
    public List<IngredientNutritionDto> DbResults { get; set; } = [];

    /// <summary>
    /// Candidates from OpenFoodFacts — not yet persisted.
    /// Frontend shows these only when DB results are empty or user clicks "Search more".
    /// </summary>
    public List<OpenFoodFactsCandidateDto> OffCandidates { get; set; } = [];
}

public class OpenFoodFactsCandidateDto
{
    public string Name { get; set; } = default!;
    public string? Brand { get; set; }
    public float CaloriesPer100g { get; set; }
    public float ProteinPer100g { get; set; }
    public float CarbsPer100g { get; set; }
    public float FatPer100g { get; set; }
    public float FiberPer100g { get; set; }
    public int CompletenessScore { get; set; }
    public string? OffId { get; set; }
}

public class CreateIngredientNutritionDto
{
    public string Name { get; set; } = default!;
    public float CaloriesPer100g { get; set; }
    public float ProteinPer100g { get; set; }
    public float CarbsPer100g { get; set; }
    public float FatPer100g { get; set; }
    public float FiberPer100g { get; set; }

    /// <summary>If saving from OFF result — for traceability.</summary>
    public string? SourceOffId { get; set; }
}