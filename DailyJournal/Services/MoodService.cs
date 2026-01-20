//using DailyJournal.Data.Database;
//using DailyJournal.Data.Entities;
//using DailyJournal.Data.Models;
//using Microsoft.EntityFrameworkCore;

//namespace DailyJournal.Services
//{
//    public class MoodService
//    {
//        private readonly AppDbContext _context;

//        public MoodService()
//        {
//            _context = new AppDbContext();
//        }

//        public async Task<List<MoodModel>> GetAllMoodsAsync()
//        {
//            try
//            {
//                var moods = await _context.Moods
//                    .OrderBy(m => m.DisplayOrder)
//                    .ToListAsync();

//                return moods.Select(m => new MoodModel
//                {
//                    Id = m.Id,
//                    Name = m.Name,
//                    MoodType = m.MoodType,
//                    Emoji = m.Emoji,
//                    Color = m.Color,
//                    IsPredefined = m.IsPredefined,
//                    DisplayOrder = m.DisplayOrder
//                }).ToList();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting moods: {ex.Message}");
//                return new List<MoodModel>();
//            }
//        }

//        public async Task<List<MoodModel>> GetMoodsByTypeAsync(string moodType)
//        {
//            try
//            {
//                var moods = await _context.Moods
//                    .Where(m => m.MoodType == moodType)
//                    .OrderBy(m => m.DisplayOrder)
//                    .ToListAsync();

//                return moods.Select(m => new MoodModel
//                {
//                    Id = m.Id,
//                    Name = m.Name,
//                    MoodType = m.MoodType,
//                    Emoji = m.Emoji,
//                    Color = m.Color,
//                    IsPredefined = m.IsPredefined,
//                    DisplayOrder = m.DisplayOrder
//                }).ToList();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting moods by type: {ex.Message}");
//                return new List<MoodModel>();
//            }
//        }

//        public async Task<MoodModel> GetMoodByIdAsync(int id)
//        {
//            try
//            {
//                var mood = await _context.Moods.FindAsync(id);
//                if (mood == null)
//                    return null;

//                return new MoodModel
//                {
//                    Id = mood.Id,
//                    Name = mood.Name,
//                    MoodType = mood.MoodType,
//                    Emoji = mood.Emoji,
//                    Color = mood.Color,
//                    IsPredefined = mood.IsPredefined,
//                    DisplayOrder = mood.DisplayOrder
//                };
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting mood by id: {ex.Message}");
//                return null;
//            }
//        }

//        public async Task<bool> CreateMoodAsync(MoodModel model)
//        {
//            try
//            {
//                var mood = new Mood
//                {
//                    Name = model.Name,
//                    MoodType = model.MoodType,
//                    Emoji = model.Emoji,
//                    Color = model.Color,
//                    IsPredefined = false,
//                    DisplayOrder = await _context.Moods.CountAsync() + 1
//                };

//                _context.Moods.Add(mood);
//                await _context.SaveChangesAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error creating mood: {ex.Message}");
//                return false;
//            }
//        }
//    }
//}