using MealPlannerAPI.Dashboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace MealPlannerAPI.Recipes.Caching
{
    [Serializable]
    public class TrendingRecipeCacheItem
    {
        public List<TrendingRecipeDto> Items { get; set; } = new();
    }
}
