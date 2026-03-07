using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories;

namespace MealPlannerAPI.Notifications
{
    public interface IUserNotificationRepository : IRepository<UserNotification, Guid>
    {
    }
}
