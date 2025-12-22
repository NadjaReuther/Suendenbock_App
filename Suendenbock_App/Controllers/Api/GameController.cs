// Controllers/Api/GameController.cs
// ALLE Spielmodus-API-Endpoints in einem Controller

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models;

namespace Suendenbock_App.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GameController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===== CHARACTER STATUS =====

        /// <summary>
        /// Aktuellen Status aller Characters abrufen
        /// GET /api/game/characters
        /// </summary>
        [HttpGet("characters")]
        public async Task<IActionResult> GetCharacters()
        {
            try
            {
                var characters = await _context.Characters
                    .Select(c => new
                    {
                        c.Id,
                        c.Nachname,
                        c.CurrentHealth,
                        c.BaseMaxHealth,
                        c.CurrentPokus,
                        c.BaseMaxPokus,
                        HealthPercent = c.BaseMaxHealth > 0
                            ? (int)((double)c.CurrentHealth / c.BaseMaxHealth * 100)
                            : 0
                    })
                    .ToListAsync();

                return Ok(characters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ===== KURZE RAST =====

        /// <summary>
        /// Kurze Rast: +5 HP für alle aktiven Characters
        /// POST /api/game/short-rest
        /// </summary>
        [HttpPost("short-rest")]
        public async Task<IActionResult> ShortRest()
        {
            try
            {
                // Alle aktiven Characters der Guild "Wolkenbruch"
                var characters = await _context.Characters
                    .ToListAsync();

                foreach (var character in characters)
                {
                    // +5 HP, aber nicht über Maximum
                    character.CurrentHealth = Math.Min(
                        character.CurrentHealth + 5,
                        character.BaseMaxHealth
                    );
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Kurze Rast abgeschlossen!",
                    characters = characters.Count,
                    healedAmount = 5
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ===== NACHTLAGER =====

        /// <summary>
        /// Nachtlager: Heilt alle Characters basierend auf Essen, setzt Pokus zurück
        /// POST /api/game/night-rest
        /// Body: { "characterIds": [1,2,3], "foodId": 2 }
        /// </summary>
        [HttpPost("night-rest")]
        public async Task<IActionResult> NightRest([FromBody] NightRestRequest request)
        {
            try
            {
                // Essen laden
                var food = await _context.RestFoods.FindAsync(request.FoodId);
                if (food == null)
                {
                    return BadRequest(new { error = "Essen nicht gefunden!" });
                }

                // Characters laden
                var characters = await _context.Characters
                    .Where(c => request.CharacterIds.Contains(c.Id))
                    .ToListAsync();

                foreach (var character in characters)
                {
                    // Heilen basierend auf Essen
                    character.CurrentHealth = Math.Min(
                        character.CurrentHealth + food.HealthBonus,
                        character.BaseMaxHealth
                    );

                    // Pokus zurücksetzen
                    character.CurrentPokus = 0;

                    // Zeitstempel
                    character.LastRestAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Nachtlager abgeschlossen! Alle Characters +{food.HealthBonus} HP geheilt.",
                    food = food.Name,
                    characters = characters.Count,
                    healedAmount = food.HealthBonus
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ===== SCHADEN & HEILUNG =====

        /// <summary>
        /// Schaden auf einen Character anwenden
        /// POST /api/game/characters/1/damage
        /// Body: 15 (raw integer)
        /// </summary>
        [HttpPost("characters/{id}/damage")]
        public async Task<IActionResult> ApplyDamage(int id, [FromBody] int damage)
        {
            try
            {
                var character = await _context.Characters.FindAsync(id);
                if (character == null)
                {
                    return NotFound(new { error = "Character nicht gefunden!" });
                }

                character.CurrentHealth = Math.Max(0, character.CurrentHealth - damage);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    characterId = id,
                    characterName = character.Nachname,
                    currentHealth = character.CurrentHealth,
                    healthPercent = character.BaseMaxHealth > 0
                        ? (int)((double)character.CurrentHealth / character.BaseMaxHealth * 100)
                        : 0,
                    damage = damage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Character heilen
        /// POST /api/game/characters/1/heal
        /// Body: 20 (raw integer)
        /// </summary>
        [HttpPost("characters/{id}/heal")]
        public async Task<IActionResult> Heal(int id, [FromBody] int amount)
        {
            try
            {
                var character = await _context.Characters.FindAsync(id);
                if (character == null)
                {
                    return NotFound(new { error = "Character nicht gefunden!" });
                }

                character.CurrentHealth = Math.Min(
                    character.CurrentHealth + amount,
                    character.BaseMaxHealth
                );
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    characterId = id,
                    characterName = character.Nachname,
                    currentHealth = character.CurrentHealth,
                    healthPercent = character.BaseMaxHealth > 0
                        ? (int)((double)character.CurrentHealth / character.BaseMaxHealth * 100)
                        : 0,
                    healed = amount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ===== POKUS =====

        /// <summary>
        /// Pokus erhöhen (wird normalerweise nur im Kampf genutzt, dann in LocalStorage)
        /// Dieser Endpoint ist für Backend-Updates wenn nötig
        /// POST /api/game/characters/1/pokus
        /// </summary>
        [HttpPost("characters/{id}/pokus")]
        public async Task<IActionResult> IncrementPokus(int id)
        {
            try
            {
                var character = await _context.Characters.FindAsync(id);
                if (character == null)
                {
                    return NotFound(new { error = "Character nicht gefunden!" });
                }

                character.CurrentPokus++;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    characterId = id,
                    characterName = character.Nachname,
                    currentPokus = character.CurrentPokus
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ===== QUESTS (später mehr) =====

        /// <summary>
        /// Alle aktiven Quests abrufen
        /// GET /api/game/quests
        /// </summary>
        [HttpGet("quests")]
        public async Task<IActionResult> GetQuests()
        {
            try
            {
                var quests = await _context.Quests
                    .Include(q => q.Character)
                    .Include(q => q.MapMarker)
                    .Where(q => q.Status == "active")
                    .Select(q => new
                    {
                        q.Id,
                        q.Title,
                        q.Description,
                        q.Type,
                        q.Status,
                        Character = q.Character != null ? q.Character.Nachname : null,
                        q.CreatedAt
                    })
                    .ToListAsync();

                return Ok(quests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ===== MONSTER/TROPHÄEN (später mehr) =====

        /// <summary>
        /// Alle Monster/Trophäen der Guild abrufen
        /// GET /api/game/monsters
        /// </summary>
        [HttpGet("monsters")]
        public async Task<IActionResult> GetMonsters()
        {
            try
            {
                var monsters = await _context.Monsters
                    .OrderBy(m => m.Name)
                    .Select(m => new
                    {
                        m.Id,
                        m.Name,
                        m.Monstertyp,
                        m.Description,
                        m.BaseEffect,
                        m.SlainEffect,
                        m.Status,
                        m.IsEquipped,
                        m.ImagePath
                    })
                    .ToListAsync();

                return Ok(monsters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    // ===== REQUEST MODELS =====

    public class NightRestRequest
    {
        public List<int> CharacterIds { get; set; } = new();
        public int FoodId { get; set; }
    }
}