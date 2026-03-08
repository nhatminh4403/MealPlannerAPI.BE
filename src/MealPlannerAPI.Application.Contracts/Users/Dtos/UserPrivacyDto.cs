using MealPlannerAPI.Enums;

namespace MealPlannerAPI.Users.Dtos
{
    public class UserPrivacyDto
    {
        public VisibilityLevel ProfileVisibility { get; set; }
        public VisibilityLevel RecipesVisibility { get; set; }
        public VisibilityLevel ShoppingListVisibility { get; set; }
    }

}
