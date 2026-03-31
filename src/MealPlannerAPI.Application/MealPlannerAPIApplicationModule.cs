using MealPlannerAPI.BackgroundJobs;
using MealPlannerAPI.MealPlans.BackgroundJobs;
using MealPlannerAPI.Notifications;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace MealPlannerAPI;

[DependsOn(
    typeof(MealPlannerAPIDomainModule),
    typeof(MealPlannerAPIApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpAspNetCoreSignalRModule)
    //typeof(mealplannerapie)
    )]
public class MealPlannerAPIApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<IMealReminderTracker, MealReminderTracker>(); 
        context.Services.AddTransient<MealPlanHardDeleteWorker>();

    }

    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {

    }
}
