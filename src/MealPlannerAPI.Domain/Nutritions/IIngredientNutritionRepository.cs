using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace MealPlannerAPI.Nutritions
{
    public interface IIngredientNutritionRepository : IRepository<IngredientNutrition, Guid>
    {
        Task<IngredientNutrition?> FindByNameAsync(string name,
                                                   CancellationToken cancellationToken = default);

        Task<List<IngredientNutrition>> GetByIdsAsync(IEnumerable<Guid> ids,
                                                      CancellationToken cancellationToken = default);
    }
}
