using MealPlannerAPI.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace MealPlannerAPI.Settings;

public class MealPlannerAPISettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {

        context.Add(new SettingDefinition(MealPlannerAPISettings.EnableUI,
                                          defaultValue: "true",
                                          description: L("EnableUI"),
                                          
                                          isVisibleToClients: true));

    }

    private static LocalizableString L(string name) =>
        LocalizableString.Create<MealPlannerAPIResource>(name);
}
