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

                // 3. Erstelle die Payload
                var payload = JsonConvert.SerializeObject(new
                {
                    title,
                    body = message,
                    url,
                    icon = "/images/logo.png",  // Anpassen an dein Logo
                    badge = "/images/badge.png" // Anpassen an dein Badge-Icon
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending push notification: {ex.Message}");
            }
        }

        public async Task SendTestNotificationAsync(string userId)
        {
            await SendNotificationAsync(
                "Test",
                "🔔 Test-Benachrichtigung",
                "Deine Push-Benachrichtigungen funktionieren!",
                "/",
                null
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
