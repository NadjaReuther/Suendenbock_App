using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Services;
using System.Security.Claims;

namespace Suendenbock_App.Controllers.Api
{
    [ApiController]
    [Route("api/events")]
    [Authorize]
    public class EventApiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPushNotificationService _pushService;
        public EventApiController(ApplicationDbContext context, IPushNotificationService pushService)
        {
            _context = context;
            _pushService = pushService;
        }
        // POST /api/events/rsvp
        [HttpPost("rsvp")]
        public async Task<IActionResult> RsvpEvent([FromBody] RsvpRequest request)
        {
            var userId = GetUserId();

            // Validierung
            if (!new[] {"yes", "maybe", "no"}.Contains(request.Status)) 
            {
                return BadRequest(new { error = "Ungültiger Status" });
            }

            var evt = await _context.MonthlyEvents.FindAsync(request.EventId);
            if(evt == null)
            {
                return NotFound(new { error = "Event nicht gefunden" });
            }

            // Prüfe ob bereits RSVP existiert
            var existingRsvp = await _context.EventRSVPs
                .FirstOrDefaultAsync(r => r.EventId == request.EventId && r.UserId == userId);

            if (existingRsvp != null) 
            {
                // Update existing RSVP
                existingRsvp.Status = request.Status;
                existingRsvp.UpdatedAt = DateTime.Now;
            }
            else
            {
                // Create new RSVP
                var rsvp = new EventRSVP
                {
                    EventId = request.EventId,
                    UserId = userId,
                    Status = request.Status,
                    CreatedAt = DateTime.Now
                };
                _context.EventRSVPs.Add(rsvp);
            }
            await _context.SaveChangesAsync();

            return Ok(new {success = true, status = request.Status});
        }

        // POST /api/events
        [HttpPost]
        [Authorize(Roles ="Gott")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
        {
            var userId = GetUserId();

            // Validierung
            if(string.IsNullOrWhiteSpace(request.Title) ||
                request.Date == default ||
                string.IsNullOrWhiteSpace(request.Type))
            {
                return BadRequest(new { error = "Pflichtfelder fehlen" });
            }

            var evt = new MonthlyEvent
            {
                Title = request.Title,
                Description = request.Description ?? string.Empty,
                Date = request.Date,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Type = request.Type,
                CreatedByUserId = userId,
                CreatedAt = DateTime.Now
            };

            _context.MonthlyEvents.Add(evt);
            await _context.SaveChangesAsync();

            // Chores hinzufügen (nur bei Spieltag)
            if(request.Type == "Spieltag" && request.Chores != null)
            {
                foreach(var chore in request.Chores)
                {
                    if(!string.IsNullOrWhiteSpace(chore.Key))
                    {
                        _context.EventChores.Add(new EventChore
                        {
                            EventId = evt.Id,
                            ChoreName = chore.Key,
                            AssignedToName = chore.Value ?? "Offen",
                            IsSpecial = ChoreNames.Special.Contains(chore.Key)
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }

            // Push an alle (außer dem, der den Termin angelegt hat)
            var title = $"📅 Neuer Termin: {evt.Title}";
            var body = $"{evt.Type} am {EventNotificationTexts.FormatWhen(evt)} – bist du dabei?";
            _ = Task.Run(async () =>
            {
                try
                {
                    await _pushService.SendNotificationAsync("Event", title, body, EventNotificationTexts.EventsUrl, userId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending event push notification: {ex.Message}");
                }
            });

            return Ok(new {success = true, EventId = evt.Id});
        }

        // PUT /api/events/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] CreateEventRequest request)
        {
            var evt = await _context.MonthlyEvents
                .Include(e => e.Chores)
                .FirstOrDefaultAsync(e => e.Id == id);

            if(evt == null)
            {
                return NotFound(new { error = "Event nicht gefunden" });
            }

            // Alte Werte merken, um zu erkennen ob sich Datum oder Uhrzeit ändern
            var oldDate = evt.Date.Date;
            var oldStart = evt.StartTime;
            var oldEnd = evt.EndTime;

            //Update Event
            evt.Title = request.Title;
            evt.Description = request.Description ?? string.Empty;
            evt.Date = request.Date;
            evt.StartTime = request.StartTime;
            evt.EndTime = request.EndTime;
            evt.Type = request.Type;

            // Chores aktualisieren
            _context.EventChores.RemoveRange(evt.Chores);

            if(request.Type == "Spieltag" && request.Chores != null)
            {
                foreach(var chore in request.Chores)
                {
                    if(!string.IsNullOrWhiteSpace(chore.Key))
                    {
                        _context.EventChores.Add(new EventChore
                        {
                            EventId = evt.Id,
                            ChoreName = chore.Key,
                            AssignedToName = chore.Value ?? "Offen",
                            IsSpecial = ChoreNames.Special.Contains(chore.Key)
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Nur bei geändertem Datum oder geänderter Uhrzeit benachrichtigen
            // (nicht bei Tippfehlern im Titel oder geänderten Diensten)
            var whenChanged = oldDate != evt.Date.Date || oldStart != evt.StartTime || oldEnd != evt.EndTime;
            if (whenChanged)
            {
                var recipients = await GetAttendingUserIdsAsync(evt.Id);
                await NotifyUsersAsync(recipients,
                    $"✏️ Termin geändert: {evt.Title}",
                    $"Jetzt: {EventNotificationTexts.FormatWhen(evt)}");
            }

            return Ok(new { success = true });
        }

        // DELETE /api/events/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var evt = await _context.MonthlyEvents
                .Include(e => e.RSVPs)
                .Include(e => e.Chores)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evt == null)
            {
                return NotFound(new { error = "Event nicht gefunden" });
            }

            // Vor dem Löschen merken, wer zugesagt hatte (nur kommende Termine)
            var recipients = evt.Date.Date >= DateTime.Today
                ? await GetAttendingUserIdsAsync(evt.Id)
                : new List<string>();
            var cancelTitle = $"❌ Termin abgesagt: {evt.Title}";
            var cancelBody = $"{evt.Type} am {EventNotificationTexts.FormatWhen(evt)} findet nicht statt.";

            _context.EventRSVPs.RemoveRange(evt.RSVPs);
            _context.EventChores.RemoveRange(evt.Chores);
            _context.MonthlyEvents.Remove(evt);

            await _context.SaveChangesAsync();

            await NotifyUsersAsync(recipients, cancelTitle, cancelBody);

            return Ok(new { success = true });
        }

        /// <summary>
        /// Benutzer, die zugesagt oder "vielleicht" gesagt haben
        /// </summary>
        private async Task<List<string>> GetAttendingUserIdsAsync(int eventId)
        {
            return await _context.EventRSVPs
                .Where(r => r.EventId == eventId && r.UserId != null && (r.Status == "yes" || r.Status == "maybe"))
                .Select(r => r.UserId!)
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        /// Schickt im Hintergrund an jeden Benutzer einzeln
        /// </summary>
        private Task NotifyUsersAsync(List<string> userIds, string title, string body)
        {
            if (!userIds.Any())
                return Task.CompletedTask;

            _ = Task.Run(async () =>
            {
                foreach (var id in userIds)
                {
                    try
                    {
                        await _pushService.SendToUserAsync(id, title, body, EventNotificationTexts.EventsUrl);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending event push notification: {ex.Message}");
                    }
                }
            });

            return Task.CompletedTask;
        }

        // Helper
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }

    // Request Models
    public class RsvpRequest
    {
        public int EventId { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CreateEventRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Dictionary<string, string>? Chores { get; set; }
    }
}
