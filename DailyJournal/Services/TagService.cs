using DailyJournal.Data.Database;
using DailyJournal.Data.Entities;
using DailyJournal.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyJournal.Services
{
    public class TagService
    {
        private readonly AppDbContext _context;

        public TagService()
        {
            _context = new AppDbContext();
        }

        public async Task<List<TagModel>> GetAllTagsAsync()
        {
            try
            {
                var tags = await _context.Tags
                    .OrderByDescending(t => t.UsageCount)
                    .ThenBy(t => t.Name)
                    .ToListAsync();

                return tags.Select(t => new TagModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Color = t.Color,
                    IsPredefined = t.IsPredefined,
                    UsageCount = t.UsageCount,
                    LastUsed = t.LastUsed
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting tags: {ex.Message}");
                return new List<TagModel>();
            }
        }

        public async Task<List<TagModel>> GetPopularTagsAsync(int count = 10)
        {
            try
            {
                var tags = await _context.Tags
                    .OrderByDescending(t => t.UsageCount)
                    .Take(count)
                    .ToListAsync();

                return tags.Select(t => new TagModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Color = t.Color,
                    IsPredefined = t.IsPredefined,
                    UsageCount = t.UsageCount,
                    LastUsed = t.LastUsed
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting popular tags: {ex.Message}");
                return new List<TagModel>();
            }
        }

        public async Task<TagModel> GetTagByNameAsync(string name)
        {
            try
            {
                var tag = await _context.Tags
                    .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

                if (tag == null)
                    return null;

                return new TagModel
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    Color = tag.Color,
                    IsPredefined = tag.IsPredefined,
                    UsageCount = tag.UsageCount,
                    LastUsed = tag.LastUsed
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting tag by name: {ex.Message}");
                return null;
            }
        }

        public async Task<TagModel> CreateTagAsync(string name, string color = "#9C27B0")
        {
            try
            {
                var tag = new Tag
                {
                    Name = name,
                    Color = color,
                    IsPredefined = false,
                    UsageCount = 0,
                    LastUsed = DateTime.UtcNow
                };

                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();

                return new TagModel
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    Color = tag.Color,
                    IsPredefined = tag.IsPredefined,
                    UsageCount = tag.UsageCount,
                    LastUsed = tag.LastUsed
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating tag: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateTagUsageAsync(int tagId)
        {
            try
            {
                var tag = await _context.Tags.FindAsync(tagId);
                if (tag != null)
                {
                    tag.UsageCount++;
                    tag.LastUsed = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating tag usage: {ex.Message}");
                return false;
            }
        }
    }
}