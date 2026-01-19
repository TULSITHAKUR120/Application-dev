using System;

namespace DailyJournal.Data.Models
{
    public class TagModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = "#9C27B0";
        public bool IsPredefined { get; set; } = true;
        public int UsageCount { get; set; }
        public DateTime LastUsed { get; set; }
        public bool IsSelected { get; set; }

        // Helper properties
        public string UsageText => $"{UsageCount} entries";
        public string LastUsedText => LastUsed.ToString("MMM d, yyyy");
        public string DisplayText => $"{Name} ({UsageCount})";
    }
}