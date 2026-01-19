using System.Security.Cryptography;
using System.Text;

namespace DailyJournal.Helpers
{
    public static class SecurityHelper
    {
        public static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                var bytes = Encoding.UTF8.GetBytes(saltedPassword);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static string GenerateSalt()
        {
            var saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string GenerateRandomPIN()
        {
            var random = new Random();
            return random.Next(1000, 9999).ToString();
        }

        public static bool ValidatePIN(string pin)
        {
            return pin.Length == 4 && pin.All(char.IsDigit);
        }
    }
}