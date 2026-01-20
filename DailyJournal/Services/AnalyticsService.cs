//using DailyJournal.Data.Database;
//using DailyJournal.Data.Entities;
//using DailyJournal.Data.Models;
//using Microsoft.EntityFrameworkCore;

//namespace DailyJournal.Services
//{
//    public class AnalyticsService
//    {
//        private readonly AppDbContext _context;

//        public AnalyticsService()
//        {
//            _context = new AppDbContext();
//        }

//        public async Task<DashboardModel> GetDashboardDataAsync(DateTime? startDate = null, DateTime? endDate = null)
//        {
//            try
//            {
//                startDate ??= DateTime.Now.AddMonths(-1);
//                endDate ??= DateTime.Now;

//                var dashboard = new DashboardModel();

//                // Get streak data
//                var streakService = new StreakService();
//                var streak = await streakService.GetCurrentStreakAsync();
//                if (streak != null)
//                {
//                    dashboard.CurrentStreak = streak.CurrentStreak;
//                    dashboard.LongestStreak = streak.LongestStreak;
//                    dashboard.TotalEntries = streak.TotalEntries;
//                    dashboard.TotalMissedDays = streak.TotalMissedDays;
//                    dashboard.StreakStartDate = streak.StreakStartDate;
//                    dashboard.StreakEndDate = streak.StreakEndDate;
//                }

//                // Get mood distribution
//                var moodDistribution = await GetMoodDistributionAsync(startDate.Value, endDate.Value);
//                dashboard.MoodDistribution = moodDistribution;

//                // Get most frequent mood
//                var mostFrequent = await GetMostFrequentMoodAsync(startDate.Value, endDate.Value);
//                dashboard.MostFrequentMood = mostFrequent;

//                // Get most used tags
//                var popularTags = await GetPopularTagsAsync(startDate.Value, endDate.Value, 10);
//                dashboard.MostUsedTags = popularTags;

//                // Get category breakdown
//                var categoryBreakdown = await GetCategoryBreakdownAsync(startDate.Value, endDate.Value);
//                dashboard.CategoryBreakdown = categoryBreakdown;

//                // Get word count statistics
//                var wordStats = await GetWordCountStatisticsAsync(startDate.Value, endDate.Value);
//                dashboard.AverageWordsPerEntry = wordStats.AverageWords;
//                dashboard.TotalWords = wordStats.TotalWords;
//                dashboard.LongestEntryWordCount = wordStats.LongestEntryWords;
//                dashboard.ShortestEntryWordCount = wordStats.ShortestEntryWords;

//                // Get word count trends
//                var trends = await GetWordCountTrendsAsync(startDate.Value, endDate.Value);
//                dashboard.WordCountTrends = trends;

//                // Get recent entries
//                var recentEntries = await GetRecentEntriesAsync(5);
//                dashboard.RecentEntries = recentEntries;
//                if (recentEntries.Any())
//                {
//                    dashboard.LastEntryDate = recentEntries.First().Date;
//                    dashboard.LastEntryTitle = recentEntries.First().Title;
//                }

//                // Calculate consistency
//                dashboard.TotalDaysTracked = (endDate.Value.Date - startDate.Value.Date).Days + 1;
//                var entriesInRange = await _context.JournalEntries
//                    .Where(e => e.EntryDate >= startDate.Value && e.EntryDate <= endDate.Value)
//                    .CountAsync();
//                dashboard.ConsistencyPercentage = dashboard.TotalDaysTracked > 0
//                    ? (double)entriesInRange / dashboard.TotalDaysTracked * 100
//                    : 0;

//                // Calculate weekly/monthly/yearly entries
//                var now = DateTime.Now;
//                var weekStart = now.AddDays(-(int)now.DayOfWeek);
//                var monthStart = new DateTime(now.Year, now.Month, 1);
//                var yearStart = new DateTime(now.Year, 1, 1);

//                dashboard.EntriesThisWeek = await _context.JournalEntries
//                    .Where(e => e.EntryDate >= weekStart && e.EntryDate <= now)
//                    .CountAsync();

//                dashboard.EntriesThisMonth = await _context.JournalEntries
//                    .Where(e => e.EntryDate >= monthStart && e.EntryDate <= now)
//                    .CountAsync();

//                dashboard.EntriesThisYear = await _context.JournalEntries
//                    .Where(e => e.EntryDate >= yearStart && e.EntryDate <= now)
//                    .CountAsync();

