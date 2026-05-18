namespace MealPlannerAPI.Recipes;

public static class RecipeConsts
{
    public const int TrendingWindowDays = 14;
    public const int TrendingMaxResults = 10;
    public const int RecentRecipesMaxResults = 6;
    public const int TrendingRefreshIntervalMs = 60 * 60 * 1000 * 15 * 24;
    public const int MaxIngredients = 50;
    public const int MaxTags = 20;
    public static readonly string CacheKey = "trending-recipes";
}
