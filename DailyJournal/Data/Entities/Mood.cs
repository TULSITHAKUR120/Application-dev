using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DailyJournal.Data.Entities
{
    public class Mood
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string MoodType { get; set; } = "Neutral"; // Positive, Neutral, Negative

        [Required]
        public string Emoji { get; set; } = "😊";

        [Required]
        public string Color { get; set; } = "#4CAF50";

        [Required]
        public bool IsPredefined { get; set; } = true;

        [Required]
        public int DisplayOrder { get; set; } = 0;
    }
}

