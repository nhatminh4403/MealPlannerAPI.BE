using MealPlannerAPI.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.AspNetCore.Components;

namespace MealPlannerAPI.BlazorWebUI
{
    public class MealPlannerAPIComponentBase : AbpComponentBase
    {
        protected MealPlannerAPIComponentBase()
        {
            LocalizationResource = typeof(MealPlannerAPIResource);
        }
    }
}
