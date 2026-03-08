using System;
using System.Collections.Generic;

namespace MealPlannerAPI.MealPlans.Dtos
{
    public class MealPlanDayDto
    {
        public DateTime DayOfWeek { get; set; }
        public List<MealPlanEntryDto> Meals { get; set; } = new List<MealPlanEntryDto>();
    }
}
