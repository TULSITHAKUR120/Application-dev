using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DailyJournal.Data.Database;
using DailyJournal.Data.Entities;
using DailyJournal.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyJournal.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            return !await UserExistsAsync(username);
        }

        public async Task<RegistrationResult> RegisterAsync(RegisterModel model)
        {
            try
            {
                // 1. Check if user already exists
                var existingUser = await _context.Users.AnyAsync(u => u.Username.ToLower() == model.Username.ToLower());
                if (existingUser)
                {
                    return new RegistrationResult { Success = false, Message = "This username is already taken." };
                }

                // 2. Map Model to Entity
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = HashText(model.Password),
                    PIN = model.UsePIN ? HashText(model.PIN) : null,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                // 3. Save to Database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return new RegistrationResult { Success = true, Message = "Account created successfully!" };
            }
            catch (Exception ex)
            {
                // Log the exception here if you have a logger
                return new RegistrationResult { Success = false, Message = "Database error: " + ex.InnerException?.Message ?? ex.Message };
            }
        }

        public async Task<LoginResult> LoginAsync(string username, string password, bool rememberMe = false)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || user.PasswordHash != HashText(password))
            {
                return new LoginResult { Success = false, Message = "Invalid credentials" };
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new LoginResult
            {
                Success = true,
                UserId = user.Id,
                Username = user.Username
            };
        }

        private string HashText(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Convert.ToHexString(hash);
        }
    }
}