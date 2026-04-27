using MealPlannerAPI.Localization;
using Volo.Abp.AspNetCore.Components;

namespace MealPlannerAPI.BlazorHost
{
    public abstract class MealPlannerAPIComponentBase : AbpComponentBase
    {
        protected MealPlannerAPIComponentBase()
        {
            LocalizationResource = typeof(MealPlannerAPIResource);
        }
    }
}
