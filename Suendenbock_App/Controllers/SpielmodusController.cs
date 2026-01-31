// Controllers/SpielmodusController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models;
using Suendenbock_App.Controllers.Api;

namespace Suendenbock_App.Controllers
{
    [Authorize] // Nur eingeloggte Benutzer können auf Spielmodus zugreifen
    public class SpielmodusController : BaseController
    {
        public SpielmodusController(ApplicationDbContext context) : base(context)
        {
        }

        // ===== DASHBOARD =====

        /// <summary>
        /// Haupt-Dashboard des Spielmodus
        /// Route: /Spielmodus/Dashboard
        /// Gott kann mit ?actId=X einen spezifischen Act laden (zum Vorbereiten)
        /// </summary>
        public async Task<IActionResult> Dashboard(int? actId)
        {
            var isGod = User.IsInRole("Gott");

            // Bestimme welchen Act geladen werden soll
            // Gott: kann via actId Parameter jeden Act laden (zum Vorbereiten)
            // Spieler: sehen nur den aktiven Act
            Act? currentAct;
            if (isGod && actId.HasValue)
            {
                currentAct = await _context.Acts
                    .Include(a => a.Map)
                    .FirstOrDefaultAsync(a => a.Id == actId.Value);
            }
            else
            {
                currentAct = await _context.Acts
                    .Include(a => a.Map)
                    .FirstOrDefaultAsync(a => a.IsActive);
            }

            // Lade alle aktiven Spieler-Characters
            // Filter: nur Spieler-Characters (UserId != null, keine Begleiter)
            var characters = await _context.Characters
                .Where(c => c.UserId != null && !c.IsCompanion)
                .Select(c => new CharacterViewModel
                {
                    Id = c.Id,
                    Name = c.Vorname,
                    CurrentHealth = c.CurrentHealth,
                    BaseMaxHealth = c.BaseMaxHealth,
                    CurrentPokus = c.CurrentPokus,
                    HealthPercent = c.BaseMaxHealth > 0
                        ? (int)((double)c.CurrentHealth / c.BaseMaxHealth * 100)
                        : 0
                })
                .ToListAsync();

            // Lade verfügbare Rest Foods
            var restFoods = await _context.RestFoods
                .Select(f => new RestFoodViewModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    HealthBonus = f.HealthBonus
                })
                .ToListAsync();

            // Lade Wetterdaten für aktuellen Act (falls vorhanden)
            List<WeatherForecastDayViewModel> weatherForecast = new();
            if (currentAct != null && !string.IsNullOrEmpty(currentAct.Weather))
            {
                // Finde die WeatherOption basierend auf dem gespeicherten Wetter-String
                var weatherOption = await _context.WeatherOptions
                    .Include(wo => wo.ForecastDays)
                    .FirstOrDefaultAsync(wo => wo.WeatherName == currentAct.Weather);

                if (weatherOption != null && weatherOption.ForecastDays.Any())
                {
                    var orderedDays = weatherOption.ForecastDays.OrderBy(fd => fd.DayOrder).ToList();
                    var relativeDayNames = new[] { "Vorgestern", "Gestern", "Heute", "Morgen", "Übermorgen" };

                    weatherForecast = orderedDays
                        .Select((fd, index) => new WeatherForecastDayViewModel
                        {
                            Day = index < relativeDayNames.Length ? relativeDayNames[index] : $"+{index - 2} Tage",
                            Icon = fd.Icon,
                            Temperature = fd.Temperature
                        })
                        .ToList();
                }
            }

            var viewModel = new SpielmodusDashboardViewModel
            {
                ActiveCharacters = characters,
                RestFoods = restFoods,
                CurrentAct = currentAct != null ? new ActViewModel
                {
                    Id = currentAct.Id,
                    Name = currentAct.Name,
                    ActNumber = currentAct.ActNumber,
                    Description = currentAct.Description,
                    IsActive = currentAct.IsActive,
                    Weather = currentAct.Weather
                } : null,
                WeatherForecast = weatherForecast,
                IsGod = isGod
            };

