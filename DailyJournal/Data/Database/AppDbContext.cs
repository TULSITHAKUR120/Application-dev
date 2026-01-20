using Microsoft.EntityFrameworkCore;
using DailyJournal.Data.Entities;

namespace DailyJournal.Data.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databasePath = Path.Combine(FileSystem.AppDataDirectory, "dailyjournal.db3");
            optionsBuilder.UseSqlite($"Data Source={databasePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(256);
                entity.Property(e => e.PIN).HasMaxLength(256);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.PrimaryColor).HasMaxLength(7);
                entity.Property(e => e.SecondaryColor).HasMaxLength(7);

                // Unique constraints
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure UserSettings entity
            modelBuilder.Entity<UserSettings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                      .WithOne(u => u.UserSettings)
                      .HasForeignKey<UserSettings>(e => e.UserId);
            });
        }
    }
}