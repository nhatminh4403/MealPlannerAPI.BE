using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MealPlannerAPI.BlazorHost;

[Dependency(ReplaceServices = true)]
public class BrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "MealPlanner Admin";

    public override string? LogoUrl => "/images/getting-started/blazor-host.svg";

    public override string? LogoReverseUrl => "/images/getting-started/blazor-host.svg";
}