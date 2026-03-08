using MealPlannerAPI.MealPlans.Dtos;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MealPlannerAPI.MealPlans.Services
{
    public interface IMealPlanAppService
        : ICrudAppService<
            MealPlanDto,
            MealPlanDto,
            Guid,
            GetMealPlansInput,
            CreateUpdateMealPlanDto,
            CreateUpdateMealPlanDto>
    {
        /// Get the meal plan for the current user for the current week.
        Task<MealPlanDto> GetCurrentWeekAsync();

        /// Add or update a single entry within an existing meal plan.
        Task<MealPlanEntryDto> SetEntryAsync(Guid mealPlanId, CreateUpdateMealPlanEntryDto input);

        /// Remove a single entry from a meal plan.
        Task DeleteEntryAsync(Guid mealPlanId, Guid entryId);
    }
}
