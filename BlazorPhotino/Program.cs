using Photino.Blazor;
using BlazorShared.Extensions;
using System.Drawing;

namespace BlazorPhotino;

public class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

        appBuilder.Services.AddServices(new() { UseNavigationLock = false });

        // register root component and selector
        appBuilder.RootComponents.Add<App>("#app");

        var app = appBuilder.Build();

        // customize window
        app.MainWindow
            .SetTitle("Photino Blazor Sample")
            .SetIconFile("favicon.ico")
            .SetUseOsDefaultSize(false)
            .SetSize(new Size(900, 600))
            .Center();

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            app.MainWindow.OpenAlertWindow("Fatal exception", error.ExceptionObject.ToString());
        };

        app.Run();

    }
}
