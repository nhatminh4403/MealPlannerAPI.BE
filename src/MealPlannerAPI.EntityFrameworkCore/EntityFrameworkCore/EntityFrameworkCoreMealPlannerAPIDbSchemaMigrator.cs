using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MealPlannerAPI.Data;
using Volo.Abp.DependencyInjection;

namespace MealPlannerAPI.EntityFrameworkCore;

public class EntityFrameworkCoreMealPlannerAPIDbSchemaMigrator
    : IMealPlannerAPIDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreMealPlannerAPIDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the MealPlannerAPIDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<MealPlannerAPIDbContext>()
            .Database
            .MigrateAsync();
    }
}