//                return dashboard;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting dashboard data: {ex.Message}");
//                return new DashboardModel();
//            }
//        }

//        private async Task<List<MoodStatModel>> GetMoodDistributionAsync(DateTime startDate, DateTime endDate)
//        {
//            try
//            {
//                var entries = await _context.JournalEntries
//                    .Include(e => e.PrimaryMood)
//                    .Where(e => e.EntryDate >= startDate && e.EntryDate <= endDate)
//                    .ToListAsync();

//                var moodTypes = new[] { "Positive", "Neutral", "Negative" };
//                var totalEntries = entries.Count;

//                return moodTypes.Select(type => new MoodStatModel
//                {
//                    MoodType = type,
//                    Count = entries.Count(e => e.PrimaryMood.MoodType == type),
//                    Percentage = totalEntries > 0 ? (double)entries.Count(e => e.PrimaryMood.MoodType == type) / totalEntries * 100 : 0,
//                    Color = type switch
//                    {
//                        "Positive" => "#4CAF50",
//                        "Neutral" => "#FF9800",
//                        "Negative" => "#F44336",
//                        _ => "#9E9E9E"
//                    },
//                    Emoji = type switch
//                    {
//                        "Positive" => "😊",
//                        "Neutral" => "😐",
//                        "Negative" => "😔",
//                        _ => "❓"
//                    }
//                }).ToList();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting mood distribution: {ex.Message}");
//                return new List<MoodStatModel>();
//            }
//        }

//        private async Task<MoodModel> GetMostFrequentMoodAsync(DateTime startDate, DateTime endDate)
//        {
//            try
//            {
//                var moodCounts = await _context.JournalEntries
//                    .Include(e => e.PrimaryMood)
//                    .Where(e => e.EntryDate >= startDate && e.EntryDate <= endDate)
//                    .GroupBy(e => new { e.PrimaryMood.Id, e.PrimaryMood.Name, e.PrimaryMood.Emoji, e.PrimaryMood.Color })
//                    .Select(g => new
//                    {
//                        MoodId = g.Key.Id,
//                        MoodName = g.Key.Name,
//                        MoodEmoji = g.Key.Emoji,
//                        MoodColor = g.Key.Color,
//                        Count = g.Count()
//                    })
//                    .OrderByDescending(x => x.Count)
//                    .FirstOrDefaultAsync();

//                if (moodCounts == null)
//                    return null;

//                return new MoodModel
//                {
//                    Id = moodCounts.MoodId,
//                    Name = moodCounts.MoodName,
//                    Emoji = moodCounts.MoodEmoji,
//                    Color = moodCounts.MoodColor
//                };
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting most frequent mood: {ex.Message}");
//                return null;
//            }
//        }

//        private async Task<List<TagStatModel>> GetPopularTagsAsync(DateTime startDate, DateTime endDate, int count)
//        {
//            try
//            {
//                var popularTags = await _context.JournalTags
//                    .Include(jt => jt.Tag)
//                    .Include(jt => jt.JournalEntry)
//                    .Where(jt => jt.JournalEntry.EntryDate >= startDate && jt.JournalEntry.EntryDate <= endDate)
//                    .GroupBy(jt => new { jt.Tag.Id, jt.Tag.Name, jt.Tag.Color })
//                    .Select(g => new
//                    {
//                        TagId = g.Key.Id,
//                        TagName = g.Key.Name,
//                        TagColor = g.Key.Color,
//                        UsageCount = g.Count()
//                    })
//                    .OrderByDescending(x => x.UsageCount)
//                    .Take(count)
//                    .ToListAsync();

//                var totalUsage = popularTags.Sum(x => x.UsageCount);

//                return popularTags.Select(t => new TagStatModel
//                {
//                    TagName = t.TagName,
//                    UsageCount = t.UsageCount,
//                    Percentage = totalUsage > 0 ? (double)t.UsageCount / totalUsage * 100 : 0,
//                    Color = t.TagColor,
//                    Icon = "Tag"
//                }).ToList();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting popular tags: {ex.Message}");
//                return new List<TagStatModel>();
//            }
//        }

