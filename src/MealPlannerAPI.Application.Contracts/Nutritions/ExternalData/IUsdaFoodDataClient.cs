using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MealPlannerAPI.Nutritions.ExternalData
{
    public interface IUsdaFoodDataClient : ITransientDependency
    {
        Task<List<UsdaFoodDataResultDTO>> SearchAsync(
        string query,
        int maxResults = 5,
        CancellationToken cancellationToken = default);
    }
}
