namespace DailyJournal.Data.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = "#2196F3";
        public bool IsPredefined { get; set; } = true;
        public int DisplayOrder { get; set; }
        public int EntryCount { get; set; }
        public bool IsSelected { get; set; }

        // Helper properties
        public string DisplayText => $"{Name} ({EntryCount})";
    }
}