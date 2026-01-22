namespace DailyJournal.Data.Models
{
    public class JournalResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public JournalEntryModel? Entry { get; set; }

        public static JournalResult SuccessResult(string message, JournalEntryModel? entry = null)
        {
            return new JournalResult
            {
                Success = true,
                Message = message,
                Entry = entry
            };
        }

        public static JournalResult FailureResult(string message)
        {
            return new JournalResult
            {
                Success = false,
                Message = message,
                Entry = null
            };
        }
    }
}
