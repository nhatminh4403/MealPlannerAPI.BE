using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.MealPlans.Dtos
{
    public class MealPlanDto : FullAuditedEntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public DateTime WeekStartDate { get; set; }
        public List<MealPlanEntryDto> Entries { get; set; } = new List<MealPlanEntryDto>();
    }
}
