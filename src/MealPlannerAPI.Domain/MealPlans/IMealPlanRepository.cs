using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories;

namespace MealPlannerAPI.MealPlans
{
    public interface IMealPlanRepository : IRepository<MealPlan, Guid>
    {
    }
}
