using MealPlannerAPI.Localization;
using MealPlannerAPI.MultiTenancy;
using MealPlannerAPI.Permissions;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Blazor;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TenantManagement.Blazor.Navigation;
using Volo.Abp.UI.Navigation;

namespace MealPlannerAPI.BlazorHost.Menus
{
    public class MealPlannerAPIMenuContributor : IMenuContributor
    {
        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if (context.Menu.Name == StandardMenus.Main)
            {
                await ConfigureMainMenuAsync(context);
            }
        }
        private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<MealPlannerAPIResource>();

            context.Menu.Items.Insert(
                0,
                new ApplicationMenuItem(
                    MealPlannerAPIMenus.Home,
                    l["Menu:Home"],
                    "/",
                    icon: "fa-solid fa-home",
                    order: 1
                )
            );

            context.Menu.Items.Insert(
                1,
                new ApplicationMenuItem(
                    "MealPlannerAPI",
                    l["Menu:Recipes"],
                    icon: "fa-solid fa-kitchen-set",
                    //url: "/recipes",
                    order: 2
                ).AddItem(
                    new ApplicationMenuItem(
                        "MealPlannerAPI.Recipes",
                        l["Menu:AllRecipes"],
                        url: "/recipes",
                        order: 0
                    ).RequirePermissions(MealPlannerAPIPermissions.Recipes.Default)
                )
                .AddItem(
                    new ApplicationMenuItem(
                        "MealPlannerAPI.Recipes.Trending",
                        l["Menu:Trending"],
                        url: "/recipes/trending",
                        order: 1
                    ).RequirePermissions(MealPlannerAPIPermissions.Recipes.Default)
                ).AddItem(
                    new ApplicationMenuItem(
                        "MealPlannerAPI.Recipes.TopRated",
                        l["Menu:TopRated"],
                        url: "/recipes/top-rated",
                        order: 2
                    ).RequirePermissions(MealPlannerAPIPermissions.Recipes.Default)
                )
            );

            context.Menu.Items.Insert(
                2,
                new ApplicationMenuItem(
                    "MealPlannerAPI.Ingredients",
                    l["Menu:Ingredients"],
                    url: "/ingredients",
                    icon: "fa-solid fa-carrot",
                    order: 3
                ).RequirePermissions(MealPlannerAPIPermissions.Recipes.Default)
            );

            context.Menu.Items.Insert(
                3,
                new ApplicationMenuItem(
                    "MealPlannerAPI.Keys",
                    l["Menu:KeyManagement"],
                    url: "/keys",
                    icon: "fa-solid fa-list",
                    order: 4
                ).RequirePermissions(MealPlannerAPIPermissions.Dashboard.Default)
            );

            //Administration
            var administration = context.Menu.GetAdministration();
            administration.Order = 6;

            if (MultiTenancyConsts.IsEnabled)
            {
                administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
            }
            else
            {
                administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
            }

            administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
            administration.SetSubItemOrder(SettingManagementMenus.GroupName, 3);
        }
    }
}
