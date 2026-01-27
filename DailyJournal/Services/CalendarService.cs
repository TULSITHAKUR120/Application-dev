// CalendarService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyJournal.Data.Database;
using DailyJournal.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DailyJournal.Services
{
    public class CalendarService
    {
        private readonly AppDbContext _context;

        public CalendarService(AppDbContext context)
        {
            _context = context;
        }

        public class CalendarDay
        {
            public DateTime Date { get; set; }
            public bool HasEntry { get; set; }
            public int? EntryId { get; set; }
            public string? PrimaryMood { get; set; }
            public string? MoodColor { get; set; }
            public bool IsToday { get; set; }
            public bool IsSelected { get; set; }
            public bool IsCurrentMonth { get; set; }
            public bool IsFavorite { get; set; }
            public int WordCount { get; set; }
        }

        public class CalendarMonth
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM");
            public List<CalendarDay> Days { get; set; } = new();
            public int TotalEntries { get; set; }
            public int PositiveDays { get; set; }
            public int NegativeDays { get; set; }
            public int NeutralDays { get; set; }
        }

        // Get calendar for specific month
        public async Task<CalendarMonth> GetCalendarMonthAsync(int userId, int year, int month)
        {
            var firstDay = new DateTime(year, month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            // Get all entries for this month
            var entries = await _context.JournalEntries
                .Where(e => e.UserId == userId &&
                           e.EntryDate.Date >= firstDay.Date &&
                           e.EntryDate.Date <= lastDay.Date)
                .ToListAsync();

            var calendar = new CalendarMonth
            {
                Year = year,
                Month = month,
                TotalEntries = entries.Count
            };

            // Create days for the month
            var daysInMonth = DateTime.DaysInMonth(year, month);

            // Add days of current month
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day);
                var entry = entries.FirstOrDefault(e => e.EntryDate.Date == date.Date);

                var calendarDay = new CalendarDay
                {
                    Date = date,
                    HasEntry = entry != null,
                    EntryId = entry?.Id,
                    PrimaryMood = entry?.PrimaryMood,
                    MoodColor = GetMoodColor(entry?.PrimaryMood),
                    IsToday = date.Date == DateTime.Today,
                    IsCurrentMonth = true,
                    IsFavorite = entry?.IsFavorite ?? false,
                    WordCount = entry?.WordCount ?? 0
                };

                // Update mood statistics
                if (entry != null)
                {
                    if (IsPositiveMood(entry.PrimaryMood))
                        calendar.PositiveDays++;
                    else if (IsNegativeMood(entry.PrimaryMood))
                        calendar.NegativeDays++;
                    else
                        calendar.NeutralDays++;
                }

                calendar.Days.Add(calendarDay);
            }

            // Add padding days from previous month
            var firstDayOfWeek = (int)firstDay.DayOfWeek;
            var previousMonth = firstDay.AddMonths(-1);
            var daysInPreviousMonth = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);

            for (int i = firstDayOfWeek - 1; i >= 0; i--)
            {
                var date = new DateTime(previousMonth.Year, previousMonth.Month, daysInPreviousMonth - i);
                calendar.Days.Insert(0, new CalendarDay
                {
                    Date = date,
                    HasEntry = false,
                    IsCurrentMonth = false
                });
            }

            // Add padding days from next month
            var lastDayOfMonth = new DateTime(year, month, daysInMonth);
            var lastDayOfWeek = (int)lastDayOfMonth.DayOfWeek;
            var daysToAdd = 6 - lastDayOfWeek;

            for (int i = 1; i <= daysToAdd; i++)
            {
                var date = new DateTime(year, month, daysInMonth).AddDays(i);
                calendar.Days.Add(new CalendarDay
                {
                    Date = date,
                    HasEntry = false,
                    IsCurrentMonth = false
                });
            }

            return calendar;
        }

        // Get entries for a specific date range
        public async Task<List<JournalEntry>> GetEntriesForDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _context.JournalEntries
                .Where(e => e.UserId == userId &&
                           e.EntryDate.Date >= startDate.Date &&
                           e.EntryDate.Date <= endDate.Date)
                .OrderByDescending(e => e.EntryDate)
                .ToListAsync();
        }

        // Get entry by date
        public async Task<JournalEntry?> GetEntryByDateAsync(int userId, DateTime date)
        {
            return await _context.JournalEntries
                .FirstOrDefaultAsync(e => e.UserId == userId && e.EntryDate.Date == date.Date);
        }

        // Get streak information
        public async Task<StreakInfo> GetStreakInfoAsync(int userId)
        {
            var entries = await _context.JournalEntries
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.EntryDate)
                .ToListAsync();

            var streakInfo = new StreakInfo();

            if (entries.Any())
            {
                // Calculate current streak
                var currentStreak = 0;
                var currentDate = DateTime.Today;

                while (true)
                {
                    var hasEntry = entries.Any(e => e.EntryDate.Date == currentDate.Date);
                    if (hasEntry)
                    {
                        currentStreak++;
                        currentDate = currentDate.AddDays(-1);
                    }
                    else
                    {
                        break;
                    }
                }

                streakInfo.CurrentStreak = currentStreak;

                // Calculate longest streak
                var dates = entries.Select(e => e.EntryDate.Date).Distinct().OrderBy(d => d).ToList();
                var longestStreak = 0;
                var currentStreakTemp = 1;

                for (int i = 1; i < dates.Count; i++)
                {
                    if ((dates[i] - dates[i - 1]).Days == 1)
                    {
                        currentStreakTemp++;
                        longestStreak = Math.Max(longestStreak, currentStreakTemp);
                    }
                    else
                    {
                        currentStreakTemp = 1;
                    }
                }

                streakInfo.LongestStreak = longestStreak > 0 ? longestStreak : 1;
                streakInfo.TotalEntries = entries.Count;
            }

            return streakInfo;
        }

        // Get mood statistics for calendar view
        public async Task<Dictionary<string, int>> GetMoodStatsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.JournalEntries.Where(e => e.UserId == userId);

            if (startDate.HasValue)
                query = query.Where(e => e.EntryDate.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(e => e.EntryDate.Date <= endDate.Value.Date);

            var entries = await query.ToListAsync();

            var moodStats = new Dictionary<string, int>();
            foreach (var entry in entries)
            {
                if (moodStats.ContainsKey(entry.PrimaryMood))
                    moodStats[entry.PrimaryMood]++;
                else
                    moodStats[entry.PrimaryMood] = 1;
            }

            return moodStats;
        }

        // Get entries count by month for heatmap
        public async Task<Dictionary<DateTime, int>> GetMonthlyHeatmapAsync(int userId, int monthsBack = 12)
        {
            var startDate = DateTime.Today.AddMonths(-monthsBack);
            var endDate = DateTime.Today;

            var entries = await _context.JournalEntries
                .Where(e => e.UserId == userId &&
                           e.EntryDate.Date >= startDate.Date &&
                           e.EntryDate.Date <= endDate.Date)
                .GroupBy(e => new { e.EntryDate.Year, e.EntryDate.Month })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.Date, x => x.Count);

            // Fill in missing months
            var result = new Dictionary<DateTime, int>();
            for (var date = startDate; date <= endDate; date = date.AddMonths(1))
            {
                var monthStart = new DateTime(date.Year, date.Month, 1);
                result[monthStart] = entries.ContainsKey(monthStart) ? entries[monthStart] : 0;
            }

            return result;
        }

        private string GetMoodColor(string? mood)
        {
            if (string.IsNullOrEmpty(mood)) return "#e2e8f0";

            if (IsPositiveMood(mood)) return "#10b981"; // Green
            if (IsNegativeMood(mood)) return "#ef4444"; // Red

            return "#f59e0b"; // Orange/Yellow for neutral
        }

        private bool IsPositiveMood(string mood)
        {
            var positiveMoods = new[] { "Happy", "Excited", "Grateful", "Content", "Proud", "Hopeful" };
            return positiveMoods.Contains(mood, StringComparer.OrdinalIgnoreCase);
        }

        private bool IsNegativeMood(string mood)
        {
            var negativeMoods = new[] { "Sad", "Angry", "Anxious", "Stressed", "Lonely", "Tired", "Frustrated" };
            return negativeMoods.Contains(mood, StringComparer.OrdinalIgnoreCase);
        }
    }

   
}


