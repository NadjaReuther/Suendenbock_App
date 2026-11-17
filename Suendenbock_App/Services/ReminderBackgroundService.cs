using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Services
{
    /// <summary>
    /// Läuft einmal am Tag (18:00 Uhr deutscher Zeit) und verschickt Erinnerungen.
    /// - Termine: Erinnerung am Vortag (mit eigenem Dienst) und Erinnerung, wenn noch nicht geantwortet
    /// - Monatsbeitrag: Erinnerung an offene Beiträge
    /// </summary>
    public class ReminderBackgroundService : BackgroundService
    {
        /// <summary>Uhrzeit (deutsche Zeit), zu der die Erinnerungen rausgehen</summary>
        private static readonly TimeSpan RunTime = new(18, 0, 0);

        /// <summary>An diesem Tag im Monat wird an offene Beiträge erinnert</summary>
        public const int PaymentReminderDay = 5;

        /// <summary>So viele Tage vor einem Termin wird erinnert, wer noch nicht geantwortet hat</summary>
        public const int RsvpReminderDaysBefore = 3;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReminderBackgroundService> _logger;
        private readonly TimeZoneInfo _timeZone;

        public ReminderBackgroundService(IServiceScopeFactory scopeFactory, ILogger<ReminderBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _timeZone = GetGermanTimeZone();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = GermanNow();
                var nextRun = now.Date + RunTime;
                if (nextRun <= now)
                {
                    nextRun = nextRun.AddDays(1);
                }

                var delay = nextRun - now;
                _logger.LogInformation($"Nächste Erinnerungsrunde: {nextRun:dd.MM.yyyy HH:mm} (in {delay:hh\\:mm})");

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    return;
                }

                try
                {
                    await RunDailyRemindersAsync(GermanNow());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Fehler in der Erinnerungsrunde: {ex.Message}");
                }
            }
        }

        private async Task RunDailyRemindersAsync(DateTime today)
        {
            await SendEventDayBeforeRemindersAsync(today);
            await SendRsvpRemindersAsync(today);

            if (today.Day == PaymentReminderDay)
            {
                await SendPaymentRemindersAsync(today);
            }
        }

        private async Task SendPaymentRemindersAsync(DateTime today)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var pushService = scope.ServiceProvider.GetRequiredService<IPushNotificationService>();

            var (year, month) = PaymentPeriod.GetTargetMonth(today);
            var monthName = PaymentPeriod.GetMonthName(month);

            var players = await PaymentPeriod.GetPayingPlayersAsync(userManager);

            // Wer hat für diesen Monat schon bezahlt?
            var paidUserNames = await context.MonthlyPayments
                .Where(mp => mp.Year == year && mp.Month == month && mp.Status == "paid")
                .Select(mp => mp.PlayerName)
                .ToListAsync();

            // Fehlender Eintrag zählt als offen
            var openPlayers = players
                .Where(u => !paidUserNames.Contains(u.UserName ?? ""))
                .ToList();

            _logger.LogInformation($"Zahlungserinnerung {monthName} {year}: {openPlayers.Count} offen");

            foreach (var player in openPlayers)
            {
                await pushService.SendToUserAsync(
                    player.Id,
                    $"💰 Monatsbeitrag {monthName} offen",
                    "Kurze Erinnerung: Dein Beitrag ist noch nicht eingegangen.",
                    "/Community"
                );
            }
        }

        /// <summary>
        /// Am Vortag: alle, die zugesagt oder "vielleicht" gesagt haben, mit ihrem Dienst
        /// </summary>
        private async Task SendEventDayBeforeRemindersAsync(DateTime today)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var pushService = scope.ServiceProvider.GetRequiredService<IPushNotificationService>();

            var tomorrow = today.Date.AddDays(1);
            var events = await context.MonthlyEvents
                .Include(e => e.Chores)
                .Include(e => e.RSVPs)
                .Where(e => e.Date.Date == tomorrow)
                .ToListAsync();

            foreach (var evt in events)
            {
                var userIds = evt.RSVPs
                    .Where(r => r.UserId != null && (r.Status == "yes" || r.Status == "maybe"))
                    .Select(r => r.UserId!)
                    .Distinct()
                    .ToList();

                var users = await context.Users
                    .Include(u => u.CurrentCharacter)
                    .Where(u => userIds.Contains(u.Id))
                    .ToListAsync();

                _logger.LogInformation($"Vortag-Erinnerung '{evt.Title}': {users.Count} Empfänger");

                var time = EventNotificationTexts.FormatTime(evt);
                foreach (var user in users)
                {
                    var parts = new List<string>();
                    if (!string.IsNullOrEmpty(time))
                        parts.Add($"{time} Uhr");

                    var chore = EventNotificationTexts.FindChoreFor(user, evt.Chores);
                    if (chore != null)
                        parts.Add($"Dein Dienst: {chore.ChoreName}");

                    await pushService.SendToUserAsync(
                        user.Id,
                        $"⏰ Morgen: {evt.Title}",
                        parts.Any() ? string.Join(" · ", parts) : "Bis morgen!",
                        EventNotificationTexts.EventsUrl
                    );
                }
            }
        }

        /// <summary>
        /// Einige Tage vorher: Spieler, die noch gar nicht geantwortet haben
        /// </summary>
        private async Task SendRsvpRemindersAsync(DateTime today)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var pushService = scope.ServiceProvider.GetRequiredService<IPushNotificationService>();

            var targetDay = today.Date.AddDays(RsvpReminderDaysBefore);
            var events = await context.MonthlyEvents
                .Include(e => e.RSVPs)
                .Where(e => e.Date.Date == targetDay)
                .ToListAsync();

            if (!events.Any())
                return;

            // Gleiche Spielerliste wie beim Monatsbeitrag (ohne Gott, Mamoschka, Biene)
            var players = await PaymentPeriod.GetPayingPlayersAsync(userManager);

            foreach (var evt in events)
            {
                var answered = evt.RSVPs
                    .Where(r => r.UserId != null)
                    .Select(r => r.UserId!)
                    .ToHashSet();

                var missing = players.Where(p => !answered.Contains(p.Id)).ToList();

                _logger.LogInformation($"Antwort-Erinnerung '{evt.Title}': {missing.Count} ohne Antwort");

                foreach (var player in missing)
                {
                    await pushService.SendToUserAsync(
                        player.Id,
                        "❓ Bist du dabei?",
                        $"{evt.Title} am {EventNotificationTexts.FormatDate(evt.Date)} – du hast noch nicht geantwortet.",
                        EventNotificationTexts.EventsUrl
                    );
                }
            }
        }

        private DateTime GermanNow() => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZone);

        private TimeZoneInfo GetGermanTimeZone()
        {
            // Linux/Docker: "Europe/Berlin", Windows: "W. Europe Standard Time"
            foreach (var id in new[] { "Europe/Berlin", "W. Europe Standard Time" })
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(id);
                }
                catch (TimeZoneNotFoundException) { }
                catch (InvalidTimeZoneException) { }
            }

            _logger.LogWarning("Zeitzone Europe/Berlin nicht gefunden, nutze Serverzeit");
            return TimeZoneInfo.Local;
        }
    }
}
