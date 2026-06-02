using MealPlannerAPI.Nutritions;
using System;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.Recipes.Dtos
{
    public class RecipeIngredientDto : EntityDto<Guid>
    {
        public string Name { get; set; } = default!;
        public float QuantityGrams { get; set; }
        public string? DisplayQuantity { get; set; }

        public NutritionalInfoDto? Nutrition { get; set; }
    }
}