using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MealPlannerAPI.MealPlans.Dtos
{
    /// <summary>
    /// Input DTO for auto-generating a meal plan based on user preferences.
    /// All fields are optional — when omitted the system pulls defaults
    /// from the current user's <c>UserProfile</c> preferences.
    /// </summary>
    public class AutoGenerateMealPlanDto
    {
        /// <summary>The Monday of the target week. Defaults to the current week.</summary>
        public DateTime? WeekStartDate { get; set; }

        /// <summary>
        /// Preferred cuisines to prioritise (e.g. "Italian", "Thai").
        /// When empty, the user's saved cuisine preferences are used.
        /// </summary>
        public List<string>? CuisinePreferences { get; set; }

        /// <summary>
        /// Dietary restrictions to honour (e.g. "vegetarian", "gluten-free").
        /// When empty, the user's saved dietary restrictions are used.
        /// </summary>
        public List<string>? DietaryRestrictions { get; set; }

        /// <summary>
        /// Maximum total cooking + prep time per recipe (minutes).
        /// Null = no limit.
        /// </summary>
        [Range(1, 480)]
        public int? MaxTotalTimeMinutes { get; set; }

        /// <summary>
        /// Maximum difficulty level to include (0 = Easy, 1 = Medium, 2 = Hard).
        /// Null = all difficulties.
        /// </summary>
        [Range(0, 2)]
        public int? MaxDifficulty { get; set; }

        /// <summary>
        /// Meal types to generate for each day.
        /// Defaults to Breakfast, Lunch, Dinner when empty.
        /// </summary>
        public List<int>? MealTypes { get; set; }
    }
}
