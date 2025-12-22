// Controllers/SpielmodusController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models;

namespace Suendenbock_App.Controllers
{
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

            var viewModel = new SpielmodusDashboardViewModel
            {
                ActiveCharacters = characters,
                RestFoods = restFoods
            };

            return View(viewModel);
        }

        // ===== WEITERE VIEWS (kommen später) =====

        /// <summary>
        /// Karten-Ansicht
        /// Route: /Spielmodus/Map
        /// </summary>
        public async Task<IActionResult> Map()
        {
            // TODO: Implementieren
            return View();
        }


        // ===== QUESTS =====

        /// <summary>
        /// Quest-Übersicht Seite
        /// Route: /Spielmodus/Quests
        /// </summary>
        public async Task<IActionResult> Quests(string? focusQuestId = null)
        {
            // Alle Quests laden (mit Character + MapMarker)
            var quests = await _context.Quests
                .Include(q => q.Character)
                .Include(q => q.MapMarker)
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
                FocusQuestId = focusQuestId
            };

            return View(viewModel);
        }

        /// <summary>
        /// Trophäen-Übersicht
        /// Route: /Spielmodus/Trophies
        /// </summary>
        public async Task<IActionResult> Trophies()
        {
            // TODO: Implementieren
            return View();
        }

        /// <summary>
        /// Kampf-Ansicht
        /// Route: /Spielmodus/Battle
        /// </summary>
        public IActionResult Battle()
        {
            // TODO: Implementieren (läuft im Browser mit LocalStorage)
            return View();
        }
    }

    // ===== VIEW MODELS =====

    public class SpielmodusDashboardViewModel
    {
        public List<CharacterViewModel> ActiveCharacters { get; set; } = new();
        public List<RestFoodViewModel> RestFoods { get; set; } = new();
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
}