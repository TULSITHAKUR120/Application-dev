//using DailyJournal.Data.Database;
//using DailyJournal.Data.Entities;
//using DailyJournal.Data.Models;
//using Microsoft.EntityFrameworkCore;

//namespace DailyJournal.Services
//{
//    public class CategoryService
//    {
//        private readonly AppDbContext _context;

//        public CategoryService()
//        {
//            _context = new AppDbContext();
//        }

//        public async Task<List<CategoryModel>> GetAllCategoriesAsync()
//        {
//            try
//            {
//                var categories = await _context.EntryCategories
//                    .OrderBy(c => c.DisplayOrder)
//                    .ToListAsync();

//                return categories.Select(c => new CategoryModel
//                {
//                    Id = c.Id,
//                    Name = c.Name,
//                    Description = c.Description,
//                    Color = c.Color,
//                    IsPredefined = c.IsPredefined,
//                    DisplayOrder = c.DisplayOrder
//                }).ToList();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting categories: {ex.Message}");
//                return new List<CategoryModel>();
//            }
//        }

//        public async Task<CategoryModel> GetCategoryByIdAsync(int id)
//        {
//            try
//            {
//                var category = await _context.EntryCategories.FindAsync(id);
//                if (category == null)
//                    return null;

//                return new CategoryModel
//                {
//                    Id = category.Id,
//                    Name = category.Name,
//                    Description = category.Description,
//                    Color = category.Color,
//                    IsPredefined = category.IsPredefined,
//                    DisplayOrder = category.DisplayOrder
//                };
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting category by id: {ex.Message}");
//                return null;
//            }
//        }

//        public async Task<bool> CreateCategoryAsync(CategoryModel model)
//        {
//            try
//            {
//                var category = new EntryCategory
//                {
//                    Name = model.Name,
//                    Description = model.Description,
//                    Color = model.Color,
//                    IsPredefined = false,
//                    DisplayOrder = await _context.EntryCategories.CountAsync() + 1
//                };

//                _context.EntryCategories.Add(category);
//                await _context.SaveChangesAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error creating category: {ex.Message}");
//                return false;
//            }
//        }
//    }
//}