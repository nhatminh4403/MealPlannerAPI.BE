using System;
using Volo.Abp.Domain.Entities;

namespace MealPlannerAPI.Recipes
{
    public class RecipeIngredient : Entity<Guid>
    {
        public Guid RecipeId { get; set; }
        public string Name { get; set; } = null!;
        public float QuantityGrams { get; private set; }
        public string? DisplayQuantity { get; private set; }
        public Guid? IngredientNutritionId { get; private set; }

        protected RecipeIngredient() { }
        public RecipeIngredient(Guid id,
                                Guid recipeId,
                                string name,
                                float quantityGrams,
                                string? displayQuantity = null,
                                Guid? ingredientNutritionId = null) : base(id)
        {
            RecipeId = recipeId;
            Name = name;
            QuantityGrams = quantityGrams;
            DisplayQuantity = displayQuantity;
            IngredientNutritionId = ingredientNutritionId;
        }
        public void Update(string name,
                           float quantityGrams,
                           string? displayQuantity = null,
                           Guid? ingredientNutritionId = null)
        {
            Name = name;
            QuantityGrams = quantityGrams;
            DisplayQuantity = displayQuantity;
            IngredientNutritionId = ingredientNutritionId;
        }
    }
}
