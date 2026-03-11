using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MealPlannerAPI.Dashboard
{
    public interface IDashboardAppService : IApplicationService
    {
        Task<DashboardDto> GetAsync();

        Task<DashboardStatsDto> GetStatsAsync();

        Task<ListResultDto<TrendingRecipeDto>> GetTrendingAsync();
        Task InvalidateTrendingCacheAsync();
    }
}
