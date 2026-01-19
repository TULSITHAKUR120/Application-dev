using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DailyJournal.Data.Entities
{
    public class JournalEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime EntryDate { get; set; } = DateTime.Today;

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = "Untitled Entry";

        [Required]
        public string Content { get; set; } = string.Empty;

        public string ContentPreview { get; set; } = string.Empty;

        [Required]
        public bool IsRichText { get; set; } = true;

        // Moods
        [Required]
        public int PrimaryMoodId { get; set; }

        public int? SecondaryMood1Id { get; set; }
        public int? SecondaryMood2Id { get; set; }

        // Category
        public int? CategoryId { get; set; }

        // Metadata
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int WordCount { get; set; }
        public int CharacterCount { get; set; }

        // Navigation properties
        [ForeignKey("PrimaryMoodId")]
        public Mood PrimaryMood { get; set; } = null!;

        [ForeignKey("SecondaryMood1Id")]
        public Mood? SecondaryMood1 { get; set; }

        [ForeignKey("SecondaryMood2Id")]
        public Mood? SecondaryMood2 { get; set; }

        [ForeignKey("CategoryId")]
        public EntryCategory? Category { get; set; }

        public ICollection<JournalTag> JournalTags { get; set; } = new List<JournalTag>();
    }
}