namespace DailyJournal.Data.Models
{
    public class MoodModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MoodType { get; set; } = "Neutral"; // Positive, Neutral, Negative
        public string Emoji { get; set; } = "😊";
        public string Color { get; set; } = "#4CAF50";
        public bool IsPredefined { get; set; } = true;
        public int DisplayOrder { get; set; }
        public bool IsSelected { get; set; }

        // Helper properties
        public string TypeColor => MoodType switch
        {
            "Positive" => "#4CAF50",
            "Neutral" => "#FF9800",
            "Negative" => "#F44336",
            _ => "#9E9E9E"
        };

        public string DisplayText => $"{Emoji} {Name}";
    }
}