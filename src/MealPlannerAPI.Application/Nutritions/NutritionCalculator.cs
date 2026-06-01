using MealPlannerAPI.Recipes;
using MealPlannerAPI.Recipes.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MealPlannerAPI.Nutritions
{
    public class NutritionCalculator : ITransientDependency
    {
        private readonly IIngredientNutritionRepository _nutritionRepo;

        public NutritionCalculator(IIngredientNutritionRepository nutritionRepo)
        {
            _nutritionRepo = nutritionRepo;
        }

        /// <summary>
        /// Enriches a list of RecipeDtos with per-ingredient and per-serving nutrition.
        /// Batches the nutrition lookup to avoid N+1 queries.
        /// </summary>
        public async Task EnrichAsync(
            IEnumerable<(Recipe Recipe, RecipeDto Dto)> pairs)
        {
            // Collect all unique nutrition IDs across all recipes
            var allNutritionIds = pairs
                .SelectMany(p => p.Recipe.Ingredients)
                .Where(i => i.IngredientNutritionId.HasValue)
                .Select(i => i.IngredientNutritionId!.Value)
                .Distinct()
                .ToList();
            var allNames = pairs
              .SelectMany(p => p.Recipe.Ingredients)
              .Select(i => i.Name.Trim().ToLowerInvariant())
              .Distinct()
              .ToList();
            var nutritionRecords = await _nutritionRepo.GetListAsync(n =>
                allNutritionIds.Contains(n.Id) || allNames.Contains(n.NormalizedName));

            var idLookup = nutritionRecords.ToDictionary(n => n.Id);
            var nameLookup = nutritionRecords
                .GroupBy(n => n.NormalizedName)
                .ToDictionary(g => g.Key, g => g.First());


            foreach (var (recipe, dto) in pairs)
            {
                EnrichRecipe(recipe, dto, idLookup, nameLookup);
            }
        }

        /// <summary>Enrich a single recipe — use for GetById scenarios.</summary>
        public async Task EnrichAsync(Recipe recipe, RecipeDto dto)
        {
            var ids = recipe.Ingredients
                .Where(i => i.IngredientNutritionId.HasValue)
                .Select(i => i.IngredientNutritionId!.Value)
                .Distinct()
                .ToList();

            var names = recipe.Ingredients
                .Select(i => i.Name.Trim().ToLowerInvariant())
                .Distinct()
                .ToList();

            // Single query matching either ID or NormalizedName
            var nutritionRecords = await _nutritionRepo.GetListAsync(n =>
                ids.Contains(n.Id) || names.Contains(n.NormalizedName));

            var idLookup = nutritionRecords.ToDictionary(n => n.Id);
            var nameLookup = nutritionRecords
                .GroupBy(n => n.NormalizedName)
                .ToDictionary(g => g.Key, g => g.First());

            EnrichRecipe(recipe, dto, idLookup, nameLookup);
        }

        // ── Private ───────────────────────────────────────────────────────────────

        private static void EnrichRecipe(
            Recipe recipe,
            RecipeDto dto,
            Dictionary<Guid, IngredientNutrition> lookup, Dictionary<string, IngredientNutrition> nameLookup)
        {
            var ingredientNutritions = new List<NutritionalInfo>();

            foreach (var ingredientDto in dto.Ingredients)
            {
                var domainIngredient = recipe.Ingredients
                    .FirstOrDefault(i => i.Id == ingredientDto.Id);

                if (domainIngredient?.IngredientNutritionId == null) continue;
                if (!lookup.TryGetValue(domainIngredient.IngredientNutritionId.Value, out var n)) continue;

                var scaled = NutritionalInfo.Calculate(
                    n.ToNutritionalInfoPer100g(),
                    domainIngredient.QuantityGrams);

                ingredientDto.Nutrition = MapToDto(scaled);
                ingredientNutritions.Add(scaled);
            }

            if (ingredientNutritions.Count == 0) return;

            var total = NutritionalInfo.Sum(ingredientNutritions);
            var perServing = total.DivideByServings(recipe.Servings > 0 ? recipe.Servings : 1);

            dto.NutritionPerServing = MapToDto(perServing);
        }

        private static NutritionalInfoDto MapToDto(NutritionalInfo n) => new()
        {
            Calories = MathF.Round(n.Calories, 1),
            ProteinGrams = MathF.Round(n.ProteinGrams, 1),
            CarbsGrams = MathF.Round(n.CarbsGrams, 1),
            FatGrams = MathF.Round(n.FatGrams, 1),
            FiberGrams = MathF.Round(n.FiberGrams, 1),
        };
    }
}
