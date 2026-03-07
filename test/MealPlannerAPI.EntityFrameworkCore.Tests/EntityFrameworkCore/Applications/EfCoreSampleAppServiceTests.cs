using MealPlannerAPI.Samples;
using Xunit;

namespace MealPlannerAPI.EntityFrameworkCore.Applications;

[Collection(MealPlannerAPITestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<MealPlannerAPIEntityFrameworkCoreTestModule>
{

}
