using System;
using System.Collections.Generic;

namespace DailyJournal.Data.Models
{
    public class FilterModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SearchTerm { get; set; }
        public List<int> MoodIds { get; set; } = new();
        public List<int> TagIds { get; set; } = new();
        public int? CategoryId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "EntryDate"; // EntryDate, CreatedAt, UpdatedAt, Title, WordCount
        public bool SortDescending { get; set; } = true;
        public bool IncludeEmptyDays { get; set; } = false;

        // Date range presets
        public string DateRangePreset { get; set; } = "all"; // all, today, yesterday, thisWeek, lastWeek, thisMonth, lastMonth, thisYear, lastYear, custom

        // Validation
        public bool HasFilters =>
            !string.IsNullOrWhiteSpace(SearchTerm) ||
            MoodIds.Count > 0 ||
            TagIds.Count > 0 ||
            CategoryId.HasValue ||
            StartDate.HasValue ||
            EndDate.HasValue;

        // Helper methods
        public void ApplyPreset(string preset)
        {
            DateRangePreset = preset;
            var today = DateTime.Today;

            switch (preset)
            {
                case "today":
                    StartDate = today;
                    EndDate = today;
                    break;

                case "yesterday":
                    StartDate = today.AddDays(-1);
                    EndDate = today.AddDays(-1);
                    break;

                case "thisWeek":
                    var firstDayOfWeek = today.AddDays(-(int)today.DayOfWeek);
                    StartDate = firstDayOfWeek;
                    EndDate = today;
                    break;

                case "lastWeek":
                    var lastWeekStart = today.AddDays(-(int)today.DayOfWeek - 7);
                    var lastWeekEnd = today.AddDays(-(int)today.DayOfWeek - 1);
                    StartDate = lastWeekStart;
                    EndDate = lastWeekEnd;
                    break;

                case "thisMonth":
                    StartDate = new DateTime(today.Year, today.Month, 1);
                    EndDate = today;
                    break;

                case "lastMonth":
                    var firstDayOfLastMonth = today.AddMonths(-1);
                    firstDayOfLastMonth = new DateTime(firstDayOfLastMonth.Year, firstDayOfLastMonth.Month, 1);
                    var lastDayOfLastMonth = firstDayOfLastMonth.AddMonths(1).AddDays(-1);
                    StartDate = firstDayOfLastMonth;
                    EndDate = lastDayOfLastMonth;
                    break;

                case "thisYear":
                    StartDate = new DateTime(today.Year, 1, 1);
                    EndDate = today;
                    break;

                case "lastYear":
                    StartDate = new DateTime(today.Year - 1, 1, 1);
                    EndDate = new DateTime(today.Year - 1, 12, 31);
                    break;

                case "all":
                default:
                    StartDate = null;
                    EndDate = null;
                    break;
            }
        }

        public string GetDateRangeText()
        {
            if (!StartDate.HasValue && !EndDate.HasValue)
                return "All Time";

            if (StartDate.HasValue && EndDate.HasValue)
            {
                if (StartDate.Value.Date == EndDate.Value.Date)
                    return StartDate.Value.ToString("MMMM d, yyyy");

                return $"{StartDate.Value:MMM d, yyyy} - {EndDate.Value:MMM d, yyyy}";
            }

            if (StartDate.HasValue)
                return $"From {StartDate.Value:MMM d, yyyy}";

            if (EndDate.HasValue)
                return $"Until {EndDate.Value:MMM d, yyyy}";

            return "Custom Range";
        }
    }
}