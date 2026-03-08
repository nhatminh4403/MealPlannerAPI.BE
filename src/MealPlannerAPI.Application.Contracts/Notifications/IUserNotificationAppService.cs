using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MealPlannerAPI.Notifications
{
    public interface IUserNotificationAppService : IApplicationService
    {
        /// <summary>Get a paged, filtered list of notifications for the current user.</summary>
        Task<PagedResultDto<UserNotificationDto>> GetListAsync(GetNotificationsInput input);

        /// <summary>Get a single notification by id.</summary>
        Task<UserNotificationDto> GetAsync(Guid id);

        /// <summary>Get the count of unread notifications for the current user.</summary>
        Task<int> GetUnreadCountAsync();

        /// <summary>Mark a single notification as read.</summary>
        Task MarkAsReadAsync(Guid id);

        /// <summary>Mark all notifications as read for the current user.</summary>
        Task MarkAllAsReadAsync();

        /// <summary>Delete a single notification.</summary>
        Task DeleteAsync(Guid id);

        /// <summary>Delete all read notifications for the current user.</summary>
        Task DeleteAllReadAsync();
    }
}
