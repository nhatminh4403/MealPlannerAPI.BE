using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using MealPlannerAPI;
using MealPlannerAPI.BlazorWebUI.Components;
using MealPlannerAPI.BlazorWebUI.Menus;
using MealPlannerAPI.Localization;
using MealPlannerAPI.MultiTenancy;
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
using Volo.Abp.AspNetCore.Components.Server.Theming;
using Volo.Abp.AspNetCore.Components.Web;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.FeatureManagement.Blazor.Server;
using Volo.Abp.Identity.Blazor.Server;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement.Blazor;
using Volo.Abp.PermissionManagement.Blazor.Server;
using Volo.Abp.Security.Claims;
using Volo.Abp.SettingManagement.Blazor.Server;
using Volo.Abp.Studio;
using Volo.Abp.Studio.Client.AspNetCore;
using Volo.Abp.TenantManagement.Blazor.Server;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
//using Blazorise;
//using Blazorise.Bootstrap5;
//using Blazorise.Icons.FontAwesome;

namespace MealPlannerAPI.BlazorWebUI;

[DependsOn(
    typeof(MealPlannerAPIHttpApiClientModule),
    typeof(MealPlannerAPIHttpApiHostModule),
    typeof(MealPlannerAPIApplicationContractsModule),
    // ============== //
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpTenantManagementBlazorServerModule),
    typeof(AbpSettingManagementBlazorServerModule),
    typeof(AbpPermissionManagementBlazorServerModule),
    typeof(AbpPermissionManagementBlazorModule),
    typeof(AbpIdentityBlazorServerModule),
    typeof(AbpFeatureManagementBlazorServerModule),
    typeof(AbpAccountBlazorModule),
    typeof(AbpStudioClientAspNetCoreModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreComponentsServerLeptonXLiteThemeModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),

    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpAspNetCoreComponentsServerThemingModule)
)]
public class MealPlannerAPIBlazorWebUIModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                //typeof(AbpSolution2Resource),
                //typeof(AbpSolution2DomainModule).Assembly,
                //typeof(AbpSolution2DomainSharedModule).Assembly,
                //typeof(AbpSolution2ApplicationModule).Assembly,
                //typeof(AbpSolution2ApplicationContractsModule).Assembly,
                typeof(MealPlannerAPIResource), // resource type (Type expected)
                typeof(MealPlannerAPIDomainModule).Assembly,
                typeof(MealPlannerAPIDomainSharedModule).Assembly,
                typeof(MealPlannerAPIApplicationModule).Assembly,
                typeof(MealPlannerAPIApplicationContractsModule).Assembly,

                typeof(MealPlannerAPIBlazorWebUIModule).Assembly
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
                serverBuilder.AddProductionEncryptionAndSigningCertificate("openiddict.pfx", configuration["AuthServer:CertificatePassPhrase"]!);
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

        // Add services to the container.
        context.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        if (!configuration.GetValue<bool>("App:DisablePII"))
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.LogCompleteSecurityArtifact = true;
        }

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

        ConfigureStudio(hostingEnvironment);
        ConfigureAuthentication(context);
        ConfigureUrls(configuration);
        ConfigureBundles(hostingEnvironment);
        //ConfigureHealthChecks(context);
        ConfigureVirtualFileSystem(context);
        //ConfigureSwaggerServices(context.Services);
        //ConfigureAutoApiControllers();
        ConfigureBlazorise(context);
        ConfigureRouter(context);
        ConfigureMenu(context);
    }
    private void ConfigureBlazorise(ServiceConfigurationContext context)
    {
        context.Services
            .AddBlazorise(options =>
            {
                // TODO (IMPORTANT): To use Blazorise, you need a license key. Get your license key directly from Blazorise, follow  the instructions at https://abp.io/faq#how-to-get-blazorise-license-key
                //options.ProductToken = "Your Product Token";
            })
        .AddBootstrap5Providers()
        .AddFontAwesomeIcons();
    }
    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<string>());
        });
    }
    private void ConfigureBundles(IHostEnvironment hostingEnvironment)
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

            options.ScriptBundles.Configure(
                LeptonXLiteThemeBundles.Scripts.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-scripts.js");
                    if (hostingEnvironment.IsDevelopment())
                    {
                        bundle.AddFiles("/dev-login-helper.js");
                    }
                }
            );

            // Blazor UI
            options.StyleBundles.Configure(
                BlazorLeptonXLiteThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });
    }
    private void ConfigureVirtualFileSystem( ServiceConfigurationContext context)
    {
        var hostingEnv = context.Services.GetHostingEnvironment();


        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            if (hostingEnv.IsDevelopment())
            {
                void ReplaceIfExists<TModule>(string path)
                {
                    var fullPath = Path.Combine(hostingEnv.ContentRootPath, path);
                    if (Directory.Exists(fullPath))
                    {
                        options.FileSets.ReplaceEmbeddedByPhysical<TModule>(fullPath);
                    }
                }


                ReplaceIfExists<MealPlannerAPIApplicationModule>($"..{Path.DirectorySeparatorChar}MealPlannerAPI.Application");
                ReplaceIfExists<MealPlannerAPIApplicationContractsModule>($"..{Path.DirectorySeparatorChar}MealPlannerAPI.Application.Contract");
                ReplaceIfExists<MealPlannerAPIDomainModule>($"..{Path.DirectorySeparatorChar}MealPlannerAPI.Domain");
                ReplaceIfExists<MealPlannerAPIDomainSharedModule>($"..{Path.DirectorySeparatorChar}MealPlannerAPI.Domain.Shared");

                ReplaceIfExists<MealPlannerAPIBlazorWebUIModule>($"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}MealPlannerAPI.BlazorWebUI");
                if (Directory.Exists(hostingEnv.ContentRootPath))
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<MealPlannerAPIBlazorWebUIModule>(hostingEnv.ContentRootPath);
                }
            }
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
    }
    private void ConfigureStudio(IHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsProduction())
        {
            Configure<AbpStudioClientOptions>(options =>
            {
                options.IsLinkEnabled = false;
            });
        }
    }


    private void ConfigureMenu(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new MenuContributor());
        });
    }


    private void ConfigureRouter(ServiceConfigurationContext context)
    {
        Configure<AbpRouterOptions>(options =>
        {
            options.AppAssembly = typeof(MealPlannerAPIBlazorWebUIModule).Assembly;
        });
    }
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var env = context.GetEnvironment();
        var app = context.GetApplicationBuilder();

        app.UseForwardedHeaders();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

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
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();

        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints(builder =>
        {
            builder.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddAdditionalAssemblies(builder.ServiceProvider.GetRequiredService<IOptions<AbpRouterOptions>>().Value.AdditionalAssemblies.ToArray());
        });
    }

}
