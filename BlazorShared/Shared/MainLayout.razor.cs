using BlazorShared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib.Services;

namespace BlazorShared.Shared;

public partial class MainLayout : LayoutComponentBase, IDisposable
{
    [Inject] private LayoutService LayoutService { get; set; } = null!;
    [Inject] private IKitchenTimerSingleton KitchenTimer { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

    private MudThemeProvider? _mudThemeProvider;

    protected override void OnInitialized()
    {
        LayoutService.MajorUpdateOccurred += LayoutServiceOnMajorUpdateOccurred;
        KitchenTimer.TimerEnded += OnEndedTimer;
    }

    public void Dispose()
    {
        LayoutService.MajorUpdateOccurred -= LayoutServiceOnMajorUpdateOccurred;
        KitchenTimer.TimerEnded -= OnEndedTimer;
        GC.SuppressFinalize(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ApplyUserPreferences();
            StateHasChanged();
        }
    }

    private async Task ApplyUserPreferences()
    {
        var defaultDarkMode = await _mudThemeProvider!.GetSystemPreference();
        await LayoutService.ApplyUserPreferences(defaultDarkMode);
    }

    private async void OnEndedTimer(object? sender, EventArgs e)
    {
        await JSRuntime.InvokeVoidAsync("alert", "時間です");
    }

    private void LayoutServiceOnMajorUpdateOccurred(object? sender, EventArgs e) => StateHasChanged();
}
