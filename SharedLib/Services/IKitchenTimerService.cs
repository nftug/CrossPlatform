namespace SharedLib.Services;

public interface IKitchenTimerService
{
    event EventHandler<SecondsChangeEventArgs>? SecondsChanged;
    int Seconds { get; set; }
    TimerStatus Status { get; set; }
    void ToggleTimer();
    event EventHandler? TimerEnded;
}
