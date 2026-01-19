using System;

namespace DailyJournal.Data.Models
{
    public class ExportModel
    {
        public DateTime StartDate { get; set; } = DateTime.Today.AddMonths(-1);
        public DateTime EndDate { get; set; } = DateTime.Today;
        public bool IncludeMoods { get; set; } = true;
        public bool IncludeTags { get; set; } = true;
        public bool IncludeCategory { get; set; } = true;
        public string ExportFormat { get; set; } = "PDF"; // PDF, HTML, TXT, JSON
        public string PageSize { get; set; } = "A4"; // A4, Letter, Legal
        public bool IncludeCoverPage { get; set; } = true;
        public string CoverPageTitle { get; set; } = "My Journal Export";
        public string CoverPageSubtitle { get; set; } = string.Empty;
        public bool IncludeTableOfContents { get; set; } = true;
        public bool IncludeStatistics { get; set; } = true;
        public string FileName { get; set; } = $"JournalExport_{DateTime.Now:yyyyMMdd_HHmmss}";

        // Validation
        public bool IsValid => StartDate <= EndDate;

        // Helper properties
        public string DateRangeText => $"{StartDate:MMMM d, yyyy} to {EndDate:MMMM d, yyyy}";
        public string ExportOptionsText
        {
            get
            {
                var options = new List<string>();
                if (IncludeMoods) options.Add("Moods");
                if (IncludeTags) options.Add("Tags");
                if (IncludeCategory) options.Add("Category");
                return string.Join(", ", options);
            }
        }
    }
}