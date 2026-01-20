//using DailyJournal.Data.Database;
//using DailyJournal.Data.Entities;
//using Microsoft.EntityFrameworkCore;

//namespace DailyJournal.Services
//{
//    public class StreakService
//    {
//        private readonly AppDbContext _context;
//        private readonly JournalService _journalService;

//        public StreakService()
//        {
//            _context = new AppDbContext();
//            _journalService = new JournalService();
//        }

//        public async Task<Streak> GetCurrentStreakAsync()
//        {
//            try
//            {
//                var user = await _context.Users.FirstOrDefaultAsync();
//                if (user == null)
//                    return null;

//                var streak = await _context.Streaks
//                    .FirstOrDefaultAsync(s => s.UserId == user.Id);

//                return streak;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting streak: {ex.Message}");
//                return null;
//            }
//        }

//        public async Task<bool> UpdateStreakAsync(DateTime entryDate)
//        {
//            try
//            {
//                var user = await _context.Users.FirstOrDefaultAsync();
//                if (user == null)
//                    return false;

//                var streak = await _context.Streaks
//                    .FirstOrDefaultAsync(s => s.UserId == user.Id);

//                if (streak == null)
//                {
//                    streak = new Streak
//                    {
//                        UserId = user.Id,
//                        CurrentStreak = 1,
//                        LongestStreak = 1,
//                        LastEntryDate = entryDate,
//                        TotalEntries = 1,
//                        TotalMissedDays = 0,
//                        StreakStartDate = DateTime.UtcNow
//                    };
//                    _context.Streaks.Add(streak);
//                }
//                else
//                {
//                    // Calculate if entry is consecutive
//                    var daysSinceLastEntry = (entryDate.Date - streak.LastEntryDate.Date).Days;

//                    if (daysSinceLastEntry == 1)
//                    {
//                        // Consecutive day
//                        streak.CurrentStreak++;
//                        streak.LastEntryDate = entryDate;
//                        streak.TotalEntries++;

//                        // Update longest streak if needed
//                        if (streak.CurrentStreak > streak.LongestStreak)
//                        {
//                            streak.LongestStreak = streak.CurrentStreak;
//                        }
//                    }
//                    else if (daysSinceLastEntry == 0)
//                    {
//                        // Same day - update existing entry
//                        // Total entries remains the same
//                    }
//                    else
//                    {
//                        // Broken streak
//                        streak.CurrentStreak = 1;
//                        streak.LastEntryDate = entryDate;
//                        streak.TotalEntries++;
//                        streak.TotalMissedDays += daysSinceLastEntry - 1;
//                        streak.StreakStartDate = DateTime.UtcNow;
//                    }
//                }

//                await _context.SaveChangesAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error updating streak: {ex.Message}");
//                return false;
//            }
//        }

//        public async Task<List<DateTime>> GetMissedDaysAsync(DateTime startDate, DateTime endDate)
//        {
//            try
//            {
//                var entryDates = await _journalService.GetEntryDatesAsync(startDate, endDate);
//                var missedDays = new List<DateTime>();

//                var currentDate = startDate.Date;
//                while (currentDate <= endDate.Date)
//                {
//                    if (!entryDates.Contains(currentDate))
//                    {
//                        missedDays.Add(currentDate);
//                    }
//                    currentDate = currentDate.AddDays(1);
//                }

//                return missedDays;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting missed days: {ex.Message}");
//                return new List<DateTime>();
//            }
//        }

//        public async Task<int> CalculateConsistencyPercentageAsync(DateTime startDate, DateTime endDate)
//        {
//            try
//            {
//                var entryDates = await _journalService.GetEntryDatesAsync(startDate, endDate);
//                var totalDays = (endDate.Date - startDate.Date).Days + 1;
//                var entryCount = entryDates.Count;

//                if (totalDays == 0)
//                    return 0;

//                return (int)((double)entryCount / totalDays * 100);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error calculating consistency: {ex.Message}");
//                return 0;
//            }
//        }
//    }
//}