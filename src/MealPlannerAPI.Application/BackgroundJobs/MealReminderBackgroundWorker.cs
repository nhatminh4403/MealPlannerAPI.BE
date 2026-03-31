using MealPlannerAPI.MealPlans;
using MealPlannerAPI.MealPlans.BackgroundJobs;
using MealPlannerAPI.Notifications;
using MealPlannerAPI.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Identity;
using Volo.Abp.Linq;
using Volo.Abp.Threading;
using Volo.Abp.Uow;
using Microsoft.EntityFrameworkCore;
namespace MealPlannerAPI.BackgroundJobs
{
    public class MealReminderBackgroundWorker : AsyncPeriodicBackgroundWorkerBase
    {
        private readonly IUserNotificationAppService _notificationAppService;
        public MealReminderBackgroundWorker(AbpAsyncTimer timer,
                                            IServiceScopeFactory serviceScopeFactory,
                                            IUserNotificationAppService notificationAppService) : base(timer, serviceScopeFactory)
        {
            _notificationAppService = notificationAppService;
            Timer.Period = 1000 * 60 * 60*24*7;

        }
        [UnitOfWork]
        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            Logger.LogInformation("[MealReminderWorker] Starting scan at {Time}", DateTime.UtcNow);

            var mealPlanRepository = workerContext.ServiceProvider
                .GetRequiredService<IMealPlanRepository>();

            var backgroundJobManager = workerContext.ServiceProvider
                .GetRequiredService<IBackgroundJobManager>();

            var reminderTracker = workerContext.ServiceProvider
                .GetRequiredService<IMealReminderTracker>();

            var today = DateTime.UtcNow.Date;
            var dayOfWeek = today.DayOfWeek;

            // Get the current week start for all active meal plans
            var weekStart = MealPlan.GetWeekStart(DateTime.UtcNow);

            var query = await mealPlanRepository.GetQueryableAsync();

            var plansWithEntriesToday = await query
                .Where(mp => mp.WeekStartDate == weekStart)
                .Where(mp => mp.Entries.Any(e =>
                    e.DayOfWeek == dayOfWeek &&
                    e.RecipeId.HasValue))
                .ToListAsync();

            foreach (var plan in plansWithEntriesToday)
            {
                var todayEntries = plan.Entries
                    .Where(e => e.DayOfWeek == dayOfWeek && e.RecipeId.HasValue)
                    .ToList();

                foreach (var entry in todayEntries)
                {
                    // Skip if we already sent this reminder today (idempotency)
                    var alreadySent = await reminderTracker.HasSentTodayAsync(plan.UserId, entry.Id);
                    if (alreadySent) continue;

                    await backgroundJobManager.EnqueueAsync(new MealReminderJobArgs
                    {
                        UserId = plan.UserId,
                        MealName = entry.MealName ?? entry.MealType.ToString(),
                        ScheduledFor = today
                    });

                    await reminderTracker.MarkSentAsync(plan.UserId, entry.Id, today);
                }
            }

            Logger.LogInformation("[MealReminderWorker] Scan complete. Processed {Count} plans.", plansWithEntriesToday.Count);

        }
    }
    
}
