using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DailyJournal.Data.Entities
{
    public class EntryCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Color { get; set; } = "#2196F3";

        [Required]
        public bool IsPredefined { get; set; } = true;

        [Required]
        public int DisplayOrder { get; set; } = 0;
    }
}