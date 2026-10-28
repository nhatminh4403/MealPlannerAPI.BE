using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace MealPlannerAPI.Users.Dtos;

public class GetFollowListInput : PagedResultRequestDto
{
    public Guid UserId { get; set; }
}
public class FollowUserDto : CommunityUserDto
{
    public DateTime FollowedAt { get; set; }
}