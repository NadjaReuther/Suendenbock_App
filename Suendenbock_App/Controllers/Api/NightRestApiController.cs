using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Hubs;

namespace Suendenbock_App.Controllers.Api
{
    /// <summary>
    /// API Controller für Nachtlager-Anfragen
    /// </summary>
    [Route("api/night-rest")]
    [ApiController]
    public class NightRestApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<GameHub> _hubContext;

        public NightRestApiController(ApplicationDbContext context, IHubContext<GameHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Prüft ob eine aktive Nachtlager-Anfrage für einen Act existiert.
        /// Wird beim Dashboard-Laden vom Gott aufgerufen.
        /// </summary>
        [HttpGet("pending/{actId}")]
        public async Task<IActionResult> GetPendingRequest(int actId)
        {
            var request = await _context.NightRestRequests
                .FirstOrDefaultAsync(r => r.ActId == actId && r.IsActive);

            if (request == null)
            {
                return NotFound(new { message = "Keine aktive Anfrage" });
            }

            // Hole Charakterdaten
            var characters = await _context.Characters
                .Where(c => c.UserId != null)
                .ToListAsync();

            var characterData = characters.Select(c => new
            {
                id = c.Id,
                name = $"{c.Vorname} {c.Nachname}".Trim(),
                currentPokus = c.CurrentPokus,
                isCompanion = false
            }).ToList();

            return Ok(new
            {
                actId = request.ActId,
                playerName = request.PlayerName,
                characters = characterData,
                timestamp = request.RequestedAt
            });
        }

        /// <summary>
        /// Storniert eine aktive Nachtlager-Anfrage.
        /// Wird vom Spieler via navigator.sendBeacon beim Verlassen der Seite aufgerufen.
        /// Deaktiviert die Anfrage in der DB und sendet NightRestCancelled an alle im Act.
        /// </summary>
        [HttpPost("cancel/{actId}")]
        public async Task<IActionResult> CancelRequest(int actId)
        {
            var pendingRequests = await _context.NightRestRequests
                .Where(r => r.ActId == actId && r.IsActive)
                .ToListAsync();

            if (pendingRequests.Count == 0)
            {
                return Ok(new { message = "Keine aktive Anfrage zu stornieren" });
            }

            foreach (var request in pendingRequests)
            {
                request.IsActive = false;
            }

            await _context.SaveChangesAsync();

            // Sende NightRestCancelled an alle im Act — damit Gott das Modal schließt
            string groupName = $"act-{actId}";
            await _hubContext.Clients.Group(groupName).SendAsync("NightRestCancelled", new
            {
                actId,
                timestamp = DateTime.UtcNow
            });

            Console.WriteLine($"[NightRestApi] Request cancelled for Act {actId} via sendBeacon");
            return Ok(new { message = "Anfrage abgebrochen" });
        }
    }
}
