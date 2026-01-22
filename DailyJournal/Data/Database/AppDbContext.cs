using Microsoft.EntityFrameworkCore;
using DailyJournal.Data.Entities;

namespace DailyJournal.Data.Database;

public class AppDbContext : DbContext
{
    // ✅ Constructor for dependency injection (used in MauiProgram)
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // ❌ Remove parameterless constructor for runtime MAUI apps
    // Design-time migrations can use IDesignTimeDbContextFactory instead

    // DbSets
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserSettings> UserSettings { get; set; } = null!;
    public DbSet<JournalEntry> JournalEntries { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== User Configuration =====
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(e => e.PIN)
                .HasMaxLength(256);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("datetime('now')");

            entity.Property(e => e.PrimaryColor)
                .HasMaxLength(7);

            entity.Property(e => e.SecondaryColor)
                .HasMaxLength(7);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.Property(e => e.LastLoginAt)
                .IsRequired(false);

            // Unique constraints
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

            // One-to-one relation with UserSettings
            entity.HasOne(u => u.UserSettings)
                  .WithOne(us => us.User)
                  .HasForeignKey<UserSettings>(us => us.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== UserSettings Configuration =====
        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Theme)
                .HasMaxLength(20)
                .HasDefaultValue("Light");

            entity.Property(e => e.FontSize)
                .HasDefaultValue(14);

            entity.Property(e => e.NotificationsEnabled)
                .HasDefaultValue(true);
        });


        // JournalEntry Configuration
        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Unique constraint for one entry per user per day
            entity.HasIndex(e => new { e.UserId, e.EntryDate })
                  .IsUnique();

            entity.Property(e => e.Title)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.Content)
                  .IsRequired();

            entity.Property(e => e.PrimaryMood)
                  .IsRequired()
                  .HasMaxLength(50);

            //entity.Property(e => e.PrimaryMoodCategory)
            //      .IsRequired()
            //      .HasMaxLength(20);

            entity.Property(e => e.Category)
                  .HasMaxLength(100);

            entity.Property(e => e.Tags)
                  .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("datetime('now')");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("datetime('now')");

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

    }
}
