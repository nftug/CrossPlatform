using Blazored.LocalStorage;

namespace BlazorShared.Services;

public class UserPreferencesService
{
    private readonly ILocalStorageService _localStorage;
    private const string Key = "userPreferences";

    public UserPreferencesService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<UserPreferences> LoadUserPreferences()
        => await _localStorage.GetItemAsync<UserPreferences>(Key);

    public async Task SaveUserPreferences(UserPreferences userPreferences)
        => await _localStorage.SetItemAsync(Key, userPreferences);
}

public class UserPreferences
{
    public bool DarkTheme { get; set; }
}