using MealPlannerAPI.Localization;
using Volo.Abp.Application.Services;

namespace MealPlannerAPI;

/* Inherit your application services from this class.
 */
public abstract class MealPlannerAPIAppService : ApplicationService
{
    protected MealPlannerAPIAppService()
    {
        LocalizationResource = typeof(MealPlannerAPIResource);
    }
}
