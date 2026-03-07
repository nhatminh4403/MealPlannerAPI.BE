using System.Threading.Tasks;

namespace MealPlannerAPI.Data;

public interface IMealPlannerAPIDbSchemaMigrator
{
    Task MigrateAsync();
}
