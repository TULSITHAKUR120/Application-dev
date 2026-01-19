using System;
using System.Collections.Generic;

namespace DailyJournal.Data.Models
{
    public class DashboardModel
    {
        // Streak Information
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int TotalEntries { get; set; }
        public int TotalMissedDays { get; set; }
        public int TotalDaysTracked { get; set; }
        public double ConsistencyPercentage { get; set; }
        public DateTime StreakStartDate { get; set; }
        public DateTime? StreakEndDate { get; set; }

        // Current streak progress
        public int CurrentStreakProgress => CurrentStreak > 0 ? 100 : 0;
        public string StreakStatus => CurrentStreak > 0 ? $"🔥 {CurrentStreak} day streak!" : "Start writing to begin your streak!";
        public string StreakMessage => CurrentStreak > 0
            ? $"You've journaled for {CurrentStreak} consecutive days!"
            : "Write your first entry to start a streak!";

        // Mood Statistics
        public List<MoodStatModel> MoodDistribution { get; set; } = new();
        public MoodModel MostFrequentMood { get; set; } = new();
        public int MostFrequentMoodCount { get; set; }

        // Tag Statistics
        public List<TagStatModel> MostUsedTags { get; set; } = new();
        public List<CategoryStatModel> CategoryBreakdown { get; set; } = new();

        // Word Count Statistics
        public int AverageWordsPerEntry { get; set; }
        public int TotalWords { get; set; }
        public int LongestEntryWordCount { get; set; }
        public int ShortestEntryWordCount { get; set; }
        public List<WordCountTrendModel> WordCountTrends { get; set; } = new();

        // Recent Activity
        public List<RecentEntryModel> RecentEntries { get; set; } = new();
        public DateTime LastEntryDate { get; set; }
        public string LastEntryTitle { get; set; } = string.Empty;

        // Productivity
        public int EntriesThisWeek { get; set; }
        public int EntriesThisMonth { get; set; }
        public int EntriesThisYear { get; set; }

        // Goals
        public int DailyGoal { get; set; } = 1;
        public bool DailyGoalMet => TotalEntries >= DailyGoal;
        public int WeeklyGoal { get; set; } = 7;
        public int WeeklyGoalProgress => WeeklyGoal > 0 ? Math.Min((EntriesThisWeek * 100) / WeeklyGoal, 100) : 0;
        public int MonthlyGoal { get; set; } = 30;
        public int MonthlyGoalProgress => MonthlyGoal > 0 ? Math.Min((EntriesThisMonth * 100) / MonthlyGoal, 100) : 0;

        // Stats summaries
        public string EntriesSummary => $"{TotalEntries} total entries";
        public string WordsSummary => $"{TotalWords:N0} total words";
        public string ConsistencySummary => $"{ConsistencyPercentage:F1}% consistency";
    }

    public class MoodStatModel
    {
        public string MoodType { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Emoji { get; set; } = string.Empty;

        // Helper properties
        public string DisplayText => $"{Emoji} {MoodType}: {Count} entries ({Percentage:F1}%)";
    }

    public class TagStatModel
    {
        public string TagName { get; set; } = string.Empty;
        public int UsageCount { get; set; }
        public double Percentage { get; set; }
        public string Color { get; set; } = string.Empty;

        // Helper properties
        public string DisplayText => $"{TagName}: {UsageCount} times";
    }

    public class CategoryStatModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public int EntryCount { get; set; }
        public double Percentage { get; set; }
        public string Color { get; set; } = string.Empty;

        // Helper properties
        public string DisplayText => $"{CategoryName}: {EntryCount} entries ({Percentage:F1}%)";
    }

    public class WordCountTrendModel
    {
        public DateTime Date { get; set; }
        public string DateLabel { get; set; } = string.Empty;
        public int WordCount { get; set; }
        public int EntryCount { get; set; }
        public int AverageWords => EntryCount > 0 ? WordCount / EntryCount : 0;

        // Helper properties
        public string DisplayText => $"{DateLabel}: {AverageWords} avg words";
    }

    public class RecentEntryModel
    {
        public DateTime Date { get; set; }
        public string DateLabel { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string MoodEmoji { get; set; } = string.Empty;
        public string MoodColor { get; set; } = string.Empty;
        public string Preview { get; set; } = string.Empty;
        public int WordCount { get; set; }
        public bool HasTags { get; set; }
        public List<string> TagNames { get; set; } = new();

        // Helper properties
        public string DisplayTitle => string.IsNullOrEmpty(Title) ? "Untitled Entry" : Title;
        public string WordCountText => $"{WordCount} words";
        public string TagsText => HasTags ? string.Join(", ", TagNames) : "No tags";
    }
}