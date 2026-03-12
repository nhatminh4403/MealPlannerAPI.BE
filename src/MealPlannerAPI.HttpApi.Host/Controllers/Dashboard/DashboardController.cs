using MealPlannerAPI.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using MealPlannerAPI.Routes;
namespace MealPlannerAPI.Controllers.Dashboard
{
    [Route(APIRoute.APIApp + "[controller]")]
    [ApiController]
    public class DashboardController : AbpControllerBase
    {
        private readonly IDashboardAppService _dashboardAppService;

        public DashboardController(IDashboardAppService dashboardAppService)
        {
            _dashboardAppService = dashboardAppService;
        }

        /// <summary>
        /// Get the full dashboard — stats, trending recipes and recent recipes in one call.
        /// Prefer the individual endpoints when you only need one section.
        /// </summary>
        [HttpGet]
        public Task<DashboardDto> GetAsync()
            => _dashboardAppService.GetAsync();

        /// <summary>Get the current user's activity stats for the dashboard.</summary>
        [HttpGet("stats")]
        public Task<DashboardStatsDto> GetStatsAsync()
            => _dashboardAppService.GetStatsAsync();

        /// <summary>Get the currently trending recipes across the platform.</summary>
        [HttpGet("trending")]
        [AllowAnonymous]
        public Task<ListResultDto<TrendingRecipeDto>> GetTrendingAsync()
            => _dashboardAppService.GetTrendingAsync();
    }
}
