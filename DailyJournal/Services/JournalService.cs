//using DailyJournal.Data.Database;
//using DailyJournal.Data.Entities;
//using DailyJournal.Data.Models;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Maui;

//namespace DailyJournal.Services
//{
//    public class JournalService
//    {
//        private readonly AppDbContext _context;

//        public JournalService()
//        {
//            _context = new AppDbContext();
//        }

//        public async Task<List<JournalEntryModel>> GetEntriesAsync(FilterModel filter)
//        {
//            try
//            {
//                var query = _context.JournalEntries
//                    .Include(e => e.PrimaryMood)
//                    .Include(e => e.SecondaryMood1)
//                    .Include(e => e.SecondaryMood2)
//                    .Include(e => e.Category)
//                    .Include(e => e.JournalTags)
//                        .ThenInclude(jt => jt.Tag)
//                    .AsQueryable();

//                // Apply filters
//                if (filter.StartDate.HasValue)
//                    query = query.Where(e => e.EntryDate >= filter.StartDate.Value);

//                if (filter.EndDate.HasValue)
//                    query = query.Where(e => e.EntryDate <= filter.EndDate.Value);

//                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
//                {
//                    query = query.Where(e =>
//                        e.Title.Contains(filter.SearchTerm) ||
//                        e.Content.Contains(filter.SearchTerm));
//                }

//                if (filter.MoodIds.Any())
//                {
//                    query = query.Where(e =>
//                        filter.MoodIds.Contains(e.PrimaryMoodId) ||
//                        (e.SecondaryMood1Id.HasValue && filter.MoodIds.Contains(e.SecondaryMood1Id.Value)) ||
//                        (e.SecondaryMood2Id.HasValue && filter.MoodIds.Contains(e.SecondaryMood2Id.Value)));
//                }

//                if (filter.TagIds.Any())
//                {
//                    query = query.Where(e =>
//                        e.JournalTags.Any(jt => filter.TagIds.Contains(jt.TagId)));
//                }

//                if (filter.CategoryId.HasValue)
//                    query = query.Where(e => e.CategoryId == filter.CategoryId.Value);

//                // Apply sorting
//                query = filter.SortBy switch
//                {
//                    "Title" => filter.SortDescending
//                        ? query.OrderByDescending(e => e.Title)
//                        : query.OrderBy(e => e.Title),
//                    "CreatedAt" => filter.SortDescending
//                        ? query.OrderByDescending(e => e.CreatedAt)
//                        : query.OrderBy(e => e.CreatedAt),
//                    "UpdatedAt" => filter.SortDescending
//                        ? query.OrderByDescending(e => e.UpdatedAt)
//                        : query.OrderBy(e => e.UpdatedAt),
//                    _ => filter.SortDescending
//                        ? query.OrderByDescending(e => e.EntryDate)
//                        : query.OrderBy(e => e.EntryDate)
//                };

//                // Pagination
//                var totalCount = await query.CountAsync();
//                var items = await query
//                    .Skip((filter.PageNumber - 1) * filter.PageSize)
//                    .Take(filter.PageSize)
//                    .ToListAsync();

//                // Convert to models
//                var models = items.Select(e => new JournalEntryModel
//                {
//                    Id = e.Id,
//                    EntryDate = e.EntryDate,
//                    Title = e.Title,
//                    Content = e.Content,
//                    ContentPreview = e.ContentPreview,
//                    IsRichText = e.IsRichText,
//                    WordCount = e.WordCount,
//                    CharacterCount = e.CharacterCount,
//                    CreatedAt = e.CreatedAt,
//                    UpdatedAt = e.UpdatedAt,
//                    PrimaryMood = e.PrimaryMood != null ? new MoodModel
//                    {
//                        Id = e.PrimaryMood.Id,
//                        Name = e.PrimaryMood.Name,
//                        MoodType = e.PrimaryMood.MoodType,
//                        Emoji = e.PrimaryMood.Emoji,
//                        Color = e.PrimaryMood.Color,
//                        IsPredefined = e.PrimaryMood.IsPredefined
//                    } : null,
//                    SecondaryMood1 = e.SecondaryMood1 != null ? new MoodModel
//                    {
//                        Id = e.SecondaryMood1.Id,
//                        Name = e.SecondaryMood1.Name,
//                        MoodType = e.SecondaryMood1.MoodType,
//                        Emoji = e.SecondaryMood1.Emoji,
//                        Color = e.SecondaryMood1.Color,
//                        IsPredefined = e.SecondaryMood1.IsPredefined
//                    } : null,
//                    SecondaryMood2 = e.SecondaryMood2 != null ? new MoodModel
//                    {
//                        Id = e.SecondaryMood2.Id,
//                        Name = e.SecondaryMood2.Name,
//                        MoodType = e.SecondaryMood2.MoodType,
//                        Emoji = e.SecondaryMood2.Emoji,
//                        Color = e.SecondaryMood2.Color,
//                        IsPredefined = e.SecondaryMood2.IsPredefined
//                    } : null,
//                    Category = e.Category != null ? new CategoryModel
//                    {
//                        Id = e.Category.Id,
//                        Name = e.Category.Name,
//                        Description = e.Category.Description,
//                        Color = e.Category.Color,
//                        IsPredefined = e.Category.IsPredefined
//                    } : null,
//                    Tags = e.JournalTags.Select(jt => new TagModel
//                    {
//                        Id = jt.Tag.Id,
//                        Name = jt.Tag.Name,
//                        Color = jt.Tag.Color,
//                        IsPredefined = jt.Tag.IsPredefined,
//                        UsageCount = jt.Tag.UsageCount,
//                        LastUsed = jt.Tag.LastUsed
//                    }).ToList()
//                }).ToList();

