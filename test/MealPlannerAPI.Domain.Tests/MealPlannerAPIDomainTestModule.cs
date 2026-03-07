using Volo.Abp.Modularity;

namespace MealPlannerAPI;

[DependsOn(
    typeof(MealPlannerAPIDomainModule),
    typeof(MealPlannerAPITestBaseModule)
)]
public class MealPlannerAPIDomainTestModule : AbpModule
{

}
