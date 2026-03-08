using MealPlannerAPI.Enums;
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace MealPlannerAPI.Notifications
{
    public class UserNotification : CreationAuditedEntity<Guid>
    {
        public Guid UserId { get; set; }

        public NotificationType Type { get; set; }

        public string Title { get; set; } = null!;

        public string Message { get; set; } = null!;

        public bool IsRead { get; set; }

        public string? AvatarUrl { get; set; }

        protected UserNotification() { }

        public UserNotification(
            Guid id,
            Guid userId,
            NotificationType type,
            string title,
            string message,
            string? avatarUrl = null)
            : base(id)
        {
            UserId = userId;
            Type = type;
            Title = title;
            Message = message;
            IsRead = false;
            AvatarUrl = avatarUrl;
        }
    }
}
