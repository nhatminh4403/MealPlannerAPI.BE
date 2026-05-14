using MealPlannerAPI.MealPlans.Dtos;
using MealPlannerAPI.MealPlans.Services;
using MealPlannerAPI.Models;
using MealPlannerAPI.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Domain.Entities;

namespace MealPlannerAPI.Controllers.MealPlans
{
    [Route(APIRoute.APIApp + "meal-plans")]
    [ApiController]
    [Authorize]
    public class MealPlanController : AbpControllerBase
    {
        private readonly IMealPlanAppService _mealPlanAppService;

        public MealPlanController(IMealPlanAppService mealPlanAppService)
        {
            _mealPlanAppService = mealPlanAppService;
        }

        /// <summary>Get a meal plan by ID.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<MealPlanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            try
            {
                var result = await _mealPlanAppService.GetAsync(id);
                return Ok(new ApiResponse<MealPlanDto>(true, "Success", result));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Meal plan not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Get the current user's meal plans, optionally filtered by week start date.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<MealPlanDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetListAsync([FromQuery] GetMealPlansInput input)
        {
            try
            {
                var result = await _mealPlanAppService.GetListAsync(input);
                return Ok(new ApiResponse<PagedResultDto<MealPlanDto>>(true, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>
        /// Get the current user's meal plan for the current week.
        /// Creates an empty plan if none exists yet.
        /// </summary>
        [HttpGet("current-week")]
        [ProducesResponseType(typeof(ApiResponse<MealPlanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentWeekAsync()
        {
            try
            {
                var result = await _mealPlanAppService.GetCurrentWeekAsync();
                return Ok(new ApiResponse<MealPlanDto>(true, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Create a new meal plan for a given week.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<MealPlanDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUpdateMealPlanDto input)
        {
            try
            {
                var result = await _mealPlanAppService.CreateAsync(input);
                return StatusCode(StatusCodes.Status201Created,
                    new ApiResponse<MealPlanDto>(true, "Meal plan created successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Replace all entries in a meal plan for a given week.</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<MealPlanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] CreateUpdateMealPlanDto input)
        {
            try
            {
                var result = await _mealPlanAppService.UpdateAsync(id, input);
                return Ok(new ApiResponse<MealPlanDto>(true, "Meal plan updated successfully", result));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Meal plan not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Delete a meal plan and all its entries.</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                await _mealPlanAppService.DeleteAsync(id);
                return Ok(new ApiResponse(true, "Meal plan deleted successfully"));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Meal plan not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>
        /// Add or replace a single meal entry for a given day and meal type.
        /// If an entry already exists for that day + meal type it will be overwritten.
        /// </summary>
        [HttpPut("{mealPlanId:guid}/entries")]
        [ProducesResponseType(typeof(ApiResponse<MealPlanEntryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetEntryAsync(Guid mealPlanId, [FromBody] CreateUpdateMealPlanEntryDto input)
        {
            try
            {
                var result = await _mealPlanAppService.SetEntryAsync(mealPlanId, input);
                return Ok(new ApiResponse<MealPlanEntryDto>(true, "Meal plan entry updated successfully", result));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Meal plan not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>Remove a single meal entry from a plan.</summary>
        [HttpDelete("{mealPlanId:guid}/entries/{entryId:guid}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEntryAsync(Guid mealPlanId, Guid entryId)
        {
            try
            {
                if (
                    mealPlanId == Guid.Empty || mealPlanId == Guid.Empty ||
                    (string.IsNullOrEmpty(mealPlanId.ToString()) && string.IsNullOrEmpty(entryId.ToString())))
                {
                    Console.WriteLine($"Checking parameters {mealPlanId}, {entryId}");
                }
                await _mealPlanAppService.DeleteEntryAsync(mealPlanId, entryId);
                return Ok(new ApiResponse(true, "Meal plan entry deleted successfully"));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new ApiResponse(false, "Meal plan entry not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }

        /// <summary>
        /// Auto-generate a full week meal plan based on user preferences and available recipes.
        /// Optionally supply overrides for cuisine, dietary restrictions, difficulty, and time.
        /// </summary>
        [HttpPost("auto-generate")]
        [ProducesResponseType(typeof(ApiResponse<MealPlanDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AutoGenerateAsync([FromBody] AutoGenerateMealPlanDto input)
        {
            try
            {
                var result = await _mealPlanAppService.AutoGenerateAsync(input);
                return StatusCode(StatusCodes.Status201Created,
                    new ApiResponse<MealPlanDto>(true, "Meal plan auto-generated successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse(false, ex.Message));
            }
        }
    }
}
