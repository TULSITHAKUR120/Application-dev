using System;
using System.ComponentModel.DataAnnotations;

namespace DailyJournal.Data.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? PIN { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDarkTheme { get; set; } = false;

        [MaxLength(7)]
        public string PrimaryColor { get; set; } = "#2196F3";

        [MaxLength(7)]
        public string SecondaryColor { get; set; } = "#FF9800";

        // Navigation property
        public virtual UserSettings UserSettings { get; set; } = null!;
    }
}