using System;
using Volo.Abp.Domain.Repositories;

namespace MealPlannerAPI.MealPlans
{
    public interface IMealPlanRepository : IRepository<MealPlan, Guid>
    {
    }
}
