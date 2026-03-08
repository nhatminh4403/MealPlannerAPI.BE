using System;
using Volo.Abp.Domain.Repositories;

namespace MealPlannerAPI.Notifications
{
    public interface IUserNotificationRepository : IRepository<UserNotification, Guid>
    {
    }
}
