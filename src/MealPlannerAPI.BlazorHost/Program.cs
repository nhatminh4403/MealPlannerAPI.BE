using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading.Tasks;

namespace MealPlannerAPI.BlazorHost;

public class Program
{
    public async static Task<int> Main(string[] args)
    {

        Log.Logger = new LoggerConfiguration()
                        .WriteTo.Async(c => c.File("Logs/blazor-ui-logs.txt"))
                        .WriteTo.Async(c => c.Console())
                        .CreateBootstrapLogger();
        try
        {
            Log.Information("Starting MealPlannerAPI.BlazorHost.");

            var builder = WebApplication.CreateBuilder(args);
            builder.Host
                .AddAppSettingsSecretsJson()
                .UseAutofac()
                .UseSerilog((context, services, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services)
                        .WriteTo.Async(c => c.AbpStudio(services));
                });
            builder.WebHost.UseStaticWebAssets();
            await builder.Services.AddApplicationAsync<MealPlannerAPIBlazorHostModule>();
            var app = builder.Build();
            app.UseExceptionHandler("/Error");
            await app.InitializeApplicationAsync();
            await app.RunAsync();
            return 0;
        }
        catch (System.Exception ex)
        {
            if (ex is HostAbortedException)
            {
                throw;
            }
            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }

    }
}