//                return models;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting entries: {ex.Message}");
//                return new List<JournalEntryModel>();
//            }
//        }

//        public async Task<JournalEntryModel> GetEntryByDateAsync(DateTime date)
//        {
//            try
//            {
//                var entry = await _context.JournalEntries
//                    .Include(e => e.PrimaryMood)
//                    .Include(e => e.SecondaryMood1)
//                    .Include(e => e.SecondaryMood2)
//                    .Include(e => e.Category)
//                    .Include(e => e.JournalTags)
//                        .ThenInclude(jt => jt.Tag)
//                    .FirstOrDefaultAsync(e => e.EntryDate.Date == date.Date);

//                if (entry == null)
//                {
//                    return new JournalEntryModel
//                    {
//                        EntryDate = date,
//                        Title = "Untitled Entry",
//                        Content = string.Empty,
//                        IsRichText = true
//                    };
//                }

//                return new JournalEntryModel
//                {
//                    Id = entry.Id,
//                    EntryDate = entry.EntryDate,
//                    Title = entry.Title,
//                    Content = entry.Content,
//                    ContentPreview = entry.ContentPreview,
//                    IsRichText = entry.IsRichText,
//                    WordCount = entry.WordCount,
//                    CharacterCount = entry.CharacterCount,
//                    CreatedAt = entry.CreatedAt,
//                    UpdatedAt = entry.UpdatedAt,
//                    PrimaryMood = entry.PrimaryMood != null ? new MoodModel
//                    {
//                        Id = entry.PrimaryMood.Id,
//                        Name = entry.PrimaryMood.Name,
//                        MoodType = entry.PrimaryMood.MoodType,
//                        Emoji = entry.PrimaryMood.Emoji,
//                        Color = entry.PrimaryMood.Color,
//                        IsPredefined = entry.PrimaryMood.IsPredefined
//                    } : null,
//                    SecondaryMood1 = entry.SecondaryMood1 != null ? new MoodModel
//                    {
//                        Id = entry.SecondaryMood1.Id,
//                        Name = entry.SecondaryMood1.Name,
//                        MoodType = entry.SecondaryMood1.MoodType,
//                        Emoji = entry.SecondaryMood1.Emoji,
//                        Color = entry.SecondaryMood1.Color,
//                        IsPredefined = entry.SecondaryMood1.IsPredefined
//                    } : null,
//                    SecondaryMood2 = entry.SecondaryMood2 != null ? new MoodModel
//                    {
//                        Id = entry.SecondaryMood2.Id,
//                        Name = entry.SecondaryMood2.Name,
//                        MoodType = entry.SecondaryMood2.MoodType,
//                        Emoji = entry.SecondaryMood2.Emoji,
//                        Color = entry.SecondaryMood2.Color,
//                        IsPredefined = entry.SecondaryMood2.IsPredefined
//                    } : null,
//                    Category = entry.Category != null ? new CategoryModel
//                    {
//                        Id = entry.Category.Id,
//                        Name = entry.Category.Name,
//                        Description = entry.Category.Description,
//                        Color = entry.Category.Color,
//                        IsPredefined = entry.Category.IsPredefined
//                    } : null,
//                    Tags = entry.JournalTags.Select(jt => new TagModel
//                    {
//                        Id = jt.Tag.Id,
//                        Name = jt.Tag.Name,
//                        Color = jt.Tag.Color,
//                        IsPredefined = jt.Tag.IsPredefined,
//                        UsageCount = jt.Tag.UsageCount,
//                        LastUsed = jt.Tag.LastUsed
//                    }).ToList()
//                };
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting entry by date: {ex.Message}");
//                return new JournalEntryModel
//                {
//                    EntryDate = date,
//                    Title = "Untitled Entry",
//                    Content = string.Empty,
//                    IsRichText = true
//                };
//            }
//        }

