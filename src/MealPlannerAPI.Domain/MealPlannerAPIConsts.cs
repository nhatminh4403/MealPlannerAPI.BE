using Volo.Abp.Identity;

namespace MealPlannerAPI;

public static class MealPlannerAPIConsts
{
    public const string DbTablePrefix = "App";
    public const string? DbSchema = null;
    public const string AdminEmailDefaultValue = IdentityDataSeedContributor.AdminEmailDefaultValue;
    public const string AdminPasswordDefaultValue = "Admin.123";
}
