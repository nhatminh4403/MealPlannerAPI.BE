using MealPlannerAPI.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MealPlannerAPI.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MealPlannerAPIEntityFrameworkCoreModule),
    typeof(MealPlannerAPIApplicationContractsModule)
)]
public class MealPlannerAPIDbMigratorModule : AbpModule
{
}
