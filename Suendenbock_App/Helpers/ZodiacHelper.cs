namespace Suendenbock_App.Helpers
{
    public static class ZodiacHelper
    {
        public static string GetZodiacSign(DateTime birthDate)
        {
            int day = birthDate.Day;
            int month = birthDate.Month;
            return month switch
            {
                1 => day <= 19 ? "Steinbock" : "Wassermann",
                2 => day <= 18 ? "Wassermann" : "Fische",
                3 => day <= 20 ? "Fische" : "Widder",
                4 => day <= 19 ? "Widder" : "Stier",
                5 => day <= 20 ? "Stier" : "Zwillinge",
                6 => day <= 20 ? "Zwillinge" : "Krebs",
                7 => day <= 22 ? "Krebs" : "Löwe",
                8 => day <= 22 ? "Löwe" : "Jungfrau",
                9 => day <= 22 ? "Jungfrau" : "Waage",
                10 => day <= 22 ? "Waage" : "Skorpion",
                11 => day <= 21 ? "Skorpion" : "Schütze",
                12 => day <= 21 ? "Schütze" : "Steinbock",
                _ => throw new ArgumentOutOfRangeException("Invalid month value")
            };
        }

        public static string? GetZodiacElement(string? birthDateString)
        {
            if(string.IsNullOrEmpty(birthDateString))
                return null;

            if(DateTime.TryParse(birthDateString, out DateTime birthDate))
            {
                return GetZodiacSign(birthDate);
            }

            return null;
        }
    }
}
