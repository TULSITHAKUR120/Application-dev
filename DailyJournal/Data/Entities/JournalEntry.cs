using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DailyJournal.Data.Entities
{
    public class JournalEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime EntryDate { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string PrimaryMood { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? SecondaryMood1 { get; set; }

        [MaxLength(50)]
        public string? SecondaryMood2 { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [MaxLength(500)]
        public string? Tags { get; set; } // Comma-separated tags

        public bool IsFavorite { get; set; }

        public bool IsPrivate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Add this property for database storage
        public int WordCount { get; set; }

        // Add mood category for analytics
        [MaxLength(20)]
        public string? MoodCategory { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        // Helper methods
        public List<string> GetTagsList()
        {
            if (string.IsNullOrWhiteSpace(Tags))
                return new List<string>();

            return Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(t => t.Trim())
                      .Where(t => !string.IsNullOrWhiteSpace(t))
                      .ToList();
        }

        // Helper to set tags from list
        public void SetTagsList(List<string> tags)
        {
            Tags = tags != null && tags.Count > 0 ? string.Join(",", tags) : null;
        }

        // Calculate word count when content changes
        public void CalculateWordCount()
        {
            WordCount = string.IsNullOrWhiteSpace(Content)
                ? 0
                : Content.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        // Get preview of content
        public string GetContentPreview(int maxLength = 150)
        {
            if (string.IsNullOrWhiteSpace(Content))
                return string.Empty;

            var preview = System.Text.RegularExpressions.Regex.Replace(Content, @"[#*_`\[\]()]", "");
            return preview.Length > maxLength ? preview.Substring(0, maxLength) + "..." : preview;
        }

        // Get mood category based on primary mood
        public string GetMoodCategory()
        {
            var positiveMoods = new List<string> { "Happy", "Excited", "Relaxed", "Grateful", "Confident" };
            var neutralMoods = new List<string> { "Calm", "Thoughtful", "Curious", "Nostalgic", "Bored" };
            var negativeMoods = new List<string> { "Sad", "Angry", "Stressed", "Lonely", "Anxious" };

            if (positiveMoods.Contains(PrimaryMood))
                return "Positive";
            else if (neutralMoods.Contains(PrimaryMood))
                return "Neutral";
            else if (negativeMoods.Contains(PrimaryMood))
                return "Negative";
            else
                return "Unknown";
        }

        // Update mood category
        public void UpdateMoodCategory()
        {
            MoodCategory = GetMoodCategory();
        }

        // Format date for display
        public string GetFormattedDate()
        {
            if (EntryDate.Date == DateTime.Today)
                return "Today";
            else if (EntryDate.Date == DateTime.Today.AddDays(-1))
                return "Yesterday";
            else
                return EntryDate.ToString("MMMM d, yyyy");
        }

        // Check if entry has secondary moods
        public bool HasSecondaryMoods()
        {
            return !string.IsNullOrEmpty(SecondaryMood1) || !string.IsNullOrEmpty(SecondaryMood2);
        }

        // Get all moods as list
        public List<string> GetAllMoods()
        {
            var moods = new List<string> { PrimaryMood };

            if (!string.IsNullOrEmpty(SecondaryMood1))
                moods.Add(SecondaryMood1);

            if (!string.IsNullOrEmpty(SecondaryMood2))
                moods.Add(SecondaryMood2);

            return moods;
        }

        // Check if entry has specific tag
        public bool HasTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(Tags) || string.IsNullOrWhiteSpace(tag))
                return false;

            var tagList = GetTagsList();
            return tagList.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
        }
    }
}