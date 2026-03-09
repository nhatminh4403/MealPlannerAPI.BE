using MealPlannerAPI.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MealPlannerAPI.MealPlans.Dtos
{
    public class CreateUpdateMealPlanEntryDto
    {
        public DayOfWeek DayOfWeek { get; set; }

        [Required, MaxLength(256)]
        public string MealName { get; set; } = null!;

        public MealType MealType { get; set; }

        [RegularExpression(@"^\d{2}:\d{2}$", ErrorMessage = "ScheduledTime must be in HH:mm format.")]
        public string? ScheduledTime { get; set; }

        public Guid? RecipeId { get; set; }
    }
}
