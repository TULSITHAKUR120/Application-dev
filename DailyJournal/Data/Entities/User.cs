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
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? PIN { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginAt { get; set; }

        [MaxLength(7)]
        public string? PrimaryColor { get; set; }

        [MaxLength(7)]
        public string? SecondaryColor { get; set; }

        // Navigation property
        public UserSettings? UserSettings { get; set; }

        public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();

    }
}