using MealPlannerAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace MealPlannerAPI.MealPlans
{
    public class MealPlan : FullAuditedAggregateRoot<Guid>
    {
        public Guid UserId { get; set; }
        public DateTime WeekStartDate { get; set; }
        public ICollection<MealPlanEntry> Entries { get; set; } = new List<MealPlanEntry>();

        protected MealPlan() { }

        public MealPlan(Guid id, Guid userId, DateTime weekStartDate) : base(id)
        {
            UserId = userId;
            WeekStartDate = weekStartDate;
        }

        public MealPlanEntry AddEntry(Guid id,
                                      DayOfWeek dayOfWeek,
                                      string mealName,
                                      MealType mealType,
                                      string? recipeName,
                                      string? scheduledTime,
                                      Guid? recipeId)
        {
            var existing = Entries.FirstOrDefault(e =>
                e.DayOfWeek == dayOfWeek && e.MealType == mealType);

            if (existing != null)
            {
                existing.MealName = mealName;
                existing.ScheduledTime = scheduledTime;
                existing.RecipeId = recipeId;
                return existing;
            }
            
            var entry = new MealPlanEntry(id: id,
                                          mealPlanId: Id,
                                          dayOfWeek,
                                          mealName,
                                          mealType,
                                          recipeName: recipeName,
                                          scheduledTime: scheduledTime,
                                          recipeId: recipeId);
            Entries.Add(entry);
            return entry;
        }

        public void RemoveEntry(Guid entryId)
        {
            var entry = Entries.FirstOrDefault(e => e.Id == entryId)
                ?? throw new EntityNotFoundException(typeof(MealPlanEntry), entryId);

            Entries.Remove(entry);
        }
        public void ReplaceEntries(
        IEnumerable<(Guid Id, DayOfWeek DayOfWeek, string MealName,MealType MealType, string? ScheduledTime, Guid? RecipeId, string? RecipeName)> entries)
        {
            Entries.Clear();
            foreach (var e in entries)
                AddEntry(id: e.Id,
                         dayOfWeek: e.DayOfWeek,
                         mealName: e.MealName,
                         mealType: e.MealType,
                         scheduledTime: e.ScheduledTime,
                         recipeId: e.RecipeId,
                         recipeName: e.RecipeName);
        }
        public static DateTime GetWeekStart(DateTime date)
        {
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.Date.AddDays(-diff);
        }
    }
}
