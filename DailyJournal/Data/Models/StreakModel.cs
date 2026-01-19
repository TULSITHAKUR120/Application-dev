using System;
using System.Collections.Generic;

namespace DailyJournal.Data.Models
{
    public class StreakModel
    {
        public int Id { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public DateTime LastEntryDate { get; set; }
        public int TotalEntries { get; set; }
        public int TotalMissedDays { get; set; }
        public DateTime StreakStartDate { get; set; }
        public DateTime? StreakEndDate { get; set; }
        public List<DateTime> MissedDates { get; set; } = new();

        // Calculated properties
        public bool IsActive => CurrentStreak > 0;
        public int DaysSinceLastEntry => (int)(DateTime.Today - LastEntryDate.Date).TotalDays;
        public bool NeedsEntryToday => LastEntryDate.Date < DateTime.Today;
        public string StatusMessage
        {
            get
            {
                if (CurrentStreak == 0)
                    return "Start your journaling streak today!";

                if (NeedsEntryToday)
                    return $"Keep your {CurrentStreak}-day streak alive! Write today's entry.";

                return $"🔥 {CurrentStreak}-day streak! Great work!";
            }
        }

        public string LongestStreakText => $"Longest: {LongestStreak} days";
        public string TotalEntriesText => $"Total: {TotalEntries} entries";

        // Progress
        public double ConsistencyPercentage
        {
            get
            {
                if (TotalEntries == 0) return 0;
                var totalDays = (DateTime.Today - StreakStartDate).Days + 1;
                return totalDays > 0 ? (double)TotalEntries / totalDays * 100 : 0;
            }
        }

        // Helper methods
        public List<DateTime> GetStreakDates()
        {
            var dates = new List<DateTime>();
            if (CurrentStreak == 0) return dates;

            for (int i = CurrentStreak - 1; i >= 0; i--)
            {
                dates.Add(LastEntryDate.AddDays(-i));
            }

            return dates;
        }
    }
}