using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DailyJournal.Data.Entities
{
    public class JournalTag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int JournalEntryId { get; set; }

        [Required]
        public int TagId { get; set; }

        [Required]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("JournalEntryId")]
        public JournalEntry JournalEntry { get; set; } = null!;

        [ForeignKey("TagId")]
        public Tag Tag { get; set; } = null!;
    }
}