using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyJournal.Data.Database;
using DailyJournal.Data.Entities;
using DailyJournal.Data.Constants;
using DailyJournal.Data.Models; // Add this
using Microsoft.EntityFrameworkCore;

namespace DailyJournal.Services
{
    public class JournalService
    {
        private readonly AppDbContext _context;

        public JournalService(AppDbContext context)
        {
            _context = context;
        }

        // Create a new journal entry (one per day constraint)
        public async Task<JournalResult> CreateEntryAsync(int userId, string title, string content,
            string primaryMood, string? secondaryMood1 = null, string? secondaryMood2 = null,
            string? tags = null, string? category = null, bool isFavorite = false)
        {
            try
            {
                // Validate primary mood
                if (!Moods.GetAllMoods().Contains(primaryMood))
                {
                    return JournalResult.FailureResult("Invalid primary mood selected.");
                }

                // Validate secondary moods if provided
                if (!string.IsNullOrEmpty(secondaryMood1) &&
                    !Moods.GetAllMoods().Contains(secondaryMood1))
                {
                    return JournalResult.FailureResult("Invalid secondary mood 1 selected.");
                }

                if (!string.IsNullOrEmpty(secondaryMood2) &&
                    !Moods.GetAllMoods().Contains(secondaryMood2))
                {
                    return JournalResult.FailureResult("Invalid secondary mood 2 selected.");
                }

                // Check if entry already exists for today
                var today = DateTime.Today;
                var existingEntry = await _context.JournalEntries
                    .FirstOrDefaultAsync(e => e.UserId == userId && e.EntryDate.Date == today);

                if (existingEntry != null)
                {
                    return JournalResult.FailureResult("You already have an entry for today. Use update instead.");
                }

                // Calculate word count
                int wordCount = CalculateWordCount(content);

                // Create new entry
                var entry = new JournalEntry
                {
                    UserId = userId,
                    Title = title,
                    Content = content,
                    EntryDate = today,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    PrimaryMood = primaryMood,
                    SecondaryMood1 = secondaryMood1,
                    SecondaryMood2 = secondaryMood2,
                    Tags = tags,
                    Category = category,
                    IsFavorite = isFavorite,
                    IsPrivate = false,
                    WordCount = wordCount,
                  //  MoodCategory = entry.GetMoodCategory() // Set mood category
                };

                // Save to database
                _context.JournalEntries.Add(entry);
                await _context.SaveChangesAsync();

                // Convert to model
                var entryModel = JournalEntryModel.FromEntity(entry);
                return JournalResult.SuccessResult("Journal entry created successfully!", entryModel);
            }
            catch (Exception ex)
            {
                return JournalResult.FailureResult($"Error creating entry: {ex.Message}");
            }
        }

        // Update existing entry
        public async Task<JournalResult> UpdateEntryAsync(int entryId, int userId,
            string? title = null, string? content = null, string? primaryMood = null,
            string? secondaryMood1 = null, string? secondaryMood2 = null,
            string? tags = null, string? category = null, bool? isFavorite = null)
        {
            try
            {
                var entry = await _context.JournalEntries
                    .FirstOrDefaultAsync(e => e.Id == entryId && e.UserId == userId);

                if (entry == null)
                {
                    return JournalResult.FailureResult("Entry not found or you don't have permission to edit it.");
                }

                // Validate moods if provided
                if (!string.IsNullOrEmpty(primaryMood))
                {
                    if (!Moods.GetAllMoods().Contains(primaryMood))
                    {
                        return JournalResult.FailureResult("Invalid primary mood selected.");
                    }
                    entry.PrimaryMood = primaryMood;
                    entry.MoodCategory = entry.GetMoodCategory(); // Update mood category
                }

                if (!string.IsNullOrEmpty(secondaryMood1))
                {
                    if (!Moods.GetAllMoods().Contains(secondaryMood1) && secondaryMood1 != "")
                    {
                        return JournalResult.FailureResult("Invalid secondary mood 1 selected.");
                    }
                    entry.SecondaryMood1 = string.IsNullOrWhiteSpace(secondaryMood1) ? null : secondaryMood1;
                }

                if (!string.IsNullOrEmpty(secondaryMood2))
                {
                    if (!Moods.GetAllMoods().Contains(secondaryMood2) && secondaryMood2 != "")
                    {
                        return JournalResult.FailureResult("Invalid secondary mood 2 selected.");
                    }
                    entry.SecondaryMood2 = string.IsNullOrWhiteSpace(secondaryMood2) ? null : secondaryMood2;
                }

                // Update other fields if provided
                if (!string.IsNullOrEmpty(title))
                    entry.Title = title;

                if (!string.IsNullOrEmpty(content))
                {
                    entry.Content = content;
                    entry.CalculateWordCount();
                }

                if (tags != null)
                    entry.Tags = tags;

                if (category != null)
                    entry.Category = category;

                if (isFavorite.HasValue)
                    entry.IsFavorite = isFavorite.Value;

                entry.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                // Convert to model
                var entryModel = JournalEntryModel.FromEntity(entry);
                return JournalResult.SuccessResult("Entry updated successfully!", entryModel);
            }
            catch (Exception ex)
            {
                return JournalResult.FailureResult($"Error updating entry: {ex.Message}");
            }
        }

