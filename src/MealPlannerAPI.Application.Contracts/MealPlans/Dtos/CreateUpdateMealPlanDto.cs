using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MealPlannerAPI.MealPlans.Dtos
{
    public class CreateUpdateMealPlanDto
    {
        [Required]
        public DateTime WeekStartDate { get; set; }

        public List<CreateUpdateMealPlanEntryDto> Entries { get; set; } = new();
    }

}
