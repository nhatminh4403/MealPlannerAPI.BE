using MealPlannerAPI.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace MealPlannerAPI.Permissions;

public class MealPlannerAPIPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(MealPlannerAPIPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(MealPlannerAPIPermissions.MyPermission1, L("Permission:MyPermission1"));
        AddRecipePermissions(myGroup);
        AddMealPlanPermissions(myGroup);
        AddShoppingListPermissions(myGroup);
        AddNotificationPermissions(myGroup);
        AddUserProfilePermissions(myGroup);
        AddDashboardPermissions(myGroup);
    }
    private static void AddRecipePermissions(PermissionGroupDefinition group)
    {
        var recipes = group.AddPermission(
            MealPlannerAPIPermissions.Recipes.Default,
            L("Permission:Recipes"));

        recipes.AddChild(MealPlannerAPIPermissions.Recipes.Create, L("Permission:Recipes.Create"));
        recipes.AddChild(MealPlannerAPIPermissions.Recipes.Update, L("Permission:Recipes.Update"));
        recipes.AddChild(MealPlannerAPIPermissions.Recipes.Delete, L("Permission:Recipes.Delete"));
    }

    private static void AddMealPlanPermissions(PermissionGroupDefinition group)
    {
        var mealPlans = group.AddPermission(
            MealPlannerAPIPermissions.MealPlans.Default,
            L("Permission:MealPlans"));

        mealPlans.AddChild(MealPlannerAPIPermissions.MealPlans.Create, L("Permission:MealPlans.Create"));
        mealPlans.AddChild(MealPlannerAPIPermissions.MealPlans.Update, L("Permission:MealPlans.Update"));
        mealPlans.AddChild(MealPlannerAPIPermissions.MealPlans.Delete, L("Permission:MealPlans.Delete"));
    }

    // ── Shopping Lists ────────────────────────────────────────────────────────

    private static void AddShoppingListPermissions(PermissionGroupDefinition group)
    {
        var shoppingLists = group.AddPermission(
            MealPlannerAPIPermissions.ShoppingLists.Default,
            L("Permission:ShoppingLists"));

        shoppingLists.AddChild(MealPlannerAPIPermissions.ShoppingLists.Create, L("Permission:ShoppingLists.Create"));
        shoppingLists.AddChild(MealPlannerAPIPermissions.ShoppingLists.Update, L("Permission:ShoppingLists.Update"));
        shoppingLists.AddChild(MealPlannerAPIPermissions.ShoppingLists.Delete, L("Permission:ShoppingLists.Delete"));
        shoppingLists.AddChild(MealPlannerAPIPermissions.ShoppingLists.ManageItems, L("Permission:ShoppingLists.ManageItems"));
    }

    // ── Notifications ─────────────────────────────────────────────────────────

    private static void AddNotificationPermissions(PermissionGroupDefinition group)
    {
        var notifications = group.AddPermission(
            MealPlannerAPIPermissions.Notifications.Default,
            L("Permission:Notifications"));

        notifications.AddChild(MealPlannerAPIPermissions.Notifications.Delete, L("Permission:Notifications.Delete"));
    }

    // ── User Profiles ─────────────────────────────────────────────────────────

    private static void AddUserProfilePermissions(PermissionGroupDefinition group)
    {
        var userProfiles = group.AddPermission(
            MealPlannerAPIPermissions.UserProfiles.Default,
            L("Permission:UserProfiles"));

        userProfiles.AddChild(
            MealPlannerAPIPermissions.UserProfiles.UpdateOthers,
            L("Permission:UserProfiles.UpdateOthers"));
    }

    // ── Dashboard ─────────────────────────────────────────────────────────────

    private static void AddDashboardPermissions(PermissionGroupDefinition group)
    {
        group.AddPermission(
            MealPlannerAPIPermissions.Dashboard.Default,
            L("Permission:Dashboard"));
    }
    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MealPlannerAPIResource>(name);
    }
}
