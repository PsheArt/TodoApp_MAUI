using Microsoft.Maui.Storage;

namespace TodoMauiApp.Services;

public class ThemeService : IThemeService
{
    private const string ThemeKey = "UserTheme";

    public event Action? ThemeChanged;

    public bool IsDarkMode { get; private set; }
    public ThemeService()
    {
        LoadTheme();
        ApplyTheme();
    }

    public void SetTheme(bool isDark)
    {
        IsDarkMode = isDark;
        Preferences.Set(ThemeKey, isDark); 
        ApplyTheme();
        ThemeChanged?.Invoke();
    }

    private void LoadTheme()
    {
        IsDarkMode = Preferences.Get(ThemeKey, AppInfo.RequestedTheme == AppTheme.Dark);
    }

    private void ApplyTheme()
    {
        if (Application.Current != null)
            Application.Current.UserAppTheme = IsDarkMode ? AppTheme.Dark : AppTheme.Light;
    }
}