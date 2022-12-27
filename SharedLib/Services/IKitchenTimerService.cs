using System.ComponentModel;
using Reactive.Bindings;

namespace SharedLib.Services;

public interface IKitchenTimerService : IDisposable
{
    ReactivePropertySlim<int> Second { get; }
    ReactivePropertySlim<TimerStatus> Status { get; }
    ReadOnlyReactivePropertySlim<TimerCombinedStatus?> CombinedStatus { get; }

    void ToggleTimer();
    event EventHandler? TimerEnded;
    void ResetTimer();
}
