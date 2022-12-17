using Blazored.LocalStorage;
using BlazorShared.Models;
using BlazorShared.Services;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using SharedLib.Services;

namespace BlazorShared.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddMudServices();
        services.AddBlazoredLocalStorage();
        services.AddScoped<LayoutService>();
        services.AddTransient<UserPreferencesService>();
        services.AddSingleton<IKitchenTimerSingleton, KitchenTimerService>();
        services.AddTransient<IKitchenTimerTransient, KitchenTimerService>();
        services.AddTransient(sp => appSettings);

        return services;
    }
}
