using DailyJournal.Data.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace DailyJournal.Data.Models
{
    public class JournalEntryModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Primary mood is required")]
        public string PrimaryMood { get; set; } = string.Empty;

        public string? SecondaryMood1 { get; set; }
        public string? SecondaryMood2 { get; set; }
        public string? Category { get; set; }
        public string? Tags { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Today;

        // Helper method to create model from entity
        public static JournalEntryModel FromEntity(JournalEntry entity)
        {
            return new JournalEntryModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Content = entity.Content,
                PrimaryMood = entity.PrimaryMood,
                SecondaryMood1 = entity.SecondaryMood1,
                SecondaryMood2 = entity.SecondaryMood2,
                Category = entity.Category,
                Tags = entity.Tags,
                IsFavorite = entity.IsFavorite,
                EntryDate = entity.EntryDate
            };
        }

        // Helper method to update entity from model
        public void UpdateEntity(JournalEntry entity)
        {
            entity.Title = Title;
            entity.Content = Content;
            entity.PrimaryMood = PrimaryMood;
            entity.SecondaryMood1 = SecondaryMood1;
            entity.SecondaryMood2 = SecondaryMood2;
            entity.Category = Category;
            entity.Tags = Tags;
            entity.IsFavorite = IsFavorite;
            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}