using System;

namespace DailyJournal.Data.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? PIN { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsDarkTheme { get; set; } = false;
        public string PrimaryColor { get; set; } = "#2196F3";
        public string SecondaryColor { get; set; } = "#FF9800";
        public bool EnableBiometric { get; set; } = false;
        public bool IsSetupComplete { get; set; } = false;

        // Helper properties
        public string DisplayName => Username;
        public string ThemeName => IsDarkTheme ? "Dark" : "Light";
        public string LastLoginText => LastLoginAt.HasValue
            ? LastLoginAt.Value.ToString("MMM d, yyyy h:mm tt")
            : "Never";
        public string AccountAge
        {
            get
            {
                var age = DateTime.UtcNow - CreatedAt;
                if (age.TotalDays >= 365)
                    return $"{age.TotalDays / 365:F0} years";
                if (age.TotalDays >= 30)
                    return $"{age.TotalDays / 30:F0} months";
                if (age.TotalDays >= 7)
                    return $"{age.TotalDays / 7:F0} weeks";
                return $"{age.TotalDays:F0} days";
            }
        }
    }
}