using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace MealPlannerAPI.BlazorHost;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
       var builder = WebApplication.CreateBuilder(args);
       builder.Host
           .AddAppSettingsSecretsJson()
           .UseAutofac();
       await builder.Services.AddApplicationAsync<MealPlannerAPIBlazorHostModule>(options =>
       {
           options.Services.ReplaceConfiguration(builder.Configuration);
       });
       var app = builder.Build();
       await app.InitializeApplicationAsync();
       await app.RunAsync();
       return 0;
    }
}