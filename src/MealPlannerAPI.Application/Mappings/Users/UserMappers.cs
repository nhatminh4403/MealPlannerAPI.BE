using MealPlannerAPI.Users;
using MealPlannerAPI.Users.Dtos;
using Riok.Mapperly.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Volo.Abp.Mapperly;

namespace MealPlannerAPI.Mappings.Users;

// ── Shared factory ────────────────────────────────────────────────────────────
// UserProfile inherits IdentityUser whose public constructors require arguments.
// FormatterServices.GetUninitializedObject bypasses all constructors safely.
// Mapperly's Map(source, destination) fills every mapped property immediately after.
// Only used for the Map(source) → new instance overload.

file static class UserProfileFactory
{
    public static UserProfile CreateEmpty()
    {
        return (UserProfile)RuntimeHelpers.GetUninitializedObject(typeof(UserProfile));
    }
}

// ── UserProfile → CommunityUserDto ───────────────────────────────────────────
// CommunityUserDto.Id maps from UserProfile.Id (inherited from IdentityUser).
// UserName maps by name convention. Name, AvatarUrl, RecipesCreated, Followers,
// Following, Specialty all match by name — no explicit MapProperty needed.

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserProfileToCommunityUserDtoMapper : MapperBase<UserProfile, CommunityUserDto>
{
    public override partial CommunityUserDto Map(UserProfile source);
    public override partial void Map(UserProfile source, CommunityUserDto destination);

    public List<CommunityUserDto> MapList(IEnumerable<UserProfile> source)
        => source.Select(Map).ToList();
}

// ── UserProfile → UserPreferencesDto ─────────────────────────────────────────
// DietaryRestrictions and CuisinePreferences are comma-separated strings on the
// entity — deserialized to List<string> manually in the app service after Map().

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserProfileToUserPreferencesDtoMapper : MapperBase<UserProfile, UserPreferencesDto>
{
    [MapperIgnoreTarget(nameof(UserPreferencesDto.DietaryRestrictions))]
    [MapperIgnoreTarget(nameof(UserPreferencesDto.CuisinePreferences))]
    public override partial UserPreferencesDto Map(UserProfile source);

    [MapperIgnoreTarget(nameof(UserPreferencesDto.DietaryRestrictions))]
    [MapperIgnoreTarget(nameof(UserPreferencesDto.CuisinePreferences))]
    public override partial void Map(UserProfile source, UserPreferencesDto destination);
}

// ── UserProfile → UserStatsDto ────────────────────────────────────────────────
// All fields match by name convention.

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserProfileToUserStatsDtoMapper : MapperBase<UserProfile, UserStatsDto>
{
    public override partial UserStatsDto Map(UserProfile source);
    public override partial void Map(UserProfile source, UserStatsDto destination);
}

// ── UserProfile → UserPrivacyDto ──────────────────────────────────────────────
// ProfileVisibility, RecipesVisibility, ShoppingListVisibility match by name.

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserProfileToUserPrivacyDtoMapper : MapperBase<UserProfile, UserPrivacyDto>
{
    public override partial UserPrivacyDto Map(UserProfile source);
    public override partial void Map(UserProfile source, UserPrivacyDto destination);
}

// ── UserProfile → UserNotificationPreferencesDto ─────────────────────────────
// Entity uses Notify* prefix; DTO drops it — explicit MapProperty required.

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserProfileToUserNotificationPreferencesDtoMapper
    : MapperBase<UserProfile, UserNotificationPreferencesDto>
{
    [MapProperty(nameof(UserProfile.NotifyMealReminders), nameof(UserNotificationPreferencesDto.MealReminders))]
    [MapProperty(nameof(UserProfile.NotifyRecipeUpdates), nameof(UserNotificationPreferencesDto.RecipeUpdates))]
    [MapProperty(nameof(UserProfile.NotifyCommunityActivity), nameof(UserNotificationPreferencesDto.CommunityActivity))]
    [MapProperty(nameof(UserProfile.NotifyShoppingListAlerts), nameof(UserNotificationPreferencesDto.ShoppingListAlerts))]
    public override partial UserNotificationPreferencesDto Map(UserProfile source);

    [MapProperty(nameof(UserProfile.NotifyMealReminders), nameof(UserNotificationPreferencesDto.MealReminders))]
    [MapProperty(nameof(UserProfile.NotifyRecipeUpdates), nameof(UserNotificationPreferencesDto.RecipeUpdates))]
    [MapProperty(nameof(UserProfile.NotifyCommunityActivity), nameof(UserNotificationPreferencesDto.CommunityActivity))]
    [MapProperty(nameof(UserProfile.NotifyShoppingListAlerts), nameof(UserNotificationPreferencesDto.ShoppingListAlerts))]
    public override partial void Map(UserProfile source, UserNotificationPreferencesDto destination);
}

