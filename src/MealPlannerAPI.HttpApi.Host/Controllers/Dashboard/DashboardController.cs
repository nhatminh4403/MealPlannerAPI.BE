using MealPlannerAPI.Dashboard;
using MealPlannerAPI.Models;
using MealPlannerAPI.Recipes.Dtos;
using MealPlannerAPI.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

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

        /// <summary>Get the current user's dashboard summary.</summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<DashboardDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var result = await _dashboardAppService.GetAsync();
                return Ok(new ApiResponse<DashboardDto>(true, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Get dashboard statistics for the current user.</summary>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(ApiResponse<DashboardStatsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStatsAsync()
        {
            try
            {
                var result = await _dashboardAppService.GetStatsAsync();
                return Ok(new ApiResponse<DashboardStatsDto>(true, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Get trending recipes (public).</summary>
        [HttpGet("trending")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ListResultDto<TrendingRecipeDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTrendingAsync()
        {
            try
            {
                var result = await _dashboardAppService.GetTrendingAsync();
                return Ok(new ApiResponse<ListResultDto<TrendingRecipeDto>>(true, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }
    }
}
