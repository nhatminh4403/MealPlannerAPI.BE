using MealPlannerAPI.Notifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace MealPlannerAPI.Controllers.Notifications
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : AbpControllerBase
    {
        private readonly IUserNotificationAppService _notificationAppService;

        public NotificationController(IUserNotificationAppService notificationAppService)
        {
            _notificationAppService = notificationAppService;
        }

        /// <summary>Get a single notification by ID.</summary>
        [HttpGet("{id:guid}")]
        public Task<UserNotificationDto> GetAsync(Guid id)
            => _notificationAppService.GetAsync(id);

        /// <summary>Get the current user's notifications, optionally filtered by read status or type.</summary>
        [HttpGet]
        public Task<PagedResultDto<UserNotificationDto>> GetListAsync([FromQuery] GetNotificationsInput input)
            => _notificationAppService.GetListAsync(input);

        /// <summary>Get the count of unread notifications for the current user.</summary>
        [HttpGet("unread-count")]
        public Task<int> GetUnreadCountAsync()
            => _notificationAppService.GetUnreadCountAsync();

        /// <summary>Mark a single notification as read.</summary>
        [HttpPatch("{id:guid}/read")]
        public Task MarkAsReadAsync(Guid id)
            => _notificationAppService.MarkAsReadAsync(id);

        /// <summary>Mark all of the current user's notifications as read.</summary>
        [HttpPatch("read-all")]
        public Task MarkAllAsReadAsync()
            => _notificationAppService.MarkAllAsReadAsync();

        /// <summary>Delete a single notification.</summary>
        [HttpDelete("{id:guid}")]
        public Task DeleteAsync(Guid id)
            => _notificationAppService.DeleteAsync(id);

        /// <summary>Delete all read notifications for the current user.</summary>
        [HttpDelete("read")]
        public Task DeleteAllReadAsync()
            => _notificationAppService.DeleteAllReadAsync();
    }
}
