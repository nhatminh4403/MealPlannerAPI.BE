using MealPlannerAPI.MealPlans.Dtos;
using MealPlannerAPI.MealPlans.Services;
using MealPlannerAPI.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

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
        public Task<MealPlanDto> GetAsync(Guid id)
            => _mealPlanAppService.GetAsync(id);

        /// <summary>Get the current user's meal plans, optionally filtered by week start date.</summary>
        [HttpGet]
        //[AllowAnonymous]
        public Task<PagedResultDto<MealPlanDto>> GetListAsync([FromQuery] GetMealPlansInput input)
            => _mealPlanAppService.GetListAsync(input);

        /// <summary>
        /// Get the current user's meal plan for the current week.
        /// Creates an empty plan if none exists yet.
        /// </summary>
        [HttpGet("current-week")]
        public Task<MealPlanDto> GetCurrentWeekAsync()
            => _mealPlanAppService.GetCurrentWeekAsync();

        /// <summary>Create a new meal plan for a given week.</summary>
        [HttpPost]
        public Task<MealPlanDto> CreateAsync([FromBody] CreateUpdateMealPlanDto input)
            => _mealPlanAppService.CreateAsync(input);

        /// <summary>Replace all entries in a meal plan for a given week.</summary>
        [HttpPut("{id:guid}")]
        public Task<MealPlanDto> UpdateAsync(Guid id, [FromBody] CreateUpdateMealPlanDto input)
            => _mealPlanAppService.UpdateAsync(id, input);

        /// <summary>Delete a meal plan and all its entries.</summary>
        [HttpDelete("{id:guid}")]
        public Task DeleteAsync(Guid id)
            => _mealPlanAppService.DeleteAsync(id);

        /// <summary>
        /// Add or replace a single meal entry for a given day and meal type.
        /// If an entry already exists for that day + meal type it will be overwritten.
        /// </summary>
        [HttpPut("{mealPlanId:guid}/entries")]
        public Task<MealPlanEntryDto> SetEntryAsync(Guid mealPlanId, [FromBody] CreateUpdateMealPlanEntryDto input)
            => _mealPlanAppService.SetEntryAsync(mealPlanId, input);

        /// <summary>Remove a single meal entry from a plan.</summary>
        [HttpDelete("{mealPlanId:guid}/entries/{entryId:guid}")]
        public async Task DeleteEntryAsync(Guid mealPlanId, Guid entryId)
        {
            if (
                mealPlanId == Guid.Empty || mealPlanId == Guid.Empty ||
                (string.IsNullOrEmpty(mealPlanId.ToString()) && string.IsNullOrEmpty(entryId.ToString())))
            {
                Console.WriteLine($"Checking parameters {mealPlanId}, {entryId}");
            }
            await _mealPlanAppService.DeleteEntryAsync(mealPlanId, entryId);
        }

        /// <summary>
        /// Auto-generate a full week meal plan based on user preferences and available recipes.
        /// Optionally supply overrides for cuisine, dietary restrictions, difficulty, and time.
        /// </summary>
        [HttpPost("auto-generate")]
        public Task<MealPlanDto> AutoGenerateAsync([FromBody] AutoGenerateMealPlanDto input)
            => _mealPlanAppService.AutoGenerateAsync(input);
    }
}
