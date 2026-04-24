using MealPlannerAPI.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace MealPlannerAPI.Settings;

public class MealPlannerAPISettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {

        context.Add(new SettingDefinition(MealPlannerAPISettings.EnableUI,
                                          defaultValue: bool.TrueString,
                                          description: L("EnableUI"),
                                          
                                          isVisibleToClients: true));
        context.Add(new SettingDefinition(MealPlannerAPISettings.EnableMockData,
                                          defaultValue: bool.FalseString,
                                          description: L("EnableMockData"),
                                          
                                          isVisibleToClients: true));


    }

    private static LocalizableString L(string name) =>
        LocalizableString.Create<MealPlannerAPIResource>(name);
}
