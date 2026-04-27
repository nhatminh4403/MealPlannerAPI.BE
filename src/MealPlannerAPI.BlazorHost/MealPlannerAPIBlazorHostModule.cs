using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using MealPlannerAPI.BlazorHost.Components;
using MealPlannerAPI.BlazorHost.Components.Settings;
using MealPlannerAPI.BlazorHost.Menus;
using MealPlannerAPI.EntityFrameworkCore;
using MealPlannerAPI.Hubs;
using MealPlannerAPI.Localization;
using MealPlannerAPI.MultiTenancy;
using MealPlannerAPI.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using System;
using System.IO;
using Volo.Abp;
using Volo.Abp.Account.Blazor;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Components.Server.LeptonXLiteTheme;
using Volo.Abp.AspNetCore.Components.Server.LeptonXLiteTheme.Bundling;
using Volo.Abp.AspNetCore.Components.Web;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.FeatureManagement.Blazor.Server;
using Volo.Abp.Identity.Blazor;
using Volo.Abp.Identity.Blazor.Server;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement.Blazor;
using Volo.Abp.PermissionManagement.Blazor.Server;
using Volo.Abp.Security.Claims;
using Volo.Abp.SettingManagement.Blazor;
using Volo.Abp.SettingManagement.Blazor.Server;
using Volo.Abp.Studio.Client.AspNetCore;
using Volo.Abp.TenantManagement.Blazor;
using Volo.Abp.TenantManagement.Blazor.Server;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace MealPlannerAPI.BlazorHost;

