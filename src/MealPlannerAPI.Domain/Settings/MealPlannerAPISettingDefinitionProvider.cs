using Volo.Abp.Settings;

namespace MealPlannerAPI.Settings;

public class MealPlannerAPISettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(MealPlannerAPISettings.MySetting1));
    }
}
