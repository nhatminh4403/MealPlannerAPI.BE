using MealPlannerAPI.Localization;
using Microsoft.AspNetCore.Components;
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
