using MealPlannerAPI.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using MealPlannerAPI.Routes;
using MealPlannerAPI.Recipes.Dtos;
namespace MealPlannerAPI.Controllers.Dashboard
{
    [Route(APIRoute.APIApp + "[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : AbpControllerBase
    {
        private readonly IDashboardAppService _dashboardAppService;

        public DashboardController(IDashboardAppService dashboardAppService)
        {
            _dashboardAppService = dashboardAppService;
        }


        [HttpGet]
        [Authorize]
        public Task<DashboardDto> GetAsync()
            => _dashboardAppService.GetAsync();

        [HttpGet("stats")]
        public Task<DashboardStatsDto> GetStatsAsync()
            => _dashboardAppService.GetStatsAsync();

        [HttpGet("trending")]
        [AllowAnonymous]
        public Task<ListResultDto<TrendingRecipeDto>> GetTrendingAsync()
            => _dashboardAppService.GetTrendingAsync();
    }
}
