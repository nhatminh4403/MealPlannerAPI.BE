using MealPlannerAPI.MealPlans.BackgroundJobs;
using MealPlannerAPI.Notifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace MealPlannerAPI.BackgroundJobs
{
    public class MealReminderJob : AsyncBackgroundJob<MealReminderJobArgs>, ITransientDependency
    {
        private readonly IUserNotificationAppService _notificationAppService;

        public MealReminderJob(IUserNotificationAppService notificationAppService)
        {
            _notificationAppService = notificationAppService;
        }

        public override async Task ExecuteAsync(MealReminderJobArgs args)
        {
            await NotificationFactory.MealReminder(
                _notificationAppService,
                args.UserId,
                args.MealName
            );
        }
    }
}
