using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace MealPlannerAPI.Nutritions
{
    public class IngredientNutrition : Entity<Guid>
    {
        /// <summary>Display name used in the seed and UI (e.g. "Chicken Breast").</summary>
        public string Name { get; private set; } = default!;

        /// <summary>Normalised lowercase name for lookups (e.g. "chicken breast").</summary>
        public string NormalizedName { get; private set; } = default!;

        // ── Nutrition per 100g ─────────────────────────────────────────────────

        public float CaloriesPer100g { get; private set; }
        public float ProteinPer100g { get; private set; }
        public float CarbsPer100g { get; private set; }
        public float FatPer100g { get; private set; }
        public float FiberPer100g { get; private set; }

        protected IngredientNutrition() { }

        public IngredientNutrition(
            Guid id,
            string name,
            float caloriesPer100g,
            float proteinPer100g,
            float carbsPer100g,
            float fatPer100g,
            float fiberPer100g) : base(id)
        {
            Name = name;
            NormalizedName = name.ToLowerInvariant();
            CaloriesPer100g = caloriesPer100g;
            ProteinPer100g = proteinPer100g;
            CarbsPer100g = carbsPer100g;
            FatPer100g = fatPer100g;
            FiberPer100g = fiberPer100g;
        }

        /// <summary>Expose as a value object for calculation.</summary>
        public NutritionalInfo ToNutritionalInfoPer100g() => new()
        {
            Calories = CaloriesPer100g,
            ProteinGrams = ProteinPer100g,
            CarbsGrams = CarbsPer100g,
            FatGrams = FatPer100g,
            FiberGrams = FiberPer100g,
        };
    }
}
