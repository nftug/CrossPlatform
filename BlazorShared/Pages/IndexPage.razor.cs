using System.Reactive.Disposables;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SharedLib.Services;

namespace BlazorShared.Pages;

public partial class IndexPage : ComponentBase, IDisposable
{
    [Inject] private IKitchenTimerTransient KitchenTimerTransient { get; set; } = null!;
    [Inject] private IKitchenTimerSingleton KitchenTimerSingleton { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

    private readonly CompositeDisposable _disposables = new();
    private IKitchenTimerService KitchenTimer = null!;
    private TimerCombinedStatus _status = null!;

    private string? _leftoverTimeText;
    private string? _toggleButtonText;

    private ReactivePropertySlim<bool> IsTimerGlobal { get; set; } = null!;

    protected override void OnInitialized()
    {
        KitchenTimerTransient.CombinedStatus.Subscribe(OnChangeTimerStatus);
        KitchenTimerSingleton.CombinedStatus.Subscribe(OnChangeTimerStatus);
        KitchenTimerTransient.TimerEnded += OnEndedTimer;

        IsTimerGlobal = new ReactivePropertySlim<bool>().AddTo(_disposables);
        IsTimerGlobal.Subscribe(OnSetIsTimerGlobal);
        IsTimerGlobal.Value =
            KitchenTimerSingleton.Second.Value > 0
            && KitchenTimerSingleton.Status.Value != TimerStatus.Stopped;
    }

    public void Dispose()
    {
        _disposables.Dispose();
        KitchenTimerTransient.TimerEnded -= OnEndedTimer;
        GC.SuppressFinalize(this);
    }

    private void OnSetIsTimerGlobal(bool isTimerGlobal)
    {
        bool isFirstRender = KitchenTimer is null;
        int seconds = KitchenTimer?.Second.Value ?? 0;

        KitchenTimer = isTimerGlobal ? KitchenTimerSingleton : KitchenTimerTransient;
        KitchenTimer.Second.Value = isFirstRender ? KitchenTimer.Second.Value : seconds;
    }

    private void OnChangeTimerStatus(TimerCombinedStatus? e)
    {
        if (e == null) return;

        _status = e;
        _leftoverTimeText = TimeSpan.FromSeconds(e.Second).ToString(@"hh\:mm\:ss");
        _toggleButtonText =
            e.IsActivated ? "Pause"
            : e.IsPaused ? "Restart"
            : "Start";

        InvokeAsync(StateHasChanged);
    }

    private async void OnEndedTimer(object? sender, EventArgs e)
    {
        await JSRuntime.InvokeVoidAsync("alert", "時間です");
    }

    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        if (IsTimerGlobal.Value) return;
        if (KitchenTimerTransient.Status.Value == TimerStatus.Stopped) return;

        var previousStatus = KitchenTimerTransient.Status.Value;

        KitchenTimerTransient.Status.Value = TimerStatus.Paused;
        bool result = await JSRuntime.InvokeAsync<bool>("confirm", "タイマーが動作中です。終了しますか？");
        KitchenTimerTransient.Status.Value = previousStatus;

        if (!result) context.PreventNavigation();
    }
}