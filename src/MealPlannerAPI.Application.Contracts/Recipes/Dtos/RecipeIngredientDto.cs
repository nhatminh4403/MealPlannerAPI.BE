using MealPlannerAPI.Nutritions;
using System;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.Recipes.Dtos
{
    public class RecipeIngredientDto : EntityDto<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public float QuantityGrams { get; set; }
        public string? DisplayQuantity { get; set; }

        /// <summary>
        /// Nutritional contribution of this ingredient (already scaled to QuantityGrams).
        /// Null if no nutrition data is linked.
        /// </summary>
        public NutritionalInfoDto? Nutrition { get; set; }
    }
}