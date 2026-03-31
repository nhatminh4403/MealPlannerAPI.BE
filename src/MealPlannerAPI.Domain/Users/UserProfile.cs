using MealPlannerAPI.Enums;
using System;
using System.Collections.Generic;
using Volo.Abp.Identity;

namespace MealPlannerAPI.Users
{
    public class UserProfile : IdentityUser
    {
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

        public void UpdateAvatar(string? avatarUrl)
       => AvatarUrl = avatarUrl;

        // ── Dietary restrictions ──────────────────────────────────────────────────

        public List<string> GetDietaryRestrictions()
            => SplitComma(DietaryRestrictions);

        public void SetDietaryRestrictions(IEnumerable<string> values)
            => DietaryRestrictions = JoinComma(values);

        // ── Cuisine preferences ───────────────────────────────────────────────────

        public List<string> GetCuisinePreferences()
            => SplitComma(CuisinePreferences);

        public void SetCuisinePreferences(IEnumerable<string> values)
            => CuisinePreferences = JoinComma(values);

        // ── Social ────────────────────────────────────────────────────────────────

        public void GainFollower() => Followers++;

        public void LoseFollower()
        {
            if (Followers == 0)
                throw new Volo.Abp.BusinessException(MealPlannerAPIDomainErrorCodes.NoFollowersToRemove);
            Followers--;
        }

        public void Follow() => Following++;

        public void Unfollow()
        {
            if (Following == 0)
                throw new Volo.Abp.BusinessException(MealPlannerAPIDomainErrorCodes.AlreadyNotFollowing);
            Following--;
        }

        // ── Private helpers ───────────────────────────────────────────────────────

        private static List<string> SplitComma(string? value)
            => string.IsNullOrWhiteSpace(value)
                ? new List<string>()
                : new List<string>(value.Split(',', StringSplitOptions.RemoveEmptyEntries));

        private static string? JoinComma(IEnumerable<string> values)
        {
            var joined = string.Join(',', values);
            return string.IsNullOrEmpty(joined) ? null : joined;
        }
    }

}
