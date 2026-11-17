using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Services
{
    /// <summary>
    /// Gemeinsame Regeln für die Monatsbeiträge, damit Seite, API und Erinnerungen
    /// immer denselben Monat und dieselben Spieler meinen.
    /// </summary>
    public static class PaymentPeriod
    {
        /// <summary>
        /// Stichtag: Bis zum 19. gilt der aktuelle Monat, ab dem 20. der nächste.
        /// </summary>
        public const int CutoffDay = 20;

        private static readonly string[] MonthNames =
        {
            "Januar", "Februar", "März", "April", "Mai", "Juni",
            "Juli", "August", "September", "Oktober", "November", "Dezember"
        };

        public static (int Year, int Month) GetTargetMonth(DateTime now)
        {
            var target = now.Day < CutoffDay ? now : now.AddMonths(1);
            return (target.Year, target.Month);
        }

        public static string GetMonthName(int month) => MonthNames[month - 1];

        /// <summary>
        /// Alle Benutzer, die einen Monatsbeitrag zahlen:
        /// keine Götter, nicht Mamoschka, nicht Biene.
        /// </summary>
        public static async Task<List<ApplicationUser>> GetPayingPlayersAsync(UserManager<ApplicationUser> userManager)
        {
            var allUsers = await userManager.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            var players = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                var userName = user.UserName ?? "";

                if (userName.Contains("Mamoschka", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (userName.Contains("Biene", StringComparison.OrdinalIgnoreCase))
                    continue;

                var roles = await userManager.GetRolesAsync(user);
                if (roles.Contains("Gott"))
                    continue;

                players.Add(user);
            }

            return players;
        }
    }
}
