using Microsoft.Extensions.DependencyInjection;
using SharedLib.Enums;
using SharedLib.Services;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using System;
using Reactive.Bindings;
using WpfApp.Commands;

namespace WpfApp.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    public readonly IKitchenTimerService _kitchenTimer;

    public ReactivePropertySlim<string> LeftOverTime { get; } = new();
    public ReactivePropertySlim<string> WindowTitle { get; } = new();
    public ReactivePropertySlim<string> ButtonStartStopText { get; } = new();

    public ReactivePropertySlim<bool> ButtonStartStopEnabled { get; } = new();
    public ReactivePropertySlim<bool> ButtonResetEnabled { get; } = new();
    public ReactivePropertySlim<bool> ButtonHourUpEnabled { get; } = new();
    public ReactivePropertySlim<bool> ButtonHourDownEnabled { get; } = new();
    public ReactivePropertySlim<bool> ButtonMinuteUpEnabled { get; } = new();
    public ReactivePropertySlim<bool> ButtonMinuteDownEnabled { get; } = new();
    public ReactivePropertySlim<bool> ButtonSecondUpEnabled { get; } = new();
    public ReactivePropertySlim<bool> ButtonSecondDownEnabled { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand StartStopCommand { get; private set; } = new StartStopCommand();
    public ICommand ResetCommand { get; private set; } = new ResetCommand();
    public ICommand HourUpCommand { get; private set; } = new SetTimeCommand(TimeUnit.Hour, UpDown.Up);
    public ICommand HourDownCommand { get; private set; } = new SetTimeCommand(TimeUnit.Hour, UpDown.Down);
    public ICommand MinuteUpCommand { get; private set; } = new SetTimeCommand(TimeUnit.Minute, UpDown.Up);
    public ICommand MinuteDownCommand { get; private set; } = new SetTimeCommand(TimeUnit.Minute, UpDown.Down);
    public ICommand SecondUpCommand { get; private set; } = new SetTimeCommand(TimeUnit.Second, UpDown.Up);
    public ICommand SecondDownCommand { get; private set; } = new SetTimeCommand(TimeUnit.Second, UpDown.Down);

    public MainWindowViewModel()
    {
        _kitchenTimer = App.Services.GetService<IKitchenTimerService>()!;
        _kitchenTimer.CombinedStatus.Subscribe(OnChangeTimerStatus);
        _kitchenTimer.TimerEnded += OnEndedTimer;
    }

    private void OnChangeTimerStatus(TimerCombinedStatus? e)
    {
        if (e == null) return;

        LeftOverTime.Value = TimeSpan.FromSeconds(e.Second).ToString(@"hh\:mm\:ss");
        WindowTitle.Value = !e.IsStopped
            ? $"残り時間　{LeftOverTime}{(e.IsPaused ? " (一時停止中)" : null)}"
            : "タイマー";
            
        ButtonStartStopText.Value =
            e.IsActivated ? "Pause"
            : e.IsPaused ? "Restart"
            : "Start";
            
        ButtonStartStopEnabled.Value = e.StartStopEnabled;
        ButtonResetEnabled.Value = e.ResetEnabled;

        ButtonHourUpEnabled.Value = e.GetSetTimeEnabled(TimeUnit.Hour, UpDown.Up);
        ButtonHourDownEnabled.Value = e.GetSetTimeEnabled(TimeUnit.Hour, UpDown.Down);
        ButtonSecondUpEnabled.Value = e.GetSetTimeEnabled(TimeUnit.Second, UpDown.Up);
        ButtonSecondDownEnabled.Value = e.GetSetTimeEnabled(TimeUnit.Second, UpDown.Down);
        ButtonMinuteUpEnabled.Value = e.GetSetTimeEnabled(TimeUnit.Minute, UpDown.Up);
        ButtonMinuteDownEnabled.Value = e.GetSetTimeEnabled(TimeUnit.Minute, UpDown.Down);
    }

    private void OnEndedTimer(object? sender, EventArgs e)
    {
        MessageBox.Show("時間です", "タイマー", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
