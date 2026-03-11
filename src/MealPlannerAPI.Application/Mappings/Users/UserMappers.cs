using MealPlannerAPI.Users;
using MealPlannerAPI.Users.Dtos;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Volo.Abp.Mapperly;

namespace MealPlannerAPI.Mappings.Users;

file static class UserProfileFactory
{
    public static UserProfile CreateEmpty()
        => (UserProfile)FormatterServices.GetUninitializedObject(typeof(UserProfile));
}

// ── UserProfile → CommunityUserDto ───────────────────────────────────────────

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[MapExtraProperties]
public partial class UserProfileToCommunityUserDtoMapper : MapperBase<UserProfile, CommunityUserDto>
{
    [MapProperty(nameof(UserProfile.Id), nameof(CommunityUserDto.Id))]
    [MapProperty(nameof(UserProfile.Name), nameof(CommunityUserDto.Name))]
    public override partial CommunityUserDto Map(UserProfile source);

    [MapProperty(nameof(UserProfile.Id), nameof(CommunityUserDto.Id))]
    [MapProperty(nameof(UserProfile.Name), nameof(CommunityUserDto.Name))]
    public override partial void Map(UserProfile source, CommunityUserDto destination);

    public List<CommunityUserDto> MapList(IEnumerable<UserProfile> source)
        => source.Select(Map).ToList();
}

// ── UserProfile → UserPreferencesDto ─────────────────────────────────────────

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserProfileToUserPreferencesDtoMapper : MapperBase<UserProfile, UserPreferencesDto>
{
    [MapperIgnoreTarget(nameof(UserPreferencesDto.DietaryRestrictions))]
    [MapperIgnoreTarget(nameof(UserPreferencesDto.CuisinePreferences))]
    public override partial UserPreferencesDto Map(UserProfile source);

    [MapperIgnoreTarget(nameof(UserPreferencesDto.DietaryRestrictions))]
    [MapperIgnoreTarget(nameof(UserPreferencesDto.CuisinePreferences))]
    public override partial void Map(UserProfile source, UserPreferencesDto destination);

    public List<string> FromCommaSeparated(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? new List<string>()
            : value.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
}

// ── UserProfile → UserStatsDto ────────────────────────────────────────────────

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserProfileToUserStatsDtoMapper : MapperBase<UserProfile, UserStatsDto>
{
    public override partial UserStatsDto Map(UserProfile source);
    public override partial void Map(UserProfile source, UserStatsDto destination);
}

// ── UserProfile → UserPrivacyDto ──────────────────────────────────────────────

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserProfileToUserPrivacyDtoMapper : MapperBase<UserProfile, UserPrivacyDto>
{
    public override partial UserPrivacyDto Map(UserProfile source);
    public override partial void Map(UserProfile source, UserPrivacyDto destination);
}

// ── UserProfile → UserNotificationPreferencesDto ──────────────────────────────

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

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateUserPreferencesDtoToUserProfileMapper
    : MapperBase<CreateUpdateUserPreferencesDto, UserProfile>
{
    [ObjectFactory]
    private UserProfile CreateUserProfile() => UserProfileFactory.CreateEmpty();

    [MapperIgnoreTarget(nameof(UserProfile.Id))]
    [MapperIgnoreTarget(nameof(UserProfile.UserName))]
    [MapperIgnoreTarget(nameof(UserProfile.Email))]
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

    [MapperIgnoreTarget(nameof(UserProfile.Id))]
    [MapperIgnoreTarget(nameof(UserProfile.UserName))]
    [MapperIgnoreTarget(nameof(UserProfile.Email))]
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

    public string ToCommaSeparated(List<string> values) => string.Join(',', values);
}

// ── CreateUpdateUserSettingsDto → UserProfile ─────────────────────────────────

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
    [MapperIgnoreTarget(nameof(UserProfile.Id))]
    [MapperIgnoreTarget(nameof(UserProfile.UserName))]
    [MapperIgnoreTarget(nameof(UserProfile.Email))]
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
    [MapperIgnoreTarget(nameof(UserProfile.Email))]
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