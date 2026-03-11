using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace MealPlannerAPI.MealPlans
{
    public class MealPlanManager : DomainService
    {
        private readonly IMealPlanRepository _mealPlanRepository;

        public MealPlanManager(IMealPlanRepository mealPlanRepository)
        {
            _mealPlanRepository = mealPlanRepository;
        }

        public async Task<MealPlan> GetOrCreateMealPlanAsync(Guid userId, DateTime weekStartDate)
        {
            var weekStart = MealPlan.GetWeekStart(weekStartDate);

            var existing = await _mealPlanRepository.FindAsync(mp =>
                mp.UserId == userId && mp.WeekStartDate == weekStart);

            if (existing != null) return existing;

            var plan = new MealPlan(GuidGenerator.Create(), userId, weekStart);
            await _mealPlanRepository.InsertAsync(plan, autoSave: true);
            return plan;
        }
    }
}
