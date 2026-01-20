using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DailyJournal.Data.Entities
{
    public class UserSettings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [MaxLength(20)]
        public string DefaultView { get; set; } = "calendar";

        public int EntriesPerPage { get; set; } = 10;
        public bool IsDarkTheme { get; set; } = false;

        // Navigation property
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}