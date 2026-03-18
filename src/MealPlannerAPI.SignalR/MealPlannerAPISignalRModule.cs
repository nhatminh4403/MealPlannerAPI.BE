using MealPlannerAPI;
using Volo.Abp.Modularity;

namespace MealPlannerAPI.SignalR;

[DependsOn(
    typeof(MealPlannerAPIApplicationContractsModule)
)]
public class MealPlannerAPISignalRModule : AbpModule
{

}