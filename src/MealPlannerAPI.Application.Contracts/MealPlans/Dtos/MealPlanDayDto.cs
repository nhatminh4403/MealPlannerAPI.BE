using System;
using System.Collections.Generic;

namespace MealPlannerAPI.MealPlans.Dtos
{
    public class MealPlanDayDto
    {
        public DayOfWeek DayOfWeek { get; set; }
        public List<MealPlanEntryDto> Meals { get; set; } = new List<MealPlanEntryDto>();
    }
}
