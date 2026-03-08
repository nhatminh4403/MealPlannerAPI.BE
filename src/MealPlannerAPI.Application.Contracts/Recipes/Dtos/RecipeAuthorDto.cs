using System;

namespace MealPlannerAPI.Recipes.Dtos
{
    public class RecipeAuthorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? AvatarUrl { get; set; }
    }
}
