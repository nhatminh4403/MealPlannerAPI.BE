using Microsoft.Extensions.Localization;
using MealPlannerAPI.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MealPlannerAPI;

[Dependency(ReplaceServices = true)]
public class MealPlannerAPIBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<MealPlannerAPIResource> _localizer;

    public MealPlannerAPIBrandingProvider(IStringLocalizer<MealPlannerAPIResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
