using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models;
using System.Text.Json;

namespace Suendenbock_App.Controllers.Api
{
    /// <summary>
    /// API Controller für Battle/Combat Management
    ///
    /// VERWENDUNG:
    /// - Gott erstellt eine CombatSession via POST /api/battle/create
    /// - Spieler rufen die aktive Session via GET /api/battle/active/{actId} ab
    /// - Battle State wird via SignalR synchronisiert (BattleHub)
    /// - Kampf-Ende via POST /api/battle/end
    /// </summary>
    [Route("api/battle")]
    [ApiController]
    public class BattleApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BattleApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Erstellt eine neue Combat Session.
        /// Wird von Gott aufgerufen, wenn er einen Kampf in CombatSetup startet.
        /// </summary>
        /// <param name="request">Combat Session Daten</param>
        /// <returns>Die erstellte Combat Session</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateCombatSession([FromBody] CreateCombatSessionRequest request)
        {
            try
            {
                // Prüfen, ob bereits eine aktive Session für diesen Akt existiert
                var existingSession = await _context.CombatSessions
                    .FirstOrDefaultAsync(cs => cs.ActId == request.ActId && cs.IsActive);

                if (existingSession != null)
                {
                    // Alte Session beenden
                    existingSession.IsActive = false;
                    existingSession.EndedAt = DateTime.Now;
                }

                // Neue Session erstellen
                var newSession = new CombatSession
                {
                    ActId = request.ActId,
                    IsActive = true,
                    CurrentRound = 1,
                    CurrentTurnIndex = 0,
                    BattleStateJson = request.BattleStateJson,
                    StartedAt = DateTime.Now
                };

                _context.CombatSessions.Add(newSession);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    sessionId = newSession.Id,
                    actId = newSession.ActId,
                    startedAt = newSession.StartedAt
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Ruft die aktive Combat Session für einen Akt ab.
        /// Wird von Spielern aufgerufen, um zu prüfen, ob ein Kampf läuft.
        /// </summary>
        /// <param name="actId">ID des Akts</param>
        /// <returns>Die aktive Combat Session oder null</returns>
        [HttpGet("active/{actId}")]
        public async Task<IActionResult> GetActiveCombatSession(int actId)
        {
            var session = await _context.CombatSessions
                .FirstOrDefaultAsync(cs => cs.ActId == actId && cs.IsActive);

            if (session == null)
            {
                return NotFound(new { message = "Keine aktive Combat Session gefunden" });
            }

            return Ok(new
            {
                sessionId = session.Id,
                actId = session.ActId,
                currentRound = session.CurrentRound,
                currentTurnIndex = session.CurrentTurnIndex,
                battleStateJson = session.BattleStateJson,
                startedAt = session.StartedAt
            });
        }

        /// <summary>
        /// Beendet eine Combat Session.
        /// Wird aufgerufen, wenn der Kampf vorbei ist.
        /// </summary>
        /// <param name="sessionId">ID der Combat Session</param>
        /// <param name="request">Result (victory/defeat)</param>
        /// <returns>Success/Error</returns>
        [HttpPost("end/{sessionId}")]
        public async Task<IActionResult> EndCombatSession(int sessionId, [FromBody] EndCombatSessionRequest request)
        {
            var session = await _context.CombatSessions
                .FirstOrDefaultAsync(cs => cs.Id == sessionId && cs.IsActive);

            if (session == null)
            {
                return NotFound(new { message = "Combat Session nicht gefunden oder bereits beendet" });
            }

            session.IsActive = false;
            session.EndedAt = DateTime.Now;
            session.Result = request.Result;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                sessionId = session.Id,
                result = session.Result,
                endedAt = session.EndedAt
            });
        }

        /// <summary>
        /// Ruft alle Combat Sessions für einen Akt ab (für History).
        /// </summary>
        [HttpGet("history/{actId}")]
        public async Task<IActionResult> GetCombatHistory(int actId)
        {
            var sessions = await _context.CombatSessions
                .Where(cs => cs.ActId == actId)
                .OrderByDescending(cs => cs.StartedAt)
                .Select(cs => new
                {
                    sessionId = cs.Id,
                    startedAt = cs.StartedAt,
                    endedAt = cs.EndedAt,
                    result = cs.Result,
                    isActive = cs.IsActive
                })
                .ToListAsync();

            return Ok(sessions);
        }
    }

    // ===== REQUEST/RESPONSE MODELS =====

    public class CreateCombatSessionRequest
    {
        public int ActId { get; set; }
        public string BattleStateJson { get; set; } = "{}";
    }

    public class EndCombatSessionRequest
    {
        public string Result { get; set; } = ""; // "victory" or "defeat"
    }
}
