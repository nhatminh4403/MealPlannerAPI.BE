using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MealPlannerAPI.Data;

/* This is used if database provider does't define
 * IMealPlannerAPIDbSchemaMigrator implementation.
 */
public class NullMealPlannerAPIDbSchemaMigrator : IMealPlannerAPIDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
