namespace DailyJournal.Data.Models
{
    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PIN { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
        public bool UseBiometric { get; set; } = false;

        // Validation
        public bool IsValid => !string.IsNullOrWhiteSpace(Username) &&
                              (!string.IsNullOrWhiteSpace(Password) || !string.IsNullOrWhiteSpace(PIN));

        public string ValidationMessage
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Username))
                    return "Username is required";

                if (string.IsNullOrWhiteSpace(Password) && string.IsNullOrWhiteSpace(PIN))
                    return "Password or PIN is required";

                return string.Empty;
            }
        }
    }
}