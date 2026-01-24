using DailyJournal.Data.Models;
using Microsoft.JSInterop;
using MudBlazor;
using System.Text.Json;

namespace DailyJournal.Services
{
    public class ThemeSettings
    {
        public string ThemeType { get; set; } = "light"; // light, dark, black
        public string PrimaryColor { get; set; } = "Primary";
        public string AccentColor { get; set; } = "Default";
        public bool IsDarkMode { get; set; } = false;
        public string? CustomBackground { get; set; }
    }

    public class ThemeService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly ISnackbar _snackbar;
        private ThemeSettings _currentTheme = new();

        public bool IsDarkMode => _currentTheme.IsDarkMode;
        public ThemeSettings CurrentTheme => _currentTheme;
        public event Action? OnThemeChanged;

        public ThemeService(IJSRuntime jsRuntime, ISnackbar snackbar)
        {
            _jsRuntime = jsRuntime;
            _snackbar = snackbar;
        }

        public async Task InitializeAsync()
        {
            await LoadThemeAsync();
            await ApplyTheme();
        }

        public async Task SetDarkMode(bool darkMode)
        {
            _currentTheme.IsDarkMode = darkMode;
            _currentTheme.ThemeType = darkMode ? "dark" : "light";
            await ApplyTheme();
            OnThemeChanged?.Invoke();
        }

        public async Task SetThemeType(string themeType)
        {
            _currentTheme.ThemeType = themeType.ToLower();
            _currentTheme.IsDarkMode = themeType != "light";
            await ApplyTheme();
            OnThemeChanged?.Invoke();
            _snackbar.Add($"Switched to {themeType} theme", Severity.Info);
        }

        public async Task SetPrimaryColor(string color)
        {
            _currentTheme.PrimaryColor = color;
            await ApplyTheme();
            OnThemeChanged?.Invoke();
        }

        public async Task SetAccentColor(string color)
        {
            _currentTheme.AccentColor = color;
            await ApplyTheme();
            OnThemeChanged?.Invoke();
        }

        private async Task ApplyTheme()
        {
            // Save to local storage
            await SaveThemeAsync();

            // Apply CSS class to body
            var bodyClass = _currentTheme.ThemeType switch
            {
                "black" => "theme-black",
                "dark" => "mud-dark-theme",
                _ => ""
            };

            await _jsRuntime.InvokeVoidAsync("setThemeClass", bodyClass);
        }

        public async Task LoadThemeAsync()
        {
            try
            {
                var saved = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "journalTheme");
                if (!string.IsNullOrEmpty(saved))
                {
                    _currentTheme = JsonSerializer.Deserialize<ThemeSettings>(saved) ?? new ThemeSettings();
                }
            }
            catch
            {
                _currentTheme = new ThemeSettings();
            }
        }

        public async Task SaveThemeAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_currentTheme);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "journalTheme", json);
            }
            catch
            {
                // Silently fail if localStorage is not available
            }
        }

        public void ResetToDefault()
        {
            _currentTheme = new ThemeSettings();
            OnThemeChanged?.Invoke();
        }
    }
}