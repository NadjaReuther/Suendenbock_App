using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Services
{
    /// <summary>
    /// Texte und Hilfen für Termin-Benachrichtigungen
    /// </summary>
    public static class EventNotificationTexts
    {
        public const string EventsUrl = "/App/Termine";

        private static readonly string[] DayNames = { "So", "Mo", "Di", "Mi", "Do", "Fr", "Sa" };

        /// <summary>z.B. "Sa, 28.11."</summary>
        public static string FormatDate(DateTime date) =>
            $"{DayNames[(int)date.DayOfWeek]}, {date:dd.MM.}";

        /// <summary>z.B. "14:00–22:00", "14:00" oder ""</summary>
        public static string FormatTime(MonthlyEvent evt)
        {
            if (string.IsNullOrWhiteSpace(evt.StartTime))
                return "";
            if (string.IsNullOrWhiteSpace(evt.EndTime))
                return evt.StartTime;
            return $"{evt.StartTime}–{evt.EndTime}";
        }

        /// <summary>z.B. "Sa, 28.11. · 14:00–22:00"</summary>
        public static string FormatWhen(MonthlyEvent evt)
        {
            var time = FormatTime(evt);
            return string.IsNullOrEmpty(time) ? FormatDate(evt.Date) : $"{FormatDate(evt.Date)} · {time}";
        }

        /// <summary>
        /// Dienste werden als freier Text eingetragen. Wir suchen den Dienst,
        /// dessen Name zum Benutzer passt (Name, Login ohne Domain, Vor- oder Rufname des aktiven Charakters).
        /// </summary>
        public static EventChore? FindChoreFor(ApplicationUser user, IEnumerable<EventChore> chores)
        {
            var names = new List<string?>
            {
                user.Name,
                user.UserName?.Split('@')[0],
                user.CurrentCharacter?.Vorname,
                user.CurrentCharacter?.Rufname
            };

            var candidates = names
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n!.Trim())
                .ToList();

            return chores.FirstOrDefault(c =>
                !string.IsNullOrWhiteSpace(c.AssignedToName) &&
                candidates.Any(n => string.Equals(n, c.AssignedToName!.Trim(), StringComparison.OrdinalIgnoreCase)));
        }
    }
}
