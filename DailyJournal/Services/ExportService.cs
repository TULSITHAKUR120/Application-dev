using DailyJournal.Data.Database;
using DailyJournal.Data.Entities;
using DailyJournal.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDocCore.Rendering;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace DailyJournal.Services
{
    public class ExportService
    {
        private readonly AppDbContext _context;

        public ExportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> ExportToPdfAsync(ExportModel exportModel)
        {
            try
            {
                // Get entries for the date range
                var entries = await _context.JournalEntries
                    .Include(e => e.PrimaryMood)
                    .Include(e => e.SecondaryMood1)
                    .Include(e => e.SecondaryMood2)
                    .Include(e => e.Category)
                    .Include(e => e.JournalTags)
                        .ThenInclude(jt => jt.Tag)
                    .Where(e => e.EntryDate >= exportModel.StartDate && e.EntryDate <= exportModel.EndDate)
                    .OrderBy(e => e.EntryDate)
                    .ToListAsync();

                if (!entries.Any())
                    throw new Exception("No entries found for the selected date range");

                // Create PDF document
                var document = new Document();
                document.Info.Title = exportModel.CoverPageTitle;
                document.Info.Subject = exportModel.CoverPageSubtitle;
                document.Info.Author = "Daily Journal App";

                // Define styles
                DefineStyles(document);

                // Add cover page
                if (exportModel.IncludeCoverPage)
                {
                    AddCoverPage(document, exportModel, entries.Count);
                }

                // Add table of contents
                AddTableOfContents(document, entries);

                // Add entries
                foreach (var entry in entries)
                {
                    AddJournalEntry(document, entry, exportModel);
                }

                // Add statistics page
                AddStatisticsPage(document, entries, exportModel);

                // Render PDF
                var pdfRenderer = new PdfDocumentRenderer();
                pdfRenderer.Document = document;
                pdfRenderer.RenderDocument();

                // Save to file
                var fileName = $"JournalExport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                pdfRenderer.PdfDocument.Save(filePath);

                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting to PDF: {ex.Message}");
                throw;
            }
        }

        private void DefineStyles(Document document)
        {
            // Normal style
            var style = document.Styles["Normal"];
            style.Font.Name = "Arial";
            style.Font.Size = 11;

            // Heading styles
            style = document.Styles.AddStyle("Heading1", "Normal");
            style.Font.Size = 16;
            style.Font.Bold = true;
            style.ParagraphFormat.SpaceAfter = 12;

            style = document.Styles.AddStyle("Heading2", "Normal");
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles.AddStyle("Heading3", "Normal");
            style.Font.Size = 12;
            style.Font.Bold = true;
            style.ParagraphFormat.SpaceAfter = 3;

            // Entry content style
            style = document.Styles.AddStyle("EntryContent", "Normal");
            style.Font.Size = 11;
            style.ParagraphFormat.LeftIndentation = "1cm";
            style.ParagraphFormat.SpaceAfter = 12;

            // Metadata style
            style = document.Styles.AddStyle("Metadata", "Normal");
            style.Font.Size = 10;
            style.Font.Color = Colors.Gray;
        }

        private void AddCoverPage(Document document, ExportModel exportModel, int entryCount)
        {
            var section = document.AddSection();
            section.PageSetup.Orientation = Orientation.Portrait;
            section.PageSetup.TopMargin = "5cm";

            // Title
            var paragraph = section.AddParagraph();
            paragraph.Style = "Heading1";
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText(exportModel.CoverPageTitle);

            // Subtitle
            if (!string.IsNullOrEmpty(exportModel.CoverPageSubtitle))
            {
                paragraph = section.AddParagraph();
                paragraph.Style = "Heading2";
                paragraph.Format.Alignment = ParagraphAlignment.Center;
                paragraph.AddText(exportModel.CoverPageSubtitle);
            }

            // Export information
            section.AddParagraph().AddLineBreak();
            paragraph = section.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText($"Exported on: {DateTime.Now:dddd, MMMM d, yyyy}");

            paragraph = section.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText($"Date Range: {exportModel.StartDate:MMMM d, yyyy} to {exportModel.EndDate:MMMM d, yyyy}");

            paragraph = section.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText($"Total Entries: {entryCount}");

            // Page break for next section
            section.AddPageBreak();
        }

        private void AddTableOfContents(Document document, List<JournalEntry> entries)
        {
            var section = document.LastSection;

            var paragraph = section.AddParagraph();
            paragraph.Style = "Heading1";
            paragraph.AddText("Table of Contents");

            foreach (var entry in entries)
            {
                paragraph = section.AddParagraph();
                paragraph.Style = "Normal";
                paragraph.AddText($"{entry.EntryDate:MMMM d, yyyy}: {entry.Title}");
            }

            section.AddPageBreak();
        }

        private void AddJournalEntry(Document document, JournalEntry entry, ExportModel exportModel)
        {
            var section = document.LastSection;

            // Entry date and title
            var paragraph = section.AddParagraph();
            paragraph.Style = "Heading2";
            paragraph.AddText($"{entry.EntryDate:dddd, MMMM d, yyyy}");

            paragraph = section.AddParagraph();
            paragraph.Style = "Heading3";
            paragraph.AddText(entry.Title);

            // Metadata
            if (exportModel.IncludeMoods || exportModel.IncludeTags || exportModel.IncludeCategory)
            {
                paragraph = section.AddParagraph();
                paragraph.Style = "Metadata";

                var metadata = new List<string>();

                if (exportModel.IncludeMoods && entry.PrimaryMood != null)
                {
                    var moodText = $"Mood: {entry.PrimaryMood.Emoji} {entry.PrimaryMood.Name}";
                    if (entry.SecondaryMood1 != null)
                        moodText += $", {entry.SecondaryMood1.Emoji} {entry.SecondaryMood1.Name}";
                    if (entry.SecondaryMood2 != null)
                        moodText += $", {entry.SecondaryMood2.Emoji} {entry.SecondaryMood2.Name}";
                    metadata.Add(moodText);
                }

                if (exportModel.IncludeCategory && entry.Category != null)
                {
                    metadata.Add($"Category: {entry.Category.Name}");
                }

                if (exportModel.IncludeTags && entry.JournalTags.Any())
                {
                    var tags = string.Join(", ", entry.JournalTags.Select(jt => jt.Tag.Name));
                    metadata.Add($"Tags: {tags}");
                }

                metadata.Add($"Word Count: {entry.WordCount}");
                metadata.Add($"Created: {entry.CreatedAt:MMMM d, yyyy h:mm tt}");
                metadata.Add($"Last Updated: {entry.UpdatedAt:MMMM d, yyyy h:mm tt}");

                paragraph.AddText(string.Join(" | ", metadata));
            }

            // Entry content
            paragraph = section.AddParagraph();
            paragraph.Style = "EntryContent";
            paragraph.AddText(entry.Content);

            // Separator
            section.AddParagraph().AddLineBreak();
            section.AddParagraph().AddLineBreak();
        }

        private void AddStatisticsPage(Document document, List<JournalEntry> entries, ExportModel exportModel)
        {
            document.LastSection.AddPageBreak();
            var section = document.LastSection;

            // Statistics title
            var paragraph = section.AddParagraph();
            paragraph.Style = "Heading1";
            paragraph.AddText("Export Statistics");

            // Basic statistics
            paragraph = section.AddParagraph();
            paragraph.Style = "Heading2";
            paragraph.AddText("Summary");

            var stats = new List<string>
            {
                $"Total Entries: {entries.Count}",
                $"Date Range: {entries.Min(e => e.EntryDate):MMMM d, yyyy} to {entries.Max(e => e.EntryDate):MMMM d, yyyy}",
                $"Total Words: {entries.Sum(e => e.WordCount)}",
                $"Average Words per Entry: {entries.Average(e => e.WordCount):F0}",
                $"Longest Entry: {entries.Max(e => e.WordCount)} words",
                $"Shortest Entry: {entries.Min(e => e.WordCount)} words"
            };

            foreach (var stat in stats)
            {
                paragraph = section.AddParagraph();
                paragraph.Style = "Normal";
                paragraph.AddText($"• {stat}");
            }

            // Mood statistics
            if (exportModel.IncludeMoods && entries.Any(e => e.PrimaryMood != null))
            {
                section.AddParagraph().AddLineBreak();
                paragraph = section.AddParagraph();
                paragraph.Style = "Heading2";
                paragraph.AddText("Mood Distribution");

                var moodGroups = entries
                    .Where(e => e.PrimaryMood != null)
                    .GroupBy(e => e.PrimaryMood.Name)
                    .OrderByDescending(g => g.Count())
                    .ToList();

                foreach (var group in moodGroups)
                {
                    var percentage = (double)group.Count() / entries.Count * 100;
                    paragraph = section.AddParagraph();
                    paragraph.Style = "Normal";
                    paragraph.AddText($"• {group.Key}: {group.Count()} entries ({percentage:F1}%)");
                }
            }

            // Tag statistics
            if (exportModel.IncludeTags && entries.Any(e => e.JournalTags.Any()))
            {
                section.AddParagraph().AddLineBreak();
                paragraph = section.AddParagraph();
                paragraph.Style = "Heading2";
                paragraph.AddText("Tag Usage");

                var tagGroups = entries
                    .SelectMany(e => e.JournalTags)
                    .GroupBy(jt => jt.Tag.Name)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .ToList();

                foreach (var group in tagGroups)
                {
                    paragraph = section.AddParagraph();
                    paragraph.Style = "Normal";
                    paragraph.AddText($"• {group.Key}: {group.Count()} times");
                }
            }
        }

        public async Task<string> ExportToTextAsync(ExportModel exportModel)
        {
            try
            {
                var entries = await _context.JournalEntries
                    .Include(e => e.PrimaryMood)
                    .Include(e => e.Category)
                    .Include(e => e.JournalTags)
                        .ThenInclude(jt => jt.Tag)
                    .Where(e => e.EntryDate >= exportModel.StartDate && e.EntryDate <= exportModel.EndDate)
                    .OrderBy(e => e.EntryDate)
                    .ToListAsync();

                if (!entries.Any())
                    throw new Exception("No entries found for the selected date range");

                var text = new System.Text.StringBuilder();

                // Header
                text.AppendLine("DAILY JOURNAL EXPORT");
                text.AppendLine("=====================");
                text.AppendLine($"Export Date: {DateTime.Now:dddd, MMMM d, yyyy}");
                text.AppendLine($"Date Range: {exportModel.StartDate:MMMM d, yyyy} to {exportModel.EndDate:MMMM d, yyyy}");
                text.AppendLine($"Total Entries: {entries.Count}");
                text.AppendLine();

                // Entries
                foreach (var entry in entries)
                {
                    text.AppendLine(new string('=', 50));
                    text.AppendLine($"{entry.EntryDate:dddd, MMMM d, yyyy}");
                    text.AppendLine(new string('-', 30));
                    text.AppendLine($"Title: {entry.Title}");

                    if (exportModel.IncludeMoods && entry.PrimaryMood != null)
                    {
                        text.AppendLine($"Mood: {entry.PrimaryMood.Emoji} {entry.PrimaryMood.Name}");
                    }

                    if (exportModel.IncludeCategory && entry.Category != null)
                    {
                        text.AppendLine($"Category: {entry.Category.Name}");
                    }

                    if (exportModel.IncludeTags && entry.JournalTags.Any())
                    {
                        var tags = string.Join(", ", entry.JournalTags.Select(jt => jt.Tag.Name));
                        text.AppendLine($"Tags: {tags}");
                    }

                    text.AppendLine($"Word Count: {entry.WordCount}");
                    text.AppendLine();
                    text.AppendLine(entry.Content);
                    text.AppendLine();
                    text.AppendLine($"Created: {entry.CreatedAt:MMMM d, yyyy h:mm tt}");
                    text.AppendLine($"Updated: {entry.UpdatedAt:MMMM d, yyyy h:mm tt}");
                    text.AppendLine();
                }

                // Save to file
                var fileName = $"JournalExport_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                await File.WriteAllTextAsync(filePath, text.ToString());

                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting to text: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ExportToHtmlAsync(ExportModel exportModel)
        {
            try
            {
                var entries = await _context.JournalEntries
                    .Include(e => e.PrimaryMood)
                    .Include(e => e.Category)
                    .Include(e => e.JournalTags)
                        .ThenInclude(jt => jt.Tag)
                    .Where(e => e.EntryDate >= exportModel.StartDate && e.EntryDate <= exportModel.EndDate)
                    .OrderBy(e => e.EntryDate)
                    .ToListAsync();

                if (!entries.Any())
                    throw new Exception("No entries found for the selected date range");

                var html = new System.Text.StringBuilder();

                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html lang='en'>");
                html.AppendLine("<head>");
                html.AppendLine("    <meta charset='UTF-8'>");
                html.AppendLine($"    <title>{exportModel.CoverPageTitle}</title>");
                html.AppendLine("    <style>");
                html.AppendLine("        body { font-family: Arial, sans-serif; line-height: 1.6; margin: 40px; }");
                html.AppendLine("        h1 { color: #333; border-bottom: 2px solid #4CAF50; padding-bottom: 10px; }");
                html.AppendLine("        h2 { color: #555; margin-top: 30px; }");
                html.AppendLine("        .entry { border: 1px solid #ddd; padding: 20px; margin-bottom: 30px; border-radius: 5px; }");
                html.AppendLine("        .metadata { color: #666; font-size: 0.9em; margin-bottom: 15px; }");
                html.AppendLine("        .content { margin-top: 15px; white-space: pre-wrap; }");
                html.AppendLine("        .tag { background-color: #e0e0e0; padding: 2px 8px; border-radius: 3px; margin-right: 5px; }");
                html.AppendLine("    </style>");
                html.AppendLine("</head>");
                html.AppendLine("<body>");

                // Header
                html.AppendLine($"<h1>{exportModel.CoverPageTitle}</h1>");
                if (!string.IsNullOrEmpty(exportModel.CoverPageSubtitle))
                {
                    html.AppendLine($"<h2>{exportModel.CoverPageSubtitle}</h2>");
                }
                html.AppendLine($"<p>Export Date: {DateTime.Now:dddd, MMMM d, yyyy}</p>");
                html.AppendLine($"<p>Date Range: {exportModel.StartDate:MMMM d, yyyy} to {exportModel.EndDate:MMMM d, yyyy}</p>");
                html.AppendLine($"<p>Total Entries: {entries.Count}</p>");
                html.AppendLine("<hr>");

                // Entries
                foreach (var entry in entries)
                {
                    html.AppendLine("<div class='entry'>");
                    html.AppendLine($"<h2>{entry.EntryDate:dddd, MMMM d, yyyy}</h2>");
                    html.AppendLine($"<h3>{entry.Title}</h3>");

                    html.AppendLine("<div class='metadata'>");

                    if (exportModel.IncludeMoods && entry.PrimaryMood != null)
                    {
                        html.AppendLine($"<span>Mood: {entry.PrimaryMood.Emoji} {entry.PrimaryMood.Name}</span> | ");
                    }

                    if (exportModel.IncludeCategory && entry.Category != null)
                    {
                        html.AppendLine($"<span>Category: {entry.Category.Name}</span> | ");
                    }

                    if (exportModel.IncludeTags && entry.JournalTags.Any())
                    {
                        html.AppendLine("<span>Tags: ");
                        foreach (var tag in entry.JournalTags.Select(jt => jt.Tag))
                        {
                            html.AppendLine($"<span class='tag'>{tag.Name}</span>");
                        }
                        html.AppendLine("</span> | ");
                    }

                    html.AppendLine($"<span>Word Count: {entry.WordCount}</span>");
                    html.AppendLine("</div>");

                    html.AppendLine($"<div class='content'>{entry.Content}</div>");
                    html.AppendLine($"<p><small>Created: {entry.CreatedAt:MMMM d, yyyy h:mm tt} | Updated: {entry.UpdatedAt:MMMM d, yyyy h:mm tt}</small></p>");
                    html.AppendLine("</div>");
                }

                html.AppendLine("</body>");
                html.AppendLine("</html>");

                // Save to file
                var fileName = $"JournalExport_{DateTime.Now:yyyyMMdd_HHmmss}.html";
                var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                await File.WriteAllTextAsync(filePath, html.ToString());

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting to HTML: {ex.Message}");
                return false;
            }
        }
    }
}