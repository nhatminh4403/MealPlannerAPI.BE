using System;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.MealPlans.Dtos
{
    public class GetMealPlansInput : PagedAndSortedResultRequestDto
    {
        public DateTime? WeekStartDate { get; set; }
        public Guid UserId { get; set; }
    }
}
