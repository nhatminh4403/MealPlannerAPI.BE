using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MealPlannerAPI.Users.Dtos
{
    public class CreateUpdateUserPreferencesDto
    {
        public List<string> DietaryRestrictions { get; set; } = new();
        public List<string> CuisinePreferences { get; set; } = new();

        [Range(1, 50)]
        public int DefaultServingSize { get; set; } = 4;

        [Range(0, 100000)]
        public decimal WeeklyBudget { get; set; }

        [Range(1, 14)]
        public int MealPlanningDays { get; set; } = 7;
    }

}
