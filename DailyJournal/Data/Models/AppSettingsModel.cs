namespace DailyJournal.Data.Models
{
    public class AppSettingsModel
    {
        // Theme Settings
        public bool IsDarkTheme { get; set; } = false;
        public string PrimaryColor { get; set; } = "#2196F3";
        public string SecondaryColor { get; set; } = "#FF9800";
        public string AccentColor { get; set; } = "#4CAF50";

        // Security Settings
        public bool RequirePassword { get; set; } = true;
        public bool UsePIN { get; set; } = false;
        public bool EnableBiometric { get; set; } = false;
        public int AutoLockMinutes { get; set; } = 5; // 0 = never auto-lock

        // Notification Settings
        public bool EnableDailyReminders { get; set; } = true;
        public string ReminderTime { get; set; } = "20:00"; // 8:00 PM
        public bool EnableStreakNotifications { get; set; } = true;

        // Display Settings
        public string DefaultView { get; set; } = "calendar"; // calendar, timeline, list
        public int EntriesPerPage { get; set; } = 10;
        public bool ShowWordCount { get; set; } = true;
        public bool ShowMoodIcons { get; set; } = true;
        public bool ShowPreviewText { get; set; } = true;

        // Editor Settings
        public bool AutoSave { get; set; } = true;
        public int AutoSaveInterval { get; set; } = 30; // seconds
        public string DefaultEditorMode { get; set; } = "richtext"; // richtext, markdown
        public bool EnableSpellCheck { get; set; } = true;

        // Backup Settings
        public bool AutoBackup { get; set; } = false;
        public int AutoBackupFrequency { get; set; } = 7; // days
        public string BackupLocation { get; set; } = string.Empty;
        public bool BackupToCloud { get; set; } = false;

        // Export Settings
        public string DefaultExportFormat { get; set; } = "PDF";
        public bool ExportIncludeMetadata { get; set; } = true;
        public bool ExportIncludeStatistics { get; set; } = true;

        // Helper properties
        public string ThemeDescription => IsDarkTheme ? "Dark Theme" : "Light Theme";
        public string SecurityDescription
        {
            get
            {
                if (EnableBiometric) return "Biometric + Password";
                if (UsePIN) return "PIN + Password";
                if (RequirePassword) return "Password Only";
                return "No Security";
            }
        }
    }
}