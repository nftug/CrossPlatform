using Microsoft.Extensions.DependencyInjection;
using SharedLib.Enums;
using SharedLib.Services;
using System;
using System.Windows.Input;

namespace WpfApp.Commands;

class SetTimeCommand : ICommand
{
    private readonly IKitchenTimerService _kitchenTimer;
    private TimeUnit Unit { get; }
    private UpDown UpDown { get; }

    public SetTimeCommand(TimeUnit unit, UpDown upDown)
    {
        _kitchenTimer = App.Services.GetService<IKitchenTimerService>()!;
        Unit = unit;
        UpDown = upDown;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        if (UpDown == UpDown.Up) _kitchenTimer.Second.Value += (int)Unit;
        else _kitchenTimer.Second.Value -= (int)Unit;
    }
}
