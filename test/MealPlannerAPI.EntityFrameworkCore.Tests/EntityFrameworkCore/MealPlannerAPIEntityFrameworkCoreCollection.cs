using Xunit;

namespace MealPlannerAPI.EntityFrameworkCore;

[CollectionDefinition(MealPlannerAPITestConsts.CollectionDefinitionName)]
public class MealPlannerAPIEntityFrameworkCoreCollection : ICollectionFixture<MealPlannerAPIEntityFrameworkCoreFixture>
{

}
