namespace MealPlannerAPI.Settings
{
    public class HardDeleteOptions
    {
        public int RetentionDays { get; set; } = 30;
        public int PeriodHours { get; set; } = 24;

    }
}
