using Timer = System.Timers.Timer;

namespace SharedLib.Services;

public class KitchenTimerService : IKitchenTimerTransient, IKitchenTimerSingleton
{
    private Timer? _timer;

    public event EventHandler<SecondsChangeEventArgs>? SecondsChanged;
    private int _seconds;
    public int Seconds
    {
        get => _seconds;
        set
        {
            _seconds = value > 0 ? value : 0;
            SecondsChanged?.Invoke(this, new() { Seconds = _seconds, Status = Status });
        }
    }

    private bool _isPaused;
    public TimerStatus Status
    {
        get =>
            _timer?.Enabled == true ? TimerStatus.Activated
            : _isPaused ? TimerStatus.Paused
            : TimerStatus.Stopped;
        set
        {
            if (value == TimerStatus.Activated)
            {
                _timer = new Timer(1000);
                _timer.Elapsed += (_, _) => CountDown();
                _timer.Enabled = true;
            }
            else if (_timer != null)
            {
                _timer.Enabled = false;
                _timer.Dispose();
            }
            _isPaused = value == TimerStatus.Paused;
            Seconds = value == TimerStatus.Stopped ? 0 : _seconds;
        }
    }

    public void ToggleTimer()
    {
        Status = Status == TimerStatus.Activated ? TimerStatus.Paused : TimerStatus.Activated;
    }

    public event EventHandler? TimerEnded;
    private void CountDown()
    {
        Seconds--;
        if (_seconds == 0)
        {
            TimerEnded?.Invoke(this, EventArgs.Empty);
            Status = TimerStatus.Stopped;
        }
    }
}

public enum TimerStatus
{
    Stopped,
    Activated,
    Paused,
}

public class SecondsChangeEventArgs : EventArgs
{
    public int Seconds { get; init; }
    public TimerStatus Status { get; init; }

    public bool IsActivated => Status == TimerStatus.Activated;
    public bool IsPaused => Status == TimerStatus.Paused;
    public bool IsStopped => Status == TimerStatus.Stopped;
}