using System;
using System.Collections.Generic;
using System.Text;

namespace MealPlannerAPI.MealPlans.BackgroundJobs
{
    public class MealReminderJobArgs
    {
        public Guid UserId { get; set; }
        public string MealName { get; set; } = null!;
        public DateTime ScheduledFor { get; set; }
    }
}
