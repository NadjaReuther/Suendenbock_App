// Controllers/Api/GameController.cs
// ALLE Spielmodus-API-Endpoints in einem Controller

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models;
using Suendenbock_App.Services;

namespace Suendenbock_App.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageUploadService _imageUploadService;

        public GameController(ApplicationDbContext context, IImageUploadService imageUploadService)
        {
            _context = context;
            _imageUploadService = imageUploadService;
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

                    // Zauber-Zähler zurücksetzen auf 0
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
        /// Alle Quests abrufen
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
                    .Select(q => new
                    {
                        q.Id,
                        q.Title,
                        q.Description,
                        q.Type,
                        q.Status,
                        q.Character,
                        q.CharacterId,
                        q.CreatedAt,
                        q.CompletedAt
                    })
                    .ToListAsync();

                return Ok(quests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Neue Quest erstellen
        /// POST /api/game/quests
        /// Body: { "title": "...", "description": "...", "type": "individual", "characterId": 1 }
        /// </summary>
        [HttpPost("quests")]
        public async Task<IActionResult> CreateQuest([FromBody] CreateQuestRequest request)
        {
            try
            {
                // Validierung
                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    return BadRequest(new { error = "Titel ist erforderlich!" });
                }

                // Wenn Type = "individual", muss CharacterId vorhanden sein
                if (request.Type == "individual" && request.CharacterId == null)
                {
                    return BadRequest(new { error = "Bei individuellen Quests muss ein Character zugewiesen sein!" });
                }

                // Aktuellen/aktiven Act laden
                var currentAct = await _context.Acts.FirstOrDefaultAsync(a => a.IsActive);
                if (currentAct == null)
                {
                    return BadRequest(new { error = "Kein aktiver Act gefunden! Bitte aktiviere zuerst einen Act." });
                }

                // Validiere PreviousQuestId falls angegeben
                if (request.PreviousQuestId.HasValue)
                {
                    var previousQuest = await _context.Quests.FindAsync(request.PreviousQuestId.Value);
                    if (previousQuest == null)
                    {
                        return BadRequest(new { error = "Vorgänger-Quest nicht gefunden!" });
                    }
                    // Stelle sicher, dass die Vorgänger-Quest zum gleichen Act gehört
                    if (previousQuest.ActId != currentAct.Id)
                    {
                        return BadRequest(new { error = "Vorgänger-Quest muss zum gleichen Act gehören!" });
                    }
                }

                var quest = new Quest
                {
                    Title = request.Title,
                    Description = request.Description ?? string.Empty,
                    Type = request.Type,
                    Status = request.Status ?? "active",
                    CharacterId = request.Type == "individual" ? request.CharacterId : null,
                    ActId = currentAct.Id, // Quest dem aktiven Act zuweisen
                    PreviousQuestId = request.PreviousQuestId,
                    PreviousQuestRequirement = request.PreviousQuestRequirement ?? "both",
                    CreatedAt = DateTime.Now
                };

                _context.Quests.Add(quest);
                await _context.SaveChangesAsync();

                // Optional: Questmarker erstellen
                if (request.CreateMarker && request.MarkerXPercent.HasValue && request.MarkerYPercent.HasValue)
                {
                    // Validiere Koordinaten
                    if (request.MarkerXPercent.Value < 0 || request.MarkerXPercent.Value > 100 ||
                        request.MarkerYPercent.Value < 0 || request.MarkerYPercent.Value > 100)
                    {
                        return BadRequest(new { error = "Marker-Koordinaten müssen zwischen 0 und 100 liegen!" });
                    }

                    // Map des aktuellen Acts laden
                    var map = await _context.Maps.FirstOrDefaultAsync(m => m.ActId == currentAct.Id);
                    if (map != null)
                    {
                        var marker = new MapMarker
                        {
                            MapId = map.Id,
                            Label = quest.Title,
                            Type = "quest",
                            XPercent = request.MarkerXPercent.Value,
                            YPercent = request.MarkerYPercent.Value,
                            Description = quest.Description,
                            CreatedAt = DateTime.Now
                        };

                        _context.MapMarkers.Add(marker);
                        await _context.SaveChangesAsync();

                        // Quest mit Marker verknüpfen
                        quest.MapMarkerId = marker.Id;
                        await _context.SaveChangesAsync();
                    }
                }

                // Quest mit Character-Namen zurückgeben
                var character = quest.CharacterId.HasValue
                    ? await _context.Characters.FindAsync(quest.CharacterId.Value)
                    : null;

                return Ok(new
                {
                    message = "Quest erfolgreich erstellt!",
                    quest = new
                    {
                        quest.Id,
                        quest.Title,
                        quest.Description,
                        quest.Type,
                        quest.Status,
                        CharacterName = character != null ? $"{character.Vorname}" : null,
                        quest.CharacterId,
                        quest.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Quest-Status ändern
        /// PUT /api/game/quests/1/status
        /// Body: "active" / "completed" / "failed" (als raw string)
        /// </summary>
        [HttpPut("quests/{id}/status")]
        public async Task<IActionResult> UpdateQuestStatus(int id, [FromBody] string status)
        {
            try
            {
                var quest = await _context.Quests.FindAsync(id);
                if (quest == null)
                {
                    return NotFound(new { error = "Quest nicht gefunden!" });
                }

                // Validierung
                if (status != "active" && status != "completed" && status != "failed")
                {
                    return BadRequest(new { error = "Ungültiger Status! Erlaubt: active, completed, failed" });
                }

                quest.Status = status;

                // Wenn completed, CompletedAt setzen
                if (status == "completed" && quest.CompletedAt == null)
                {
                    quest.CompletedAt = DateTime.Now;
                }
                // Wenn wieder active, CompletedAt zurücksetzen
                else if (status == "active")
                {
                    quest.CompletedAt = null;
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Quest-Status auf '{status}' gesetzt!",
                    questId = id,
                    status = quest.Status,
                    completedAt = quest.CompletedAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Quest bearbeiten (Titel, Beschreibung, Character)
        /// PUT /api/game/quests/1
        /// Body: { "title": "...", "description": "...", "characterId": 1 }
        /// </summary>
        [HttpPut("quests/{id}")]
        public async Task<IActionResult> UpdateQuest(int id, [FromBody] UpdateQuestRequest request)
        {
            try
            {
                var quest = await _context.Quests
                    .Include(q => q.MapMarker)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (quest == null)
                {
                    return NotFound(new { error = "Quest nicht gefunden!" });
                }

                // Update fields
                if (!string.IsNullOrWhiteSpace(request.Title))
                {
                    quest.Title = request.Title;
                }

                if (request.Description != null)
                {
                    quest.Description = request.Description;
                }

                if (request.CharacterId.HasValue)
                {
                    quest.CharacterId = request.CharacterId.Value;
                }

                // Update PreviousQuestId und Requirement
                if (request.PreviousQuestId.HasValue)
                {
                    // Validiere dass die Quest existiert
                    var previousQuest = await _context.Quests.FindAsync(request.PreviousQuestId.Value);
                    if (previousQuest == null)
                    {
                        return BadRequest(new { error = "Vorgänger-Quest nicht gefunden!" });
                    }
                    // Stelle sicher, dass die Vorgänger-Quest zum gleichen Act gehört
                    if (previousQuest.ActId != quest.ActId)
                    {
                        return BadRequest(new { error = "Vorgänger-Quest muss zum gleichen Act gehören!" });
                    }
                    // Verhindere Zirkelbezüge
                    if (previousQuest.Id == quest.Id)
                    {
                        return BadRequest(new { error = "Eine Quest kann nicht ihre eigene Vorgänger-Quest sein!" });
                    }
                    quest.PreviousQuestId = request.PreviousQuestId.Value;
                    quest.PreviousQuestRequirement = request.PreviousQuestRequirement ?? "both";
                }

                // Marker entfernen falls gewünscht
                if (request.RemoveMarker && quest.MapMarkerId.HasValue)
                {
                    var marker = await _context.MapMarkers.FindAsync(quest.MapMarkerId.Value);
                    if (marker != null)
                    {
                        _context.MapMarkers.Remove(marker);
                    }
                    quest.MapMarkerId = null;
                }

                // Neuen Marker erstellen falls gewünscht
                if (request.CreateMarker && request.MarkerXPercent.HasValue && request.MarkerYPercent.HasValue)
                {
                    // Validiere Koordinaten
                    if (request.MarkerXPercent.Value < 0 || request.MarkerXPercent.Value > 100 ||
                        request.MarkerYPercent.Value < 0 || request.MarkerYPercent.Value > 100)
                    {
                        return BadRequest(new { error = "Marker-Koordinaten müssen zwischen 0 und 100 liegen!" });
                    }

                    // Map des Acts laden
                    var map = await _context.Maps.FirstOrDefaultAsync(m => m.ActId == quest.ActId);
                    if (map != null)
                    {
                        // Falls bereits ein Marker existiert, aktualisiere ihn
                        if (quest.MapMarkerId.HasValue)
                        {
                            var existingMarker = await _context.MapMarkers.FindAsync(quest.MapMarkerId.Value);
                            if (existingMarker != null)
                            {
                                existingMarker.XPercent = request.MarkerXPercent.Value;
                                existingMarker.YPercent = request.MarkerYPercent.Value;
                                existingMarker.Label = quest.Title;
                                existingMarker.Description = quest.Description;
                            }
                        }
                        else
                        {
                            // Erstelle neuen Marker
                            var marker = new MapMarker
                            {
                                MapId = map.Id,
                                Label = quest.Title,
                                Type = "quest",
                                XPercent = request.MarkerXPercent.Value,
                                YPercent = request.MarkerYPercent.Value,
                                Description = quest.Description,
                                CreatedAt = DateTime.Now
                            };

                            _context.MapMarkers.Add(marker);
                            await _context.SaveChangesAsync();

                            quest.MapMarkerId = marker.Id;
                        }
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Quest erfolgreich aktualisiert!",
                    quest = new
                    {
                        quest.Id,
                        quest.Title,
                        quest.Description,
                        quest.Type,
                        quest.Status,
                        quest.CharacterId,
                        quest.MapMarkerId,
                        quest.PreviousQuestId
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Quest löschen
        /// DELETE /api/game/quests/1
        /// </summary>
        [HttpDelete("quests/{id}")]
        public async Task<IActionResult> DeleteQuest(int id)
        {
            try
            {
                var quest = await _context.Quests.FindAsync(id);
                if (quest == null)
                {
                    return NotFound(new { error = "Quest nicht gefunden!" });
                }

                _context.Quests.Remove(quest);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Quest erfolgreich gelöscht!",
                    questId = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ===== TROPHY-ENDPOINTS =====

        /// <summary>
        /// Alle Trophäen abrufen
        /// GET /api/game/trophies
        /// </summary>
        [HttpGet("trophies")]
        public async Task<IActionResult> GetTrophies()
        {
            try
            {
                var trophies = await _context.Monsters
                    .Include(m => m.Monstertyp)
                    .Where(m => m.meet == true) // Nur freigespielte
                    .Select(m => new
                    {
                        m.Id,
                        m.Name,
                        m.ImagePath,
                        MonsterType = m.Monstertyp != null ? m.Monstertyp.Name : "Unbekannt",
                        m.Description,
                        m.BaseEffect,
                        m.SlainEffect,
                        m.Status,
                        m.IsEquipped
                    })
                    .ToListAsync();

                return Ok(trophies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Trophy Status wechseln (bought ↔ slain)
        /// POST /api/game/trophies/1/toggle-status
        /// Nur möglich wenn beide Varianten verfügbar sind
        /// </summary>
        [HttpPost("trophies/{id}/toggle-status")]
        public async Task<IActionResult> ToggleTrophyStatus(int id)
        {
            try
            {
                var trophy = await _context.Monsters.FindAsync(id);
                if (trophy == null)
                {
                    return NotFound(new { error = "Trophäe nicht gefunden!" });
                }

                // Validierung: Toggle nur möglich wenn beide Varianten vorhanden sind
                if (!trophy.HasBothVariants)
                {
                    return BadRequest(new { error = "Toggle nur möglich wenn beide Trophäen-Varianten vorhanden sind!" });
                }

                // Präferenz wechseln: bought ↔ slain
                if (trophy.PreferredVariant == "bought")
                {
                    trophy.PreferredVariant = "slain";
                }
                else
                {
                    trophy.PreferredVariant = "bought";
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Status gewechselt!",
                    trophyId = id,
                    newStatus = trophy.Status
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Trophy ausrüsten/entfernen
        /// POST /api/game/trophies/1/toggle-equip
        /// </summary>
        [HttpPost("trophies/{id}/toggle-equip")]
        public async Task<IActionResult> ToggleTrophyEquip(int id)
        {
            try
            {
                var trophy = await _context.Monsters.FindAsync(id);
                if (trophy == null)
                {
                    return NotFound(new { error = "Trophäe nicht gefunden!" });
                }

                // Wenn bereits ausgerüstet → entfernen
                if (trophy.IsEquipped)
                {
                    trophy.IsEquipped = false;
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        message = "Trophäe von der Wand genommen!",
                        trophyId = id,
                        isEquipped = false
                    });
                }

                // Wenn noch nicht ausgerüstet → prüfe Max-Limit (3)
                var equippedCount = await _context.Monsters
                    .Where(m => m.IsEquipped)
                    .CountAsync();

                if (equippedCount >= 3)
                {
                    return BadRequest(new { error = "Maximal 3 Trophäen können ausgerüstet sein!" });
                }

                // Ausrüsten (Status wird automatisch aus HasBoughtTrophy/HasSlainTrophy berechnet)
                trophy.IsEquipped = true;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Trophäe an die Wand gehängt!",
                    trophyId = id,
                    isEquipped = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Trophäe an bestimmter Position ausrüsten (Drag & Drop)
        /// POST /api/game/trophies/1/equip
        /// Body: { "position": 1 } (Position 1-3)
        /// </summary>
        [HttpPost("trophies/{id}/equip")]
        public async Task<IActionResult> EquipTrophyAtPosition(int id, [FromBody] EquipTrophyRequest request)
        {
            try
            {
                var trophy = await _context.Monsters.FindAsync(id);
                if (trophy == null)
                {
                    return NotFound(new { error = "Trophäe nicht gefunden!" });
                }

                // Validierung: Position muss 1-3 sein
                if (request.Position < 1 || request.Position > 3)
                {
                    return BadRequest(new { error = "Position muss zwischen 1 und 3 liegen!" });
                }

                // Prüfe ob an dieser Position bereits eine Trophäe hängt
                var existingTrophy = await _context.Monsters
                    .FirstOrDefaultAsync(m => m.IsEquipped && m.EquippedPosition == request.Position);

                if (existingTrophy != null)
                {
                    // Entferne die alte Trophäe von dieser Position
                    existingTrophy.IsEquipped = false;
                    existingTrophy.EquippedPosition = null;
                }

                // Ausrüsten der neuen Trophäe an dieser Position
                trophy.IsEquipped = true;
                trophy.EquippedPosition = request.Position;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Trophäe an Position {request.Position} aufgehängt!",
                    trophyId = id,
                    position = request.Position,
                    isEquipped = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Wechselt zwischen "bought" und "slain" Variante (nur wenn beide vorhanden)
        /// PUT /api/game/trophies/1/status
        /// Body: { "status": "bought" } oder { "status": "slain" }
        /// </summary>
        [HttpPut("trophies/{id}/status")]
        public async Task<IActionResult> SetTrophyStatus(int id, [FromBody] SetTrophyStatusRequest request)
        {
            try
            {
                var trophy = await _context.Monsters.FindAsync(id);
                if (trophy == null)
                {
                    return NotFound(new { error = "Trophäe nicht gefunden!" });
                }

                // Nur erlaubt, wenn beide Varianten vorhanden
                if (!trophy.HasBothVariants)
                {
                    return BadRequest(new { error = "Kann nur wechseln, wenn beide Varianten vorhanden sind!" });
                }

                // Validierung
                if (request.Status != "bought" && request.Status != "slain")
                {
                    return BadRequest(new { error = "Ungültiger Status! Erlaubt: bought, slain" });
                }

                trophy.PreferredVariant = request.Status;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Variante auf '{request.Status}' gewechselt!",
                    trophyId = id,
                    status = trophy.Status,
                    preferredVariant = trophy.PreferredVariant
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ===== MAP-ENDPOINTS =====

        /// <summary>
        /// Alle Acts abrufen (nur für Gott)
        /// GET /api/game/acts
        /// </summary>
        [HttpGet("acts")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> GetActs()
        {
            try
            {
                var acts = await _context.Acts
                    .OrderBy(a => a.ActNumber)
                    .Select(a => new
                    {
                        a.Id,
                        a.Name,
                        a.ActNumber,
                        a.Description,
                        a.IsActive,
                        a.Country,
                        a.Companion1,
                        a.Companion2,
                        a.Month,
                        a.Weather,
                        a.CreatedAt,
                        MapImageUrl = _context.Maps
                            .Where(m => m.ActId == a.Id && m.IsWorldMap)
                            .Select(m => m.ImageUrl)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                return Ok(acts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Einzelnen Act abrufen (nur für Gott)
        /// GET /api/game/acts/1
        /// </summary>
        [HttpGet("acts/{id}")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> GetAct(int id)
        {
            try
            {
                var act = await _context.Acts
                    .Where(a => a.Id == id)
                    .Select(a => new
                    {
                        a.Id,
                        a.Name,
                        a.ActNumber,
                        a.Description,
                        a.IsActive,
                        a.Country,
                        a.Companion1,
                        a.Companion2,
                        a.Month,
                        a.Weather,
                        a.CreatedAt,
                        MapImageUrl = _context.Maps
                            .Where(m => m.ActId == a.Id && m.IsWorldMap)
                            .Select(m => m.ImageUrl)
                            .FirstOrDefault()
                    })
                    .FirstOrDefaultAsync();

                if (act == null)
                {
                    return NotFound(new { error = "Act nicht gefunden!" });
                }

                return Ok(act);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Neuen Act erstellen (nur für Gott)
        /// POST /api/game/acts
        /// FormData: actNumber, name, description, country, companion1, companion2, month, weather, mapImage (IFormFile)
        /// </summary>
        [HttpPost("acts")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> CreateAct([FromForm] CreateActFormRequest request)
        {
            try
            {
                // Validierung
                if (request.ActNumber <= 0)
                {
                    return BadRequest(new { error = "ActNumber muss größer als 0 sein!" });
                }

                if (string.IsNullOrWhiteSpace(request.Companion1))
                {
                    return BadRequest(new { error = "Mindestens ein Begleiter muss ausgewählt sein!" });
                }

                // Erstelle Act
                var act = new Act
                {
                    ActNumber = request.ActNumber,
                    Name = request.Name ?? $"Akt {request.ActNumber}",
                    Description = request.Description ?? string.Empty,
                    Country = request.Country,
                    Companion1 = request.Companion1,
                    Companion2 = request.Companion2,
                    Month = request.Month,
                    Weather = request.Weather,
                    IsActive = false,
                    CreatedAt = DateTime.Now
                };

                _context.Acts.Add(act);
                await _context.SaveChangesAsync();

                // Erstelle Map falls Bild hochgeladen wurde
                if (request.MapImage != null && request.MapImage.Length > 0)
                {
                    try
                    {
                        // Validierung der Bilddatei
                        if (!_imageUploadService.ValidateImageFile(request.MapImage))
                        {
                            return BadRequest(new { error = "Ungültige Bilddatei! Erlaubt: JPG, PNG (max. 5MB)" });
                        }

                        // Bild hochladen und Pfad erhalten
                        var fileName = $"act-{act.ActNumber}-map";
                        var imagePath = await _imageUploadService.UploadImageAsync(request.MapImage, "maps", fileName);

                        // Map-Eintrag erstellen
                        var map = new Map
                        {
                            ActId = act.Id,
                            Name = $"Karte {act.Name}",
                            ImageUrl = imagePath,
                            CreatedAt = DateTime.Now
                        };

                        _context.Maps.Add(map);
                        await _context.SaveChangesAsync();

                        return Ok(new
                        {
                            message = "Act und Karte erfolgreich erstellt!",
                            actId = act.Id,
                            mapImageUrl = imagePath,
                            act = new
                            {
                                act.Id,
                                act.ActNumber,
                                act.Name,
                                act.Description,
                                act.Country,
                                act.Companion1,
                                act.Companion2,
                                act.Month,
                                act.Weather,
                                act.IsActive,
                                MapImageUrl = imagePath
                            }
                        });
                    }
                    catch (Exception mapEx)
                    {
                        // Log inner exception details
                        var innerMessage = mapEx.InnerException != null ? mapEx.InnerException.Message : mapEx.Message;
                        return StatusCode(500, new
                        {
                            error = "Fehler beim Speichern der Karte",
                            details = innerMessage,
                            actCreated = true,
                            actId = act.Id
                        });
                    }
                }

                return Ok(new
                {
                    message = "Act erfolgreich erstellt!",
                    actId = act.Id,
                    act = new
                    {
                        act.Id,
                        act.ActNumber,
                        act.Name,
                        act.Description,
                        act.Country,
                        act.Companion1,
                        act.Companion2,
                        act.Month,
                        act.Weather,
                        act.IsActive,
                        MapImageUrl = (string?)null
                    }
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { error = "Fehler beim Erstellen des Akts", details = innerMessage });
            }
        }

        /// <summary>
        /// Act aktualisieren (nur für Gott)
        /// PUT /api/game/acts/1
        /// FormData: name, description, country, companion1, companion2, month, weather, mapImage (IFormFile)
        /// </summary>
        [HttpPut("acts/{id}")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> UpdateAct(int id, [FromForm] UpdateActFormRequest request)
        {
            try
            {
                var act = await _context.Acts
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (act == null)
                {
                    return NotFound(new { error = "Act nicht gefunden!" });
                }

                // Lade Weltkarte falls vorhanden
                act.Map = await _context.Maps
                    .Where(m => m.ActId == act.Id && m.IsWorldMap)
                    .FirstOrDefaultAsync();

                // Update Act
                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    act.Name = request.Name;
                }

                if (request.Description != null)
                {
                    act.Description = request.Description;
                }

                if (request.Country != null)
                {
                    act.Country = request.Country;
                }

                if (request.Companion1 != null)
                {
                    act.Companion1 = request.Companion1;
                }

                if (request.Companion2 != null)
                {
                    act.Companion2 = request.Companion2;
                }

                if (request.Month != null)
                {
                    act.Month = request.Month;
                }

                if (request.Weather != null)
                {
                    act.Weather = request.Weather;
                }

                // Update Map falls neues Bild hochgeladen wurde
                if (request.MapImage != null && request.MapImage.Length > 0)
                {
                    // Validierung der Bilddatei
                    if (!_imageUploadService.ValidateImageFile(request.MapImage))
                    {
                        return BadRequest(new { error = "Ungültige Bilddatei! Erlaubt: JPG, PNG (max. 5MB)" });
                    }

                    // Bild hochladen und Pfad erhalten
                    var fileName = $"act-{act.ActNumber}-map";
                    var imagePath = await _imageUploadService.UploadImageAsync(request.MapImage, "maps", fileName);

                    if (act.Map != null)
                    {
                        // Altes Bild überschreiben
                        act.Map.ImageUrl = imagePath;
                    }
                    else
                    {
                        // Map erstellen falls noch nicht vorhanden
                        var map = new Map
                        {
                            ActId = act.Id,
                            Name = $"Karte {act.Name}",
                            ImageUrl = imagePath,
                            CreatedAt = DateTime.Now
                        };
                        _context.Maps.Add(map);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Act erfolgreich aktualisiert!",
                    actId = id,
                    act = new
                    {
                        act.Id,
                        act.ActNumber,
                        act.Name,
                        act.Description,
                        act.Country,
                        act.Companion1,
                        act.Companion2,
                        act.Month,
                        act.Weather,
                        act.IsActive,
                        MapImageUrl = act.Map?.ImageUrl
                    }
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { error = "Fehler beim Aktualisieren des Akts", details = innerMessage });
            }
        }

        /// <summary>
        /// Act löschen (nur für Gott)
        /// DELETE /api/game/acts/1
        /// </summary>
        [HttpDelete("acts/{id}")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> DeleteAct(int id)
        {
            try
            {
                var act = await _context.Acts
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (act == null)
                {
                    return NotFound(new { error = "Act nicht gefunden!" });
                }

                // Kein Include nötig - Cascade Delete kümmert sich um Maps
                _context.Acts.Remove(act);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Act erfolgreich gelöscht!",
                    actId = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Act aktivieren (nur für Gott)
        /// POST /api/game/acts/1/activate
        /// Deaktiviert alle anderen Acts und aktiviert diesen
        /// </summary>
        [HttpPost("acts/{id}/activate")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> ActivateAct(int id)
        {
            try
            {
                var act = await _context.Acts.FindAsync(id);
                if (act == null)
                {
                    return NotFound(new { error = "Act nicht gefunden!" });
                }

                // Alle Acts deaktivieren
                var allActs = await _context.Acts.ToListAsync();
                foreach (var a in allActs)
                {
                    a.IsActive = false;
                }

                // Diesen Act aktivieren
                act.IsActive = true;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Act '{act.Name}' wurde aktiviert!",
                    actId = id,
                    actName = act.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Alle Marker für eine Karte abrufen
        /// GET /api/game/markers?mapId=1
        /// </summary>
        [HttpGet("markers")]
        public async Task<IActionResult> GetMarkers([FromQuery] int mapId)
        {
            try
            {
                var markers = await _context.MapMarkers
                    .Where(m => m.MapId == mapId)
                    .Select(m => new
                    {
                        m.Id,
                        m.Label,
                        m.Type,
                        m.XPercent,
                        m.YPercent,
                        m.Description
                    })
                    .ToListAsync();

                return Ok(markers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Marker erstellen (nur für Gott)
        /// POST /api/game/markers
        /// Body: { "mapId": 1, "label": "...", "type": "quest", "xPercent": 50, "yPercent": 50, "description": "..." }
        /// </summary>
        [HttpPost("markers")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> CreateMarker([FromBody] CreateMarkerRequest request)
        {
            try
            {
                // Validierung
                if (string.IsNullOrWhiteSpace(request.Label))
                {
                    return BadRequest(new { error = "Label ist erforderlich!" });
                }

                if (request.XPercent < 0 || request.XPercent > 100 || request.YPercent < 0 || request.YPercent > 100)
                {
                    return BadRequest(new { error = "Koordinaten müssen zwischen 0 und 100 liegen!" });
                }

                var validTypes = new[] { "quest", "info", "danger", "settlement", "region" };
                if (!validTypes.Contains(request.Type))
                {
                    return BadRequest(new { error = "Ungültiger Marker-Typ!" });
                }

                // Map existiert?
                var map = await _context.Maps.FindAsync(request.MapId);
                if (map == null)
                {
                    return NotFound(new { error = "Karte nicht gefunden!" });
                }

                var marker = new MapMarker
                {
                    MapId = request.MapId,
                    Label = request.Label,
                    Type = request.Type,
                    XPercent = request.XPercent,
                    YPercent = request.YPercent,
                    Description = request.Description,
                    CreatedAt = DateTime.Now
                };

                _context.MapMarkers.Add(marker);
                await _context.SaveChangesAsync();

                // Wenn QuestId angegeben: Quest mit Marker verknüpfen
                if (request.QuestId.HasValue && request.Type == "quest")
                {
                    var quest = await _context.Quests.FindAsync(request.QuestId.Value);
                    if (quest != null)
                    {
                        quest.MapMarkerId = marker.Id;
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok(new
                {
                    message = "Marker erfolgreich erstellt!",
                    marker = new
                    {
                        marker.Id,
                        marker.Label,
                        marker.Type,
                        marker.XPercent,
                        marker.YPercent,
                        marker.Description
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Marker-Position aktualisieren (Drag & Drop, nur für Gott)
        /// PUT /api/game/markers/1/position
        /// Body: { "xPercent": 45.5, "yPercent": 67.8 }
        /// </summary>
        [HttpPut("markers/{id}/position")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> UpdateMarkerPosition(int id, [FromBody] UpdateMarkerPositionRequest request)
        {
            try
            {
                var marker = await _context.MapMarkers.FindAsync(id);
                if (marker == null)
                {
                    return NotFound(new { error = "Marker nicht gefunden!" });
                }

                // Validierung
                if (request.XPercent < 0 || request.XPercent > 100 ||
                    request.YPercent < 0 || request.YPercent > 100)
                {
                    return BadRequest(new { error = "Koordinaten müssen zwischen 0 und 100 liegen!" });
                }

                // Position aktualisieren
                marker.XPercent = request.XPercent;
                marker.YPercent = request.YPercent;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Marker-Position aktualisiert!",
                    marker = new
                    {
                        id = marker.Id,
                        xPercent = marker.XPercent,
                        yPercent = marker.YPercent
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Marker löschen (nur für Gott)
        /// DELETE /api/game/markers/1
        /// </summary>
        [HttpDelete("markers/{id}")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> DeleteMarker(int id)
        {
            try
            {
                var marker = await _context.MapMarkers.FindAsync(id);
                if (marker == null)
                {
                    return NotFound(new { error = "Marker nicht gefunden!" });
                }

                _context.MapMarkers.Remove(marker);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Marker erfolgreich gelöscht!",
                    markerId = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ===== WEATHER-ENDPOINTS =====

        /// <summary>
        /// Wetteroptionen für einen Monat abrufen
        /// GET /api/game/weather?month=Januar
        /// </summary>
        [HttpGet("weather")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> GetWeatherOptions([FromQuery] string month)
        {
            try
            {
                var weatherOptions = await _context.WeatherOptions
                    .Include(wo => wo.ForecastDays.OrderBy(fd => fd.DayOrder))
                    .Where(wo => wo.Month == month)
                    .Select(wo => new
                    {
                        wo.Id,
                        wo.WeatherName,
                        Forecast = wo.ForecastDays.Select(fd => new
                        {
                            fd.Day,
                            fd.Icon,
                            fd.Temperature
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(weatherOptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Marker aktualisieren (nur für Gott)
        /// PUT /api/game/markers/1
        /// Body: { "label": "...", "description": "...", "xPercent": 50, "yPercent": 50 }
        /// </summary>
        [HttpPut("markers/{id}")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> UpdateMarker(int id, [FromBody] UpdateMarkerRequest request)
        {
            try
            {
                var marker = await _context.MapMarkers.FindAsync(id);
                if (marker == null)
                {
                    return NotFound(new { error = "Marker nicht gefunden!" });
                }

                // Update
                if (!string.IsNullOrWhiteSpace(request.Label))
                {
                    marker.Label = request.Label;
                }

                if (request.Description != null)
                {
                    marker.Description = request.Description;
                }

                if (request.XPercent.HasValue)
                {
                    if (request.XPercent.Value < 0 || request.XPercent.Value > 100)
                    {
                        return BadRequest(new { error = "XPercent muss zwischen 0 und 100 liegen!" });
                    }
                    marker.XPercent = request.XPercent.Value;
                }

                if (request.YPercent.HasValue)
                {
                    if (request.YPercent.Value < 0 || request.YPercent.Value > 100)
                    {
                        return BadRequest(new { error = "YPercent muss zwischen 0 und 100 liegen!" });
                    }
                    marker.YPercent = request.YPercent.Value;
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Marker aktualisiert!",
                    marker = new
                    {
                        marker.Id,
                        marker.Label,
                        marker.Type,
                        marker.XPercent,
                        marker.YPercent,
                        marker.Description
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }




        // ===== MAP REGIONS API =====

        /// <summary>
        /// Alle Regionen einer Map laden
        /// GET /api/game/maps/1/regions
        /// </summary>
        [HttpGet("maps/{mapId}/regions")]
        public async Task<IActionResult> GetMapRegions(int mapId)
        {
            try
            {
                var regions = await _context.MapRegions
                    .Where(r => r.MapId == mapId)
                    .Select(r => new
                    {
                        r.Id,
                        r.RegionName,
                        r.PolygonPoints,
                        r.LinkedMapId
                    })
                    .ToListAsync();

                return Ok(regions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Fehler beim Laden der Regionen: {ex.Message}" });
            }
        }

        /// <summary>
        /// Region erstellen
        /// POST /api/game/map-regions
        /// </summary>
        [HttpPost("map-regions")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> CreateMapRegion([FromBody] CreateMapRegionRequest request)
        {
            try
            {
                // Validierung
                if (request.MapId <= 0)
                {
                    return BadRequest(new { error = "Map-ID ist erforderlich!" });
                }

                if (request.LinkedMapId <= 0)
                {
                    return BadRequest(new { error = "Verlinkte Map-ID ist erforderlich!" });
                }

                if (string.IsNullOrEmpty(request.RegionName))
                {
                    return BadRequest(new { error = "Regionsname ist erforderlich!" });
                }

                if (string.IsNullOrEmpty(request.PolygonPoints))
                {
                    return BadRequest(new { error = "Polygon-Punkte sind erforderlich!" });
                }

                // Map existiert?
                var map = await _context.Maps.FindAsync(request.MapId);
                if (map == null)
                {
                    return NotFound(new { error = "Map nicht gefunden!" });
                }

                // Verlinkte Map existiert?
                var linkedMap = await _context.Maps.FindAsync(request.LinkedMapId);
                if (linkedMap == null)
                {
                    return NotFound(new { error = "Verlinkte Map nicht gefunden!" });
                }

                // Region erstellen
                var region = new MapRegion
                {
                    MapId = request.MapId,
                    LinkedMapId = request.LinkedMapId,
                    RegionName = request.RegionName,
                    PolygonPoints = request.PolygonPoints,
                    CreatedAt = DateTime.Now
                };

                _context.MapRegions.Add(region);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Region erfolgreich erstellt!",
                    region = new
                    {
                        region.Id,
                        region.RegionName,
                        region.PolygonPoints,
                        region.LinkedMapId
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Fehler beim Erstellen der Region: {ex.Message}" });
            }
        }

        /// <summary>
        /// Region löschen
        /// DELETE /api/game/map-regions/5
        /// </summary>
        [HttpDelete("map-regions/{id}")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> DeleteMapRegion(int id)
        {
            try
            {
                var region = await _context.MapRegions.FindAsync(id);
                if (region == null)
                {
                    return NotFound(new { error = "Region nicht gefunden!" });
                }

                _context.MapRegions.Remove(region);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Region erfolgreich gelöscht!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Fehler beim Löschen der Region: {ex.Message}" });
            }
        }

        /// <summary>
        /// UPDATE: Polygon-Region aktualisieren
        /// PUT /api/game/map-regions/{id}
        /// </summary>
        [HttpPut("map-regions/{id}")]
        public async Task<IActionResult> UpdateMapRegion(int id, [FromBody] CreateMapRegionRequest request)
        {
            try
            {
                var region = await _context.MapRegions.FindAsync(id);
                if (region == null)
                {
                    return NotFound(new { error = "Region nicht gefunden!" });
                }

                // Validierung
                if (string.IsNullOrEmpty(request.RegionName))
                {
                    return BadRequest(new { error = "Regionsname ist erforderlich!" });
                }

                if (string.IsNullOrEmpty(request.PolygonPoints))
                {
                    return BadRequest(new { error = "Polygon-Punkte sind erforderlich!" });
                }

                // Aktualisieren
                region.RegionName = request.RegionName;
                region.LinkedMapId = request.LinkedMapId;
                region.PolygonPoints = request.PolygonPoints;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Region erfolgreich aktualisiert!", region = new { region.Id, region.RegionName, region.PolygonPoints, region.LinkedMapId } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Fehler beim Aktualisieren der Region: {ex.Message}" });
            }
        }
    }
    
    // ===== REQUEST MODELS =====

    public class NightRestRequest
    {
        public List<int> CharacterIds { get; set; } = new();
        public int FoodId { get; set; }
    }

    public class CreateQuestRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = "individual"; // "individual" oder "group"
        public string? Status { get; set; } = "active"; // "active", "completed", "failed"
        public int? CharacterId { get; set; } // Nur für Type = "individual"
        public int? PreviousQuestId { get; set; } // Für Questfolgen: Vorgänger-Quest
        public string? PreviousQuestRequirement { get; set; } = "both"; // "completed", "failed", "both"

        // Questmarker-Felder (optional)
        public bool CreateMarker { get; set; } = false; // Soll ein Marker erstellt werden?
        public double? MarkerXPercent { get; set; } // X-Position des Markers (0-100)
        public double? MarkerYPercent { get; set; } // Y-Position des Markers (0-100)
    }

    public class UpdateQuestRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? CharacterId { get; set; }
        public int? PreviousQuestId { get; set; } // Für Questfolgen: Vorgänger-Quest
        public string? PreviousQuestRequirement { get; set; } = "both"; // "completed", "failed", "both"

        // Questmarker-Felder (optional)
        public bool CreateMarker { get; set; } = false; // Soll ein Marker erstellt werden?
        public double? MarkerXPercent { get; set; } // X-Position des Markers (0-100)
        public double? MarkerYPercent { get; set; } // Y-Position des Markers (0-100)
        public bool RemoveMarker { get; set; } = false; // Soll der bestehende Marker entfernt werden?
    }

    public class SetTrophyStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    public class EquipTrophyRequest
    {
        public int Position { get; set; }
    }

    public class UpdateMarkerPositionRequest
    {
        public double XPercent { get; set; }
        public double YPercent { get; set; }
    }

    public class CreateMarkerRequest
    {
        public int MapId { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Type { get; set; } = "info"; // "quest", "info", "danger", "settlement"
        public double XPercent { get; set; }
        public double YPercent { get; set; }
        public string? Description { get; set; }
        public int? QuestId { get; set; } // Für Quest-Marker: ID der zugeordneten Quest
    }

    public class UpdateMarkerRequest
    {
        public string? Label { get; set; }
        public string? Description { get; set; }
        public double? XPercent { get; set; }
        public double? YPercent { get; set; }
    }

    // FormData Request Models für Act-Verwaltung
    public class CreateActFormRequest
    {
        public int ActNumber { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Country { get; set; }
        public string? Companion1 { get; set; }
        public string? Companion2 { get; set; }
        public string? Month { get; set; }
        public string? Weather { get; set; }
        public IFormFile? MapImage { get; set; } // Bilddatei statt Base64-String
    }

    public class UpdateActFormRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Country { get; set; }
        public string? Companion1 { get; set; }
        public string? Companion2 { get; set; }
        public string? Month { get; set; }
        public string? Weather { get; set; }
        public IFormFile? MapImage { get; set; } // Bilddatei statt Base64-String
    }
    public class CreateMapRegionRequest
    {
        public int MapId { get; set; }
        public int LinkedMapId { get; set; }
        public string RegionName { get; set; } = string.Empty;
        public string PolygonPoints { get; set; } = string.Empty;
    }
}