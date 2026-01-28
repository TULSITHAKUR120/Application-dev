using Microsoft.JSInterop;
using MudBlazor;
using System.Text.Json;

namespace DailyJournal.Services
{
    public enum AppTheme
    {
        Light,
        Dark,
        Black,
        System
    }

    public class ThemeSettings
    {
        public AppTheme Theme { get; set; } = AppTheme.System;
        public string PrimaryColor { get; set; } = "Primary";
        public string AccentColor { get; set; } = "Default";

        // Calculated property for MudBlazor's IsDarkMode toggle
        public bool IsDarkMode(AppTheme systemTheme)
        {
            var effective = Theme == AppTheme.System ? systemTheme : Theme;
            return effective == AppTheme.Dark || effective == AppTheme.Black;
        }
    }

    public class ThemeService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly ISnackbar _snackbar;
        private ThemeSettings _currentSettings = new();

        public ThemeSettings CurrentSettings => _currentSettings;

        // Helper to check dark mode based on current MAUI system theme
        public bool IsDarkMode => _currentSettings.IsDarkMode(GetMauiSystemTheme());

        public event Action<AppTheme>? ThemeChanged;

        public ThemeService(IJSRuntime jsRuntime, ISnackbar snackbar)
        {
            _jsRuntime = jsRuntime;
            _snackbar = snackbar;
        }

        public async Task InitializeAsync()
        {
            await LoadThemeAsync();

            if (Application.Current != null)
            {
                // Sync with MAUI system theme changes
                Application.Current.RequestedThemeChanged += (s, e) =>
                {
                    if (_currentSettings.Theme == AppTheme.System)
                    {
                        // Fire-and-forget theme update
                        _ = ApplyTheme();
                        ThemeChanged?.Invoke(GetEffectiveTheme());
                    }
                };
            }
        }

        public async Task SetTheme(AppTheme theme)
        {
            if (_currentSettings.Theme != theme)
            {
                _currentSettings.Theme = theme;
                await ApplyTheme();
                await SaveThemeAsync();
                ThemeChanged?.Invoke(GetEffectiveTheme());
                _snackbar.Add($"Switched to {theme} theme", Severity.Info);
            }
        }

        public async Task SetPrimaryColor(string color)
        {
            _currentSettings.PrimaryColor = color;
            await SaveThemeAsync();
            ThemeChanged?.Invoke(GetEffectiveTheme());
        }

        public AppTheme GetEffectiveTheme()
        {
            if (_currentSettings.Theme == AppTheme.System)
            {
                return GetMauiSystemTheme();
            }
            return _currentSettings.Theme;
        }

        private AppTheme GetMauiSystemTheme()
        {
            return Application.Current?.RequestedTheme == Microsoft.Maui.ApplicationModel.AppTheme.Dark
                ? AppTheme.Dark
                : AppTheme.Light;
        }

        private async Task ApplyTheme()
        {
            var effectiveTheme = GetEffectiveTheme();

            // Updating Native MAUI (Status bars, etc.)
            if (Application.Current != null)
            {
                Application.Current.UserAppTheme = effectiveTheme switch
                {
                    AppTheme.Dark => Microsoft.Maui.ApplicationModel.AppTheme.Dark,
                    AppTheme.Black => Microsoft.Maui.ApplicationModel.AppTheme.Dark,
                    AppTheme.Light => Microsoft.Maui.ApplicationModel.AppTheme.Light,
                    _ => Microsoft.Maui.ApplicationModel.AppTheme.Unspecified
                };
            }

            //  Update CSS for OLED Black
            var bodyClass = effectiveTheme == AppTheme.Black ? "theme-black" : "";
            try
            {
                await _jsRuntime.InvokeVoidAsync("setThemeClass", bodyClass);
            }
            catch { /* JS not ready */ }
        }

        public async Task LoadThemeAsync()
        {
            try
            {
                // In MAUI, you can also use Microsoft.Maui.Storage.Preferences for better reliability
                var saved = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "journalTheme");
                if (!string.IsNullOrEmpty(saved))
                {
                    _currentSettings = JsonSerializer.Deserialize<ThemeSettings>(saved) ?? new ThemeSettings();
                }
            }
            catch { }
            await ApplyTheme();
        }

        public async Task SaveThemeAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_currentSettings);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "journalTheme", json);
            }
            catch { }
        }

        public void ResetToDefault()
        {
            _currentSettings = new ThemeSettings();
            _ = ApplyTheme();
            _ = SaveThemeAsync();
            ThemeChanged?.Invoke(AppTheme.System);
        }
    }
}

