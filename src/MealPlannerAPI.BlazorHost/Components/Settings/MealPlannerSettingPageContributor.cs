using MealPlannerAPI.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using Volo.Abp.Localization;
using Volo.Abp.SettingManagement.Blazor;

namespace MealPlannerAPI.BlazorHost.Components.Settings
{
    public class MealPlannerSettingPageContributor : ISettingComponentContributor
    {


        public Task<bool> CheckPermissionsAsync(SettingComponentCreationContext context)
        {
            return Task.FromResult(true);
        }

        public Task ConfigureAsync(SettingComponentCreationContext context)
        {
            var localizer = context.ServiceProvider
                                    .GetRequiredService<IStringLocalizer<MealPlannerAPIResource>>();

            context.Groups.Add(
                new SettingComponentGroup(
                    id: "MealPlannerAPI.General",
                    displayName: localizer["MealPlannerSettings"],
                    componentType: typeof(MealPlannerGeneralSettingComponent),
                    order: 1
                ));
            return Task.CompletedTask;
        }


        private static LocalizableString L(string name) =>
            LocalizableString.Create<MealPlannerAPIResource>(name);
    }
}
