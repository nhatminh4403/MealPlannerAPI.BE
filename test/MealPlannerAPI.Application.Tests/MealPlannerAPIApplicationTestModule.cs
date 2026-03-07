using Volo.Abp.Modularity;

namespace MealPlannerAPI;

[DependsOn(
    typeof(MealPlannerAPIApplicationModule),
    typeof(MealPlannerAPIDomainTestModule)
)]
public class MealPlannerAPIApplicationTestModule : AbpModule
{

}
