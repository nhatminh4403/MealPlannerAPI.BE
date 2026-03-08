using MealPlannerAPI.Enums;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.Recipes.Dtos
{
    public class RecipeSummaryDto : EntityDto<Guid>
    {
        public string Name { get; set; } = null!;
        public string Cuisine { get; set; } = null!;
        public DifficultyLevel Difficulty { get; set; }
        public int TotalTimeMinutes { get; set; }
        public int Servings { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public string? ImageUrl { get; set; }
        public string Description { get; set; } = null!;
        public List<string> Tags { get; set; } = new();
    }
}
