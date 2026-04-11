using MealPlannerAPI.BackgroundJobs;
using MealPlannerAPI.EntityFrameworkCore;
using MealPlannerAPI.HealthChecks;
using MealPlannerAPI.Hubs;
using MealPlannerAPI.MealPlans.BackgroundJobs;
using MealPlannerAPI.MultiTenancy;
using MealPlannerAPI.Nutritions.ExternalData;
using MealPlannerAPI.Settings;
using MealPlannerAPI.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using Polly;
using System;
using System.IO;
using System.Linq;
using System.Threading.RateLimiting;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.Security.Claims;
using Volo.Abp.Studio;
using Volo.Abp.Studio.Client.AspNetCore;
using Volo.Abp.Swashbuckle;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace MealPlannerAPI;

[DependsOn(
    typeof(MealPlannerAPISignalRModule),
    typeof(MealPlannerAPIHttpApiModule),
    typeof(AbpStudioClientAspNetCoreModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(MealPlannerAPIApplicationModule),
    typeof(MealPlannerAPIEntityFrameworkCoreModule),
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpAspNetCoreSignalRModule),
    typeof(AbpAspNetCoreAuthenticationJwtBearerModule)
    //typeof(MealPlannerAPIBlazorWebUIModule)

    )]
public class MealPlannerAPIHttpApiHostModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

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
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var services = context.Services;


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
        services.Configure<HardDeleteOptions>(configuration.GetSection("HardDelete"));

        ConfigureStudio(hostingEnvironment);
        ConfigureAuthentication(context);
        ConfigureUrls(configuration);
        ConfigureBundles(hostingEnvironment);
        ConfigureConventionalControllers();
        ConfigureHealthChecks(context);
        ConfigureSwagger(context, configuration);
        ConfigureVirtualFileSystem(context);
        ConfigureCors(context, configuration);
        ConfigureRouting(services);
        ConfigureSignalR(services);
        ConfigureDistributedCacheOptions(context);
        ConfigureHttpClient(services);
        services.AddTransient<IMealPlannerHubPublisher, MealPlannerAPIPublisher>();
        ConfigureRateLimiter(services);
    }
    private void ConfigureRateLimiter(IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 50,
                        QueueLimit = 100, // Queueing prevents immediate 429s on bursts
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        Window = TimeSpan.FromSeconds(10)
                    }));

            options.AddFixedWindowLimiter("SignalR", configureOptions: signalrOptions =>
            {
                signalrOptions.AutoReplenishment = true;
                signalrOptions.PermitLimit = 100;
                signalrOptions.QueueLimit = 100;
                signalrOptions.Window = TimeSpan.FromSeconds(10);
            });

            options.RejectionStatusCode = StatusCodes.Status503ServiceUnavailable;
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
    private void ConfigureHttpClient(IServiceCollection services)
    {
        services.AddHttpClient<IUsdaFoodDataClient, UsdaFoodDataClient>()
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(2, _ => TimeSpan.FromMilliseconds(500)));
    }
    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
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
        });
    }
    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();


        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            if (hostingEnvironment.IsDevelopment())
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
                ReplaceIfExists<MealPlannerAPIApplicationContractsModule>($"..{Path.DirectorySeparatorChar}MealPlannerAPI.Application.Contract");
                ReplaceIfExists<MealPlannerAPIDomainModule>($"..{Path.DirectorySeparatorChar}MealPlannerAPI.Domain");
                ReplaceIfExists<MealPlannerAPIDomainSharedModule>($"..{Path.DirectorySeparatorChar}MealPlannerAPI.Domain.Shared");

                ReplaceIfExists<MealPlannerAPIHttpApiModule>($"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}MealPlannerAPI.HttpApi.Host");
                if (Directory.Exists(hostingEnvironment.ContentRootPath))
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<MealPlannerAPIHttpApiHostModule>(hostingEnvironment.ContentRootPath);
                }
            }
        });

        //if (hostingEnvironment.IsDevelopment())
        //{

        //    Configure<AbpVirtualFileSystemOptions>(options =>
        //    {
        //        options.FileSets.ReplaceEmbeddedByPhysical<MealPlannerAPIDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}MealPlannerAPI.Domain.Shared"));
        //        options.FileSets.ReplaceEmbeddedByPhysical<MealPlannerAPIDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}MealPlannerAPI.Domain"));
        //        options.FileSets.ReplaceEmbeddedByPhysical<MealPlannerAPIApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}MealPlannerAPI.Application.Contracts"));
        //        options.FileSets.ReplaceEmbeddedByPhysical<MealPlannerAPIApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}MealPlannerAPI.Application"));
        //    });
        //}
    }

    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(MealPlannerAPIApplicationModule).Assembly);
        });
    }
    private void ConfigureRouting(IServiceCollection services)
    {
        services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
        });
    }
    private static void ConfigureSwagger(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAbpSwaggerGenWithOidc(
            configuration["AuthServer:Authority"]!,
            ["MealPlannerAPI"],
            [AbpSwaggerOidcFlows.AuthorizationCode],
            null,
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "MealPlannerAPI API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            });
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]?
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.Trim().RemovePostFix("/"))
                            .ToArray() ?? Array.Empty<string>()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }
    private void ConfigureSignalR(IServiceCollection services)
    {
        services.AddSignalR(options =>
        {
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
        });

        Configure<AbpSignalROptions>(options =>
        {
            options.Hubs.AddOrUpdate(
                typeof(MealPlannerAPIHub),
                config =>
                {
                    config.RoutePattern = "/signalr-hubs/meal-planner";
                    config.ConfigureActions.Add(connectionOptions =>
                    {
                        connectionOptions.LongPolling.PollTimeout = TimeSpan.FromSeconds(15);
                    });
                });
        });
    }

    private void ConfigureDistributedCacheOptions(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();
        if (hostingEnvironment.IsDevelopment())
        {
            context.Services.AddDistributedMemoryCache();
            //context.Services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = configuration.GetConnectionString("Redis");
            //});
        }
        else
        {
            Configure<AbpDistributedCacheOptions>(options =>
            {
                options.KeyPrefix = "MealPlanner:";
            });
            context.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
            });
        }

    }
    private void ConfigureHealthChecks(ServiceConfigurationContext context)
    {
        context.Services.AddMealPlannerAPIHealthChecks();
    }

    // Registers background workers application-wide. They will be started automatically by the ABP framework.
    private void BackgroundJobsInitialization(ApplicationInitializationContext context)
    {
        context.AddBackgroundWorkerAsync<MealPlanHardDeleteWorker>();
        context.AddBackgroundWorkerAsync<MealReminderBackgroundWorker>();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();
        BackgroundJobsInitialization(context);

        app.UseForwardedHeaders();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseRouting();
        app.UseRateLimiter();
        app.MapAbpStaticAssets();
        app.UseAbpStudioLink();
        app.UseAbpSecurityHeaders();
        app.UseCors();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "MealPlannerAPI API");

            var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);

            options.OAuthScopes("MealPlannerAPI");
        });
        //app.UseConfiguredEndpoints(endpoints =>
        // {
        //     endpoints.MapHub<MealPlannerAPIHub>("/signalr-hubs/mealPlanner-api");
        // });

        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}
