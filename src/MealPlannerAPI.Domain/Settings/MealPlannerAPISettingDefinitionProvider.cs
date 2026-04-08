using MealPlannerAPI.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace MealPlannerAPI.Settings;

public class MealPlannerAPISettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(MealPlannerAPISettings.MySetting1));
        context.Add(new SettingDefinition(MealPlannerAPISettings.EnableMockData, defaultValue: "true"));
        context.Add(new SettingDefinition(MealPlannerAPISettings.EnableUI, defaultValue: "true"));
    }

    private static LocalizableString L(string name) =>
        LocalizableString.Create<MealPlannerAPIResource>(name);
}
