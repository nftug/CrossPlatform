namespace BlazorShared.Services;

public class LayoutService
{
    private readonly UserPreferencesService _userPreferencesService;
    private UserPreferences _userPreferences = null!;

    public LayoutService(UserPreferencesService userPreferencesService)
    {
        _userPreferencesService = userPreferencesService;
    }

    public bool IsDarkMode { get; private set; } = false;

    public void SetDarkMode(bool value)
    {
        IsDarkMode = value;
    }

    public async Task ApplyUserPreferences(bool isDarkModeDefaultTheme)
    {
        _userPreferences = await _userPreferencesService.LoadUserPreferences();

        if (_userPreferences != null)
        {
            IsDarkMode = _userPreferences.DarkTheme;
        }
        else
        {
            IsDarkMode = isDarkModeDefaultTheme;
            _userPreferences = new UserPreferences { DarkTheme = IsDarkMode };
            await _userPreferencesService.SaveUserPreferences(_userPreferences);
        }
    }

    public event EventHandler? MajorUpdateOccurred;

    private void OnMajorUpdateOccurred() => MajorUpdateOccurred?.Invoke(this, EventArgs.Empty);

    public async Task ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
        _userPreferences.DarkTheme = IsDarkMode;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccurred();
    }
}