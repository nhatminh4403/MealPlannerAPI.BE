using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MealPlannerAPI.Notifications
{
    public class MealReminderTracker : IMealReminderTracker, ITransientDependency
    {
        private readonly IDistributedCache _cache;

        public MealReminderTracker(IDistributedCache cache)
        {
            _cache = cache;
        }

        private static string Key(Guid userId, Guid entryId, DateTime date)
            => $"meal-reminder:{userId}:{entryId}:{date:yyyy-MM-dd}";

        public async Task<bool> HasSentTodayAsync(Guid userId, Guid entryId)
        {
            var value = await _cache.GetStringAsync(Key(userId, entryId, DateTime.UtcNow.Date));
            return value != null;
        }

        public async Task MarkSentAsync(Guid userId, Guid entryId, DateTime date)
        {
            await _cache.SetStringAsync(
                Key(userId, entryId, date),
                "1",
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(25)
                });
        }
    }
}
