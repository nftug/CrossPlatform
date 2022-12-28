using Microsoft.Extensions.DependencyInjection;
using SharedLib.Services;
using System;
using System.Windows.Input;

namespace WpfApp.Commands;

class StartStopCommand : ICommand
{
    private readonly IKitchenTimerService _kitchenTimer;

    public StartStopCommand()
    {
        _kitchenTimer = App.Services.GetService<IKitchenTimerService>()!;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => _kitchenTimer.ToggleTimer();
}
