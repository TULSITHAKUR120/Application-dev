namespace DailyJournal.Data.Entities
{
    public class UserSettings
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Theme { get; set; } = "Light";

        public int FontSize { get; set; } = 14;

        public bool NotificationsEnabled { get; set; } = true;

        // Navigation property
        public User? User { get; set; }
    }
}