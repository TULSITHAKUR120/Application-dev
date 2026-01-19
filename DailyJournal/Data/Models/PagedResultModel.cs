using System.Collections.Generic;

namespace DailyJournal.Data.Models
{
    public class PagedResultModel<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        // Navigation properties
        public int FirstItemIndex => (PageNumber - 1) * PageSize + 1;
        public int LastItemIndex => Math.Min(PageNumber * PageSize, TotalCount);

        // Helper methods
        public string PageInfo => $"{FirstItemIndex}-{LastItemIndex} of {TotalCount}";
        public bool IsEmpty => Items == null || Items.Count == 0;
    }
}