// ── CreateUpdateUserPreferencesDto → UserProfile ──────────────────────────────
// Only maps DefaultServingSize, WeeklyBudget, MealPlanningDays by name.
// DietaryRestrictions and CuisinePreferences are comma-separated — set manually
// in the app service via entity methods after Map().
// All IdentityUser fields and unrelated UserProfile fields are ignored so
// RequiredMappingStrategy.Target is satisfied without unintended side effects.

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateUserPreferencesDtoToUserProfileMapper
    : MapperBase<CreateUpdateUserPreferencesDto, UserProfile>
{
    [ObjectFactory]
    private UserProfile CreateUserProfile() => UserProfileFactory.CreateEmpty();

    // ── Ignored: DTO fields not mapped to UserProfile ──
    [MapperIgnoreSource(nameof(CreateUpdateUserPreferencesDto.DietaryRestrictions))]
    [MapperIgnoreSource(nameof(CreateUpdateUserPreferencesDto.CuisinePreferences))]
    // ── Ignored: IdentityUser infrastructure fields ────
    [MapperIgnoreTarget(nameof(UserProfile.Id))]
    [MapperIgnoreTarget(nameof(UserProfile.UserName))]
    [MapperIgnoreTarget(nameof(UserProfile.NormalizedUserName))]
    [MapperIgnoreTarget(nameof(UserProfile.Email))]
    [MapperIgnoreTarget(nameof(UserProfile.NormalizedEmail))]
    [MapperIgnoreTarget(nameof(UserProfile.EmailConfirmed))]
    [MapperIgnoreTarget(nameof(UserProfile.PasswordHash))]
    [MapperIgnoreTarget(nameof(UserProfile.SecurityStamp))]
    [MapperIgnoreTarget(nameof(UserProfile.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(UserProfile.PhoneNumber))]
    [MapperIgnoreTarget(nameof(UserProfile.PhoneNumberConfirmed))]
    [MapperIgnoreTarget(nameof(UserProfile.TwoFactorEnabled))]
    [MapperIgnoreTarget(nameof(UserProfile.LockoutEnd))]
    [MapperIgnoreTarget(nameof(UserProfile.LockoutEnabled))]
    [MapperIgnoreTarget(nameof(UserProfile.AccessFailedCount))]
    [MapperIgnoreTarget(nameof(UserProfile.IsActive))]
    [MapperIgnoreTarget(nameof(UserProfile.Name))]
    [MapperIgnoreTarget(nameof(UserProfile.Surname))]
    [MapperIgnoreTarget(nameof(UserProfile.TenantId))]
    [MapperIgnoreTarget(nameof(UserProfile.ExtraProperties))]
    // ── Ignored: unrelated UserProfile fields ─────────
    [MapperIgnoreTarget(nameof(UserProfile.AvatarUrl))]
    [MapperIgnoreTarget(nameof(UserProfile.Specialty))]
    [MapperIgnoreTarget(nameof(UserProfile.RecipesCreated))]
    [MapperIgnoreTarget(nameof(UserProfile.RecipesLiked))]
    [MapperIgnoreTarget(nameof(UserProfile.MealsPlanned))]
    [MapperIgnoreTarget(nameof(UserProfile.ShoppingListsGenerated))]
    [MapperIgnoreTarget(nameof(UserProfile.Followers))]
    [MapperIgnoreTarget(nameof(UserProfile.Following))]
    [MapperIgnoreTarget(nameof(UserProfile.ProfileVisibility))]
    [MapperIgnoreTarget(nameof(UserProfile.RecipesVisibility))]
    [MapperIgnoreTarget(nameof(UserProfile.ShoppingListVisibility))]
    [MapperIgnoreTarget(nameof(UserProfile.NotifyMealReminders))]
    [MapperIgnoreTarget(nameof(UserProfile.NotifyRecipeUpdates))]
    [MapperIgnoreTarget(nameof(UserProfile.NotifyCommunityActivity))]
    [MapperIgnoreTarget(nameof(UserProfile.NotifyShoppingListAlerts))]
    [MapperIgnoreTarget(nameof(UserProfile.DietaryRestrictions))]
    [MapperIgnoreTarget(nameof(UserProfile.CuisinePreferences))]
    [MapperIgnoreTarget(nameof(UserProfile.CreationTime))]
    [MapperIgnoreTarget(nameof(UserProfile.CreatorId))]
    [MapperIgnoreTarget(nameof(UserProfile.LastModificationTime))]
    [MapperIgnoreTarget(nameof(UserProfile.LastModifierId))]
    [MapperIgnoreTarget(nameof(UserProfile.IsDeleted))]
    [MapperIgnoreTarget(nameof(UserProfile.DeletionTime))]
    [MapperIgnoreTarget(nameof(UserProfile.DeleterId))]
    public override partial UserProfile Map(CreateUpdateUserPreferencesDto source);

    [MapperIgnoreSource(nameof(CreateUpdateUserPreferencesDto.DietaryRestrictions))]
    [MapperIgnoreSource(nameof(CreateUpdateUserPreferencesDto.CuisinePreferences))]
    [MapperIgnoreTarget(nameof(UserProfile.Id))]
    [MapperIgnoreTarget(nameof(UserProfile.UserName))]
    [MapperIgnoreTarget(nameof(UserProfile.NormalizedUserName))]
    [MapperIgnoreTarget(nameof(UserProfile.Email))]
    [MapperIgnoreTarget(nameof(UserProfile.NormalizedEmail))]
    [MapperIgnoreTarget(nameof(UserProfile.EmailConfirmed))]
    [MapperIgnoreTarget(nameof(UserProfile.PasswordHash))]
    [MapperIgnoreTarget(nameof(UserProfile.SecurityStamp))]
    [MapperIgnoreTarget(nameof(UserProfile.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(UserProfile.PhoneNumber))]
    [MapperIgnoreTarget(nameof(UserProfile.PhoneNumberConfirmed))]
    [MapperIgnoreTarget(nameof(UserProfile.TwoFactorEnabled))]
    [MapperIgnoreTarget(nameof(UserProfile.LockoutEnd))]
    [MapperIgnoreTarget(nameof(UserProfile.LockoutEnabled))]
    [MapperIgnoreTarget(nameof(UserProfile.AccessFailedCount))]
    [MapperIgnoreTarget(nameof(UserProfile.IsActive))]
    [MapperIgnoreTarget(nameof(UserProfile.Name))]
    [MapperIgnoreTarget(nameof(UserProfile.Surname))]
    [MapperIgnoreTarget(nameof(UserProfile.TenantId))]
    [MapperIgnoreTarget(nameof(UserProfile.ExtraProperties))]
    [MapperIgnoreTarget(nameof(UserProfile.AvatarUrl))]
    [MapperIgnoreTarget(nameof(UserProfile.Specialty))]
    [MapperIgnoreTarget(nameof(UserProfile.RecipesCreated))]
    [MapperIgnoreTarget(nameof(UserProfile.RecipesLiked))]
    [MapperIgnoreTarget(nameof(UserProfile.MealsPlanned))]
    [MapperIgnoreTarget(nameof(UserProfile.ShoppingListsGenerated))]
    [MapperIgnoreTarget(nameof(UserProfile.Followers))]
    [MapperIgnoreTarget(nameof(UserProfile.Following))]
    [MapperIgnoreTarget(nameof(UserProfile.ProfileVisibility))]
    [MapperIgnoreTarget(nameof(UserProfile.RecipesVisibility))]
    [MapperIgnoreTarget(nameof(UserProfile.ShoppingListVisibility))]
    [MapperIgnoreTarget(nameof(UserProfile.NotifyMealReminders))]
    [MapperIgnoreTarget(nameof(UserProfile.NotifyRecipeUpdates))]
    [MapperIgnoreTarget(nameof(UserProfile.NotifyCommunityActivity))]
    [MapperIgnoreTarget(nameof(UserProfile.NotifyShoppingListAlerts))]
    [MapperIgnoreTarget(nameof(UserProfile.DietaryRestrictions))]
    [MapperIgnoreTarget(nameof(UserProfile.CuisinePreferences))]
    [MapperIgnoreTarget(nameof(UserProfile.CreationTime))]
    [MapperIgnoreTarget(nameof(UserProfile.CreatorId))]
    [MapperIgnoreTarget(nameof(UserProfile.LastModificationTime))]
    [MapperIgnoreTarget(nameof(UserProfile.LastModifierId))]
    [MapperIgnoreTarget(nameof(UserProfile.IsDeleted))]
    [MapperIgnoreTarget(nameof(UserProfile.DeletionTime))]
    [MapperIgnoreTarget(nameof(UserProfile.DeleterId))]
    public override partial void Map(CreateUpdateUserPreferencesDto source, UserProfile destination);
}

// ── CreateUpdateUserSettingsDto → UserProfile ─────────────────────────────────
// Maps ProfileVisibility, RecipesVisibility, ShoppingListVisibility by name.
// Maps Notify* fields via explicit MapProperty (DTO has same Notify* prefix).
// All IdentityUser and unrelated UserProfile fields are ignored.

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateUserSettingsDtoToUserProfileMapper
    : MapperBase<CreateUpdateUserSettingsDto, UserProfile>
{
    [ObjectFactory]
    private UserProfile CreateUserProfile() => UserProfileFactory.CreateEmpty();

    [MapProperty(nameof(CreateUpdateUserSettingsDto.NotifyMealReminders), nameof(UserProfile.NotifyMealReminders))]
    [MapProperty(nameof(CreateUpdateUserSettingsDto.NotifyRecipeUpdates), nameof(UserProfile.NotifyRecipeUpdates))]
    [MapProperty(nameof(CreateUpdateUserSettingsDto.NotifyCommunityActivity), nameof(UserProfile.NotifyCommunityActivity))]
    [MapProperty(nameof(CreateUpdateUserSettingsDto.NotifyShoppingListAlerts), nameof(UserProfile.NotifyShoppingListAlerts))]
    // ── Ignored: IdentityUser infrastructure fields ────
    [MapperIgnoreTarget(nameof(UserProfile.Id))]
    [MapperIgnoreTarget(nameof(UserProfile.UserName))]
    [MapperIgnoreTarget(nameof(UserProfile.NormalizedUserName))]
    [MapperIgnoreTarget(nameof(UserProfile.Email))]
    [MapperIgnoreTarget(nameof(UserProfile.NormalizedEmail))]
    [MapperIgnoreTarget(nameof(UserProfile.EmailConfirmed))]
    [MapperIgnoreTarget(nameof(UserProfile.PasswordHash))]
    [MapperIgnoreTarget(nameof(UserProfile.SecurityStamp))]
    [MapperIgnoreTarget(nameof(UserProfile.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(UserProfile.PhoneNumber))]
    [MapperIgnoreTarget(nameof(UserProfile.PhoneNumberConfirmed))]
    [MapperIgnoreTarget(nameof(UserProfile.TwoFactorEnabled))]
    [MapperIgnoreTarget(nameof(UserProfile.LockoutEnd))]
    [MapperIgnoreTarget(nameof(UserProfile.LockoutEnabled))]
    [MapperIgnoreTarget(nameof(UserProfile.AccessFailedCount))]
    [MapperIgnoreTarget(nameof(UserProfile.IsActive))]
    [MapperIgnoreTarget(nameof(UserProfile.Name))]
    [MapperIgnoreTarget(nameof(UserProfile.Surname))]
    [MapperIgnoreTarget(nameof(UserProfile.TenantId))]
    [MapperIgnoreTarget(nameof(UserProfile.ExtraProperties))]
    // ── Ignored: unrelated UserProfile fields ─────────
    [MapperIgnoreTarget(nameof(UserProfile.AvatarUrl))]
    [MapperIgnoreTarget(nameof(UserProfile.Specialty))]
    [MapperIgnoreTarget(nameof(UserProfile.RecipesCreated))]
    [MapperIgnoreTarget(nameof(UserProfile.RecipesLiked))]
    [MapperIgnoreTarget(nameof(UserProfile.MealsPlanned))]
    [MapperIgnoreTarget(nameof(UserProfile.ShoppingListsGenerated))]
    [MapperIgnoreTarget(nameof(UserProfile.Followers))]
    [MapperIgnoreTarget(nameof(UserProfile.Following))]
    [MapperIgnoreTarget(nameof(UserProfile.DefaultServingSize))]
    [MapperIgnoreTarget(nameof(UserProfile.WeeklyBudget))]
    [MapperIgnoreTarget(nameof(UserProfile.MealPlanningDays))]
    [MapperIgnoreTarget(nameof(UserProfile.DietaryRestrictions))]
    [MapperIgnoreTarget(nameof(UserProfile.CuisinePreferences))]
    [MapperIgnoreTarget(nameof(UserProfile.CreationTime))]
    [MapperIgnoreTarget(nameof(UserProfile.CreatorId))]
    [MapperIgnoreTarget(nameof(UserProfile.LastModificationTime))]
    [MapperIgnoreTarget(nameof(UserProfile.LastModifierId))]
    [MapperIgnoreTarget(nameof(UserProfile.IsDeleted))]
    [MapperIgnoreTarget(nameof(UserProfile.DeletionTime))]
    [MapperIgnoreTarget(nameof(UserProfile.DeleterId))]
    public override partial UserProfile Map(CreateUpdateUserSettingsDto source);

    [MapProperty(nameof(CreateUpdateUserSettingsDto.NotifyMealReminders), nameof(UserProfile.NotifyMealReminders))]
    [MapProperty(nameof(CreateUpdateUserSettingsDto.NotifyRecipeUpdates), nameof(UserProfile.NotifyRecipeUpdates))]
    [MapProperty(nameof(CreateUpdateUserSettingsDto.NotifyCommunityActivity), nameof(UserProfile.NotifyCommunityActivity))]
    [MapProperty(nameof(CreateUpdateUserSettingsDto.NotifyShoppingListAlerts), nameof(UserProfile.NotifyShoppingListAlerts))]
    [MapperIgnoreTarget(nameof(UserProfile.Id))]
    [MapperIgnoreTarget(nameof(UserProfile.UserName))]
    [MapperIgnoreTarget(nameof(UserProfile.NormalizedUserName))]
    [MapperIgnoreTarget(nameof(UserProfile.Email))]
    [MapperIgnoreTarget(nameof(UserProfile.NormalizedEmail))]
    [MapperIgnoreTarget(nameof(UserProfile.EmailConfirmed))]
    [MapperIgnoreTarget(nameof(UserProfile.PasswordHash))]
    [MapperIgnoreTarget(nameof(UserProfile.SecurityStamp))]
    [MapperIgnoreTarget(nameof(UserProfile.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(UserProfile.PhoneNumber))]
    [MapperIgnoreTarget(nameof(UserProfile.PhoneNumberConfirmed))]
    [MapperIgnoreTarget(nameof(UserProfile.TwoFactorEnabled))]
    [MapperIgnoreTarget(nameof(UserProfile.LockoutEnd))]
    [MapperIgnoreTarget(nameof(UserProfile.LockoutEnabled))]
    [MapperIgnoreTarget(nameof(UserProfile.AccessFailedCount))]
    [MapperIgnoreTarget(nameof(UserProfile.IsActive))]
    [MapperIgnoreTarget(nameof(UserProfile.Name))]
    [MapperIgnoreTarget(nameof(UserProfile.Surname))]
    [MapperIgnoreTarget(nameof(UserProfile.TenantId))]
    [MapperIgnoreTarget(nameof(UserProfile.ExtraProperties))]
    [MapperIgnoreTarget(nameof(UserProfile.AvatarUrl))]
    [MapperIgnoreTarget(nameof(UserProfile.Specialty))]
    [MapperIgnoreTarget(nameof(UserProfile.RecipesCreated))]
    [MapperIgnoreTarget(nameof(UserProfile.RecipesLiked))]
    [MapperIgnoreTarget(nameof(UserProfile.MealsPlanned))]
    [MapperIgnoreTarget(nameof(UserProfile.ShoppingListsGenerated))]
    [MapperIgnoreTarget(nameof(UserProfile.Followers))]
    [MapperIgnoreTarget(nameof(UserProfile.Following))]
    [MapperIgnoreTarget(nameof(UserProfile.DefaultServingSize))]
    [MapperIgnoreTarget(nameof(UserProfile.WeeklyBudget))]
    [MapperIgnoreTarget(nameof(UserProfile.MealPlanningDays))]
    [MapperIgnoreTarget(nameof(UserProfile.DietaryRestrictions))]
    [MapperIgnoreTarget(nameof(UserProfile.CuisinePreferences))]
    [MapperIgnoreTarget(nameof(UserProfile.CreationTime))]
    [MapperIgnoreTarget(nameof(UserProfile.CreatorId))]
    [MapperIgnoreTarget(nameof(UserProfile.LastModificationTime))]
    [MapperIgnoreTarget(nameof(UserProfile.LastModifierId))]
    [MapperIgnoreTarget(nameof(UserProfile.IsDeleted))]
    [MapperIgnoreTarget(nameof(UserProfile.DeletionTime))]
    [MapperIgnoreTarget(nameof(UserProfile.DeleterId))]
    public override partial void Map(CreateUpdateUserSettingsDto source, UserProfile destination);
}