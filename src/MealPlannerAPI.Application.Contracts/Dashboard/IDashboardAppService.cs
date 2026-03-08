using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MealPlannerAPI.Dashboard
{
    public interface IDashboardAppService : IApplicationService
    {
        /// <summary>Get the full dashboard including stats, recent and trending recipes.</summary>
        Task<DashboardDto> GetAsync();

        /// <summary>Get only the summary stats for the current user.</summary>
        Task<DashboardStatsDto> GetStatsAsync();

        /// <summary>Get the current trending recipes.</summary>
        Task<ListResultDto<TrendingRecipeDto>> GetTrendingAsync();
    }
}
