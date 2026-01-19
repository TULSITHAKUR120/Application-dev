using System;
using System.Collections.Generic;

namespace DailyJournal.Data.Models
{
    public class StatisticsModel
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        // General Statistics
        public int TotalEntries { get; set; }
        public int TotalWords { get; set; }
        public int AverageWordsPerEntry { get; set; }
        public int LongestEntryWordCount { get; set; }
        public int ShortestEntryWordCount { get; set; }

        // Time-based Statistics
        public Dictionary<DayOfWeek, int> EntriesByDayOfWeek { get; set; } = new();
        public Dictionary<int, int> EntriesByMonth { get; set; } = new(); // Month number (1-12) to count
        public Dictionary<int, int> EntriesByYear { get; set; } = new(); // Year to count

        // Mood Statistics
        public Dictionary<string, int> MoodCounts { get; set; } = new(); // Mood name to count
        public Dictionary<string, int> MoodTypeCounts { get; set; } = new(); // Positive/Neutral/Negative to count
        public string MostCommonMood { get; set; } = string.Empty;
        public int MostCommonMoodCount { get; set; }

        // Category Statistics
        public Dictionary<string, int> CategoryCounts { get; set; } = new(); // Category name to count
        public string MostCommonCategory { get; set; } = string.Empty;
        public int MostCommonCategoryCount { get; set; }

        // Tag Statistics
        public Dictionary<string, int> TagCounts { get; set; } = new(); // Tag name to count
        public List<string> MostUsedTags { get; set; } = new(); // Top 10 tags

        // Word Count Trends
        public List<DailyWordCount> DailyWordCounts { get; set; } = new();
        public List<MonthlyWordCount> MonthlyWordCounts { get; set; } = new();

        // Helper properties
        public string PeriodText => $"{PeriodStart:MMMM d, yyyy} - {PeriodEnd:MMMM d, yyyy}";
        public double AverageEntriesPerDay => TotalDays > 0 ? (double)TotalEntries / TotalDays : 0;
        public int TotalDays => (PeriodEnd - PeriodStart).Days + 1;
        public double ConsistencyRate => TotalDays > 0 ? (double)TotalEntries / TotalDays * 100 : 0;

        public class DailyWordCount
        {
            public DateTime Date { get; set; }
            public int WordCount { get; set; }
            public int EntryCount { get; set; }
            public int AverageWords => EntryCount > 0 ? WordCount / EntryCount : 0;
        }

        public class MonthlyWordCount
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public string MonthName => new DateTime(Year, Month, 1).ToString("MMM yyyy");
            public int WordCount { get; set; }
            public int EntryCount { get; set; }
            public int AverageWords => EntryCount > 0 ? WordCount / EntryCount : 0;
        }
    }
}