using DailyJournal.Data.Database;
using DailyJournal.Data.Entities;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DailyJournal.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService()
        {
            _context = new AppDbContext();
        }

        public async Task<bool> RegisterUserAsync(string username, string password, string pin = null)
        {
            try
            {
                // Check if user already exists
                if (await _context.Users.AnyAsync(u => u.Username == username))
                    return false;

                // Generate salt
                var salt = GenerateSalt();

                // Create user
                var user = new User
                {
                    Username = username,
                    Salt = salt,
                    PasswordHash = HashPassword(password, salt),
                    PIN = pin,
                    CreatedAt = DateTime.UtcNow,
                    IsDarkTheme = false,
                    PrimaryColor = "#2196F3",
                    SecondaryColor = "#FF9800",
                    EnableBiometric = false
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Create streak record for user
                var streak = new Streak
                {
                    UserId = user.Id,
                    CurrentStreak = 0,
                    LongestStreak = 0,
                    LastEntryDate = DateTime.MinValue,
                    TotalEntries = 0,
                    TotalMissedDays = 0,
                    StreakStartDate = DateTime.UtcNow
                };

                _context.Streaks.Add(streak);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
                return false;
            }
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                    return null;

                var hashedPassword = HashPassword(password, user.Salt);
                if (hashedPassword != user.PasswordHash)
                    return null;

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error authenticating user: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AuthenticatePinAsync(string pin)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync();
                if (user == null)
                    return false;

                return user.PIN == pin;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error authenticating PIN: {ex.Message}");
                return false;
            }
        }

        public async Task<User> GetCurrentUserAsync()
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting current user: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateUserSettingsAsync(bool isDarkTheme, string primaryColor, string secondaryColor, bool enableBiometric)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync();
                if (user == null)
                    return false;

                user.IsDarkTheme = isDarkTheme;
                user.PrimaryColor = primaryColor;
                user.SecondaryColor = secondaryColor;
                user.EnableBiometric = enableBiometric;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user settings: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdatePinAsync(string newPin)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync();
                if (user == null)
                    return false;

                user.PIN = newPin;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating PIN: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync();
                if (user == null)
                    return false;

                var currentHash = HashPassword(currentPassword, user.Salt);
                if (currentHash != user.PasswordHash)
                    return false;

                user.PasswordHash = HashPassword(newPassword, user.Salt);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing password: {ex.Message}");
                return false;
            }
        }

        private string GenerateSalt()
        {
            var saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                var bytes = Encoding.UTF8.GetBytes(saltedPassword);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}