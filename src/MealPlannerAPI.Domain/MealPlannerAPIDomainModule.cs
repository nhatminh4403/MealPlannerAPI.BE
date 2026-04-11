//using MealPlannerAPI.MealPlans.BackgroundJobs;
using MealPlannerAPI.MultiTenancy;
using MealPlannerAPI.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.Caching;
using Volo.Abp.Emailing;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.OpenIddict;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.TenantManagement;

namespace MealPlannerAPI;

[DependsOn(
    typeof(MealPlannerAPIDomainSharedModule),
    typeof(AbpAuditLoggingDomainModule),
    typeof(AbpCachingModule),
    typeof(AbpBackgroundJobsDomainModule),
    typeof(AbpFeatureManagementDomainModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpPermissionManagementDomainOpenIddictModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(AbpEmailingModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpOpenIddictDomainModule),
    typeof(AbpTenantManagementDomainModule),
    typeof(AbpBackgroundWorkersModule),
    typeof(BlobStoringDatabaseDomainModule)
    )]
public class MealPlannerAPIDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = !MultiTenancyConsts.IsEnabled;
        });

        Configure<AbpSettingOptions>(options =>
        {
            options.DefinitionProviders.Add<MealPlannerAPISettingDefinitionProvider>();
        });

        var services = context.Services;

#if DEBUG
        context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
#endif

    }
}
