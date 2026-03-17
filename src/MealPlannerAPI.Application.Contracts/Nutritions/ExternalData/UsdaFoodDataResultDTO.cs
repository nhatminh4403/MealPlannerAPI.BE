using System;
using System.Collections.Generic;
using System.Text;

namespace MealPlannerAPI.Nutritions.ExternalData
{
    public class UsdaFoodDataResultDTO
    {
        public string Name { get; init; } = default!;
        public string? BrandOwner { get; init; }
        public float CaloriesPer100g { get; init; }
        public float ProteinPer100g { get; init; }
        public float CarbsPer100g { get; init; }
        public float FatPer100g { get; init; }
        public float FiberPer100g { get; init; }

        /// <summary>
        /// 0–100 confidence score based on how many macro fields are present.
        /// Used in frontend completeness bar.
        /// </summary>
        public int CompletenessScore { get; init; }

        /// <summary>USDA fdcId — stored in SourceOffId for traceability.</summary>
        public string? FdcId { get; init; }
    }

}
