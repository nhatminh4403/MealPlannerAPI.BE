using MealPlannerAPI.Enums;
using System;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.MealPlans.Dtos
{
    public class MealPlanEntryDto : EntityDto<Guid>
    {
        public DayOfWeek DayOfWeek { get; set; }
        public string MealName { get; set; }
        public MealType MealType { get; set; }
        public string? ScheduledTime { get; set; }
        public Guid? RecipeId { get; set; }
        public string? RecipeName { get; set; }

    }
}
