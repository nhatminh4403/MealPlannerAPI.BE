using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MealPlannerAPI.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class MealPlannerAPIDbContextFactory : IDesignTimeDbContextFactory<MealPlannerAPIDbContext>
{
    public MealPlannerAPIDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        MealPlannerAPIEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<MealPlannerAPIDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));

        return new MealPlannerAPIDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../MealPlannerAPI.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}
