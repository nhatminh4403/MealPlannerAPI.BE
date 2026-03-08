using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace MealPlannerAPI.MealPlans
{
    public class MealPlan : FullAuditedAggregateRoot<Guid>
    {
        public Guid UserId { get; set; }
        public DateTime WeekStartDate { get; set; }
        public ICollection<MealPlanEntry> Entries { get; set; } = new List<MealPlanEntry>();

        protected MealPlan() { }

        public MealPlan(Guid id, Guid userId, DateTime weekStartDate) : base(id)
        {
            UserId = userId;
            WeekStartDate = weekStartDate;
        }
    }
}