            // Für SignalR Global: ActId setzen
            if (currentAct != null)
            {
                ViewBag.CurrentActId = currentAct.ActNumber;
            }

            return View(viewModel);
        }

        // ===== WEITERE VIEWS (kommen später) =====

        /// <summary>
        /// Map-Seite
        /// Route: /Spielmodus/Map
        /// Gott kann mit ?actId=X einen spezifischen Act laden
        /// </summary>
        public async Task<IActionResult> Map(int? actId, int? focusMarkerId = null)
        {
            var isGod = User.IsInRole("Gott");

            // Bestimme welchen Act geladen werden soll
            Act? currentAct;
            if (isGod && actId.HasValue)
            {
                currentAct = await _context.Acts
                    .Include(a => a.Map)
                        .ThenInclude(m => m.Markers)
                            .ThenInclude(marker => marker.Quests)
                                .ThenInclude(q => q.Character)
                    .Include(a => a.Map)
                        .ThenInclude(m => m.Markers)
                            .ThenInclude(marker => marker.Quests)
                                .ThenInclude(q => q.PreviousQuest)
                    .FirstOrDefaultAsync(a => a.Id == actId.Value);
            }
            else
            {
                currentAct = await _context.Acts
                    .Include(a => a.Map)
                        .ThenInclude(m => m.Markers)
                            .ThenInclude(marker => marker.Quests)
                                .ThenInclude(q => q.Character)
                    .Include(a => a.Map)
                        .ThenInclude(m => m.Markers)
                            .ThenInclude(marker => marker.Quests)
                                .ThenInclude(q => q.PreviousQuest)
                    .FirstOrDefaultAsync(a => a.IsActive);
            }

            // Alle aktiven Quests DES AKTUELLEN ACTS für Marker-Zuordnung (nur für Gott relevant)
            var activeQuests = isGod && currentAct != null
                ? await _context.Quests
                    .Include(q => q.Character)
                    .Where(q => q.Status == "active" && q.ActId == currentAct.Id)
                    .Select(q => new QuestOption
                    {
                        Id = q.Id,
                        Title = q.Title
                    })
                    .ToListAsync()
                : new List<QuestOption>();

            var viewModel = new MapViewModel
            {
                CurrentAct = currentAct != null ? new ActViewModel
                {
                    Id = currentAct.Id,
                    Name = currentAct.Name,
                    ActNumber = currentAct.ActNumber,
                    Description = currentAct.Description,
                    IsActive = currentAct.IsActive
                } : null,
                FocusMarkerId = focusMarkerId,
                CurrentMap = currentAct?.Map != null ? new MapViewModelData
                {
                    Id = currentAct.Map.Id,
                    Name = currentAct.Map.Name,
                    ImageUrl = currentAct.Map.ImageUrl,
                    Markers = currentAct.Map.Markers
                        .Where(m =>
                        {
                            // Zeige alle Nicht-Quest-Marker
                            if (m.Type != "quest") return true;

                            // Für Quest-Marker: Prüfe ob Quest sichtbar ist
                            var quest = m.Quests.FirstOrDefault();
                            if (quest == null) return false;

                            // Quest muss aktiv sein
                            if (quest.Status != "active") return false;

                            // Wenn Quest eine Folgequest ist, prüfe ob Vorgänger-Bedingung erfüllt
                            if (quest.PreviousQuestId.HasValue)
                            {
                                if (quest.PreviousQuest == null) return false;

                                var requirement = quest.PreviousQuestRequirement ?? "both";
                                var isVisible = requirement switch
                                {
                                    "completed" => quest.PreviousQuest.Status == "completed",
                                    "failed" => quest.PreviousQuest.Status == "failed",
                                    "both" => quest.PreviousQuest.Status == "completed" || quest.PreviousQuest.Status == "failed",
                                    _ => false
                                };
                                if (!isVisible) return false;
                            }

                            return true;
                        })
                        .Select(m =>
                        {
                            var quest = m.Quests.FirstOrDefault();
                            return new MarkerViewModel
                            {
                                Id = m.Id,
                                Label = m.Label,
                                Type = m.Type,
                                XPercent = m.XPercent,
                                YPercent = m.YPercent,
                                Description = m.Description,
                                QuestId = quest?.Id,
                                QuestTitle = quest?.Title,
                                QuestCharacterName = quest?.Character != null
                                    ? $"{quest.Character.Vorname} {quest.Character.Nachname}"
                                    : null,
                                QuestType = quest?.Type
                            };
                        }).ToList()
                } : null,
                ActiveQuests = activeQuests,
                IsGod = isGod
            };

            return View(viewModel);
        }

