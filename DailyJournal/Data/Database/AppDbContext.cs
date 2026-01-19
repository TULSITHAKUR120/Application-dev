using Microsoft.EntityFrameworkCore;
using DailyJournal.Data.Entities;
using System.Reflection;

namespace DailyJournal.Data.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<Mood> Moods { get; set; }
        public DbSet<EntryCategory> EntryCategories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<JournalTag> JournalTags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Streak> Streaks { get; set; }

        public AppDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databasePath = Path.Combine(FileSystem.AppDataDirectory, "dailyjournal.db3");
            optionsBuilder.UseSqlite($"Filename={databasePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<JournalEntry>()
                .HasOne(e => e.PrimaryMood)
                .WithMany()
                .HasForeignKey(e => e.PrimaryMoodId);

            modelBuilder.Entity<JournalEntry>()
                .HasOne(e => e.SecondaryMood1)
                .WithMany()
                .HasForeignKey(e => e.SecondaryMood1Id);

            modelBuilder.Entity<JournalEntry>()
                .HasOne(e => e.SecondaryMood2)
                .WithMany()
                .HasForeignKey(e => e.SecondaryMood2Id);

            modelBuilder.Entity<JournalEntry>()
                .HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId);

            // Configure JournalTag as join table
            modelBuilder.Entity<JournalTag>()
                .HasKey(jt => jt.Id);

            modelBuilder.Entity<JournalTag>()
                .HasOne(jt => jt.JournalEntry)
                .WithMany(je => je.JournalTags)
                .HasForeignKey(jt => jt.JournalEntryId);

            modelBuilder.Entity<JournalTag>()
                .HasOne(jt => jt.Tag)
                .WithMany()
                .HasForeignKey(jt => jt.TagId);

            // Configure User-Streak relationship
            modelBuilder.Entity<Streak>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Moods
            var moods = new List<Mood>
            {
                // Positive moods
                new Mood { Id = 1, Name = "Happy", MoodType = "Positive", Emoji = "😊", Color = "#4CAF50", IsPredefined = true, DisplayOrder = 1 },
                new Mood { Id = 2, Name = "Excited", MoodType = "Positive", Emoji = "🎉", Color = "#FF9800", IsPredefined = true, DisplayOrder = 2 },
                new Mood { Id = 3, Name = "Relaxed", MoodType = "Positive", Emoji = "😌", Color = "#00BCD4", IsPredefined = true, DisplayOrder = 3 },
                new Mood { Id = 4, Name = "Grateful", MoodType = "Positive", Emoji = "🙏", Color = "#8BC34A", IsPredefined = true, DisplayOrder = 4 },
                new Mood { Id = 5, Name = "Confident", MoodType = "Positive", Emoji = "💪", Color = "#FFC107", IsPredefined = true, DisplayOrder = 5 },
                
                // Neutral moods
                new Mood { Id = 6, Name = "Calm", MoodType = "Neutral", Emoji = "😐", Color = "#9E9E9E", IsPredefined = true, DisplayOrder = 6 },
                new Mood { Id = 7, Name = "Thoughtful", MoodType = "Neutral", Emoji = "🤔", Color = "#607D8B", IsPredefined = true, DisplayOrder = 7 },
                new Mood { Id = 8, Name = "Curious", MoodType = "Neutral", Emoji = "🧐", Color = "#009688", IsPredefined = true, DisplayOrder = 8 },
                new Mood { Id = 9, Name = "Nostalgic", MoodType = "Neutral", Emoji = "📜", Color = "#795548", IsPredefined = true, DisplayOrder = 9 },
                new Mood { Id = 10, Name = "Bored", MoodType = "Neutral", Emoji = "😑", Color = "#9C27B0", IsPredefined = true, DisplayOrder = 10 },
                
                // Negative moods
                new Mood { Id = 11, Name = "Sad", MoodType = "Negative", Emoji = "😔", Color = "#2196F3", IsPredefined = true, DisplayOrder = 11 },
                new Mood { Id = 12, Name = "Angry", MoodType = "Negative", Emoji = "😠", Color = "#F44336", IsPredefined = true, DisplayOrder = 12 },
                new Mood { Id = 13, Name = "Stressed", MoodType = "Negative", Emoji = "😫", Color = "#FF5722", IsPredefined = true, DisplayOrder = 13 },
                new Mood { Id = 14, Name = "Lonely", MoodType = "Negative", Emoji = "😞", Color = "#673AB7", IsPredefined = true, DisplayOrder = 14 },
                new Mood { Id = 15, Name = "Anxious", MoodType = "Negative", Emoji = "😰", Color = "#FF4081", IsPredefined = true, DisplayOrder = 15 }
            };
            modelBuilder.Entity<Mood>().HasData(moods);

            // Seed Categories
            var categories = new List<EntryCategory>
            {
                new EntryCategory { Id = 1, Name = "Personal", Description = "Personal thoughts", Color = "#2196F3", IsPredefined = true, DisplayOrder = 1 },
                new EntryCategory { Id = 2, Name = "Work", Description = "Work-related", Color = "#4CAF50", IsPredefined = true, DisplayOrder = 2 },
                new EntryCategory { Id = 3, Name = "Health", Description = "Health and fitness", Color = "#FF9800", IsPredefined = true, DisplayOrder = 3 },
                new EntryCategory { Id = 4, Name = "Travel", Description = "Travel experiences", Color = "#9C27B0", IsPredefined = true, DisplayOrder = 4 },
                new EntryCategory { Id = 5, Name = "Learning", Description = "Learning", Color = "#00BCD4", IsPredefined = true, DisplayOrder = 5 },
                new EntryCategory { Id = 6, Name = "Relationships", Description = "Family and friends", Color = "#FF5722", IsPredefined = true, DisplayOrder = 6 },
                new EntryCategory { Id = 7, Name = "Hobbies", Description = "Hobbies", Color = "#795548", IsPredefined = true, DisplayOrder = 7 },
                new EntryCategory { Id = 8, Name = "Goals", Description = "Goals", Color = "#607D8B", IsPredefined = true, DisplayOrder = 8 },
                new EntryCategory { Id = 9, Name = "Creative", Description = "Creative", Color = "#E91E63", IsPredefined = true, DisplayOrder = 9 },
                new EntryCategory { Id = 10, Name = "Other", Description = "Other", Color = "#9E9E9E", IsPredefined = true, DisplayOrder = 10 }
            };
            modelBuilder.Entity<EntryCategory>().HasData(categories);

            // Seed Tags
            var tags = new List<Tag>
            {
                new Tag { Id = 1, Name = "Work", Color = "#4CAF50", IsPredefined = true, UsageCount = 0, LastUsed = DateTime.UtcNow },
                new Tag { Id = 2, Name = "Career", Color = "#2196F3", IsPredefined = true, UsageCount = 0, LastUsed = DateTime.UtcNow },
                new Tag { Id = 3, Name = "Studies", Color = "#9C27B0", IsPredefined = true, UsageCount = 0, LastUsed = DateTime.UtcNow },
                new Tag { Id = 4, Name = "Family", Color = "#FF9800", IsPredefined = true, UsageCount = 0, LastUsed = DateTime.UtcNow },
                new Tag { Id = 5, Name = "Friends", Color = "#00BCD4", IsPredefined = true, UsageCount = 0, LastUsed = DateTime.UtcNow },
                new Tag { Id = 6, Name = "Health", Color = "#F44336", IsPredefined = true, UsageCount = 0, LastUsed = DateTime.UtcNow },
                new Tag { Id = 7, Name = "Fitness", Color = "#8BC34A", IsPredefined = true, UsageCount = 0, LastUsed = DateTime.UtcNow },
                new Tag { Id = 8, Name = "Travel", Color = "#FF5722", IsPredefined = true, UsageCount = 0, LastUsed = DateTime.UtcNow },
                new Tag { Id = 9, Name = "Nature", Color = "#4CAF50", IsPredefined = true, UsageCount = 0, LastUsed = DateTime.UtcNow },
                new Tag { Id = 10, Name = "Finance", Color = "#FFC107", IsPredefined = true, UsageCount = 0, LastUsed = DateTime.UtcNow },
                new Tag { Id = 11, Name = "Spirituality", Color = "#673AB7", IsPredefined = true, UsageCount = 0, LastUsed = DateTime.UtcNow },
                new Tag { Id = 12, Name = "Birthday", Color = "#E91E63", IsPredefined = true, UsageCount = 0, LastUsed = DateTime.UtcNow },
                new Tag { Id = 13, Name = "Holiday", Color = "#FF4081", IsPredefined = true, UsageCount = 0, LastUsed = DateTime.UtcNow },
                new Tag { Id = 14, Name = "Reading", Color = "#795548", IsPredefined = true, UsageCount = 0, LastUsed = DateTime.UtcNow },
                new Tag { Id = 15, Name = "Music", Color = "#9C27B0", IsPredefined = true, UsageCount = 0, LastUsed = DateTime.UtcNow }
            };
            modelBuilder.Entity<Tag>().HasData(tags);
        }
    }
}