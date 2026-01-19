using System.Collections.Generic;

namespace DailyJournal.Data.Models
{
    public class SearchResultModel
    {
        public List<JournalEntryModel> Entries { get; set; } = new();
        public int TotalResults { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchQuery { get; set; } = string.Empty;
        public FilterModel AppliedFilters { get; set; } = new();

        // Helper properties
        public bool HasResults => Entries.Count > 0;
        public string ResultSummary
        {
            get
            {
                if (TotalResults == 0)
                    return "No results found";

                var start = (PageNumber - 1) * PageSize + 1;
                var end = Math.Min(PageNumber * PageSize, TotalResults);

                return $"Showing {start}-{end} of {TotalResults} results";
            }
        }

        // Search statistics
        public Dictionary<string, int> MoodCounts { get; set; } = new();
        public Dictionary<string, int> TagCounts { get; set; } = new();
        public Dictionary<string, int> CategoryCounts { get; set; } = new();
        public DateRange DateRange { get; set; } = new();

        public class DateRange
        {
            public DateTime? OldestEntry { get; set; }
            public DateTime? NewestEntry { get; set; }

            public string DisplayText
            {
                get
                {
                    if (!OldestEntry.HasValue || !NewestEntry.HasValue)
                        return "No date range";

                    if (OldestEntry.Value.Date == NewestEntry.Value.Date)
                        return OldestEntry.Value.ToString("MMMM d, yyyy");

                    return $"{OldestEntry.Value:MMM d, yyyy} - {NewestEntry.Value:MMM d, yyyy}";
                }
            }
        }
    }
}