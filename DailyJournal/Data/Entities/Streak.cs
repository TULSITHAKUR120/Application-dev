using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DailyJournal.Data.Entities
{
    public class Streak
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CurrentStreak { get; set; } = 0;

        [Required]
        public int LongestStreak { get; set; } = 0;

        [Required]
        public DateTime LastEntryDate { get; set; } = DateTime.MinValue;

        [Required]
        public int TotalEntries { get; set; } = 0;

        [Required]
        public int TotalMissedDays { get; set; } = 0;

        [Required]
        public DateTime StreakStartDate { get; set; } = DateTime.UtcNow;

        public DateTime? StreakEndDate { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}

