using Blazorise;
using Castle.Core.Logging;
using MealPlannerAPI.Settings;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Notifications;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.Uow;

namespace MealPlannerAPI.BlazorHost.Components.Settings;

public partial class MealPlannerGeneralSettingComponent
{
    [Inject]
    private ISettingManager SettingManager { get; set; } = default!;

    [Inject]
    private ISettingProvider SettingProvider { get; set; } = default!;
    [Inject]
    private IUiNotificationService UiNotificationService { get; set; } = default!;
    [Inject]
    private IUnitOfWorkManager UnitOfWorkManager { get; set; } = default!;

    protected bool EnableUI { get; set; }
    protected override async Task OnInitializedAsync()
    {

        EnableUI = await SettingProvider.GetAsync<bool>(MealPlannerAPISettings.EnableUI, defaultValue: true);

    }

    protected async Task SaveAsync()
    {
        try
        {
            // 1. Begin the Unit of Work transaction
            //using (var uow = UnitOfWorkManager.Begin())
            //{

            //}
            Console.WriteLine($"Saving setting: {MealPlannerAPISettings.EnableUI} = {EnableUI}");
            // 2. Queue the setting change
            await SettingManager.SetGlobalAsync(
               name: MealPlannerAPISettings.EnableUI,
               value: EnableUI.ToString().ToLower()
            );

            // 3. Commit the transaction to the database! (This runs EF Core SaveChanges)
            //await uow.CompleteAsync();
            Console.WriteLine($"Setting saved successfully: {MealPlannerAPISettings.EnableUI} = {EnableUI}");
            await UiNotificationService.Success(L["SavedSuccessfully"]);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving setting: {ex.Message}");
            await UiNotificationService.Error("Failed to save: " + ex.Message);
        }
    }
    private async Task OnSwitchValueChanged(bool newValue)
    {
        EnableUI = newValue;
        await SaveAsync(); // Wait for the save to finish
    }
}
