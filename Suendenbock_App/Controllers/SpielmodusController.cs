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
                    .FirstOrDefaultAsync(a => a.Id == actId.Value);
            }
            else
            {
                currentAct = await _context.Acts
                    .FirstOrDefaultAsync(a => a.IsActive);
            }

            // Lade Weltkarte des Acts
            if (currentAct != null)
            {
                currentAct.Map = await _context.Maps
                    .Where(m => m.ActId == currentAct.Id && m.IsWorldMap)
                    .FirstOrDefaultAsync();
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

        /// <summary>
        /// Map-Seite
        /// Route: /Spielmodus/Map
        /// Gott kann mit ?actId=X einen spezifischen Act laden
        /// HIERARCHIE: ?mapId=X lädt spezifische Karte (Weltkarte oder Detail-Karte)
        /// </summary>
        public async Task<IActionResult> Map(int? actId, int? mapId, int? focusMarkerId = null)
        {
            var isGod = User.IsInRole("Gott");

            Map? selectedMap = null;
            Act? currentAct = null;

            if (mapId.HasValue)
            {
                selectedMap = await _context.Maps
                    .AsNoTracking()
                    .Include(m => m.Act)
                    .Include(m => m.ParentMap)
                    .FirstOrDefaultAsync(m => m.Id == mapId.Value);

                currentAct = selectedMap?.Act;
            }
            else if (isGod && actId.HasValue)
            {
                currentAct = await _context.Acts
                    .FirstOrDefaultAsync(a => a.Id == actId.Value);

                if (currentAct != null)
                {
                    selectedMap = await _context.Maps
                        .AsNoTracking()
                        .Where(m => m.ActId == currentAct.Id && m.IsWorldMap)
                        .FirstOrDefaultAsync();
                }
            }
            else
            {
                currentAct = await _context.Acts
                    .FirstOrDefaultAsync(a => a.IsActive);

                if (currentAct != null)
                {
                    selectedMap = await _context.Maps
                        .AsNoTracking()
                        .Where(m => m.ActId == currentAct.Id && m.IsWorldMap)
                        .FirstOrDefaultAsync();
                }
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
                CurrentMap = selectedMap != null ? new MapViewModelData
                {
                    Id = selectedMap.Id,
                    Name = selectedMap.Name,
                    ImageUrl = selectedMap.ImageUrl,
                    IsWorldMap = selectedMap.IsWorldMap,
                    ParentMapId = selectedMap.ParentMapId,
                    ParentMapName = selectedMap.ParentMap?.Name,
                    ChildMaps = new List<ChildMapViewModel>(),
                    Markers = new List<MarkerViewModel>()
                } : null,
                ActiveQuests = activeQuests,
                IsGod = isGod
            };

            if (viewModel.CurrentMap != null)
            {
                var allMarkersWithQuests = await _context.MapMarkers
                    .AsNoTracking()
                    .Where(m => m.MapId == viewModel.CurrentMap.Id)
                    .Select(m => new
                    {
                        Id = m.Id,
                        Label = m.Label,
                        Type = m.Type,
                        XPercent = m.XPercent,
                        YPercent = m.YPercent,
                        Description = m.Description,
                        LinkedMapId = m.LinkedMapId,
                        QuestId = m.Quests.FirstOrDefault() != null ? m.Quests.FirstOrDefault().Id : (int?)null,
                        QuestTitle = m.Quests.FirstOrDefault() != null ? m.Quests.FirstOrDefault().Title : null,
                        QuestStatus = m.Quests.FirstOrDefault() != null ? m.Quests.FirstOrDefault().Status : null,
                        QuestType = m.Quests.FirstOrDefault() != null ? m.Quests.FirstOrDefault().Type : null,
                        QuestPreviousQuestId = m.Quests.FirstOrDefault() != null ? m.Quests.FirstOrDefault().PreviousQuestId : null,
                        QuestPreviousQuestRequirement = m.Quests.FirstOrDefault() != null ? m.Quests.FirstOrDefault().PreviousQuestRequirement : null,
                        PreviousQuestStatus = m.Quests.FirstOrDefault() != null && m.Quests.FirstOrDefault().PreviousQuest != null
                            ? m.Quests.FirstOrDefault().PreviousQuest.Status
                            : null,
                        CharacterVorname = m.Quests.FirstOrDefault() != null && m.Quests.FirstOrDefault().Character != null
                            ? m.Quests.FirstOrDefault().Character.Vorname
                            : null,
                        CharacterNachname = m.Quests.FirstOrDefault() != null && m.Quests.FirstOrDefault().Character != null
                            ? m.Quests.FirstOrDefault().Character.Nachname
                            : null
                    })
                    .ToListAsync();

                viewModel.CurrentMap.Markers = allMarkersWithQuests
                    .Where(x =>
                    {
                        if (x.Type != "quest") return true;
                        if (!x.QuestId.HasValue) return false;
                        if (x.QuestStatus != "active") return false;
                        if (x.QuestPreviousQuestId.HasValue)
                        {
                            var requirement = x.QuestPreviousQuestRequirement ?? "both";
                            var isVisible = requirement switch
                            {
                                "completed" => x.PreviousQuestStatus == "completed",
                                "failed" => x.PreviousQuestStatus == "failed",
                                "both" => x.PreviousQuestStatus == "completed" || x.PreviousQuestStatus == "failed",
                                _ => false
                            };
                            if (!isVisible) return false;
                        }

                        return true;
                    })
                    .Select(x => new MarkerViewModel
                    {
                        Id = x.Id,
                        Label = x.Label,
                        Type = x.Type,
                        XPercent = x.XPercent,
                        YPercent = x.YPercent,
                        Description = x.Description,
                        QuestId = x.QuestId,
                        QuestTitle = x.QuestTitle,
                        QuestCharacterName = !string.IsNullOrEmpty(x.CharacterVorname)
                            ? $"{x.CharacterVorname} {x.CharacterNachname}"
                            : null,
                        QuestType = x.QuestType,
                        LinkedMapId = x.LinkedMapId,
                        LinkedMapName = null
                    }).ToList();
            }

            if (viewModel.CurrentMap != null && viewModel.CurrentMap.IsWorldMap)
            {
                viewModel.CurrentMap.Regions = await _context.MapRegions
                    .Where(r => r.MapId == viewModel.CurrentMap.Id)
                    .Select(r => new MapRegionViewModel
                    {
                        Id = r.Id,
                        RegionName = r.RegionName,
                        PolygonPoints = r.PolygonPoints,
                        LinkedMapId = r.LinkedMapId
                    })
                    .ToListAsync();
            }

            if (viewModel.CurrentMap != null && viewModel.CurrentMap.IsWorldMap)
            {
                viewModel.CurrentMap.ChildMaps = await _context.Maps
                    .Where(cm => cm.ParentMapId == viewModel.CurrentMap.Id)
                    .Select(cm => new ChildMapViewModel
                    {
                        Id = cm.Id,
                        Name = cm.Name,
                        ImageUrl = cm.ImageUrl
                    })
                    .ToListAsync();
            }

            return View(viewModel);
        }

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
                return View(new QuestsViewModel
                {
                    Quests = new List<QuestViewModel>(),
                    Characters = new List<string>(),
                    FocusQuestId = focusQuestId,
                    IsGod = isGod
                });
            }

            var allQuests = await _context.Quests
                .Include(q => q.Character)
                .Include(q => q.MapMarker)
                .Include(q => q.PreviousQuest)
                .Where(q => q.ActId == currentAct.Id)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            bool IsQuestAvailable(Quest q)
            {
                if (!q.PreviousQuestId.HasValue) return true;
                if (q.PreviousQuest == null) return false;
                var requirement = q.PreviousQuestRequirement ?? "both";
                return requirement switch
                {
                    "completed" => q.PreviousQuest.Status == "completed",
                    "failed" => q.PreviousQuest.Status == "failed",
                    "both" => q.PreviousQuest.Status == "completed" || q.PreviousQuest.Status == "failed",
                    _ => false
                };
            }

            var visibleQuests = allQuests
                .Where(q => isGod || IsQuestAvailable(q))
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
                    IsAvailable = IsQuestAvailable(q)
                })
                .ToList();

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
                AllQuestsForDropdown = allQuests.ToDictionary(q => q.Id, q => q.Title)
            };

            return View(viewModel);
        }

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

                if (!string.IsNullOrWhiteSpace(request.Title))
                {
                    quest.Title = request.Title;
                }

                quest.Description = request.Description ?? string.Empty;
                quest.Type = request.Type;
                quest.Status = request.Status;

                if (request.Type == "individual" && request.CharacterId.HasValue)
                {
                    quest.CharacterId = request.CharacterId.Value;
                }
                else if (request.Type == "group")
                {
                    quest.CharacterId = null;
                }

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

        /// <summary>
        /// Kampf-Seite
        /// Route: /Spielmodus/Battle
        /// </summary>
        public async Task<IActionResult> Battle()
        {
            var isGod = User.IsInRole("Gott");

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

            var fieldEffects = await _context.FeldEffekte
                .Include(fe => fe.LightCard)
                .OrderBy(fe => fe.Name)
                .Select(fe => new FeldEffektOption
                {
                    Id = fe.Id,
                    Name = fe.Name,
                    Beschreibung = fe.Beschreibung,
                    ColorCode = fe.LightCard != null ? fe.LightCard.Farbcode : "#ffffff",
                    LightCardName = fe.LightCard != null ? fe.LightCard.Bezeichnung : "Unbekannt"
                })
                .ToListAsync();

            var viewModel = new BattleViewModel
            {
                Characters = characters,
                Monsters = monsters,
                AllFieldEffects = fieldEffects,
                IsGod = isGod
            };

            return View(viewModel);
        }

        /// <summary>
        /// Combat-Setup Seite
        /// Route: /Spielmodus/CombatSetup
        /// </summary>
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> CombatSetup()
        {
            var currentAct = await _context.Acts
                .FirstOrDefaultAsync(a => a.IsActive);

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

    public class QuestsViewModel
    {
        public List<QuestViewModel> Quests { get; set; } = new();
        public List<string> Characters { get; set; } = new();
        public Dictionary<int, string> CharactersForDropdown { get; set; } = new();
        public Dictionary<int, string> AllQuestsForDropdown { get; set; } = new();
        public string? FocusQuestId { get; set; }
        public bool IsGod { get; set; }
    }

    public class QuestViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? CharacterName { get; set; }
        public int? CharacterId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? PreviousQuestId { get; set; }
        public string? PreviousQuestTitle { get; set; }
        public int? MapMarkerId { get; set; }
        public bool IsAvailable { get; set; } = true;
    }

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
        public string Status { get; set; } = "none";
        public bool IsEquipped { get; set; }
        public int? EquippedPosition { get; set; }
        public bool HasBoughtTrophy { get; set; }
        public bool HasSlainTrophy { get; set; }
        public bool Meet { get; set; }
        public bool Encounter { get; set; }
        public bool Perfected { get; set; }
        public string? Basics { get; set; }
        public string? EncounterDescription { get; set; }
        public string? ProcessedDescription { get; set; }
        public bool HasBothVariants => HasBoughtTrophy && HasSlainTrophy;
    }
    public class MapViewModel
    {
        public ActViewModel? CurrentAct { get; set; }
        public MapViewModelData? CurrentMap { get; set; }
        public List<QuestOption> ActiveQuests { get; set; } = new();
        public bool IsGod { get; set; }
        public int? FocusMarkerId { get; set; }
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
        public int? ParentMapId { get; set; }
        public string? ParentMapName { get; set; }
        public bool IsWorldMap { get; set; }
        public List<MapRegionViewModel> Regions { get; set; } = new();
        public List<ChildMapViewModel> ChildMaps { get; set; } = new();
    }

    public class ChildMapViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class MarkerViewModel
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double XPercent { get; set; }
        public double YPercent { get; set; }
        public string? Description { get; set; }
        public int? QuestId { get; set; }
        public string? QuestTitle { get; set; }
        public string? QuestCharacterName { get; set; }
        public string? QuestType { get; set; }
        public int? LinkedMapId { get; set; }
        public string? LinkedMapName { get; set; }
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
        public List<FeldEffektOption> AllFieldEffects { get; set; } = new();
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

    public class FeldEffektOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Beschreibung { get; set; }
        public string ColorCode { get; set; } = string.Empty;
        public string LightCardName { get; set; } = string.Empty;
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

    public class MapRegionViewModel
    {
        public int Id { get; set; }
        public string RegionName { get; set; } = string.Empty;
        public string PolygonPoints { get; set; } = string.Empty;
        public int LinkedMapId { get; set; }
    }
}