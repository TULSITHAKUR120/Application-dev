using DailyJournal.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.IO;

namespace DailyJournal
{
    public static class MigrationHelper
    {
        public static void RunMigrations()
        {
            Console.WriteLine("=== Running Manual Migrations ===");

            // Create database in app data directory
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "dailyjournal.db3");
            Console.WriteLine($"Database will be at: {dbPath}");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            using var context = new AppDbContext(optionsBuilder.Options);

            try
            {
                Console.WriteLine("Creating database...");
                context.Database.EnsureCreated();
                Console.WriteLine("✅ Database created successfully!");

                // Check if tables were created
                var userTableExists = context.Database.ExecuteSqlRaw("SELECT name FROM sqlite_master WHERE type='table' AND name='Users'");
                var settingsTableExists = context.Database.ExecuteSqlRaw("SELECT name FROM sqlite_master WHERE type='table' AND name='UserSettings'");

                Console.WriteLine($"Users table exists: {userTableExists > 0}");
                Console.WriteLine($"UserSettings table exists: {settingsTableExists > 0}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine($"Details: {ex.InnerException?.Message}");
            }
        }
    }
}