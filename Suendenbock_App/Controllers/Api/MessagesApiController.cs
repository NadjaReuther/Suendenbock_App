using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Services;
using System.Security.Claims;

namespace Suendenbock_App.Controllers.Api
{
    [Authorize(Roles = "Spieler,Moderator,Gott")] // Nur Spieler, Moderatoren und Götter - KEINE Gäste
    [Route("api/messages")]
    [ApiController]
    public class MessagesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPushNotificationService _pushService;

        public MessagesApiController(ApplicationDbContext context, IPushNotificationService pushService)
        {
            _context = context;
            _pushService = pushService;
        }

        // POST: api/messages - Neue Nachricht senden (mit Gruppen-Unterstützung)
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Subject) || string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Betreff und Inhalt sind erforderlich.");
            }

            // Validierung: Entweder Charaktere ODER UserId (für Antwort an Gott/Moderator)
            bool hasCharacterReceivers = request.ReceiverCharacterIds != null && request.ReceiverCharacterIds.Any();
            bool hasUserReceiver = !string.IsNullOrEmpty(request.ReceiverUserId);

            if (!hasCharacterReceivers && !hasUserReceiver)
            {
                return BadRequest("Mindestens ein Empfänger ist erforderlich.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isGott = User.IsInRole("Gott");
            var isModerator = User.IsInRole("Moderator");

            // Bestimme Absender-Typ und Daten
            MessageSenderType senderType;
            int? senderCharacterId = null;
            string? senderDisplayName = null;
            string notificationSenderName;

            if (isGott)
            {
                senderType = MessageSenderType.Gott;
                senderDisplayName = "Gott";
                notificationSenderName = "Gott";
            }
            else if (isModerator)
            {
                senderType = MessageSenderType.Moderator;
                senderDisplayName = "Moderator";
                notificationSenderName = "Moderator";
            }
            else
            {
                // Normaler Spieler
                senderType = MessageSenderType.Character;
                var senderCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == userId);
                if (senderCharacter == null)
                {
                    return BadRequest("Kein Charakter gefunden.");
                }
                senderCharacterId = senderCharacter.Id;
                notificationSenderName = senderCharacter.Vorname;
            }

            var createdMessageIds = new List<int>();

            // Thread-Logik: Bestimme ThreadId und ParentMessageId
            int? threadId = null;
            int? parentMessageId = request.ParentMessageId;

            if (parentMessageId.HasValue)
            {
                // Antwort auf existierende Nachricht - übernehme ThreadId
                var parentMessage = await _context.Messages.FindAsync(parentMessageId.Value);
                if (parentMessage != null)
                {
                    threadId = parentMessage.ThreadId;
                }
            }

            // Fall 1: Antwort an Gott/Moderator (ReceiverUserId)
            if (hasUserReceiver && !hasCharacterReceivers)
            {
                var message = new Message
                {
                    SenderType = senderType,
                    SenderCharacterId = senderCharacterId,
                    SenderUserId = (isGott || isModerator) ? userId : null,
                    SenderDisplayName = senderDisplayName,
                    ReceiverUserId = request.ReceiverUserId,
                    ReceiverType = request.ReceiverType,
                    Subject = request.Subject,
                    Content = request.Content,
                    SentAt = DateTime.Now,
                    ThreadId = 0, // Wird nach SaveChanges gesetzt
                    ParentMessageId = parentMessageId
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                // Setze ThreadId: Bei neuer Konversation = eigene ID, sonst übernommene ThreadId
                message.ThreadId = threadId ?? message.Id;
                await _context.SaveChangesAsync();

                createdMessageIds.Add(message.Id);

                // Push-Benachrichtigung
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _pushService.SendNotificationAsync(
                            "Message",
                            $"📬 Neue Nachricht von {notificationSenderName}",
                            request.Subject,
                            "/Messages/Inbox",
                            request.ReceiverUserId!
                        );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending message push notification: {ex.Message}");
                    }
                });
            }
            // Fall 2: Normale Nachrichten an Charaktere
            else if (hasCharacterReceivers)
            {
                // Validiere alle Empfänger
                var receiverCharacters = await _context.Characters
                    .Where(c => request.ReceiverCharacterIds!.Contains(c.Id))
                    .ToListAsync();

                if (receiverCharacters.Count != request.ReceiverCharacterIds!.Count)
                {
                    return NotFound("Einer oder mehrere Empfänger nicht gefunden.");
                }

                // Erstelle separate Nachricht für jeden Empfänger (wie BCC)
                foreach (var receiverCharacter in receiverCharacters)
                {
                    var message = new Message
                    {
                        SenderType = senderType,
                        SenderCharacterId = senderCharacterId,
                        SenderUserId = (isGott || isModerator) ? userId : null,
                        SenderDisplayName = senderDisplayName,
                        ReceiverCharacterId = receiverCharacter.Id,
                        Subject = request.Subject,
                        Content = request.Content,
                        SentAt = DateTime.Now,
                        ThreadId = 0, // Wird nach SaveChanges gesetzt
                        ParentMessageId = parentMessageId
                    };

                    _context.Messages.Add(message);
                    await _context.SaveChangesAsync();

                    // Setze ThreadId: Bei neuer Konversation = eigene ID, sonst übernommene ThreadId
                    message.ThreadId = threadId ?? message.Id;
                    await _context.SaveChangesAsync();

                    createdMessageIds.Add(message.Id);

                    // Push-Benachrichtigung an jeden Empfänger
                    if (!string.IsNullOrEmpty(receiverCharacter.UserId))
                    {
                        var rcvUserId = receiverCharacter.UserId;
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await _pushService.SendNotificationAsync(
                                    "Message",
                                    $"📬 Neue Nachricht von {notificationSenderName}",
                                    request.Subject,
                                    "/Messages/Inbox",
                                    rcvUserId
                                );
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error sending message push notification: {ex.Message}");
                            }
                        });
                    }
                }
            }

            return Ok(new { ids = createdMessageIds, count = createdMessageIds.Count });
        }

        // PUT: api/messages/{id}/read - Nachricht als gelesen markieren
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isGott = User.IsInRole("Gott");
            var isModerator = User.IsInRole("Moderator");

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound("Nachricht nicht gefunden.");
            }

            bool isReceiver = false;

            // Prüfe ob User der Empfänger ist
            if (isGott || isModerator)
            {
                // Gott/Moderator: Prüfe ReceiverUserId
                isReceiver = message.ReceiverUserId == userId;
            }
            else
            {
                // Spieler: Prüfe ReceiverCharacterId
                var userCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == userId);

                if (userCharacter == null)
                {
                    return BadRequest("Kein Charakter gefunden.");
                }

                isReceiver = message.ReceiverCharacterId == userCharacter.Id;
            }

            if (!isReceiver)
            {
                return Forbid();
            }

            message.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/messages/{id} - Nachricht löschen
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isGott = User.IsInRole("Gott");
            var isModerator = User.IsInRole("Moderator");

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound("Nachricht nicht gefunden.");
            }

            bool isAuthorized = false;

            // Prüfe ob User berechtigt ist
            if (isGott || isModerator)
            {
                // Gott/Moderator: Prüfe SenderUserId oder ReceiverUserId
                if (message.SenderUserId == userId)
                {
                    message.IsDeletedBySender = true;
                    isAuthorized = true;
                }
                else if (message.ReceiverUserId == userId)
                {
                    message.IsDeletedByReceiver = true;
                    isAuthorized = true;
                }
            }
            else
            {
                // Spieler: Prüfe SenderCharacterId oder ReceiverCharacterId
                var userCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == userId);

                if (userCharacter == null)
                {
                    return BadRequest("Kein Charakter gefunden.");
                }

                if (message.SenderCharacterId == userCharacter.Id)
                {
                    message.IsDeletedBySender = true;
                    isAuthorized = true;
                }
                else if (message.ReceiverCharacterId == userCharacter.Id)
                {
                    message.IsDeletedByReceiver = true;
                    isAuthorized = true;
                }
            }

            if (!isAuthorized)
            {
                return Forbid();
            }

            // Wenn beide gelöscht haben, komplett aus DB entfernen
            if (message.IsDeletedBySender && message.IsDeletedByReceiver)
            {
                _context.Messages.Remove(message);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET: api/messages/unread-count - Anzahl ungelesener Nachrichten
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isGott = User.IsInRole("Gott");
            var isModerator = User.IsInRole("Moderator");

            int count;

            if (isGott || isModerator)
            {
                // Gott/Moderator: Suche nach ReceiverUserId
                count = await _context.Messages
                    .Where(m => m.ReceiverUserId == userId
                        && !m.IsRead
                        && !m.IsDeletedByReceiver)
                    .CountAsync();
            }
            else
            {
                // Spieler: Suche nach ReceiverCharacterId
                var userCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == userId);

                if (userCharacter == null)
                {
                    return Ok(new { count = 0 });
                }

                count = await _context.Messages
                    .Where(m => m.ReceiverCharacterId == userCharacter.Id
                        && !m.IsRead
                        && !m.IsDeletedByReceiver)
                    .CountAsync();
            }

            return Ok(new { count });
        }
    }

    // Request Models
    public class SendMessageRequest
    {
        // Für Rückwärtskompatibilität: Einzelner Empfänger
        public int ReceiverCharacterId { get; set; }

        // Für Gruppennachrichten: Mehrere Empfänger (Charaktere)
        public List<int>? ReceiverCharacterIds { get; set; } = new();

        // Für Antwort an Gott/Moderator
        public string? ReceiverUserId { get; set; }
        public MessageSenderType? ReceiverType { get; set; }

        // Für Thread-System
        public int? ParentMessageId { get; set; }

        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
