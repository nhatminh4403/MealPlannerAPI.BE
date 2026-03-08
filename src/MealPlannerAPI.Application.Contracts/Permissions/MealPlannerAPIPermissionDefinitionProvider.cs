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
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MealPlannerAPIResource>(name);
    }
}
