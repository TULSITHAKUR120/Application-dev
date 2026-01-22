using DailyJournal.Data.Database;
using DailyJournal.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DailyJournal.Services
{
    public class DashboardService
    {
        private readonly AppDbContext _context;
        private readonly JournalService _journalService;

        public DashboardService(AppDbContext context, JournalService journalService)
        {
            _context = context;
            _journalService = journalService;
        }

        // Get current streak
        public async Task<StreakInfo> GetStreakInfoAsync(int userId)
        {
            var entries = await _context.JournalEntries
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.EntryDate)
                .Select(e => e.EntryDate.Date)
                .Distinct()
                .ToListAsync();

            if (entries.Count == 0)
                return new StreakInfo { CurrentStreak = 0, LongestStreak = 0 };

            // Calculate current streak
            int currentStreak = 0;
            var today = DateTime.UtcNow.Date;
            var currentDate = today;

            // Check if today has an entry
            if (entries.Contains(today))
            {
                currentStreak = 1;
                currentDate = today.AddDays(-1);
            }
            else
            {
                // Check yesterday
                var yesterday = today.AddDays(-1);
                if (entries.Contains(yesterday))
                {
                    currentStreak = 1;
                    currentDate = yesterday.AddDays(-1);
                }
                else
                {
                    return new StreakInfo
                    {
                        CurrentStreak = 0,
                        LongestStreak = CalculateLongestStreak(entries)
                    };
                }
            }

            // Continue counting backwards
            while (entries.Contains(currentDate))
            {
                currentStreak++;
                currentDate = currentDate.AddDays(-1);
            }

            // Calculate longest streak
            var longestStreak = CalculateLongestStreak(entries);

            return new StreakInfo
            {
                CurrentStreak = currentStreak,
                LongestStreak = longestStreak,
                TotalEntries = entries.Count
            };
        }

        private int CalculateLongestStreak(List<DateTime> entryDates)
        {
            if (entryDates.Count == 0) return 0;

            var sortedDates = entryDates.OrderBy(d => d).ToList();
            int longestStreak = 1;
            int currentStreak = 1;

            for (int i = 1; i < sortedDates.Count; i++)
            {
                if ((sortedDates[i] - sortedDates[i - 1]).TotalDays == 1)
                {
                    currentStreak++;
                    longestStreak = Math.Max(longestStreak, currentStreak);
                }
                else
                {
                    currentStreak = 1;
                }
            }

            return longestStreak;
        }

        // Get missed days in a date range
        public async Task<List<DateTime>> GetMissedDaysAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var entriesInRange = await _context.JournalEntries
                .Where(e => e.UserId == userId &&
                           e.EntryDate.Date >= startDate.Date &&
                           e.EntryDate.Date <= endDate.Date)
                .Select(e => e.EntryDate.Date)
                .ToListAsync();

            var missedDays = new List<DateTime>();
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                if (!entriesInRange.Contains(date))
                {
                    missedDays.Add(date);
                }
            }

            return missedDays;
        }

        // Get mood distribution
        public async Task<MoodDistribution> GetMoodDistributionAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.JournalEntries.Where(e => e.UserId == userId);

            if (startDate.HasValue)
                query = query.Where(e => e.EntryDate >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(e => e.EntryDate <= endDate.Value.Date);

            var entries = await query.ToListAsync();

            // Define mood categories
            var positiveMoods = new List<string> { "Happy", "Excited", "Relaxed", "Grateful", "Confident" };
            var neutralMoods = new List<string> { "Calm", "Thoughtful", "Curious", "Nostalgic", "Bored" };
            var negativeMoods = new List<string> { "Sad", "Angry", "Stressed", "Lonely", "Anxious" };

            int positiveCount = 0;
            int neutralCount = 0;
            int negativeCount = 0;

            foreach (var entry in entries)
            {
                if (positiveMoods.Contains(entry.PrimaryMood))
                    positiveCount++;
                else if (neutralMoods.Contains(entry.PrimaryMood))
                    neutralCount++;
                else if (negativeMoods.Contains(entry.PrimaryMood))
                    negativeCount++;
            }

            var distribution = new MoodDistribution
            {
                TotalEntries = entries.Count,
                PositiveCount = positiveCount,
                NeutralCount = neutralCount,
                NegativeCount = negativeCount
            };

            return distribution;
        }
        // Get most frequent mood
        public async Task<MostFrequentMood> GetMostFrequentMoodAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.JournalEntries.Where(e => e.UserId == userId);

            if (startDate.HasValue)
                query = query.Where(e => e.EntryDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.EntryDate <= endDate.Value);

            var moods = await query
                .GroupBy(e => e.PrimaryMood)
                .Select(g => new { Mood = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            if (moods == null)
                return new MostFrequentMood { Mood = "No entries yet", Count = 0 };

            return new MostFrequentMood
            {
                Mood = moods.Mood,
                Count = moods.Count
            };
        }
        // Get most used tags
        public async Task<List<TagUsage>> GetMostUsedTagsAsync(int userId, int topN = 10, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.JournalEntries.Where(e => e.UserId == userId && !string.IsNullOrEmpty(e.Tags));

                if (startDate.HasValue)
                    query = query.Where(e => e.EntryDate >= startDate.Value.Date);

                if (endDate.HasValue)
                    query = query.Where(e => e.EntryDate <= endDate.Value.Date);

                var entries = await query.ToListAsync();

                var tagDictionary = new Dictionary<string, int>();

                foreach (var entry in entries)
                {
                    var tags = entry.GetTagsList(); // Changed from GetTags() to GetTagsList()
                    foreach (var tag in tags)
                    {
                        if (string.IsNullOrWhiteSpace(tag))
                            continue;

                        var normalizedTag = tag.Trim().ToLower();
                        if (tagDictionary.ContainsKey(normalizedTag))
                            tagDictionary[normalizedTag]++;
                        else
                            tagDictionary[normalizedTag] = 1;
                    }
                }

                return tagDictionary
                    .OrderByDescending(kv => kv.Value)
                    .ThenBy(kv => kv.Key) // Sort alphabetically for ties
                    .Take(topN)
                    .Select(kv => new TagUsage
                    {
                        Tag = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(kv.Key), // Capitalize first letters
                        Count = kv.Value
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error getting most used tags: {ex.Message}");
                return new List<TagUsage>();
            }
        }
        // Get word count trends
        public async Task<List<WordCountTrend>> GetWordCountTrendsAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var entries = await _context.JournalEntries
                .Where(e => e.UserId == userId &&
                           e.EntryDate.Date >= startDate.Date &&
                           e.EntryDate.Date <= endDate.Date)
                .OrderBy(e => e.EntryDate)
                .Select(e => new { e.EntryDate, e.WordCount })
                .ToListAsync();

            var trends = new List<WordCountTrend>();
            var currentWeek = 1;
            var weekTotal = 0;
            var weekCount = 0;
            DateTime? weekStart = null;

            foreach (var entry in entries)
            {
                if (!weekStart.HasValue)
                    weekStart = entry.EntryDate;

                weekTotal += entry.WordCount;
                weekCount++;

                // Group by week
                if ((entry.EntryDate - weekStart.Value).TotalDays >= 7 || entry == entries.Last())
                {
                    trends.Add(new WordCountTrend
                    {
                        Week = currentWeek,
                        AverageWordCount = weekCount > 0 ? (int)Math.Round((double)weekTotal / weekCount) : 0,
                        WeekStart = weekStart.Value,
                        WeekEnd = entry.EntryDate
                    });

                    currentWeek++;
                    weekTotal = 0;
                    weekCount = 0;
                    weekStart = entry.EntryDate.AddDays(1);
                }
            }

            return trends;
        }

        // Get overall statistics
        public async Task<DashboardStatistics> GetDashboardStatisticsAsync(int userId)
        {
            var streakInfo = await GetStreakInfoAsync(userId);
            var moodDistribution = await GetMoodDistributionAsync(userId);
            var mostFrequentMood = await GetMostFrequentMoodAsync(userId);
            var mostUsedTags = await GetMostUsedTagsAsync(userId, 5);

            return new DashboardStatistics
            {
                StreakInfo = streakInfo,
                MoodDistribution = moodDistribution,
                MostFrequentMood = mostFrequentMood,
                MostUsedTags = mostUsedTags,
                TotalWordCount = await GetTotalWordCountAsync(userId),
                AverageWordCount = await GetAverageWordCountAsync(userId)
            };
        }

        private async Task<int> GetTotalWordCountAsync(int userId)
        {
            return await _context.JournalEntries
                .Where(e => e.UserId == userId)
                .SumAsync(e => e.WordCount);
        }

        private async Task<int> GetAverageWordCountAsync(int userId)
        {
            var entryCount = await _context.JournalEntries
                .Where(e => e.UserId == userId)
                .CountAsync();

            if (entryCount == 0) return 0;

            var totalWords = await GetTotalWordCountAsync(userId);
            return (int)Math.Round((double)totalWords / entryCount);
        }
    }

    // Data classes for dashboard
    public class StreakInfo
    {
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int TotalEntries { get; set; }
    }

    public class MoodDistribution
    {
        public int TotalEntries { get; set; }
        public int PositiveCount { get; set; }
        public int NeutralCount { get; set; }
        public int NegativeCount { get; set; }

        public double PositivePercentage => TotalEntries > 0 ? (PositiveCount * 100.0) / TotalEntries : 0;
        public double NeutralPercentage => TotalEntries > 0 ? (NeutralCount * 100.0) / TotalEntries : 0;
        public double NegativePercentage => TotalEntries > 0 ? (NegativeCount * 100.0) / TotalEntries : 0;
    }

    public class MostFrequentMood
    {
        public string Mood { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TagUsage
    {
        public string Tag { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class WordCountTrend
    {
        public int Week { get; set; }
        public int AverageWordCount { get; set; }
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
    }

    public class DashboardStatistics
    {
        public StreakInfo StreakInfo { get; set; } = new();
        public MoodDistribution MoodDistribution { get; set; } = new();
        public MostFrequentMood MostFrequentMood { get; set; } = new();
        public List<TagUsage> MostUsedTags { get; set; } = new();
        public int TotalWordCount { get; set; }
        public int AverageWordCount { get; set; }
    }
}