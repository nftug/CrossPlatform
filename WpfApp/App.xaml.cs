using Microsoft.Extensions.DependencyInjection;
using SharedLib.Services;
using System;
using System.Windows;

namespace WpfApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider Services { get; } = ConfigureServices();

    private static IServiceProvider ConfigureServices()
    {
        IServiceCollection? services = new ServiceCollection()
            .AddScoped<IKitchenTimerService, KitchenTimerService>();
        return services.BuildServiceProvider();
    }
}
