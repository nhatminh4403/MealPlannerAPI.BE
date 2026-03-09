using MealPlannerAPI.Notifications;
using Riok.Mapperly.Abstractions;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Mapperly;

namespace MealPlannerAPI;

/*
 * You can add your own mappings here.
 * [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
 * public partial class MealPlannerAPIApplicationMappers : MapperBase<BookDto, CreateUpdateBookDto>
 * {
 *    public override partial CreateUpdateBookDto Map(BookDto source);
 * 
 *    public override partial void Map(BookDto source, CreateUpdateBookDto destination);
 * }
 */
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserNotificationToUserNotificationDtoMapper
    : MapperBase<UserNotification, UserNotificationDto>
{
    public override partial UserNotificationDto Map(UserNotification source);

    public override partial void Map(UserNotification source, UserNotificationDto destination);

    public List<UserNotificationDto> MapList(ICollection<UserNotification> source)
        => source.Select(Map).ToList();
}