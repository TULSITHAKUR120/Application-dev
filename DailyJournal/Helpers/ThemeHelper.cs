using DailyJournal.Data.Entities;
using DailyJournal.Services;

namespace DailyJournal.Helpers
{
    public static class ThemeHelper
    {
        public static async Task ApplyThemeAsync()
        {
            try
            {
                var userService = new UserService();
                var user = await userService.GetCurrentUserAsync();

                if (user != null)
                {
                    if (user.IsDarkTheme)
                    {
                        Application.Current.UserAppTheme = AppTheme.Dark;
                    }
                    else
                    {
                        Application.Current.UserAppTheme = AppTheme.Light;
                    }

                    // Apply custom colors if needed
                    ApplyCustomColors(user.PrimaryColor, user.SecondaryColor);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying theme: {ex.Message}");
            }
        }

        private static void ApplyCustomColors(string primaryColor, string secondaryColor)
        {
            // This is a simplified example - in a real app you would update resources
            Application.Current.Resources["PrimaryColor"] = Color.FromArgb(primaryColor);
            Application.Current.Resources["SecondaryColor"] = Color.FromArgb(secondaryColor);
        }

        public static Color GetColorFromHex(string hexColor)
        {
            if (string.IsNullOrEmpty(hexColor))
                return Colors.Black;

            hexColor = hexColor.TrimStart('#');

            if (hexColor.Length == 6)
            {
                hexColor = "FF" + hexColor; // Add alpha
            }

            if (uint.TryParse(hexColor, System.Globalization.NumberStyles.HexNumber, null, out uint argb))
            {
                return Color.FromUint(argb);
            }

            return Colors.Black;
        }
    }
}