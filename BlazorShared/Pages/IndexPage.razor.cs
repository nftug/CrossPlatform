using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using SharedLib.Services;

namespace BlazorShared.Pages;

public partial class IndexPage : ComponentBase, IDisposable
{
    [Inject] private IKitchenTimerTransient KitchenTimerTransient { get; set; } = null!;
    [Inject] private IKitchenTimerSingleton KitchenTimerSingleton { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

    private IKitchenTimerService KitchenTimer = null!;

    private string? _leftoverTimeText;
    private string? _toggleButtonText;
    private bool _toggleButtonEnabled;
    private bool _resetButtonEnabled;
    private bool _secUpButtonEnabled;
    private bool _secDownButtonEnabled;
    private bool _minUpButtonEnabled;
    private bool _minDownButtonEnabled;
    private bool _hourUpButtonEnabled;
    private bool _hourDownButtonEnabled;

    private bool _isTimerGlobal;
    private bool IsTimerGlobal
    {
        get => _isTimerGlobal;
        set
        {
            _isTimerGlobal = value;
            OnAfterSetIsTimerGlobal();
        }
    }

    protected override void OnInitialized()
    {
        KitchenTimerTransient.SecondsChanged += OnChangeSeconds;
        KitchenTimerSingleton.SecondsChanged += OnChangeSeconds;
        KitchenTimerTransient.TimerEnded += OnEndedTimer;

        _isTimerGlobal =
            KitchenTimerSingleton.Seconds > 0 && KitchenTimerSingleton.Status != TimerStatus.Stopped;
        OnAfterSetIsTimerGlobal();
    }

    public void Dispose()
    {
        KitchenTimerTransient.SecondsChanged -= OnChangeSeconds;
        KitchenTimerSingleton.SecondsChanged -= OnChangeSeconds;
        KitchenTimerTransient.TimerEnded -= OnEndedTimer;
        GC.SuppressFinalize(this);
    }

    private void OnAfterSetIsTimerGlobal()
    {
        bool isFirstRender = KitchenTimer is null;
        int seconds = KitchenTimer?.Seconds ?? 0;

        KitchenTimer = _isTimerGlobal ? KitchenTimerSingleton : KitchenTimerTransient;
        KitchenTimer.Seconds = isFirstRender ? KitchenTimer.Seconds : seconds;
    }

    private void OnChangeSeconds(object? sender, SecondsChangeEventArgs e)
    {
        InvokeAsync(() =>
        {
            _leftoverTimeText = TimeSpan.FromSeconds(e.Seconds).ToString(@"hh\:mm\:ss");

            _toggleButtonText =
                e.IsActivated ? "Pause"
                : e.IsPaused ? "Restart"
                : "Start";

            _toggleButtonEnabled = e.Seconds > 0;
            _resetButtonEnabled = e.Seconds > 0 && !e.IsActivated;
            _secUpButtonEnabled = !e.IsActivated;
            _secDownButtonEnabled = e.Seconds > 0 && !e.IsActivated;
            _minUpButtonEnabled = !e.IsActivated;
            _minDownButtonEnabled = e.Seconds > 0 && !e.IsActivated;
            _hourUpButtonEnabled = !e.IsActivated;
            _hourDownButtonEnabled = e.Seconds > 0 && !e.IsActivated;

            StateHasChanged();
        });
    }

    private async void OnEndedTimer(object? sender, EventArgs e)
    {
        await JSRuntime.InvokeVoidAsync("alert", "時間です");
    }

    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        if (_isTimerGlobal) return;
        if (KitchenTimerTransient.Status == TimerStatus.Stopped) return;

        var previousStatus = KitchenTimerTransient.Status;

        KitchenTimerTransient.Status = TimerStatus.Paused;
        bool result = await JSRuntime.InvokeAsync<bool>("confirm", "タイマーが動作中です。終了しますか？");
        KitchenTimerTransient.Status = previousStatus;

        if (!result) context.PreventNavigation();
    }
}