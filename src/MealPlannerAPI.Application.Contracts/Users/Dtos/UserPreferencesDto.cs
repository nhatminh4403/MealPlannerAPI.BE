using System.Collections.Generic;

namespace MealPlannerAPI.Users.Dtos
{
    public class UserPreferencesDto
    {
        public List<string> DietaryRestrictions { get; set; } = new();
        public List<string> CuisinePreferences { get; set; } = new();
        public int DefaultServingSize { get; set; }
        public decimal WeeklyBudget { get; set; }
        public int MealPlanningDays { get; set; }
    }

}
