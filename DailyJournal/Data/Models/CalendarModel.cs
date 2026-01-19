using System;
using System.Collections.Generic;

namespace DailyJournal.Data.Models
{
    public class CalendarModel
    {
        public DateTime CurrentMonth { get; set; } = DateTime.Today;
        public List<CalendarDayModel> Days { get; set; } = new();
        public Dictionary<DateTime, JournalEntryModel> EntriesByDate { get; set; } = new();

        // Helper properties
        public string MonthYear => CurrentMonth.ToString("MMMM yyyy");
        public int Year => CurrentMonth.Year;
        public int Month => CurrentMonth.Month;

        // Navigation
        public void NavigateToPreviousMonth()
        {
            CurrentMonth = CurrentMonth.AddMonths(-1);
        }

        public void NavigateToNextMonth()
        {
            CurrentMonth = CurrentMonth.AddMonths(1);
        }

        public void NavigateToToday()
        {
            CurrentMonth = DateTime.Today;
        }
    }

    public class CalendarDayModel
    {
        public DateTime Date { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
        public bool HasEntry { get; set; }
        public JournalEntryModel Entry { get; set; }
        public string MoodEmoji { get; set; } = "📝";
        public string MoodColor { get; set; } = "#808080";
        public bool IsSelected { get; set; }

        // Helper properties
        public string DayNumber => Date.Day.ToString();
        public string DayOfWeek => Date.ToString("ddd");
        public string DisplayDate => Date.ToString("dddd, MMMM d, yyyy");
        public string ShortDate => Date.ToString("MMM d");

        // UI properties
        public string BackgroundColor
        {
            get
            {
                if (IsSelected) return "#E3F2FD";
                if (IsToday) return "#FFF3E0";
                if (!IsCurrentMonth) return "#F5F5F5";
                return "transparent";
            }
        }

        public string BorderColor => IsSelected ? "#2196F3" : (IsToday ? "#FF9800" : "#E0E0E0");
        public string TextColor => IsCurrentMonth ? "#212121" : "#9E9E9E";
        public string FontWeight => IsToday ? "bold" : "normal";
    }
}