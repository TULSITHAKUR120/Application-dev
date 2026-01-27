using DailyJournal.Data.Database;
using DailyJournal.Data.Entities;
using DailyJournal.Data.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyJournal.Services;

public class ExportService
{
    private readonly AppDbContext _context;

    public ExportService(AppDbContext context)
    {
        _context = context;
        // Optimization: Ensure license is set once
        QuestPDF.Settings.License = LicenseType.Community;
    }

   

    // Main export method
    public async Task<ExportResult> ExportEntriesAsync(ExportRequest request)
    {
        try
        {
            // Validate dates
            if (request.StartDate > request.EndDate)
            {
                return ExportResult.ErrorResult("Start date cannot be later than end date");
            }

            // Get entries for the date range
            var entries = await GetEntriesAsync(request.UserId, request.StartDate?? DateTime.Today, request.EndDate ?? DateTime.Today);

            if (entries.Count == 0)
            {
                return ExportResult.ErrorResult("No entries found for the selected date range");
            }

            // Generate PDF
            var pdfContent = GeneratePdf(entries, request);
            var fileName = GenerateFileName(request, entries.Count);

            return ExportResult.SuccessResult(fileName, pdfContent, entries.Count);
        }
        catch (Exception ex)
        {
            return ExportResult.ErrorResult($"Error generating export: {ex.Message}");
        }
    }

    private async Task<List<JournalEntry>> GetEntriesAsync(int userId, DateTime start, DateTime end)
    {
        var startDate = start.Date;
        var endDate = end.Date.AddDays(1).AddTicks(-1);

        return await _context.JournalEntries
            .AsNoTracking()
            .Where(e => e.UserId == userId && e.EntryDate >= startDate && e.EntryDate <= endDate)
            .OrderBy(e => e.EntryDate)
            .ToListAsync();
    }

    private byte[] GeneratePdf(List<JournalEntry> entries, ExportRequest request)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(QuestPDF.Helpers.Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Verdana));

                page.Header().Element(c => ComposeHeader(c, request));
                page.Content().Element(c => ComposeContent(c, entries, request));

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    private void ComposeHeader(QuestPDF.Infrastructure.IContainer container, ExportRequest request)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text(request.Title).FontSize(24).SemiBold().FontColor(QuestPDF.Helpers.Colors.Blue.Medium);
                column.Item().Text(request.Subtitle).FontSize(12).FontColor(QuestPDF.Helpers.Colors.Grey.Medium);
                column.Item().Text($"{request.StartDate:yyyy-MM-dd} to {request.EndDate:yyyy-MM-dd}").FontSize(10);
            });

            row.ConstantItem(100).AlignRight().Text(DateTime.Now.ToString("d")).FontSize(10).FontColor(QuestPDF.Helpers.Colors.Grey.Medium);
        });
    }

    private void ComposeContent(QuestPDF.Infrastructure.IContainer container, List<JournalEntry> entries, ExportRequest request)
    {
        container.PaddingVertical(10).Column(column =>
        {
            column.Spacing(15);

            if (request.IncludeStats)
            {
                column.Item().Background(QuestPDF.Helpers.Colors.Grey.Lighten4).Padding(10).Column(c =>
                {
                    c.Item().Text("Quick Stats").Bold();
                    c.Item().Text($"Total Entries: {entries.Count}");
                    // Use the WordCount property directly from your entity
                    c.Item().Text($"Total Word Count: {entries.Sum(e => e.WordCount)}");
                });
            }

            foreach (var entry in entries)
            {
                column.Item().ShowEntire().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten3).PaddingBottom(10).Column(e =>
                {
                    e.Spacing(5);
                    e.Item().Row(r => {
                        r.RelativeItem().Text(entry.EntryDate.ToString("dddd, MMMM dd, yyyy")).Bold().FontSize(12);
                        if (entry != null && !string.IsNullOrEmpty(entry.PrimaryMood))
                        {
                            r.AutoItem().Text(entry.PrimaryMood).FontColor(GetMoodColor(entry.PrimaryMood)).Italic();
                        }
                    });

                    e.Item().Text(entry.Title).FontSize(14).SemiBold();

                    if (request.IncludeTags && !string.IsNullOrEmpty(entry.Tags))
                    {
                        e.Item().Text($"Tags: {entry.Tags}").FontSize(9).FontColor(QuestPDF.Helpers.Colors.Blue.Medium);
                    }

                    e.Item().PaddingTop(5).Text(entry.Content);
                });
            }
        });
    }

    // FIX: Method now returns proper QuestPDF Color
    private static QuestPDF.Infrastructure.Color GetMoodColor(string mood)
    {
        if (string.IsNullOrEmpty(mood)) return QuestPDF.Helpers.Colors.Black;

        var m = mood.ToLower();
        if (new[] { "happy", "excited", "grateful", "content" }.Contains(m))
            return QuestPDF.Helpers.Colors.Green.Medium;

        if (new[] { "sad", "angry", "anxious", "stressed" }.Contains(m))
            return QuestPDF.Helpers.Colors.Red.Medium;

        return QuestPDF.Helpers.Colors.Orange.Medium;
    }

    private static string GenerateFileName(ExportRequest request, int count)
    {
        return $"Journal_Export_{request.StartDate:yyyyMMdd}_to_{request.EndDate:yyyyMMdd}_{count}_entries_{DateTime.Now:HHmm}.pdf";
    }
}

