using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;

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

        public NightRestApiController(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