        // Delete entry
        public async Task<JournalResult> DeleteEntryAsync(int entryId, int userId)
        {
            try
            {
                var entry = await _context.JournalEntries
                    .FirstOrDefaultAsync(e => e.Id == entryId && e.UserId == userId);

                if (entry == null)
                {
                    return JournalResult.FailureResult("Entry not found or you don't have permission to delete it.");
                }

                _context.JournalEntries.Remove(entry);
                await _context.SaveChangesAsync();

                return JournalResult.SuccessResult("Entry deleted successfully!");
            }
            catch (Exception ex)
            {
                return JournalResult.FailureResult($"Error deleting entry: {ex.Message}");
            }
        }

        // Get today's entry
        public async Task<JournalEntry?> GetTodaysEntryAsync(int userId)
        {
            var today = DateTime.Today;
            return await _context.JournalEntries
                .FirstOrDefaultAsync(e => e.UserId == userId && e.EntryDate.Date == today);
        }

        // Get entry by date
        public async Task<JournalEntry?> GetEntryByDateAsync(int userId, DateTime date)
        {
            var targetDate = date.Date;
            return await _context.JournalEntries
                .FirstOrDefaultAsync(e => e.UserId == userId && e.EntryDate.Date == targetDate);
        }

        // Get entry by ID
        public async Task<JournalEntry?> GetEntryByIdAsync(int entryId, int userId)
        {
            return await _context.JournalEntries
                .FirstOrDefaultAsync(e => e.Id == entryId && e.UserId == userId);
        }

        // Get all entries for a user
        public async Task<List<JournalEntry>> GetEntriesByUserIdAsync(int userId)
        {
            return await _context.JournalEntries
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.EntryDate)
                .ToListAsync();
        }

        // Get all entries (paginated)
        public async Task<PaginatedEntriesResult> GetPaginatedEntriesAsync(int userId, int page = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.JournalEntries
                    .Where(e => e.UserId == userId)
                    .OrderByDescending(e => e.EntryDate);

                var totalEntries = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalEntries / (double)pageSize);

