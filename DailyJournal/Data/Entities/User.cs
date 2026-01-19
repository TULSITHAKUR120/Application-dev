using System;
using System.ComponentModel.DataAnnotations;

namespace DailyJournal.Data.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? PIN { get; set; }

        [Required]
        public string Salt { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        [Required]
        public bool IsDarkTheme { get; set; } = false;

        [MaxLength(7)]
        public string PrimaryColor { get; set; } = "#2196F3";

        [MaxLength(7)]
        public string SecondaryColor { get; set; } = "#FF9800";

        [Required]
        public bool EnableBiometric { get; set; } = false;
    }
}