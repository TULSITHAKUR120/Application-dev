using MudBlazor;

public static class LavenderTheme
{
    public static MudTheme GetTheme()
    {
        return new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = "#967bb6",
                Secondary = "#d8bfd8",
                Background = "#f5f5fa",
                Surface = "#ffffff",
                DrawerBackground = "#ffffff",
                AppbarBackground = "#967bb6",
                AppbarText = "#ffffff",
                TextPrimary = "#333333",
                TextSecondary = "#666666",
                Success = "#9ccc65",
                Warning = "#ffb74d",
                Error = "#ef5350",
                Info = "#42a5f5"
            },

            PaletteDark = new PaletteDark()
            {
                Primary = "#b39ddb",
                Secondary = "#9575cd",
                Background = "#121212",
                Surface = "#1e1e1e",
                DrawerBackground = "#1e1e1e",
                AppbarBackground = "#1e1e1e",
                AppbarText = "#ffffff",
                TextPrimary = "#e0e0e0",
                TextSecondary = "#b0b0b0"
            },

            Typography = new Typography()
            {
                // FIX: Use DefaultTypography instead of Default
                Default = new DefaultTypography()
                {
                    FontFamily = new[]
                    {
                        "Inter",
                        "Segoe UI",
                        "Roboto",
                        "Helvetica Neue",
                        "Arial",
                        "sans-serif"
                    }
                }
            },

            LayoutProperties = new LayoutProperties()
            {
                DefaultBorderRadius = "8px"
            }
        };
    }
}