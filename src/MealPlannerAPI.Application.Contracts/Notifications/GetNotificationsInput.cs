using MealPlannerAPI.Enums;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.Notifications
{
    public class GetNotificationsInput : PagedAndSortedResultRequestDto
    {
        public bool? IsRead { get; set; }
        public NotificationType? Type { get; set; }
    }

}
