using MealPlannerAPI.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace MealPlannerAPI.Users
{
    public class UserProfile : IdentityUser
    {
        public Guid UserId { get; set; }

        public string? AvatarUrl { get; set; }

        // ── Preferences ──────────────────────────────────────────────
        /// <summary>Comma-separated dietary restrictions, e.g. "vegetarian,gluten-free".</summary>
        public string? DietaryRestrictions { get; set; }

        /// <summary>Comma-separated preferred cuisines, e.g. "Italian,Thai".</summary>
        public string? CuisinePreferences { get; set; }

        public int DefaultServingSize { get; set; } = 4;

        public decimal WeeklyBudget { get; set; }

        // ── Stats ─────────────────────────────────────────────────────
        public int RecipesCreated { get; set; }

        public int RecipesLiked { get; set; }

        public int MealsPlanned { get; set; }

        public int ShoppingListsGenerated { get; set; }

        public int Followers { get; set; }

        public int Following { get; set; }

        public string? Specialty { get; set; }

        public VisibilityLevel ProfileVisibility { get; set; } = VisibilityLevel.Public;

        public VisibilityLevel RecipesVisibility { get; set; } = VisibilityLevel.Public;

        public VisibilityLevel ShoppingListVisibility { get; set; } = VisibilityLevel.Private;

        // ── Notification preferences ──────────────────────────────────
        public bool NotifyMealReminders { get; set; } = true;

        public bool NotifyRecipeUpdates { get; set; } = true;

        public bool NotifyCommunityActivity { get; set; } = false;

        public bool NotifyShoppingListAlerts { get; set; } = true;

        public int MealPlanningDays { get; set; } = 7;

        protected UserProfile() { }

        public UserProfile(Guid id, string userName, string email, Guid? tenantId = null) 
            : base(id, userName, email, tenantId)
        {
        }
    }

}
