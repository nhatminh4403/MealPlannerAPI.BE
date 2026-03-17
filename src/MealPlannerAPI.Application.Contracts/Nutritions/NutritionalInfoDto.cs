using System;
using System.Collections.Generic;
using System.Text;
namespace MealPlannerAPI.Nutritions
{
    public class NutritionalInfoDto
    {
        public float Calories { get; set; }
        public float ProteinGrams { get; set; }
        public float CarbsGrams { get; set; }
        public float FatGrams { get; set; }
        public float FiberGrams { get; set; }
        /// <summary>Total macro calories (for percentage calculations in UI).</summary>
        public float TotalMacroCalories =>
            (ProteinGrams * 4f) + (CarbsGrams * 4f) + (FatGrams * 9f);
        public float ProteinPercent =>
            TotalMacroCalories > 0 ? (ProteinGrams * 4f / TotalMacroCalories) * 100f : 0;
        public float CarbsPercent =>
            TotalMacroCalories > 0 ? (CarbsGrams * 4f / TotalMacroCalories) * 100f : 0;
        public float FatPercent =>
            TotalMacroCalories > 0 ? (FatGrams * 9f / TotalMacroCalories) * 100f : 0;
    }
}
