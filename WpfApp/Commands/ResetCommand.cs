using Microsoft.Extensions.DependencyInjection;
using SharedLib.Services;
using System;
using System.Windows.Input;

namespace WpfApp.Commands;

class ResetCommand : ICommand
{
    private readonly IKitchenTimerService _kitchenTimer;

    public ResetCommand()
    {
        _kitchenTimer = App.Services.GetService<IKitchenTimerService>()!;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => _kitchenTimer.ResetTimer();
}
