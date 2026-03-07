using MealPlannerAPI.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;

namespace MealPlannerAPI.Recipes
{
    public class Recipe : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; set; } = null!;

        public string Cuisine { get; set; } = null!;

        public DifficultyLevel Difficulty { get; set; }

        public int CookingTimeMinutes { get; set; }

        public int PrepTimeMinutes { get; set; }

        public int Servings { get; set; }

        public double Rating { get; set; }

        public int ReviewCount { get; set; }

        public string? ImageUrl { get; set; }

        public string Description { get; set; } = null!;

        /// <summary>Comma-separated tags, e.g. "pasta,italian,dinner"</summary>
        public string? Tags { get; set; }

        /// <summary>JSON-serialised list of step strings</summary>
        public string InstructionsJson { get; set; } = "[]";

        public Guid AuthorId { get; set; }

        public ICollection<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();

        protected Recipe() { }

        public Recipe(
            Guid id,
            string name,
            string cuisine,
            DifficultyLevel difficulty,
            int cookingTimeMinutes,
            int prepTimeMinutes,
            int servings,
            string description,
            Guid authorId)
            : base(id)
        {
            Name = name;
            Cuisine = cuisine;
            Difficulty = difficulty;
            CookingTimeMinutes = cookingTimeMinutes;
            PrepTimeMinutes = prepTimeMinutes;
            Servings = servings;
            Description = description;
            AuthorId = authorId;
        }
    }
}
