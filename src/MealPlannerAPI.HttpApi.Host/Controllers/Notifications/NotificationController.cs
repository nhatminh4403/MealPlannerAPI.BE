using MealPlannerAPI.Models;
using MealPlannerAPI.Notifications;
using MealPlannerAPI.Routes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Domain.Entities;

namespace MealPlannerAPI.Controllers.Notifications
{
    [Route(APIRoute.APIApp + "[controller]")]
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
        [ProducesResponseType(typeof(ApiResponse<UserNotificationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            try
            {
                var result = await _notificationAppService.GetAsync(id);
                return Ok(new ApiResponse<UserNotificationDto>(true, "Success", result));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Notification not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Get the current user's notifications, optionally filtered by read status or type.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<UserNotificationDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetListAsync([FromQuery] GetNotificationsInput input)
        {
            try
            {
                var result = await _notificationAppService.GetListAsync(input);
                return Ok(new ApiResponse<PagedResultDto<UserNotificationDto>>(true, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Mark a single notification as read.</summary>
        [HttpPatch("{id:guid}/read")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MarkAsReadAsync(Guid id)
        {
            try
            {
                await _notificationAppService.MarkAsReadAsync(id);
                return Ok(new ApiResponse(true, "Notification marked as read"));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Notification not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Mark a single notification as unread.</summary>
        [HttpPatch("{id:guid}/unread")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MarkAsUnreadAsync(Guid id)
        {
            try
            {
                await _notificationAppService.MarkAsUnreadAsync(id);
                return Ok(new ApiResponse(true, "Notification marked as unread"));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Notification not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Mark all of the current user's notifications as read.</summary>
        [HttpPatch("read-all")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MarkAllAsReadAsync()
        {
            try
            {
                await _notificationAppService.MarkAllAsReadAsync();
                return Ok(new ApiResponse(true, "All notifications marked as read"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Delete a single notification.</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                await _notificationAppService.DeleteAsync(id);
                return Ok(new ApiResponse(true, "Notification deleted successfully"));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Notification not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Delete all read notifications for the current user.</summary>
        [HttpDelete("read")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAllReadAsync()
        {
            try
            {
                await _notificationAppService.DeleteAllReadAsync();
                return Ok(new ApiResponse(true, "All read notifications deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }
    }
}
