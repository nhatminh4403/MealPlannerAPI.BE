using MealPlannerAPI.Recipes.Dtos;
using System.Collections.Generic;

namespace MealPlannerAPI.Dashboard
{
    public class DashboardStatsDto
    {
        public int ThisWeekMeals { get; set; }
        public int RecipesSaved { get; set; }
        public int TotalCookingMinutes { get; set; }
        public int TotalRecipes { get; set; }
        public int MealPlans { get; set; }
        public int RecipesShared { get; set; }
        public int ShoppingLists { get; set; }

    }

    public class TrendingRecipeDto
    {
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public double TrendingScore { get; set; }
        public string TrendingSince { get; set; } = null!;
    }

    public class DashboardDto
    {
        public DashboardStatsDto Stats { get; set; } = null!;
        public List<RecipeSummaryDto> RecentRecipes { get; set; } = new();
        public List<TrendingRecipeDto> TrendingRecipes { get; set; } = new();
    }

}
