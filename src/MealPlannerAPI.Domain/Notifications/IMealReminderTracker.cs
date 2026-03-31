using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MealPlannerAPI.Notifications
{    public interface IMealReminderTracker
    {
        Task<bool> HasSentTodayAsync(Guid userId, Guid entryId);
        Task MarkSentAsync(Guid userId, Guid entryId, DateTime date);
    }
}
