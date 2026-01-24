using DailyJournal.Data.Entities;

public class ExportStatistics
{
    public int TotalEntries { get; set; }
    public int TotalWords { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double AverageWordsPerEntry { get; set; }
    public string MostFrequentMood { get; set; } = string.Empty;
    public bool HasEntries { get; set; }

    // Additional statistics you might want
    public int PositiveEntries { get; set; }
    public int NeutralEntries { get; set; }
    public int NegativeEntries { get; set; }
    public List<string> MostUsedTags { get; set; } = new();
    public Dictionary<string, int> MoodDistribution { get; set; } = new();
    public int LongestEntryWordCount { get; set; }
    public int ShortestEntryWordCount { get; set; }
    public DateTime MostRecentEntryDate { get; set; }
    public DateTime OldestEntryDate { get; set; }

    // Constructor
    public ExportStatistics()
    {
        // Initialize with default values
        StartDate = DateTime.MinValue;
        EndDate = DateTime.MinValue;
        MostRecentEntryDate = DateTime.MinValue;
        OldestEntryDate = DateTime.MaxValue;
    }

    // Helper method to calculate all statistics from entries
    public static ExportStatistics CalculateFromEntries(List<JournalEntry> entries)
    {
        var stats = new ExportStatistics();

        if (entries == null || !entries.Any())
        {
            stats.HasEntries = false;
            return stats;
        }

        stats.HasEntries = true;
        stats.TotalEntries = entries.Count;
        stats.TotalWords = entries.Sum(e => e.WordCount);
        stats.StartDate = entries.Min(e => e.EntryDate);
        stats.EndDate = entries.Max(e => e.EntryDate);
        stats.OldestEntryDate = stats.StartDate;
        stats.MostRecentEntryDate = stats.EndDate;

        if (stats.TotalEntries > 0)
        {
            stats.AverageWordsPerEntry = entries.Average(e => e.WordCount);
            stats.LongestEntryWordCount = entries.Max(e => e.WordCount);
            stats.ShortestEntryWordCount = entries.Min(e => e.WordCount);

            // Calculate mood statistics
            var moodGroups = entries
                .Where(e => !string.IsNullOrEmpty(e.PrimaryMood))
                .GroupBy(e => e.PrimaryMood)
                .OrderByDescending(g => g.Count());

            stats.MostFrequentMood = moodGroups.FirstOrDefault()?.Key ?? "N/A";

            // Mood distribution
            stats.MoodDistribution = moodGroups.ToDictionary(g => g.Key, g => g.Count());

            // Categorize moods
            var positiveMoods = new[] { "Happy", "Excited", "Grateful", "Content", "Proud", "Hopeful" };
            var negativeMoods = new[] { "Sad", "Angry", "Anxious", "Stressed", "Lonely", "Tired", "Frustrated" };

            stats.PositiveEntries = entries.Count(e =>
                positiveMoods.Contains(e.PrimaryMood, StringComparer.OrdinalIgnoreCase));
            stats.NegativeEntries = entries.Count(e =>
                negativeMoods.Contains(e.PrimaryMood, StringComparer.OrdinalIgnoreCase));
            stats.NeutralEntries = stats.TotalEntries - stats.PositiveEntries - stats.NegativeEntries;

            // Calculate most used tags
            var allTags = entries
                .Where(e => !string.IsNullOrEmpty(e.Tags))
                .SelectMany(e => e.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(tag => tag.Trim())
                .Where(tag => !string.IsNullOrEmpty(tag))
                .GroupBy(tag => tag)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToList();

            stats.MostUsedTags = allTags;
        }

        return stats;
    }

    // Method to get mood percentage
    public double GetPositivePercentage()
    {
        if (TotalEntries == 0) return 0;
        return Math.Round((PositiveEntries / (double)TotalEntries) * 100, 1);
    }

    public double GetNegativePercentage()
    {
        if (TotalEntries == 0) return 0;
        return Math.Round((NegativeEntries / (double)TotalEntries) * 100, 1);
    }

    public double GetNeutralPercentage()
    {
        if (TotalEntries == 0) return 0;
        return Math.Round((NeutralEntries / (double)TotalEntries) * 100, 1);
    }

    // Method to get date range as string
    public string GetDateRangeString()
    {
        if (!HasEntries) return "No entries";
        return $"{StartDate:MMM dd, yyyy} - {EndDate:MMM dd, yyyy}";
    }

    // Method to get duration in days
    public int GetDurationInDays()
    {
        if (!HasEntries) return 0;
        return (int)(EndDate - StartDate).TotalDays + 1;
    }

    // Method to get average entries per day
    public double GetEntriesPerDay()
    {
        var days = GetDurationInDays();
        if (days == 0) return 0;
        return Math.Round(TotalEntries / (double)days, 2);
    }

    // Method to get consistency percentage
    public double GetConsistencyPercentage()
    {
        var days = GetDurationInDays();
        if (days == 0) return 0;
        return Math.Round((TotalEntries / (double)days) * 100, 1);
    }
}