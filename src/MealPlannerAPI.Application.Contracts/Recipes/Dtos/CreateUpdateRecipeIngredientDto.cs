using System;
using System.ComponentModel.DataAnnotations;

namespace MealPlannerAPI.Recipes.Dtos
{
    public class CreateUpdateRecipeIngredientDto
    {
        [Required, MaxLength(128)]
        public string Name { get; set; } = null!;

        [Range(0.001, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Required, MaxLength(32)]
        public string Unit { get; set; } = null!; 
        public Guid? NutritionId { get; set; }
    }
}
