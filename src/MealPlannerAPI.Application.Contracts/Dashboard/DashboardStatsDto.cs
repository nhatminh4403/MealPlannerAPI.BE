using MealPlannerAPI.Recipes.Dtos;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

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
    public class DashboardDto
    {
        public DashboardStatsDto Stats { get; set; } = null!;
        public List<RecipeSummaryDto> RecentRecipes { get; set; } = new();
        public List<TrendingRecipeDto> TrendingRecipes { get; set; } = new();
    }

}