                var entries = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PaginatedEntriesResult
                {
                    Success = true,
                    Entries = entries,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    TotalEntries = totalEntries
                };
            }
            catch (Exception ex)
            {
                return new PaginatedEntriesResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        // Search entries
        public async Task<List<JournalEntry>> SearchEntriesAsync(int userId, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetEntriesByUserIdAsync(userId);

            return await _context.JournalEntries
                .Where(e => e.UserId == userId &&
                           (e.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            e.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(e => e.EntryDate)
                .ToListAsync();
        }

        // Filter entries by mood
        public async Task<List<JournalEntry>> GetEntriesByMoodAsync(int userId, string mood)
        {
            return await _context.JournalEntries
                .Where(e => e.UserId == userId &&
                           (e.PrimaryMood == mood ||
                            e.SecondaryMood1 == mood ||
                            e.SecondaryMood2 == mood))
                .OrderByDescending(e => e.EntryDate)
                .ToListAsync();
        }

        // Filter entries by tag
        public async Task<List<JournalEntry>> GetEntriesByTagAsync(int userId, string tag)
        {
            return await _context.JournalEntries
                .Where(e => e.UserId == userId &&
                           e.Tags != null &&
                           e.Tags.Contains(tag, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(e => e.EntryDate)
                .ToListAsync();
        }

        // Toggle favorite status
        public async Task<JournalResult> ToggleFavoriteAsync(int entryId, int userId)
        {
            try
            {
                var entry = await _context.JournalEntries
                    .FirstOrDefaultAsync(e => e.Id == entryId && e.UserId == userId);

                if (entry == null)
                {
                    return JournalResult.FailureResult("Entry not found.");
                }

                entry.IsFavorite = !entry.IsFavorite;
                entry.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var entryModel = JournalEntryModel.FromEntity(entry);
                return JournalResult.SuccessResult(
                    entry.IsFavorite ? "Added to favorites!" : "Removed from favorites!",
                    entryModel
                );
            }
            catch (Exception ex)
            {
                return JournalResult.FailureResult($"Error: {ex.Message}");
            }
        }

        // Get entries by date range
        public async Task<List<JournalEntry>> GetEntriesByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _context.JournalEntries
                .Where(e => e.UserId == userId &&
                           e.EntryDate.Date >= startDate.Date &&
                           e.EntryDate.Date <= endDate.Date)
                .OrderByDescending(e => e.EntryDate)
                .ToListAsync();
        }

        // Get calendar entries for a specific month
        public async Task<Dictionary<DateTime, bool>> GetCalendarEntriesAsync(int userId, int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var entries = await _context.JournalEntries
                .Where(e => e.UserId == userId &&
                           e.EntryDate.Date >= startDate &&
                           e.EntryDate.Date <= endDate)
                .Select(e => e.EntryDate.Date)
                .ToListAsync();

            var result = new Dictionary<DateTime, bool>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                result[date] = entries.Contains(date);
            }

            return result;
        }

        // Get entry count by user
        public async Task<int> GetEntryCountAsync(int userId)
        {
            return await _context.JournalEntries
                .Where(e => e.UserId == userId)
                .CountAsync();
        }

        // Get total word count by user
        public async Task<int> GetTotalWordCountAsync(int userId)
        {
            var entries = await _context.JournalEntries
                .Where(e => e.UserId == userId)
                .ToListAsync();

            return entries.Sum(e => e.WordCount);
        }

        // Get favorite entries
        public async Task<List<JournalEntry>> GetFavoriteEntriesAsync(int userId)
        {
            return await _context.JournalEntries
                .Where(e => e.UserId == userId && e.IsFavorite)
                .OrderByDescending(e => e.EntryDate)
                .ToListAsync();
        }

        // Get entries by category
        public async Task<List<JournalEntry>> GetEntriesByCategoryAsync(int userId, string category)
        {
            return await _context.JournalEntries
                .Where(e => e.UserId == userId && e.Category == category)
                .OrderByDescending(e => e.EntryDate)
                .ToListAsync();
        }

        // Get all categories for a user
        public async Task<List<string>> GetCategoriesAsync(int userId)
        {
            return await _context.JournalEntries
                .Where(e => e.UserId == userId && !string.IsNullOrEmpty(e.Category))
                .Select(e => e.Category!)
                .Distinct()
                .ToListAsync();
        }

        // Get all tags for a user
        public async Task<List<string>> GetTagsAsync(int userId)
        {
            var entries = await _context.JournalEntries
                .Where(e => e.UserId == userId && !string.IsNullOrEmpty(e.Tags))
                .ToListAsync();

            var allTags = new List<string>();
            foreach (var entry in entries)
            {
                allTags.AddRange(entry.GetTagsList());
            }

            return allTags.Distinct().ToList();
        }

        // Get mood statistics
        public async Task<Dictionary<string, int>> GetMoodStatisticsAsync(int userId)
        {
            var entries = await _context.JournalEntries
                .Where(e => e.UserId == userId)
                .ToListAsync();

            var moodStats = new Dictionary<string, int>();

            foreach (var entry in entries)
            {
                // Count primary mood
                if (moodStats.ContainsKey(entry.PrimaryMood))
                    moodStats[entry.PrimaryMood]++;
                else
                    moodStats[entry.PrimaryMood] = 1;

                // Count secondary moods
                if (!string.IsNullOrEmpty(entry.SecondaryMood1))
                {
                    if (moodStats.ContainsKey(entry.SecondaryMood1))
                        moodStats[entry.SecondaryMood1]++;
                    else
                        moodStats[entry.SecondaryMood1] = 1;
                }

                if (!string.IsNullOrEmpty(entry.SecondaryMood2))
                {
                    if (moodStats.ContainsKey(entry.SecondaryMood2))
                        moodStats[entry.SecondaryMood2]++;
                    else
                        moodStats[entry.SecondaryMood2] = 1;
                }
            }

            return moodStats;
        }

        // Helper method to calculate word count
        private int CalculateWordCount(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return 0;

            return content.Split(new[] { ' ', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries).Length;
        }
    }

    // Remove or rename this class since we're using Data.Models.JournalResult
    // Rename it to ServiceJournalResult to avoid conflict
    public class ServiceJournalResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public JournalEntry? Entry { get; set; }
        public List<JournalEntry>? Entries { get; set; }
    }

    public class PaginatedEntriesResult
    {
        public bool Success { get; set; }
        public List<JournalEntry> Entries { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalEntries { get; set; }
        public string? ErrorMessage { get; set; }
    }
}