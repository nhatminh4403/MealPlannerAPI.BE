namespace MealPlannerAPI;

public static class MealPlannerAPIDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */
    public const string NoFollowersToRemove = "UserProfile:NoFollowersToRemove";
    public const string AlreadyNotFollowing = "UserProfile:AlreadyNotFollowing";
    public const string TooManyTags = "Recipe:TooManyTags";
    public const string TooManyIngredients = "Recipe:TooManyIngredients";
    public const string DuplicateIngredient = "Recipe:DuplicateIngredient";
}
