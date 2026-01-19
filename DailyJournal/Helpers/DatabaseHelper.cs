using DailyJournal.Data.Database;

namespace DailyJournal.Helpers
{
    public static class DatabaseHelper
    {
        public static AppDbContext GetDbContext()
        {
            return new AppDbContext();
        }

        public static async Task InitializeDatabaseAsync()
        {
            try
            {
                using var context = GetDbContext();
                await context.Database.EnsureCreatedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
            }
        }

        public static async Task<bool> BackupDatabaseAsync(string backupPath)
        {
            try
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "dailyjournal.db3");
                if (File.Exists(dbPath))
                {
                    File.Copy(dbPath, backupPath, true);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error backing up database: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> RestoreDatabaseAsync(string backupPath)
        {
            try
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "dailyjournal.db3");
                if (File.Exists(backupPath))
                {
                    File.Copy(backupPath, dbPath, true);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restoring database: {ex.Message}");
                return false;
            }
        }
    }
}