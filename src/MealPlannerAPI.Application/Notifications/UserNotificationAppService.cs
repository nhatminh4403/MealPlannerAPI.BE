using MealPlannerAPI.Enums;
using MealPlannerAPI.Hubs;
using MealPlannerAPI.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using static MealPlannerAPI.Permissions.MealPlannerAPIPermissions;

namespace MealPlannerAPI.Notifications
{
    [RemoteService(false)]
    public class UserNotificationAppService : MealPlannerAPIAppService, IUserNotificationAppService
    {
        private readonly IUserNotificationRepository _notificationRepository;
        private readonly UserNotificationToUserNotificationDtoMapper _toNotificationDtoMapper;
        private readonly IMealPlannerHubPublisher _hubPublisher;

        public UserNotificationAppService(IUserNotificationRepository notificationRepository, UserNotificationToUserNotificationDtoMapper toNotificationDtoMapper, IMealPlannerHubPublisher hubPublisher)
        {
            _notificationRepository = notificationRepository;
            _toNotificationDtoMapper = toNotificationDtoMapper;
            _hubPublisher = hubPublisher;
        }
        [Authorize(MealPlannerAPIPermissions.Notifications.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            var notification = await _notificationRepository.GetAsync(id);
            if (notification.UserId != CurrentUser.GetId())
                throw new AbpAuthorizationException("You are not authorized to delete this notification.");
            await _notificationRepository.DeleteAsync(notification, autoSave: true);
        }
        [Authorize(MealPlannerAPIPermissions.Notifications.Delete)]
        public async Task DeleteAllReadAsync()
        {
            var query = await _notificationRepository.GetQueryableAsync();
            var read = await AsyncExecuter.ToListAsync(
                query.Where(n => n.UserId == CurrentUser.GetId() && n.IsRead));

            await _notificationRepository.DeleteManyAsync(read, autoSave: true);
        }

        [Authorize(MealPlannerAPIPermissions.Notifications.Default)]
        public async Task<UserNotificationDto> GetAsync(Guid id)
        {
            var notification = await _notificationRepository.GetAsync(id);
            if (notification.UserId != CurrentUser.GetId())
                throw new AbpAuthorizationException("You are not authorized to access this notification.");
            return _toNotificationDtoMapper.Map(notification);
        }

        [Authorize(MealPlannerAPIPermissions.Notifications.Default)]
        public async Task<PagedResultDto<UserNotificationDto>> GetListAsync(GetNotificationsInput input)
        {
            var query = await _notificationRepository.GetQueryableAsync();

            query = query.Where(n => n.UserId == CurrentUser.GetId());

            if (input.IsRead.HasValue)
                query = query.Where(n => n.IsRead == input.IsRead.Value);

            if (input.Type.HasValue)
                query = query.Where(n => n.Type == input.Type.Value);

            var totalCount = await AsyncExecuter.CountAsync(query);

            var notifications = await AsyncExecuter.ToListAsync(
                query.OrderByDescending(n => n.CreationTime)
                     .Skip(input.SkipCount)
                     .Take(input.MaxResultCount));

            return new PagedResultDto<UserNotificationDto>(
                totalCount,
                _toNotificationDtoMapper.MapList(notifications));
        }

        [Authorize(MealPlannerAPIPermissions.Notifications.Default)]
        public async Task<int> GetUnreadCountAsync()
        {
            var query = await _notificationRepository.GetQueryableAsync();
            return await AsyncExecuter.CountAsync(
                query.Where(n => n.UserId == CurrentUser.GetId() && !n.IsRead));
        }
        private async Task<int> GetOrComputeUnreadCountAsync(Guid userId)
        {
            var query = await _notificationRepository.GetQueryableAsync();
            return await AsyncExecuter.CountAsync(
                query.Where(n => n.UserId == CurrentUser.GetId() && !n.IsRead));
        }

        [Authorize(MealPlannerAPIPermissions.Notifications.Default)]
        public async Task MarkAllAsReadAsync()
        {
            var query = await _notificationRepository.GetQueryableAsync();
            var unread = await AsyncExecuter.ToListAsync(
                query.Where(n => n.UserId == CurrentUser.GetId() && !n.IsRead));

            foreach (var notification in unread)
                notification.IsRead = true;

            await _notificationRepository.UpdateManyAsync(unread, autoSave: true);
            await _hubPublisher.NotifyUnreadCountChangedAsync(CurrentUser.GetId(), 0);
        }

        [Authorize(MealPlannerAPIPermissions.Notifications.Default)]
        public async Task MarkAsReadAsync(Guid id)
        {
            var notification = await _notificationRepository.GetAsync(id);
            if (notification.UserId != CurrentUser.GetId())
                throw new AbpAuthorizationException("You are not authorized to access this notification.");
            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification, autoSave: true);

            var newCount = await GetUnreadCountAsync();
            await _hubPublisher.NotifyUnreadCountChangedAsync(CurrentUser.GetId(), newCount);
        }
        [Authorize(MealPlannerAPIPermissions.Notifications.Default)]
        public async Task MarkAsUnreadAsync(Guid id)
        {
            var notification = await _notificationRepository.GetAsync(id);
            if (notification.UserId != CurrentUser.GetId())
                throw new AbpAuthorizationException("You are not authorized to access this notification.");

            notification.IsRead = false;
            await _notificationRepository.UpdateAsync(notification, autoSave: true);

            var count = await GetOrComputeUnreadCountAsync(notification.UserId);
            await _hubPublisher.NotifyUnreadCountChangedAsync(notification.UserId, count);
        }

        public async Task SendAsync(Guid userId, NotificationType type, string title, string message, string? avatarUrl = null, bool preventDuplicateToday = false)
        {
            var notification = new UserNotification(GuidGenerator.Create(),
                                                    userId,
                                                    type,
                                                    title,
                                                    message,
                                                    avatarUrl);

            await _notificationRepository.InsertAsync(notification, autoSave: true);

            var dto = _toNotificationDtoMapper.Map(notification);

            // Push to the user's SignalR group
            await _hubPublisher.NotifyNotificationReceivedAsync(userId, dto);

            // Also update their unread badge count
            var query = await _notificationRepository.GetQueryableAsync();
            var unreadCount = await AsyncExecuter.CountAsync(
                query.Where(n => n.UserId == userId && !n.IsRead));

            await _hubPublisher.NotifyUnreadCountChangedAsync(userId, unreadCount);
        }
    }
}
