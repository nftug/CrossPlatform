using Gtk;
using Microsoft.Extensions.DependencyInjection;
using SharedLib.Services;

namespace GtkApp;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Application.Init();

        var app = new Application("org.GtkApp.GtkApp", GLib.ApplicationFlags.None);
        app.Register(GLib.Cancellable.Current);

        var services = new ServiceCollection();
        services.AddScoped<IKitchenTimerService, KitchenTimerService>();
        services.AddTransient<MainWindow>();

        var provider = services.BuildServiceProvider();
        var win = provider.GetRequiredService<MainWindow>();
        app.AddWindow(win);
        win.Show();

        Application.Run();
    }
}
