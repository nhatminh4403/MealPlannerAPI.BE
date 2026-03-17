using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MealPlannerAPI.Nutritions
{
    public sealed class NutritionalInfo
    {
        public float Calories { get; init; }
        public float ProteinGrams { get; init; }
        public float CarbsGrams { get; init; }
        public float FatGrams { get; init; }
        public float FiberGrams { get; init; }

        public static readonly NutritionalInfo Zero = new();

        /// <summary>
        /// Calculate nutrition for a given ingredient amount.
        /// </summary>
        /// <param name="per100g">Nutrition values per 100g (from IngredientNutrition lookup).</param>
        /// <param name="quantityGrams">Actual quantity used in the recipe.</param>
        public static NutritionalInfo Calculate(NutritionalInfo per100g, float quantityGrams)
        {
            var ratio = quantityGrams / 100f;
            return new NutritionalInfo
            {
                Calories = per100g.Calories * ratio,
                ProteinGrams = per100g.ProteinGrams * ratio,
                CarbsGrams = per100g.CarbsGrams * ratio,
                FatGrams = per100g.FatGrams * ratio,
                FiberGrams = per100g.FiberGrams * ratio,
            };
        }

        /// <summary>Sum multiple nutritional values together.</summary>
        public static NutritionalInfo Sum(IEnumerable<NutritionalInfo> values)
            => values.Aggregate(Zero, Add);

        /// <summary>Divide total recipe nutrition by number of servings.</summary>
        public NutritionalInfo DivideByServings(int servings)
        {
            if (servings <= 0) return this;
            return new NutritionalInfo
            {
                Calories = Calories / servings,
                ProteinGrams = ProteinGrams / servings,
                CarbsGrams = CarbsGrams / servings,
                FatGrams = FatGrams / servings,
                FiberGrams = FiberGrams / servings,
            };
        }

        private static NutritionalInfo Add(NutritionalInfo a, NutritionalInfo b) => new()
        {
            Calories = a.Calories + b.Calories,
            ProteinGrams = a.ProteinGrams + b.ProteinGrams,
            CarbsGrams = a.CarbsGrams + b.CarbsGrams,
            FatGrams = a.FatGrams + b.FatGrams,
            FiberGrams = a.FiberGrams + b.FiberGrams,
        };
    }
}
