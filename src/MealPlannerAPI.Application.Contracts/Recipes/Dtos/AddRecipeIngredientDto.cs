using System;
using System.Collections.Generic;
using System.Text;

namespace MealPlannerAPI.Recipes.Dtos
{
    public class AddRecipeIngredientDto
    {
        public string Name { get; set; }
        public float Quantity { get; set; }
        public string DisplayUnit { get; set; }
        public Guid? IngredientId { get; set; }
    }
}