        // ===== QUESTS =====

        /// <summary>
        /// Quest-Übersicht Seite
        /// Route: /Spielmodus/Quests
        /// Gott kann mit ?actId=X einen spezifischen Act laden
        /// </summary>
        public async Task<IActionResult> Quests(int? actId, string? focusQuestId = null)
        {
            var isGod = User.IsInRole("Gott");

            // Bestimme welchen Act geladen werden soll
            Act? currentAct;
            if (isGod && actId.HasValue)
            {
                currentAct = await _context.Acts
                    .FirstOrDefaultAsync(a => a.Id == actId.Value);
            }
            else
            {
                currentAct = await _context.Acts
                    .FirstOrDefaultAsync(a => a.IsActive);
            }

            if (currentAct == null)
            {
                // Kein Act -> keine Quests anzeigen
                return View(new QuestsViewModel
                {
                    Quests = new List<QuestViewModel>(),
                    Characters = new List<string>(),
                    FocusQuestId = focusQuestId,
                    IsGod = isGod
                });
            }

            // Alle Quests DES AKTUELLEN ACTS laden (mit Character + MapMarker + PreviousQuest)
            var allQuests = await _context.Quests
                .Include(q => q.Character)
                .Include(q => q.MapMarker)
                .Include(q => q.PreviousQuest)
                .Where(q => q.ActId == currentAct.Id)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            // Hilfsfunktion: Prüft ob eine Quest verfügbar ist (Bedingung erfüllt)
            bool IsQuestAvailable(Quest q)
            {
                // Keine Vorgänger-Quest = immer verfügbar
                if (!q.PreviousQuestId.HasValue) return true;

                // Vorgänger-Quest muss existieren
                if (q.PreviousQuest == null) return false;

                // Prüfe Bedingung basierend auf PreviousQuestRequirement
                var requirement = q.PreviousQuestRequirement ?? "both";
                return requirement switch
                {
                    "completed" => q.PreviousQuest.Status == "completed",
                    "failed" => q.PreviousQuest.Status == "failed",
                    "both" => q.PreviousQuest.Status == "completed" || q.PreviousQuest.Status == "failed",
                    _ => false // Ungültige Bedingung = nicht verfügbar
                };
            }

            // Gott sieht alle Quests (mit IsAvailable-Flag), normale User nur verfügbare
            var visibleQuests = allQuests
                .Where(q => isGod || IsQuestAvailable(q)) // Gott: alle, User: nur verfügbare
                .Select(q => new QuestViewModel
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    Type = q.Type,
                    Status = q.Status,
                    CharacterName = q.Character != null ? $"{q.Character.Vorname}" : null,
                    CharacterId = q.CharacterId,
                    CreatedAt = q.CreatedAt,
                    CompletedAt = q.CompletedAt,
                    PreviousQuestId = q.PreviousQuestId,
                    PreviousQuestTitle = q.PreviousQuest?.Title,
                    MapMarkerId = q.MapMarkerId,
                    IsAvailable = IsQuestAvailable(q) // Setze Flag für View
                })
                .ToList();

            // Alle aktiven Spieler-Characters für Filter + Dropdown
            var characters = await _context.Characters
                .Where(c => c.UserId != null)
                .Select(c => new { c.Id, FullName = $"{c.Vorname}" })
                .ToListAsync();

