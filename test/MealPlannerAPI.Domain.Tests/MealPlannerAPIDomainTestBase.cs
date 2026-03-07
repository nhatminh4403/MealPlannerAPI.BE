using Volo.Abp.Modularity;

namespace MealPlannerAPI;

/* Inherit from this class for your domain layer tests. */
public abstract class MealPlannerAPIDomainTestBase<TStartupModule> : MealPlannerAPITestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
