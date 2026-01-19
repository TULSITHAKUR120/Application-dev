using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DailyJournal.Data.Entities
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Color { get; set; } = "#9C27B0";

        [Required]
        public bool IsPredefined { get; set; } = true;

        [Required]
        public int UsageCount { get; set; } = 0;

        [Required]
        public DateTime LastUsed { get; set; } = DateTime.UtcNow;
    }
}