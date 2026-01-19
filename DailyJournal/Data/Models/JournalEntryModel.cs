using System;
using System.Collections.Generic;

namespace DailyJournal.Data.Models
{
    public class JournalEntryModel
    {
        public int Id { get; set; }
        public DateTime EntryDate { get; set; }
        public string Title { get; set; } = "Untitled Entry";
        public string Content { get; set; } = string.Empty;
        public string ContentPreview { get; set; } = string.Empty;
        public bool IsRichText { get; set; } = true;

        // Formatted dates for UI
        public string DisplayDate => EntryDate.ToString("dddd, MMMM d, yyyy");
        public string ShortDate => EntryDate.ToString("MMM d, yyyy");
        public string MonthYear => EntryDate.ToString("MMMM yyyy");
        public string DayOfWeek => EntryDate.ToString("dddd");
        public string DayNumber => EntryDate.Day.ToString();

        // Moods
        public MoodModel PrimaryMood { get; set; } = new();
        public MoodModel? SecondaryMood1 { get; set; }
        public MoodModel? SecondaryMood2 { get; set; }

        // Category
        public CategoryModel? Category { get; set; }

        // Tags
        public List<TagModel> Tags { get; set; } = new();

        // Metadata
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string LastUpdated => UpdatedAt.ToString("MMM d, yyyy h:mm tt");
        public int WordCount { get; set; }
        public int CharacterCount { get; set; }

        // UI Properties
        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsToday => EntryDate.Date == DateTime.Today;
        public bool HasEntry => !string.IsNullOrWhiteSpace(Content);

        // Navigation helpers
        public bool HasSecondaryMoods => SecondaryMood1 != null || SecondaryMood2 != null;
        public string MoodSummary =>
            $"{PrimaryMood.Name}" +
            $"{(SecondaryMood1 != null ? $", {SecondaryMood1.Name}" : "")}" +
            $"{(SecondaryMood2 != null ? $", {SecondaryMood2.Name}" : "")}";

        // Color based on mood type
        public string MoodColor => PrimaryMood?.MoodType switch
        {
            "Positive" => "#4CAF50",
            "Neutral" => "#FF9800",
            "Negative" => "#F44336",
            _ => "#9E9E9E"
        };
    }
}