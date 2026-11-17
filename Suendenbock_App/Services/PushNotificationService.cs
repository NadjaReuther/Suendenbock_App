using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using WebPush;
using Newtonsoft.Json;

namespace Suendenbock_App.Services
{
    /// <summary>
    /// Service für das Versenden von Web Push-Benachrichtigungen
    /// </summary>
    public class PushNotificationService : IPushNotificationService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PushNotificationService> _logger;
        private readonly WebPushClient _webPushClient;

        public PushNotificationService(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<PushNotificationService> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
            _webPushClient = new WebPushClient();
        }

        public async Task SendNotificationAsync(
            string notificationType,
            string title,
            string message,
            string url,
            string? excludeUserId = null)
        {

            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                _logger.LogInformation($"SendNotificationAsync called: Type={notificationType}, ExcludeUser={excludeUserId ?? "none"}");

                // 1. Hole alle Benutzer die für diese Art von Benachrichtigung abonniert sind
                var userIds = await GetSubscribedUserIdsAsync(context, notificationType, excludeUserId);

                _logger.LogInformation($"Found {userIds.Count} subscribed users for {notificationType}");

                if (!userIds.Any())
                {
                    _logger.LogInformation($"No users subscribed for {notificationType}");
                    return;
                }

                // 2. Hole alle aktiven Push-Subscriptions für diese Benutzer
                var subscriptions = await context.PushSubscriptions
                    .Where(ps => userIds.Contains(ps.UserId) && ps.IsActive)
                    .ToListAsync();

                if (!subscriptions.Any())
                {
                    _logger.LogInformation($"No active push subscriptions found for {notificationType}");
                    return;
                }

                await SendToSubscriptionsAsync(context, subscriptions, title, message, url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending push notification: {ex.Message}");
            }
        }

        public async Task SendToUserAsync(string userId, string title, string message, string url, string? notificationType = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                // Schalter in den Einstellungen beachten (Termine / Monatsbeitrag)
                if (notificationType != null)
                {
                    var prefs = await context.NotificationPreferences
                        .FirstOrDefaultAsync(np => np.UserId == userId);
                    if (prefs != null &&
                        ((notificationType == "Event" && !prefs.NotifyEvents) ||
                         (notificationType == "Payment" && !prefs.NotifyPayments)))
                    {
                        _logger.LogInformation($"User {userId} hat {notificationType}-Benachrichtigungen ausgeschaltet");
                        return;
                    }
                }

                // Nur die Geräte dieses einen Benutzers
                var subscriptions = await context.PushSubscriptions
                    .Where(ps => ps.UserId == userId && ps.IsActive)
                    .ToListAsync();

                if (!subscriptions.Any())
                {
                    _logger.LogInformation($"No active push subscriptions found for user {userId}");
                    return;
                }

                await SendToSubscriptionsAsync(context, subscriptions, title, message, url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending push notification to user {userId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Schickt die Benachrichtigung an die übergebenen Geräte und pflegt deren Fehlerzähler
        /// </summary>
        private async Task SendToSubscriptionsAsync(
            ApplicationDbContext context,
            List<Suendenbock_App.Models.Domain.PushSubscription> subscriptions,
            string title,
            string message,
            string url)
        {
            // 3. Erstelle die Payload
            var payload = JsonConvert.SerializeObject(new
            {
                title,
                body = message,
                url,
                icon = "/icons/icon-192.png",  // App-Icon (Wachssiegel)
                badge = "/icons/badge-96.png" // kleines weißes Siegel für die Statusleiste (Android)
            });

            // 4. VAPID-Details laden
            var vapidDetails = new VapidDetails(
                _configuration["PushNotification:Subject"] ?? "mailto:noreply@example.com",
                _configuration["PushNotification:PublicKey"] ?? "",
                _configuration["PushNotification:PrivateKey"] ?? ""
            );

            // 5. Sende an alle Subscriptions
            int successCount = 0;
            int failureCount = 0;

            foreach (var subscription in subscriptions)
            {
                try
                {
                    var pushSubscription = new WebPush.PushSubscription(
                        subscription.Endpoint,
                        subscription.P256dh,
                        subscription.Auth
                    );

                    await _webPushClient.SendNotificationAsync(
                        pushSubscription,
                        payload,
                        vapidDetails
                    );

                    // Update LastSuccessfulPush
                    subscription.LastSuccessfulPush = DateTime.Now;
                    subscription.FailureCount = 0;
                    successCount++;
                }
                catch (WebPushException ex)
                {
                    _logger.LogWarning($"Push notification failed for subscription {subscription.Id}: {ex.Message}");

                    subscription.FailureCount++;

                    // Deaktiviere Subscription nach 3 Fehlversuchen (wahrscheinlich ungültig)
                    if (subscription.FailureCount >= 3 ||
                        ex.StatusCode == System.Net.HttpStatusCode.Gone ||
                        ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        subscription.IsActive = false;
                        _logger.LogInformation($"Deactivated subscription {subscription.Id} after failures");
                    }

                    failureCount++;
                }
            }

            await context.SaveChangesAsync();

            _logger.LogInformation(
                $"Push notification '{title}' sent: {successCount} success, {failureCount} failures");
        }

        public async Task SendTestNotificationAsync(string userId)
        {
            // Nur an den Benutzer, der den Test ausgelöst hat
            await SendToUserAsync(
                userId,
                "🔔 Test-Benachrichtigung",
                "Deine Push-Benachrichtigungen funktionieren!",
                "/"
            );
        }

        /// <summary>
        /// Ermittelt alle UserIds die für einen Benachrichtigungstyp abonniert sind
        /// </summary>
        private async Task<List<string>> GetSubscribedUserIdsAsync(ApplicationDbContext context, string notificationType, string? excludeUserId)
        {
            var query = context.NotificationPreferences.AsQueryable();

            // Filter nach Benachrichtigungstyp
            query = notificationType switch
            {
                "ForumThread" => query.Where(np => np.NotifyForumThreads),
                "ForumReply" => query.Where(np => np.NotifyForumReplies),
                "News" => query.Where(np => np.NotifyNews),
                "Poll" => query.Where(np => np.NotifyPolls),
                "Event" => query.Where(np => np.NotifyEvents),
                "Payment" => query.Where(np => np.NotifyPayments),
                _ => query
            };

            // Schließe bestimmten Benutzer aus (z.B. Autor)
            if (!string.IsNullOrEmpty(excludeUserId))
            {
                query = query.Where(np => np.UserId != excludeUserId);
            }

            return await query.Select(np => np.UserId).ToListAsync();
        }
    }
}
