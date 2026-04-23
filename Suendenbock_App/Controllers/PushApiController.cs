using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Services;
using System.Security.Claims;

namespace Suendenbock_App.Controllers
{
    [ApiController]
    [Route("api/push")]
    [Authorize]
    public class PushApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPushNotificationService _pushService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PushApiController> _logger;

        public PushApiController(
            ApplicationDbContext context,
            IPushNotificationService pushService,
            IConfiguration configuration,
            ILogger<PushApiController> logger)
        {
            _context = context;
            _pushService = pushService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Gibt den Public VAPID-Key zurück für die Browser-Subscription
        /// </summary>
        [HttpGet("vapid-public-key")]
        [AllowAnonymous]
        public IActionResult GetVapidPublicKey()
        {
            var publicKey = _configuration["PushNotification:PublicKey"];
            return Ok(new { publicKey });
        }

        /// <summary>
        /// Registriert eine neue Push-Subscription
        /// </summary>
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] PushSubscriptionDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Prüfe ob diese Subscription bereits existiert
                var existing = await _context.PushSubscriptions
                    .FirstOrDefaultAsync(ps => ps.UserId == userId && ps.Endpoint == dto.Endpoint);

                if (existing != null)
                {
                    // Reaktiviere falls deaktiviert
                    existing.IsActive = true;
                    existing.FailureCount = 0;
                    existing.P256dh = dto.Keys.P256dh;
                    existing.Auth = dto.Keys.Auth;
                }
                else
                {
                    // Erstelle neue Subscription
                    var subscription = new PushSubscription
                    {
                        UserId = userId,
                        Endpoint = dto.Endpoint,
                        P256dh = dto.Keys.P256dh,
                        Auth = dto.Keys.Auth,
                        UserAgent = Request.Headers["User-Agent"].ToString(),
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };

                    _context.PushSubscriptions.Add(subscription);
                }

                // Erstelle Standard-Notification-Preferences falls nicht vorhanden
                var prefs = await _context.NotificationPreferences
                    .FirstOrDefaultAsync(np => np.UserId == userId);

                if (prefs == null)
                {
                    prefs = new NotificationPreference
                    {
                        UserId = userId,
                        NotifyForumThreads = true,
                        NotifyForumReplies = true,
                        NotifyNews = true,
                        NotifyPolls = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.NotificationPreferences.Add(prefs);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Push subscription created/updated for user {userId}");

                return Ok(new { success = true, message = "Benachrichtigungen aktiviert!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing to push notifications");
                return StatusCode(500, new { success = false, message = "Fehler beim Aktivieren der Benachrichtigungen" });
            }
        }

        /// <summary>
        /// Deaktiviert eine Push-Subscription
        /// </summary>
        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe([FromBody] UnsubscribeDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var subscription = await _context.PushSubscriptions
                    .FirstOrDefaultAsync(ps => ps.UserId == userId && ps.Endpoint == dto.Endpoint);

                if (subscription != null)
                {
                    subscription.IsActive = false;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Push subscription deactivated for user {userId}");
                }

                return Ok(new { success = true, message = "Benachrichtigungen deaktiviert!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unsubscribing from push notifications");
                return StatusCode(500, new { success = false, message = "Fehler beim Deaktivieren der Benachrichtigungen" });
            }
        }

        /// <summary>
        /// Gibt die aktuellen Benachrichtigungs-Einstellungen zurück
        /// </summary>
        [HttpGet("preferences")]
        public async Task<IActionResult> GetPreferences()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var prefs = await _context.NotificationPreferences
                .FirstOrDefaultAsync(np => np.UserId == userId);

            if (prefs == null)
            {
                // Standard-Einstellungen zurückgeben
                return Ok(new
                {
                    notifyForumThreads = true,
                    notifyForumReplies = true,
                    notifyNews = true,
                    notifyPolls = true
                });
            }

            return Ok(new
            {
                notifyForumThreads = prefs.NotifyForumThreads,
                notifyForumReplies = prefs.NotifyForumReplies,
                notifyNews = prefs.NotifyNews,
                notifyPolls = prefs.NotifyPolls
            });
        }

        /// <summary>
        /// Aktualisiert die Benachrichtigungs-Einstellungen
        /// </summary>
        [HttpPost("preferences")]
        public async Task<IActionResult> UpdatePreferences([FromBody] NotificationPreferencesDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var prefs = await _context.NotificationPreferences
                    .FirstOrDefaultAsync(np => np.UserId == userId);

                if (prefs == null)
                {
                    prefs = new NotificationPreference { UserId = userId, CreatedAt = DateTime.Now };
                    _context.NotificationPreferences.Add(prefs);
                }

                prefs.NotifyForumThreads = dto.NotifyForumThreads;
                prefs.NotifyForumReplies = dto.NotifyForumReplies;
                prefs.NotifyNews = dto.NotifyNews;
                prefs.NotifyPolls = dto.NotifyPolls;
                prefs.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Notification preferences updated for user {userId}");

                return Ok(new { success = true, message = "Einstellungen gespeichert!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification preferences");
                return StatusCode(500, new { success = false, message = "Fehler beim Speichern der Einstellungen" });
            }
        }

        /// <summary>
        /// Sendet eine Test-Benachrichtigung
        /// </summary>
        [HttpPost("test")]
        public async Task<IActionResult> SendTestNotification()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                await _pushService.SendTestNotificationAsync(userId);

                return Ok(new { success = true, message = "Test-Benachrichtigung wurde versendet!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test notification");
                return StatusCode(500, new { success = false, message = "Fehler beim Versenden der Test-Benachrichtigung" });
            }
        }
    }

    // DTOs
    public class PushSubscriptionDto
    {
        public string Endpoint { get; set; } = string.Empty;
        public PushKeysDto Keys { get; set; } = new();
    }

    public class PushKeysDto
    {
        public string P256dh { get; set; } = string.Empty;
        public string Auth { get; set; } = string.Empty;
    }

    public class UnsubscribeDto
    {
        public string Endpoint { get; set; } = string.Empty;
    }

    public class NotificationPreferencesDto
    {
        public bool NotifyForumThreads { get; set; }
        public bool NotifyForumReplies { get; set; }
        public bool NotifyNews { get; set; }
        public bool NotifyPolls { get; set; }
    }
}
