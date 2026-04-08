using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Web.Theming.Layout;
using Volo.Abp.BlazoriseUI;

namespace MealPlannerAPI.BlazorWebUI.Components.Pages;

public partial class Index
{
    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;

    private void Login()
    {
        Navigation.NavigateTo("/Account/Login", true);
    }

    protected override void Dispose(bool disposing)
    {
        PageLayout.ShowToolbar = true;
    }
}
