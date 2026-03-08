using MealPlannerAPI.Enums;
using MealPlannerAPI.Recipes.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MealPlannerAPI.Recipes
{
    public class CreateUpdateRecipeDto
    {
        [Required, MaxLength(256)]
        public string Name { get; set; } = null!;

        [Required, MaxLength(128)]
        public string Cuisine { get; set; } = null!;

        public DifficultyLevel Difficulty { get; set; }

        [Range(0, 1440)]
        public int CookingTimeMinutes { get; set; }

        [Range(0, 1440)]
        public int PrepTimeMinutes { get; set; }

        [Range(1, 100)]
        public int Servings { get; set; } = 4;

        public string? ImageUrl { get; set; }

        [Required, MaxLength(2000)]
        public string Description { get; set; } = null!;

        public List<string> Tags { get; set; } = new();

        [MinLength(1)]
        public List<string> Instructions { get; set; } = new();

        [MinLength(1)]
        public List<CreateUpdateRecipeIngredientDto> Ingredients { get; set; } = new();
    }
}
