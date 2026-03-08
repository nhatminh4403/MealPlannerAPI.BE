using MealPlannerAPI.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MealPlannerAPI.MealPlans.Dtos
{
    public class CreateUpdateMealPlanEntryDto
    {
        [Required, MaxLength(16)]
        public string DayOfWeek { get; set; } = null!;

        [Required, MaxLength(256)]
        public string MealName { get; set; } = null!;

        public MealType MealType { get; set; }

        [RegularExpression(@"^\d{2}:\d{2}$", ErrorMessage = "ScheduledTime must be in HH:mm format.")]
        public string? ScheduledTime { get; set; }

        public Guid? RecipeId { get; set; }
    }
}
