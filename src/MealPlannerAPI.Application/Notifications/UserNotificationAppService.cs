using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace MealPlannerAPI.Notifications
{
    [RemoteService(false)]
    public class UserNotificationAppService : MealPlannerAPIAppService, IUserNotificationAppService
    {
        private readonly IRepository<UserNotification, Guid> _notificationRepository;
        private readonly UserNotificationToUserNotificationDtoMapper _toNotificationDtoMapper;

        public UserNotificationAppService(IRepository<UserNotification, Guid> notificationRepository, UserNotificationToUserNotificationDtoMapper toNotificationDtoMapper)
        {
            _notificationRepository = notificationRepository;
            _toNotificationDtoMapper = toNotificationDtoMapper;
        }
        public async Task DeleteAsync(Guid id)
        {
            var notification = await _notificationRepository.GetAsync(id);
            await _notificationRepository.DeleteAsync(notification, autoSave: true);
        }

        public async Task DeleteAllReadAsync()
        {
            var query = await _notificationRepository.GetQueryableAsync();
            var read = await AsyncExecuter.ToListAsync(
                query.Where(n => n.UserId == CurrentUser.GetId() && n.IsRead));

            await _notificationRepository.DeleteManyAsync(read, autoSave: true);
        }

        public async Task<UserNotificationDto> GetAsync(Guid id)
        {
            var notification = await _notificationRepository.GetAsync(id);
            if (notification.UserId != CurrentUser.GetId())
                throw new AbpAuthorizationException("You are not authorized to access this notification.");
            return _toNotificationDtoMapper.Map(notification);
        }

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

        public async Task<int> GetUnreadCountAsync()
        {
            var query = await _notificationRepository.GetQueryableAsync();
            return await AsyncExecuter.CountAsync(
                query.Where(n => n.UserId == CurrentUser.GetId() && !n.IsRead));
        }

        public async Task MarkAllAsReadAsync()
        {
            var query = await _notificationRepository.GetQueryableAsync();
            var unread = await AsyncExecuter.ToListAsync(
                query.Where(n => n.UserId == CurrentUser.GetId() && !n.IsRead));

            foreach (var notification in unread)
                notification.IsRead = true;

            await _notificationRepository.UpdateManyAsync(unread, autoSave: true);
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            var notification = await _notificationRepository.GetAsync(id);
            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification, autoSave: true);
        }
    }
}
