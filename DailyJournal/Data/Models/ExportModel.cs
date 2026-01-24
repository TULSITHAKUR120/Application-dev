using System;

namespace DailyJournal.Data.Models
{
    public class ExportRequest
    {
        public int UserId { get; set; }
        public DateTime? StartDate { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime? EndDate { get; set; } = DateTime.Today;
        public string Title { get; set; } = "Journal Entries";
        public string Subtitle { get; set; } = "Personal Reflections";
        public bool IncludeMoods { get; set; } = true;
        public bool IncludeStats { get; set; } = true;
        public bool IncludeTags { get; set; } = true;

        // Optional: Add these properties if you want more export options
        public bool IncludeCoverPage { get; set; } = true;
        public bool IncludeTableOfContents { get; set; } = true;
        public string PaperSize { get; set; } = "A4";
        public string PageOrientation { get; set; } = "Portrait";
        public string FontSize { get; set; } = "12pt";
        public string FontFamily { get; set; } = "Arial";
    }

    public class ExportResult
    {
        public bool Success { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public int EntryCount { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime ExportDate { get; set; } = DateTime.Now;

        public static ExportResult SuccessResult(string fileName, byte[] content, int entryCount)
        {
            return new ExportResult
            {
                Success = true,
                FileName = fileName,
                Content = content,
                ContentType = "application/pdf",
                EntryCount = entryCount
            };
        }

        public static ExportResult ErrorResult(string errorMessage)
        {
            return new ExportResult
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }
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