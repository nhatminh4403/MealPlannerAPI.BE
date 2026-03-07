using Volo.Abp.Modularity;

namespace MealPlannerAPI;

public abstract class MealPlannerAPIApplicationTestBase<TStartupModule> : MealPlannerAPITestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
