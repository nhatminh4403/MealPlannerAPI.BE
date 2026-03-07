using MealPlannerAPI.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MealPlannerAPI.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MealPlannerAPIController : AbpControllerBase
{
    protected MealPlannerAPIController()
    {
        LocalizationResource = typeof(MealPlannerAPIResource);
    }
}
