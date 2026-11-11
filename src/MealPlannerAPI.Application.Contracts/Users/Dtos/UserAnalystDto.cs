using MealPlannerAPI.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MealPlannerAPI.Users.Dtos;

public class UserAnalystDto
{
    public long TotalUserCount { get; set; }
     
}
public class GetFollowStatsInput
{
    public StatGranularity Granularity { get; set; } = StatGranularity.Monthly;
    public int? Year { get; set; }          
    public int Years { get; set; } = 5;     
}

public class FollowStatPointDto
{
    public DateTime PeriodStart { get; set; }   // first day of the month/year (UTC)
    public int FollowersGained { get; set; }
    public int FollowersLost { get; set; }
    public int FollowingGained { get; set; }
    public int FollowingLost { get; set; }
    public int FollowersTotal { get; set; }     // total at the END of the period
    public int FollowingTotal { get; set; }
}