//        private async Task<List<CategoryStatModel>> GetCategoryBreakdownAsync(DateTime startDate, DateTime endDate)
//        {
//            try
//            {
//                var categoryStats = await _context.JournalEntries
//                    .Include(e => e.Category)
//                    .Where(e => e.EntryDate >= startDate && e.EntryDate <= endDate && e.Category != null)
//                    .GroupBy(e => new { e.Category.Id, e.Category.Name, e.Category.Color })
//                    .Select(g => new
//                    {
//                        CategoryId = g.Key.Id,
//                        CategoryName = g.Key.Name,
//                        CategoryColor = g.Key.Color,
//                        EntryCount = g.Count()
//                    })
//                    .OrderByDescending(x => x.EntryCount)
//                    .ToListAsync();

//                var totalEntries = categoryStats.Sum(x => x.EntryCount);

//                return categoryStats.Select(c => new CategoryStatModel
//                {
//                    CategoryName = c.CategoryName,
//                    EntryCount = c.EntryCount,
//                    Percentage = totalEntries > 0 ? (double)c.EntryCount / totalEntries * 100 : 0,
//                    Color = c.CategoryColor,
//                    Icon = "Folder"
//                }).ToList();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting category breakdown: {ex.Message}");
//                return new List<CategoryStatModel>();
//            }
//        }

//        private async Task<(int AverageWords, int TotalWords, int LongestEntryWords, int ShortestEntryWords)> GetWordCountStatisticsAsync(DateTime startDate, DateTime endDate)
//        {
//            try
//            {
//                var entries = await _context.JournalEntries
//                    .Where(e => e.EntryDate >= startDate && e.EntryDate <= endDate)
//                    .ToListAsync();

//                if (!entries.Any())
//                    return (0, 0, 0, 0);

//                var totalWords = entries.Sum(e => e.WordCount);
//                var averageWords = totalWords / entries.Count;
//                var longestEntryWords = entries.Max(e => e.WordCount);
//                var shortestEntryWords = entries.Min(e => e.WordCount);

//                return (averageWords, totalWords, longestEntryWords, shortestEntryWords);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting word count statistics: {ex.Message}");
//                return (0, 0, 0, 0);
//            }
//        }

//        private async Task<List<WordCountTrendModel>> GetWordCountTrendsAsync(DateTime startDate, DateTime endDate)
//        {
//            try
//            {
//                var trends = new List<WordCountTrendModel>();

//                // Group by month
//                var monthlyStats = await _context.JournalEntries
//                    .Where(e => e.EntryDate >= startDate && e.EntryDate <= endDate)
//                    .GroupBy(e => new { e.EntryDate.Year, e.EntryDate.Month })
//                    .Select(g => new
//                    {
//                        Year = g.Key.Year,
//                        Month = g.Key.Month,
//                        TotalWords = g.Sum(e => e.WordCount),
//                        EntryCount = g.Count()
//                    })
//                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
//                    .ToListAsync();

//                foreach (var stat in monthlyStats)
//                {
//                    var date = new DateTime(stat.Year, stat.Month, 1);
//                    trends.Add(new WordCountTrendModel
//                    {
//                        Date = date,
//                        DateLabel = date.ToString("MMM yyyy"),
//                        WordCount = stat.TotalWords,
//                        EntryCount = stat.EntryCount
//                    });
//                }

//                return trends;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting word count trends: {ex.Message}");
//                return new List<WordCountTrendModel>();
//            }
//        }

//        private async Task<List<RecentEntryModel>> GetRecentEntriesAsync(int count)
//        {
//            try
//            {
//                var recentEntries = await _context.JournalEntries
//                    .Include(e => e.PrimaryMood)
//                    .Include(e => e.JournalTags)
//                        .ThenInclude(jt => jt.Tag)
//                    .OrderByDescending(e => e.EntryDate)
//                    .Take(count)
//                    .ToListAsync();

//                return recentEntries.Select(e => new RecentEntryModel
//                {
//                    Date = e.EntryDate,
//                    DateLabel = e.EntryDate.ToString("MMM d"),
//                    Title = e.Title,
//                    MoodEmoji = e.PrimaryMood?.Emoji ?? "❓",
//                    MoodColor = e.PrimaryMood?.Color ?? "#9E9E9E",
//                    Preview = e.ContentPreview,
//                    WordCount = e.WordCount,
//                    HasTags = e.JournalTags.Any(),
//                    TagNames = e.JournalTags.Select(jt => jt.Tag.Name).ToList()
//                }).ToList();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting recent entries: {ex.Message}");
//                return new List<RecentEntryModel>();
//            }
//        }
//    }
//}