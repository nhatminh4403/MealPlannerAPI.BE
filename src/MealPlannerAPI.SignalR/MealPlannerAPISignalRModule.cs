using MealPlannerAPI;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Modularity;

namespace MealPlannerAPI.SignalR;

[DependsOn(
    typeof(MealPlannerAPIApplicationContractsModule),
    typeof(AbpAspNetCoreSignalRModule)
)]
public class MealPlannerAPISignalRModule : AbpModule
{

}