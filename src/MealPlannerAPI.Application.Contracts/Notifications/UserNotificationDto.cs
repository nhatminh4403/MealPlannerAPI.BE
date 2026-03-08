using MealPlannerAPI.Enums;
using System;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.Notifications;

public class UserNotificationDto : CreationAuditedEntityDto<Guid>
{
    public NotificationType Type { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public bool IsRead { get; set; }
    public string? AvatarUrl { get; set; }
}
