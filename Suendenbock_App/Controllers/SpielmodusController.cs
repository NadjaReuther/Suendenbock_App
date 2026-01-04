// Controllers/SpielmodusController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models;

namespace Suendenbock_App.Controllers
{
    [Authorize] // Nur eingeloggte Benutzer können auf Spielmodus zugreifen
    public class SpielmodusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SpielmodusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===== DASHBOARD =====

        /// <summary>
        /// Haupt-Dashboard des Spielmodus
        /// Route: /Spielmodus/Dashboard
        /// </summary>
        public async Task<IActionResult> Dashboard()
        {
            var isGod = User.IsInRole("Gott");

            // Lade alle aktiven Spieler-Characters
            // Filter: nur Spieler-Characters (UserId != null)
            var characters = await _context.Characters
                .Where(c => c.UserId != null || c.IsCompanion)
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

            // Lade aktuellen Act
            var currentAct = await _context.Acts
                .Include(a => a.Map)
                .FirstOrDefaultAsync(a => a.IsActive);

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

            return View(viewModel);
        }

        // ===== WEITERE VIEWS (kommen später) =====

        /// <summary>
        /// Map-Seite
        /// Route: /Spielmodus/Map
        ///
        /// Zeigt immer den aktiven Act (IsActive = true) an
        /// </summary>
        public async Task<IActionResult> Map()
        {
            var isGod = User.IsInRole("Gott");

            // Aktuellen/aktiven Act laden
            var currentAct = await _context.Acts
                .Include(a => a.Map)
                    .ThenInclude(m => m.Markers)
                .FirstOrDefaultAsync(a => a.IsActive);

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
                CurrentMap = currentAct?.Map != null ? new MapViewModelData
                {
                    Id = currentAct.Map.Id,
                    Name = currentAct.Map.Name,
                    ImageUrl = currentAct.Map.ImageUrl,
                    Markers = currentAct.Map.Markers.Select(m => new MarkerViewModel
                    {
                        Id = m.Id,
                        Label = m.Label,
                        Type = m.Type,
                        XPercent = m.XPercent,
                        YPercent = m.YPercent,
                        Description = m.Description
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
        /// </summary>
        public async Task<IActionResult> Quests(string? focusQuestId = null)
        {
            var isGod = User.IsInRole("Gott");

            // Aktuellen Act laden
            var currentAct = await _context.Acts
                .FirstOrDefaultAsync(a => a.IsActive);

            if (currentAct == null)
            {
                // Kein aktiver Act -> keine Quests anzeigen
                return View(new QuestsViewModel
                {
                    Quests = new List<QuestViewModel>(),
                    Characters = new List<string>(),
                    FocusQuestId = focusQuestId,
                    IsGod = isGod
                });
            }

            // Alle Quests DES AKTUELLEN ACTS laden (mit Character + MapMarker)
            var quests = await _context.Quests
                .Include(q => q.Character)
                .Include(q => q.MapMarker)
                .Where(q => q.ActId == currentAct.Id)
                .OrderByDescending(q => q.CreatedAt)
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
                    CompletedAt = q.CompletedAt
                })
                .ToListAsync();

            // Alle aktiven Spieler-Characters für Filter + Dropdown
            var characters = await _context.Characters
                 .Where(c => c.UserId != null || c.IsCompanion)
                .Select(c => new { c.Id, FullName = $"{c.Vorname}" })
                .ToListAsync();

            var viewModel = new QuestsViewModel
            {
                Quests = quests,
                Characters = characters.Select(c => c.FullName).ToList(),
                CharactersForDropdown = characters.ToDictionary(c => c.Id, c => c.FullName),
                FocusQuestId = focusQuestId,
                IsGod = isGod
            };

            return View(viewModel);
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
            // 1. Begegnet wurden (meet = true) UND
            // 2. Mindestens eine Trophäe verfügbar haben (BoughtTrophyAvailable ODER SlainTrophyAvailable) UND
            // 3. Einen Status haben (nicht "none" = noch nicht erworben)
            var monsters = await _context.Monsters
                .Include(m => m.Monstertyp)
                .Where(m => m.meet == true &&
                           (m.BoughtTrophyAvailable || m.SlainTrophyAvailable) &&
                           m.Status != "none")
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
                    BoughtTrophyAvailable = m.BoughtTrophyAvailable,
                    SlainTrophyAvailable = m.SlainTrophyAvailable
                })
                .ToListAsync();

            // Aufteilen: Equipped vs Inventory
            var viewModel = new TrophiesViewModel
            {
                EquippedTrophies = monsters.Where(m => m.IsEquipped).ToList(),
                InventoryTrophies = monsters.Where(m => !m.IsEquipped).ToList()
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
            // Alle Characters (für Auswahl)
            var characters = await _context.Characters
                .OrderBy(c => c.Nachname)
                .Select(c => new BattleCharacterOption
                {
                    Id = c.Id,
                    Name = c.Nachname,
                    CurrentHealth = c.CurrentHealth,
                    MaxHealth = c.BaseMaxHealth,
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
                    MonstertypName = m.Monstertyp.Name,
                    ImageUrl = m.ImagePath ?? "/images/monsters/default.png"
                })
                .ToListAsync();

            var viewModel = new BattleViewModel
            {
                Characters = characters,
                Monsters = monsters
            };

            return View(viewModel);
        }
        /// <summary>
        /// Combat-Setup Seite
        /// Route: /Spielmodus/CombatSetup
        /// 
        /// Gott wählt:
        /// 1. Monster pro Monstertyp (mehrere möglich via Dropdown)
        /// 2. Für jedes Monster: Health + Pokus eingeben
        /// 3. Characters (Spieler + Begleiter)
        /// 4. Setup speichern
        /// </summary>
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> CombatSetup()
        {
            // Alle Monstertypen mit ihren Monstern
            var monstertypen = await _context.MonsterTypes
                .Include(mt => mt.Monster)
                .OrderBy(mt => mt.Name)
                .Select(mt => new MonstertypViewModel
                {
                    Id = mt.Id,
                    Name = mt.Name,
                    Monsters = mt.Monster
                        .OrderBy(m => m.Name)
                        .Select(m => new MonsterOptionViewModel
                        {
                            Id = m.Id,
                            Name = m.Name,
                            ImagePath = m.ImagePath
                        })
                        .ToList()
                })
                .ToListAsync();

            // Alle Spieler-Characters + Begleiter (UserId != null ODER IsCompanion = true)
            var characters = await _context.Characters
                .Where(c => c.UserId != null || c.IsCompanion) // Spieler ODER Begleiter
                .OrderBy(c => c.Nachname)
                .Select(c => new CharacterOptionViewModel
                {
                    Id = c.Id,
                    Name = $"{c.Vorname} {c.Nachname}",
                    Nachname = c.Nachname,
                    CurrentHealth = c.CurrentHealth,
                    MaxHealth = c.BaseMaxHealth,
                    IsBegleiter = c.IsCompanion
                })
                .ToListAsync();

            var viewModel = new CombatSetupViewModel
            {
                Monstertypen = monstertypen,
                Characters = characters
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
        public string Status { get; set; } = "none"; // "none", "bought", "slain", "both"
        public bool IsEquipped { get; set; }
        public bool BoughtTrophyAvailable { get; set; }
        public bool SlainTrophyAvailable { get; set; }
    }
    public class MapViewModel
    {
        public ActViewModel? CurrentAct { get; set; }
        public MapViewModelData? CurrentMap { get; set; }
        public List<QuestOption> ActiveQuests { get; set; } = new();
        public bool IsGod { get; set; }
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
    }

    public class BattleCharacterOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
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
        public List<MonstertypViewModel> Monstertypen { get; set; } = new();
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
    }

    public class CharacterOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Nachname { get; set; } = string.Empty;
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public bool IsBegleiter { get; set; }
    }
}