[DependsOn(
    typeof(MealPlannerAPIHttpApiClientModule),
    typeof(MealPlannerAPIHttpApiModule),
    typeof(MealPlannerAPIEntityFrameworkCoreModule),
    typeof(MealPlannerAPIApplicationModule),
    typeof(MealPlannerAPISignalRModule),

    typeof(AbpTenantManagementBlazorServerModule),
    typeof(AbpTenantManagementBlazorModule),
    typeof(AbpSettingManagementBlazorServerModule),
    typeof(AbpSettingManagementBlazorModule),
    typeof(AbpPermissionManagementBlazorServerModule),
    typeof(AbpPermissionManagementBlazorModule),
    typeof(AbpIdentityBlazorServerModule),
    typeof(AbpIdentityBlazorModule),
    typeof(AbpFeatureManagementBlazorServerModule),
    typeof(AbpAccountBlazorModule),
    typeof(AbpStudioClientAspNetCoreModule),
    typeof(AbpAspNetCoreComponentsServerLeptonXLiteThemeModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMvcModule),
    //typeof(AbpAspNetCoreMvcUiBasicThemeModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    //typeof(AbpAspNetCoreComponentsServerBasicThemeModule),
    typeof(AbpAccountWebOpenIddictModule)
)]
public class MealPlannerAPIBlazorHostModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(MealPlannerAPIResource),
                typeof(MealPlannerAPIDomainModule).Assembly,
                typeof(MealPlannerAPIDomainSharedModule).Assembly,
                typeof(MealPlannerAPIApplicationModule).Assembly,
                typeof(MealPlannerAPIApplicationContractsModule).Assembly,
                typeof(MealPlannerAPIBlazorHostModule).Assembly
            );
        });

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("MealPlannerAPI");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });

        if (!hostingEnvironment.IsDevelopment())
        {
            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });

            PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
            {
                serverBuilder.AddProductionEncryptionAndSigningCertificate(
                    "openiddict.pfx",
                    configuration["AuthServer:CertificatePassPhrase"]!);
                serverBuilder.SetIssuer(new Uri(configuration["AuthServer:Authority"]!));
            });
        }

        PreConfigure<AbpAspNetCoreComponentsWebOptions>(options =>
        {
            options.IsBlazorWebApp = true;
        });
    }
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();
        var services = context.Services;
        context.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        if (!configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata"))
        {
            Configure<OpenIddictServerAspNetCoreOptions>(options =>
            {
                options.DisableTransportSecurityRequirement = true;
            });

            Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }
        ConfigureAuthentication(context);
        ConfigureUrls(configuration);
        ConfigureBundles();
        ConfigureVirtualFileSystem(hostingEnvironment, services);
        ConfigureBlazorise(context);
        ConfigureRouter();
        ConfigureMenu(context);
        ConfigureAutoApiControllers();
        ConfigureSettingPages();
        services.AddTransient<IMealPlannerHubPublisher, MealPlannerAPIPublisher>();

    }
    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(
                configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<string>());
        });
    }
    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(
            OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
    }
    private void ConfigureRouter()
    {
        Configure<AbpRouterOptions>(options =>
        {
            options.AppAssembly = typeof(MealPlannerAPIBlazorHostModule).Assembly;
        });
    }
    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment, IServiceCollection services)
    {
        //var hostingEnvironment = context.Services.GetHostingEnvironment();

        var hostEnv = services.GetHostingEnvironment();
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            if (hostEnv.IsDevelopment())
            {
                void ReplaceIfExists<TModule>(string path)
                {
                    var fullPath = Path.Combine(hostingEnvironment.ContentRootPath, path);
                    if (Directory.Exists(fullPath))
                    {
                        options.FileSets.ReplaceEmbeddedByPhysical<TModule>(fullPath);
                    }
                }


                ReplaceIfExists<MealPlannerAPIApplicationModule>($"..{Path.DirectorySeparatorChar}MealPlannerAPI.Application");
                ReplaceIfExists<MealPlannerAPIHttpApiModule>($"..{Path.DirectorySeparatorChar}MealPlannerAPI.HttpApi");
                ReplaceIfExists<MealPlannerAPIHttpApiClientModule>($"..{Path.DirectorySeparatorChar}MealPlannerAPI.HttpApi.Client");

                ReplaceIfExists<MealPlannerAPIHttpApiModule>($"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}MealPlannerAPI.HttpApi.Host");
                if (Directory.Exists(hostingEnvironment.ContentRootPath))
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<MealPlannerAPIBlazorHostModule>(hostingEnvironment.ContentRootPath);
                }
            }
        });
    }
    private void ConfigureBlazorise(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        context.Services
            .AddBlazorise(
            options =>
            {
                options.Debounce = true;
                options.DebounceInterval = 300;
                options.ProductToken = configuration["Blazorise:ProductToken"];
            }
            )
            //.AddBootstrapComponents()
            .AddBootstrap5Providers()
            .AddBootstrap5Components()
            //.AddBootstrapProviders()
            .AddFontAwesomeIcons();
    }
    private void ConfigureMenu(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new MealPlannerAPIMenuContributor());
        });
    }

    private void ConfigureAutoApiControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(
                typeof(MealPlannerAPIApplicationModule).Assembly);
        });
    }
    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            // MVC UI
            options.StyleBundles.Configure(
                LeptonXLiteThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );

            //BLAZOR UI
            options.StyleBundles.Configure(
                BlazorLeptonXLiteThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });
    }
    private void ConfigureSettingPages()
    {
        Configure<SettingManagementComponentOptions>(options =>
        {
            options.Contributors.Add(new MealPlannerSettingPageContributor());
        });
    }
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var env = context.GetEnvironment();
        var app = context.GetApplicationBuilder();

        app.UseForwardedHeaders();

        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
            app.UseHsts();
        }

        app.UseCorrelationId();
        app.UseRouting();
        app.MapAbpStaticAssets();
        app.UseAbpStudioLink();
        app.UseAbpSecurityHeaders();
        app.UseAntiforgery();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
            app.UseMultiTenancy();

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints(builder =>
        {
            builder.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddAdditionalAssemblies(
                    builder.ServiceProvider
                        .GetRequiredService<IOptions<AbpRouterOptions>>()
                        .Value.AdditionalAssemblies.ToArray());
        });
    }


}