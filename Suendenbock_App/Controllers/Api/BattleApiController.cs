using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Suendenbock_App.Hubs;

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
        private readonly IHubContext<GameHub> _gameHub;

        public BattleApiController(ApplicationDbContext context, IHubContext<GameHub> gameHub)
        {
            _context = context;
            _gameHub = gameHub;
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

                // Act laden um ActNumber zu bekommen (für SignalR Group Name)
                var act = await _context.Acts.FindAsync(newSession.ActId);
                if (act == null)
                {
                    return BadRequest(new { error = "Act nicht gefunden!" });
                }

                // WICHTIG: Broadcast an alle Spieler im Act, dass ein Kampf begonnen hat
                // Group Name verwendet ActNumber (logische Nummer) statt ActId (Datenbank-ID)
                string groupName = $"act-{act.ActNumber}";
                await _gameHub.Clients.Group(groupName).SendAsync("CombatStarted", new
                {
                    actId = newSession.ActId,
                    combatSessionId = newSession.Id,
                    message = "Ein Kampf hat begonnen!",
                    timestamp = DateTime.UtcNow
                });

                Console.WriteLine($"[BattleApi] Combat started broadcast sent to Act Number {act.ActNumber} (DB-ID: {newSession.ActId}), SessionId: {newSession.Id}");

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
        /// Ruft die aktive Combat Session für einen Akt ab (verwendet Datenbank-ID).
        /// Wird von Spielern aufgerufen, um zu prüfen, ob ein Kampf läuft.
        /// </summary>
        /// <param name="actId">Datenbank-ID des Akts</param>
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
        /// Ruft die aktive Combat Session für einen Akt ab (verwendet ActNumber).
        /// Wird von Spielern aufgerufen, um zu prüfen, ob ein Kampf läuft.
        /// </summary>
        /// <param name="actNumber">Logische Nummer des Akts (1, 2, 3...)</param>
        /// <returns>Die aktive Combat Session oder null</returns>
        [HttpGet("active/act/{actNumber}")]
        public async Task<IActionResult> GetActiveCombatSessionByActNumber(int actNumber)
        {
            // Finde Act anhand ActNumber
            var act = await _context.Acts.FirstOrDefaultAsync(a => a.ActNumber == actNumber);
            if (act == null)
            {
                return NotFound(new { message = "Act nicht gefunden" });
            }

            // Finde aktive Combat Session für diesen Act
            var session = await _context.CombatSessions
                .FirstOrDefaultAsync(cs => cs.ActId == act.Id && cs.IsActive);

            if (session == null)
            {
                return NotFound(new { message = "Keine aktive Combat Session gefunden" });
            }

            return Ok(new
            {
                sessionId = session.Id,
                actId = session.ActId,
                actNumber = act.ActNumber,
                currentRound = session.CurrentRound,
                currentTurnIndex = session.CurrentTurnIndex,
                battleStateJson = session.BattleStateJson,
                startedAt = session.StartedAt
            });
        }

        /// <summary>
        /// Ruft eine Combat Session anhand ihrer SessionId ab.
        /// Wird von Spielern aufgerufen, die via URL beitreten (?sessionId=123)
        /// </summary>
        /// <param name="sessionId">ID der Combat Session</param>
        /// <returns>Die Combat Session</returns>
        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetCombatSessionById(int sessionId)
        {
            var session = await _context.CombatSessions
                .FirstOrDefaultAsync(cs => cs.Id == sessionId);

            if (session == null)
            {
                return NotFound(new { message = "Combat Session nicht gefunden" });
            }

            return Ok(new
            {
                sessionId = session.Id,
                actId = session.ActId,
                currentRound = session.CurrentRound,
                currentTurnIndex = session.CurrentTurnIndex,
                battleStateJson = session.BattleStateJson,
                startedAt = session.StartedAt,
                isActive = session.IsActive
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

        /// <summary>
        /// Speichert Charakterdaten nach dem Kampf (HP, Pokus, Wunden).
        /// Wird aufgerufen wenn der Kampf endet, um den Spielstand zu persistieren.
        /// Nur Spieler-Charaktere werden gespeichert. Begleiter werden zurückgesetzt.
        /// </summary>
        [HttpPost("save-character-data")]
        public async Task<IActionResult> SaveCharacterData([FromBody] SaveCharacterDataRequest request)
        {
            try
            {
                foreach (var charData in request.Characters)
                {
                    // Nur Spieler-Charaktere speichern (type === 'player')
                    if (charData.Type != "player") continue;

                    var character = await _context.Characters.FindAsync(charData.CharacterId);
                    if (character == null) continue;

                    // HP und Pokus aktualisieren
                    character.CurrentHealth = charData.CurrentHealth;
                    character.CastedSpellsCount = charData.CurrentPokus;
                }

                // Begleiter zurücksetzen (volle HP, 0 Pokus)
                var companions = await _context.Characters
                    .Where(c => c.IsCompanion)
                    .ToListAsync();

                foreach (var companion in companions)
                {
                    companion.CurrentHealth = companion.BaseMaxHealth;
                    companion.CastedSpellsCount = 0;
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Charakterdaten gespeichert, Begleiter zurückgesetzt" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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

    public class SaveCharacterDataRequest
    {
        public List<CharacterDataItem> Characters { get; set; } = new();
    }

    public class CharacterDataItem
    {
        public int CharacterId { get; set; }
        public string Type { get; set; } = ""; // "player", "companion", "enemy"
        public int CurrentHealth { get; set; }
        public int CurrentPokus { get; set; }
    }
}
