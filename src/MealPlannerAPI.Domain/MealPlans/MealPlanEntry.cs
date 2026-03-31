using MealPlannerAPI.Enums;
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace MealPlannerAPI.MealPlans
{
    public class MealPlanEntry : FullAuditedEntity<Guid>
    {
        public Guid MealPlanId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }   // "Monday" … "Sunday"

        public string MealName { get; set; } = null!;

        public MealType MealType { get; set; }

        /// <summary>Scheduled time in HH:mm format, e.g. "08:00".</summary>
        public string? ScheduledTime { get; set; }
        public string? RecipeName { get; set; }
        public Guid? RecipeId { get; set; }
        protected MealPlanEntry() { }
        public MealPlanEntry(Guid id,
                             Guid mealPlanId,
                             DayOfWeek dayOfWeek,
                             string mealName,
                             MealType mealType,
                             Guid? recipeId,
                             string? scheduledTime,
                             string? recipeName)
            : base(id)
        {
            MealPlanId = mealPlanId;
            DayOfWeek = dayOfWeek;
            MealType = mealType;
            MealName = mealName;
            RecipeId = recipeId;
            ScheduledTime = scheduledTime;
            RecipeName = recipeName;
        }
    }
}
