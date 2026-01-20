using System.ComponentModel.DataAnnotations;

namespace DailyJournal.Data.Models
{
    public class RegisterModel
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string PIN { get; set; } = string.Empty;
        public string ConfirmPIN { get; set; } = string.Empty;
        public bool UsePIN { get; set; } = false;
        public bool PrefersDarkTheme { get; set; } = false;
        public string ThemeColor { get; set; } = "#2196F3";
    }

    public class RegistrationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
        public bool IsPINLogin { get; set; }
    }
}