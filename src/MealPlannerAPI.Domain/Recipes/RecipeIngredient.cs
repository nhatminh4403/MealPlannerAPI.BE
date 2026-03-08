using System;
using Volo.Abp.Domain.Entities;

namespace MealPlannerAPI.Recipes
{
    public class RecipeIngredient : Entity<Guid>
    {
        public Guid RecipeId { get; set; }
        public string Name { get; set; } = null!;
        public double Quantity { get; set; }
        public string Unit { get; set; } = null!;
        protected RecipeIngredient() { }
        public RecipeIngredient(Guid id, Guid recipeId, string name, double quantity, string unit)
            : base(id)
        {
            RecipeId = recipeId;
            Name = name;
            Quantity = quantity;
            Unit = unit;
        }
    }
}
