using System.Reactive.Disposables;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SharedLib.Enums;

namespace SharedLib.Services;

public class KitchenTimerService : IKitchenTimerTransient, IKitchenTimerSingleton
{
    private readonly CompositeDisposable _disposables = new();
    public readonly ReactiveTimer _timer;

    public ReactivePropertySlim<int> Second { get; }
    public ReactivePropertySlim<TimerStatus> Status { get; }
    public ReadOnlyReactivePropertySlim<TimerCombinedStatus?> CombinedStatus { get; }

    public KitchenTimerService()
    {
        Second = new ReactivePropertySlim<int>().AddTo(_disposables);
        Status = new ReactivePropertySlim<TimerStatus>().AddTo(_disposables);

        CombinedStatus = Second
            .CombineLatest(Status, (second, status) => new TimerCombinedStatus(second, status))
            .ToReadOnlyReactivePropertySlim()
            .AddTo(_disposables);

        _timer = new ReactiveTimer(TimeSpan.FromSeconds(1)).AddTo(_disposables);
        _timer.Subscribe(async _ =>
        {
            Second.Value--;

            if (Second.Value <= 0)
            {
                Status.Value = TimerStatus.Stopped;
                await Task.Delay(10);
                TimerEnded?.Invoke(this, EventArgs.Empty);
            }
        });

        Second.Where(v => v < 0).Subscribe(_ => Second.Value = 0);

        Status
            .Where(v => v == TimerStatus.Activated)
            .Subscribe(_ => _timer.Start(TimeSpan.FromSeconds(1)));
        Status
            .Where(v => v != TimerStatus.Activated)
            .Subscribe(_ => _timer.Stop());
        Status
            .Where(v => v == TimerStatus.Stopped)
            .Subscribe(_ => Second.Value = 0);

        CombinedStatus
            .Subscribe(v => Console.WriteLine($"sec: {v!.Second}, status: {v.Status}"));
    }

    public void Dispose()
    {
        _disposables.Dispose();
        GC.SuppressFinalize(this);
    }

    public void ToggleTimer()
    {
        Status.Value = Status.Value == TimerStatus.Activated ? TimerStatus.Paused : TimerStatus.Activated;
    }

    public void ResetTimer()
    {
        Second.Value = 0;
        Status.Value = TimerStatus.Stopped;
    }

    public event EventHandler? TimerEnded;
}

public enum TimerStatus
{
    Stopped,
    Activated,
    Paused,
}

public class TimerCombinedStatus
{
    public int Second { get; init; }
    public TimerStatus Status { get; init; }

    public TimerCombinedStatus(int second, TimerStatus status)
    {
        Second = second;
        Status = status;
    }

    public bool StartStopEnabled => Second > 0;
    public bool ResetEnabled => Second > 0 && !IsActivated;

    public bool GetSetTimeEnabled(TimeUnit unit, UpDown upDown)
        => upDown == UpDown.Down
            ? Second > 0 && !IsActivated
            : Second < TimeSpan.FromHours(24).TotalSeconds - (int)unit && !IsActivated;

    public bool IsActivated => Status == TimerStatus.Activated;
    public bool IsPaused => Status == TimerStatus.Paused;
    public bool IsStopped => Status == TimerStatus.Stopped;
}