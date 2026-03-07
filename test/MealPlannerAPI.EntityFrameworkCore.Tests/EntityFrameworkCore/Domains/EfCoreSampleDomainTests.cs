using MealPlannerAPI.Samples;
using Xunit;

namespace MealPlannerAPI.EntityFrameworkCore.Domains;

[Collection(MealPlannerAPITestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<MealPlannerAPIEntityFrameworkCoreTestModule>
{

}