//        public async Task<bool> SaveEntryAsync(JournalEntryModel model)
//        {
//            try
//            {
//                // Check if entry already exists for this date
//                var existingEntry = await _context.JournalEntries
//                    .Include(e => e.JournalTags)
//                    .FirstOrDefaultAsync(e => e.EntryDate.Date == model.EntryDate.Date);

//                JournalEntry entry;
//                bool isNew = false;

//                if (existingEntry == null)
//                {
//                    // Create new entry
//                    entry = new JournalEntry
//                    {
//                        EntryDate = model.EntryDate.Date,
//                        CreatedAt = DateTime.UtcNow,
//                        JournalTags = new List<JournalTag>()
//                    };
//                    _context.JournalEntries.Add(entry);
//                    isNew = true;
//                }
//                else
//                {
//                    entry = existingEntry;
//                }

//                // Update entry properties
//                entry.Title = model.Title ?? "Untitled Entry";
//                entry.Content = model.Content ?? string.Empty;
//                entry.IsRichText = model.IsRichText;
//                entry.UpdatedAt = DateTime.UtcNow;

//                // Calculate word count
//                entry.WordCount = CalculateWordCount(entry.Content);
//                entry.CharacterCount = entry.Content.Length;
//                entry.ContentPreview = entry.Content.Length > 100
//                    ? entry.Content.Substring(0, 100) + "..."
//                    : entry.Content;

//                // Update moods
//                if (model.PrimaryMood != null)
//                    entry.PrimaryMoodId = model.PrimaryMood.Id;

//                if (model.SecondaryMood1 != null)
//                    entry.SecondaryMood1Id = model.SecondaryMood1.Id;
//                else
//                    entry.SecondaryMood1Id = null;

//                if (model.SecondaryMood2 != null)
//                    entry.SecondaryMood2Id = model.SecondaryMood2.Id;
//                else
//                    entry.SecondaryMood2Id = null;

//                // Update category
//                if (model.Category != null)
//                    entry.CategoryId = model.Category.Id;
//                else
//                    entry.CategoryId = null;

//                // Update tags
//                entry.JournalTags.Clear();
//                if (model.Tags != null && model.Tags.Any())
//                {
//                    foreach (var tagModel in model.Tags)
//                    {
//                        var tag = await _context.Tags.FindAsync(tagModel.Id);
//                        if (tag != null)
//                        {
//                            entry.JournalTags.Add(new JournalTag
//                            {
//                                JournalEntry = entry,
//                                Tag = tag,
//                                AddedAt = DateTime.UtcNow
//                            });

//                            // Update tag usage
//                            tag.UsageCount++;
//                            tag.LastUsed = DateTime.UtcNow;
//                        }
//                    }
//                }

//                await _context.SaveChangesAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error saving entry: {ex.Message}");
//                return false;
//            }
//        }

//        public async Task<bool> DeleteEntryAsync(DateTime date)
//        {
//            try
//            {
//                var entry = await _context.JournalEntries
//                    .FirstOrDefaultAsync(e => e.EntryDate.Date == date.Date);

//                if (entry != null)
//                {
//                    _context.JournalEntries.Remove(entry);
//                    await _context.SaveChangesAsync();
//                    return true;
//                }

//                return false;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error deleting entry: {ex.Message}");
//                return false;
//            }
//        }

//        public async Task<bool> EntryExistsForDateAsync(DateTime date)
//        {
//            try
//            {
//                return await _context.JournalEntries
//                    .AnyAsync(e => e.EntryDate.Date == date.Date);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error checking entry: {ex.Message}");
//                return false;
//            }
//        }

//        public async Task<List<DateTime>> GetEntryDatesAsync(DateTime startDate, DateTime endDate)
//        {
//            try
//            {
//                return await _context.JournalEntries
//                    .Where(e => e.EntryDate >= startDate && e.EntryDate <= endDate)
//                    .Select(e => e.EntryDate.Date)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting entry dates: {ex.Message}");
//                return new List<DateTime>();
//            }
//        }

//        private int CalculateWordCount(string text)
//        {
//            if (string.IsNullOrWhiteSpace(text))
//                return 0;

//            var wordCount = 0;
//            var isWord = false;

//            foreach (var c in text)
//            {
//                if (char.IsLetterOrDigit(c))
//                {
//                    if (!isWord)
//                    {
//                        wordCount++;
//                        isWord = true;
//                    }
//                }
//                else
//                {
//                    isWord = false;
//                }
//            }

//            return wordCount;
//        }
//    }
//}