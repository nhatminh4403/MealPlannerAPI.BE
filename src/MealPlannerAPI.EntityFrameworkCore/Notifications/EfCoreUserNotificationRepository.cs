using MealPlannerAPI.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MealPlannerAPI.Notifications
{
    internal class EfCoreUserNotificationRepository : EfCoreRepository<MealPlannerAPIDbContext, UserNotification, Guid>, IUserNotificationRepository
    {
        public EfCoreUserNotificationRepository(IDbContextProvider<MealPlannerAPIDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
