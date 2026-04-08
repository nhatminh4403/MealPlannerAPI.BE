using MealPlannerAPI.Localization;
using MealPlannerAPI.MultiTenancy;
using MealPlannerAPI.Permissions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Blazor;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TenantManagement.Blazor.Navigation;
using Volo.Abp.UI.Navigation;

namespace MealPlannerAPI.BlazorWebUI.Menus
{
    public class MenuContributor : IMenuContributor
    {
        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if (context.Menu.Name == StandardMenus.Main)
            {
                await ConfigureMainMenuAsync(context);
            }
        }
        private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<MealPlannerAPIResource>();

            context.Menu.Items.Insert(
                0,
                new ApplicationMenuItem(
                    MealPlannerAPIMenus.Home,
                    l["Menu:Home"],
                    "/",
                    icon: "fas fa-home",
                    order: 1
                )
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

            context.Menu.AddItem(
             new ApplicationMenuItem(
                 "MealPlanner",
                 l["Menu:Recipes"],
                 icon: "fa fa-book"
             ).AddItem(
                 new ApplicationMenuItem(
                     "Recipes.Recipes",
                     l["Menu:Recipes"],
                     url: "/recipes"
                 ).RequirePermissions(MealPlannerAPIPermissions.Recipes.Default)
             )
            );

            return Task.CompletedTask;
        }
    }
}
