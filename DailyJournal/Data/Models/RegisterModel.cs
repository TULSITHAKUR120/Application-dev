namespace DailyJournal.Data.Models
{
    public class RegisterModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string PIN { get; set; } = string.Empty;
        public string ConfirmPIN { get; set; } = string.Empty;
        public bool UsePIN { get; set; } = true;
        public bool EnableBiometric { get; set; } = false;

        // Validation
        public bool IsValid =>
            !string.IsNullOrWhiteSpace(Username) &&
            Username.Length >= 3 &&
            !string.IsNullOrWhiteSpace(Password) &&
            Password.Length >= 6 &&
            Password == ConfirmPassword &&
            (!UsePIN || (PIN.Length == 4 && PIN == ConfirmPIN));

        public string ValidationMessage
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Username))
                    return "Username is required";

                if (Username.Length < 3)
                    return "Username must be at least 3 characters";

                if (string.IsNullOrWhiteSpace(Password))
                    return "Password is required";

                if (Password.Length < 6)
                    return "Password must be at least 6 characters";

                if (Password != ConfirmPassword)
                    return "Passwords do not match";

                if (UsePIN)
                {
                    if (string.IsNullOrWhiteSpace(PIN))
                        return "PIN is required when using PIN access";

                    if (PIN.Length != 4)
                        return "PIN must be 4 digits";

                    if (PIN != ConfirmPIN)
                        return "PINs do not match";
                }

                return string.Empty;
            }
        }
    }
}