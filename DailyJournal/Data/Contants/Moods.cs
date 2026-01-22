namespace DailyJournal.Data.Constants
{
    public static class Moods
    {
        public static class Positive
        {
            public const string Happy = "Happy";
            public const string Excited = "Excited";
            public const string Relaxed = "Relaxed";
            public const string Grateful = "Grateful";
            public const string Confident = "Confident";

            public static List<string> GetAll() => new()
            {
                Happy, Excited, Relaxed, Grateful, Confident
            };
        }

        public static class Neutral
        {
            public const string Calm = "Calm";
            public const string Thoughtful = "Thoughtful";
            public const string Curious = "Curious";
            public const string Nostalgic = "Nostalgic";
            public const string Bored = "Bored";

            public static List<string> GetAll() => new()
            {
                Calm, Thoughtful, Curious, Nostalgic, Bored
            };
        }

        public static class Negative
        {
            public const string Sad = "Sad";
            public const string Angry = "Angry";
            public const string Stressed = "Stressed";
            public const string Lonely = "Lonely";
            public const string Anxious = "Anxious";

            public static List<string> GetAll() => new()
            {
                Sad, Angry, Stressed, Lonely, Anxious
            };
        }

        public static List<string> GetAllMoods()
        {
            var allMoods = new List<string>();
            allMoods.AddRange(Positive.GetAll());
            allMoods.AddRange(Neutral.GetAll());
            allMoods.AddRange(Negative.GetAll());
            return allMoods;
        }

        public static string GetCategoryForMood(string mood)
        {
            if (Positive.GetAll().Contains(mood))
                return "Positive";
            if (Neutral.GetAll().Contains(mood))
                return "Neutral";
            if (Negative.GetAll().Contains(mood))
                return "Negative";
            return "Neutral";
        }
    }
}