            var viewModel = new QuestsViewModel
            {
                Quests = visibleQuests,
                Characters = characters.Select(c => c.FullName).ToList(),
                CharactersForDropdown = characters.ToDictionary(c => c.Id, c => c.FullName),
                FocusQuestId = focusQuestId,
                IsGod = isGod,
                // Für Vorgänger-Quest Dropdown: Alle aktiven Quests des Acts (ohne Bedingung)
                AllQuestsForDropdown = allQuests.ToDictionary(q => q.Id, q => q.Title)
            };

            return View(viewModel);
        }

        // ===== QUEST EDIT & DELETE =====

        /// <summary>
        /// Quest bearbeiten (nur für Gott)
        /// Route: POST /Spielmodus/EditQuest?id={id}
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> EditQuest(int id, [FromBody] CreateQuestRequest request)
        {
            try
            {
                var quest = await _context.Quests
                    .Include(q => q.MapMarker)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (quest == null)
                {
                    return NotFound("Quest nicht gefunden!");
                }

                // Update quest properties
                if (!string.IsNullOrWhiteSpace(request.Title))
                {
                    quest.Title = request.Title;
                }

                quest.Description = request.Description ?? string.Empty;
                quest.Type = request.Type;
                quest.Status = request.Status;

                // Update character assignment
                if (request.Type == "individual" && request.CharacterId.HasValue)
                {
                    quest.CharacterId = request.CharacterId.Value;
                }
                else if (request.Type == "group")
                {
                    quest.CharacterId = null; // Gruppenquests haben keinen einzelnen Character
                }

                // Update PreviousQuestId und Requirement
                if (request.PreviousQuestId.HasValue)
                {
                    var previousQuest = await _context.Quests.FindAsync(request.PreviousQuestId.Value);
                    if (previousQuest == null)
                    {
                        return BadRequest("Vorgänger-Quest nicht gefunden!");
                    }
                    if (previousQuest.ActId != quest.ActId)
                    {
                        return BadRequest("Vorgänger-Quest muss zum gleichen Act gehören!");
                    }
                    if (previousQuest.Id == quest.Id)
                    {
                        return BadRequest("Eine Quest kann nicht ihre eigene Vorgänger-Quest sein!");
                    }
                    quest.PreviousQuestId = request.PreviousQuestId.Value;
                    quest.PreviousQuestRequirement = request.PreviousQuestRequirement ?? "both";
                }

                // Optional: Questmarker erstellen/aktualisieren
                if (request.CreateMarker && request.MarkerXPercent.HasValue && request.MarkerYPercent.HasValue)
                {
                    if (request.MarkerXPercent.Value < 0 || request.MarkerXPercent.Value > 100 ||
                        request.MarkerYPercent.Value < 0 || request.MarkerYPercent.Value > 100)
                    {
                        return BadRequest("Marker-Koordinaten müssen zwischen 0 und 100 liegen!");
                    }

                    var map = await _context.Maps.FirstOrDefaultAsync(m => m.ActId == quest.ActId);
                    if (map != null)
                    {
                        if (quest.MapMarkerId.HasValue)
                        {
                            // Marker aktualisieren
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
                            // Neuen Marker erstellen
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

                return Ok(new { message = "Quest erfolgreich aktualisiert!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fehler beim Aktualisieren: {ex.Message}");
            }
        }

        /// <summary>
        /// Quest löschen (nur für Gott)
        /// Route: POST /Spielmodus/DeleteQuest?id={id}
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> DeleteQuest(int id)
        {
            try
            {
                var quest = await _context.Quests.FindAsync(id);
                if (quest == null)
                {
                    return NotFound("Quest nicht gefunden!");
                }

                _context.Quests.Remove(quest);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Quest erfolgreich gelöscht!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fehler beim Löschen: {ex.Message}");
            }
        }

        // ===== TROPHIES =====

        /// <summary>
        /// Trophäen-Seite
        /// Route: /Spielmodus/Trophies
        /// </summary>
        public async Task<IActionResult> Trophies()
        {
            // Alle Monster/Trophäen laden (mit Monstertyp)
            // Nur Monster anzeigen, die:
            // 1. In der Trophäen-Truhe sind (HasBoughtTrophy ODER HasSlainTrophy)
            var monsters = await _context.Monsters
                .Include(m => m.Monstertyp)
                .Where(m => m.HasBoughtTrophy || m.HasSlainTrophy)
                .OrderBy(m => m.Name)
                .Select(m => new TrophyViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    ImagePath = m.ImagePath,
                    MonsterType = m.Monstertyp != null ? m.Monstertyp.Name : "Unbekannt",
                    Description = m.Description ?? string.Empty,
                    BaseEffect = m.BaseEffect ?? string.Empty,
                    SlainEffect = m.SlainEffect ?? string.Empty,
                    Status = m.Status,
                    IsEquipped = m.IsEquipped,
                    EquippedPosition = m.EquippedPosition,
                    HasBoughtTrophy = m.HasBoughtTrophy,
                    HasSlainTrophy = m.HasSlainTrophy,
                    Meet = m.meet,
                    Encounter = m.encounter,
                    Perfected = m.perfected,
                    Basics = m.Basics,
                    EncounterDescription = m.Description,
                    ProcessedDescription = m.ProcessedDescription
                })
                .ToListAsync();

            // Aufteilen: Equipped vs Inventory
            // Equipped = nur Trophäen mit Position (1, 2, oder 3)
            // Inventory = alle anderen (inkl. IsEquipped ohne Position)
            var viewModel = new TrophiesViewModel
            {
                EquippedTrophies = monsters.Where(m => m.IsEquipped && m.EquippedPosition.HasValue).ToList(),
                InventoryTrophies = monsters.Where(m => !m.IsEquipped || !m.EquippedPosition.HasValue).ToList()
            };

            return View(viewModel);
        }

        // ===== BATTLE =====

        /// <summary>
        /// Kampf-Seite
        /// Route: /Spielmodus/Battle
        /// 
        /// Zeigt Monster-Auswahl (nur meet=true Monster)
        /// Dann Character-Auswahl
        /// Dann Start Combat
        /// </summary>
        public async Task<IActionResult> Battle()
        {
            var isGod = User.IsInRole("Gott");

            // Alle Characters laden (Spieler + Begleiter)
            var characters = await _context.Characters
                .Where(c => c.UserId != null || c.IsCompanion)
                .OrderBy(c => c.Nachname)
                .Select(c => new BattleCharacterOption
                {
                    Id = c.Id,
                    Name = $"{c.Vorname} {c.Nachname}",
                    CurrentHealth = c.CurrentHealth,
                    MaxHealth = c.BaseMaxHealth,
                    CurrentPokus = c.CurrentPokus,
                    MaxPokus = c.BaseMaxPokus,
                    IsBegleiter = c.IsCompanion,
                    HealthPercent = c.BaseMaxHealth > 0
                        ? (int)((double)c.CurrentHealth / c.BaseMaxHealth * 100)
                        : 0
                })
                .ToListAsync();

            // Alle freigeschalteten Monster (meet = true)
            var monsters = await _context.Monsters
                .Include(m => m.Monstertyp)
                .Where(m => m.meet)
                .OrderBy(m => m.Name)
                .Select(m => new BattleMonsterOption
                {
                    Id = m.Id,
                    Name = m.Name,
                    MonstertypName = m.Monstertyp != null ? m.Monstertyp.Name : "Unbekannt",
                    ImageUrl = m.ImagePath ?? "/images/monsters/default.png",
                    Health = 150,
                    Attack = 10,
                    Defense = 10,
                    IsBoss = false
                })
                .ToListAsync();

            var viewModel = new BattleViewModel
            {
                Characters = characters,
                Monsters = monsters,
                IsGod = isGod
            };

            return View(viewModel);
        }
        /// <summary>
        /// Combat-Setup Seite
        /// Route: /Spielmodus/CombatSetup
        ///
        /// Gott wählt:
        /// 1. Monster (alle mit meet == true, mehrere möglich via Dropdown)
        /// 2. Für jedes Monster: Health + Pokus eingeben
        /// 3. Characters (Spieler + Begleiter des aktuellen Akts)
        /// 4. Setup speichern
        /// </summary>
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> CombatSetup()
        {
            // Aktuellen Act laden
            var currentAct = await _context.Acts
                .FirstOrDefaultAsync(a => a.IsActive);

            // Alle Monster mit meet == true (keine Gruppierung nach Typ)
            var monsters = await _context.Monsters
                .Include(m => m.Monstertyp)
                .Where(m => m.meet == true)
                .OrderBy(m => m.Name)
                .Select(m => new MonsterOptionViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    ImagePath = m.ImagePath,
                    MonstertypName = m.Monstertyp != null ? m.Monstertyp.Name : "Unbekannt",
                    Lebenspunkte = m.Lebenspunkte
                })
                .ToListAsync();

            // Alle Spieler-Characters (UserId != null)
            var playerCharacters = await _context.Characters
                .Where(c => c.UserId != null)
                .OrderBy(c => c.Nachname)
                .Select(c => new CharacterOptionViewModel
                {
                    Id = c.Id,
                    Name = $"{c.Vorname} {c.Nachname}",
                    Nachname = c.Nachname,
                    CurrentHealth = c.CurrentHealth,
                    MaxHealth = c.BaseMaxHealth,
                    CurrentPokus = c.CurrentPokus,
                    IsBegleiter = false
                })
                .ToListAsync();

            // Begleiter des aktuellen Akts laden
            var companions = new List<CharacterOptionViewModel>();
            if (currentAct != null)
            {
                var companionNames = new List<string>();
                if (!string.IsNullOrEmpty(currentAct.Companion1))
                {
                    companionNames.Add(currentAct.Companion1.Trim());
                }
                if (!string.IsNullOrEmpty(currentAct.Companion2))
                {
                    companionNames.Add(currentAct.Companion2.Trim());
                }

                if (companionNames.Any())
                {

                    //// Begleiter laden - voller Name-Vergleich (Companion1/2 speichern "Vorname Nachname")
                    var allCompanions = await _context.Characters
                        .ToListAsync();

                    companions = allCompanions
                        .Where(c => companionNames.Contains($"{c.Vorname} {c.Nachname}"))
                        .OrderBy(c => c.Nachname)
                        .Select(c => new CharacterOptionViewModel
                        {
                            Id = c.Id,
                            Name = $"{c.Vorname} {c.Nachname}",
                            Nachname = c.Nachname,
                            CurrentHealth = c.CurrentHealth,
                            MaxHealth = c.BaseMaxHealth,
                            CurrentPokus = c.CurrentPokus,
                            IsBegleiter = true
                        })
                        .ToList();
                }
            }

            // Zusammenführen: Spieler + Begleiter des Akts
            var allCharacters = playerCharacters.Concat(companions).ToList();

            var viewModel = new CombatSetupViewModel
            {
                ActId = currentAct?.Id ?? 0,
                Monsters = monsters,
                Characters = allCharacters
            };

            return View(viewModel);
        }
    }

    // ===== VIEW MODELS =====

    public class SpielmodusDashboardViewModel
    {
        public List<CharacterViewModel> ActiveCharacters { get; set; } = new();
        public List<RestFoodViewModel> RestFoods { get; set; } = new();
        public ActViewModel? CurrentAct { get; set; }
        public List<WeatherForecastDayViewModel> WeatherForecast { get; set; } = new();
        public bool IsGod { get; set; }
    }

    public class CharacterViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CurrentHealth { get; set; }
        public int BaseMaxHealth { get; set; }
        public int CurrentPokus { get; set; }
        public int HealthPercent { get; set; }
    }

    public class RestFoodViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Quality { get; set; } = string.Empty;
        public int HealthBonus { get; set; }
    }
    // ===== VIEW MODELS =====

    public class QuestsViewModel
    {
        public List<QuestViewModel> Quests { get; set; } = new();
        public List<string> Characters { get; set; } = new();
        public Dictionary<int, string> CharactersForDropdown { get; set; } = new();
        public Dictionary<int, string> AllQuestsForDropdown { get; set; } = new(); // Für Vorgänger-Quest Auswahl
        public string? FocusQuestId { get; set; }
        public bool IsGod { get; set; }
    }

    public class QuestViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "individual" oder "group"
        public string Status { get; set; } = string.Empty; // "active", "completed", "failed"
        public string? CharacterName { get; set; }
        public int? CharacterId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? PreviousQuestId { get; set; } // Vorgänger-Quest für Questfolgen
        public string? PreviousQuestTitle { get; set; } // Titel der Vorgänger-Quest
        public int? MapMarkerId { get; set; } // MapMarker-ID für Link zur Karte
        public bool IsAvailable { get; set; } = true; // Gibt an, ob die Quest für Spieler verfügbar ist (Bedingung erfüllt)
    }

    // ===== VIEW MODELS =====

    public class TrophiesViewModel
    {
        public List<TrophyViewModel> EquippedTrophies { get; set; } = new();
        public List<TrophyViewModel> InventoryTrophies { get; set; } = new();
    }

    public class TrophyViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string MonsterType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BaseEffect { get; set; } = string.Empty;
        public string SlainEffect { get; set; } = string.Empty;
        public string Status { get; set; } = "none"; // "bought" oder "slain" (wird aus Model berechnet)
        public bool IsEquipped { get; set; }
        public int? EquippedPosition { get; set; } // Position an der Wand: 1, 2, oder 3
        public bool HasBoughtTrophy { get; set; }
        public bool HasSlainTrophy { get; set; }

        // Monster-Beschreibungen für verschiedene Stufen
        public bool Meet { get; set; }
        public bool Encounter { get; set; }
        public bool Perfected { get; set; }
        public string? Basics { get; set; }
        public string? EncounterDescription { get; set; }
        public string? ProcessedDescription { get; set; }

        // Helper für die View
        public bool HasBothVariants => HasBoughtTrophy && HasSlainTrophy;
    }
    public class MapViewModel
    {
        public ActViewModel? CurrentAct { get; set; }
        public MapViewModelData? CurrentMap { get; set; }
        public List<QuestOption> ActiveQuests { get; set; } = new();
        public bool IsGod { get; set; }
        public int? FocusMarkerId { get; set; } // Marker-ID zum Fokussieren/Highlighten
    }

    public class ActViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ActNumber { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Weather { get; set; }
    }

    public class WeatherForecastDayViewModel
    {
        public string Day { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Temperature { get; set; } = string.Empty;
    }

    public class MapViewModelData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public List<MarkerViewModel> Markers { get; set; } = new();
    }

    public class MarkerViewModel
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "quest", "info", "danger", "settlement"
        public double XPercent { get; set; }
        public double YPercent { get; set; }
        public string? Description { get; set; }

        // Quest-spezifische Felder
        public int? QuestId { get; set; }
        public string? QuestTitle { get; set; }
        public string? QuestCharacterName { get; set; }
        public string? QuestType { get; set; } // "individual" oder "group"
    }

    public class QuestOption
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }
    public class BattleViewModel
    {
        public List<BattleCharacterOption> Characters { get; set; } = new();
        public List<BattleMonsterOption> Monsters { get; set; } = new();
        public bool IsGod { get; set; }
    }

    public class BattleCharacterOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentPokus { get; set; }
        public int MaxPokus { get; set; }
        public bool IsBegleiter { get; set; }
        public int HealthPercent { get; set; }
    }

    public class BattleMonsterOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MonstertypName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public bool IsBoss { get; set; }
    }
    public class CombatSetupViewModel
    {
        public int ActId { get; set; }
        public List<MonsterOptionViewModel> Monsters { get; set; } = new();
        public List<CharacterOptionViewModel> Characters { get; set; } = new();
    }

    public class MonstertypViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<MonsterOptionViewModel> Monsters { get; set; } = new();
    }

    public class MonsterOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string MonstertypName { get; set; } = string.Empty;
        public int Lebenspunkte { get; set; }
    }

    public class CharacterOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Nachname { get; set; } = string.Empty;
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentPokus { get; set; }
        public bool IsBegleiter { get; set; }
    }
}