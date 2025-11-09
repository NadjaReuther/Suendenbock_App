using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Controllers
{
    [Authorize(Roles = "Spieler")]
    public class PlayerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlayerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Zeigt nur die Character die dem User zugeordnet sind
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var character = await _context.Characters
                .Where(c => c.UserId == userId)
                .Include(c => c.CharacterMagicClasses)
                    .ThenInclude(cm => cm.MagicClass)
                    .ThenInclude(o => o.Obermagie)
                    .ThenInclude(l => l.LightCard)
                .FirstOrDefaultAsync();

            return View(character);
        }

        /// <summary>
        /// Übersichtsseite "Meine Daten bearbeiten"
        /// </summary>
        [HttpGet]
        public IActionResult MeineDaten()
        {
            return View();
        }

        /// <summary>
        /// Zeigt Formular nur für Geburtstag
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Geburtstag()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        /// <summary>
        /// Speichert nur den Geburtstag
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Geburtstag(DateTime? birthday)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            user.Birthday = birthday;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Dein Geburtstag wurde erfolgreich gespeichert!";
            return RedirectToAction("MeineDaten");
        }
        // ===============================
        // TRIGGERPUNKTE VERWALTUNG
        // ===============================

        [HttpGet]
        public async Task<IActionResult> Triggerpunkte()
        {
            var userId = _userManager.GetUserId(User);

            var categories = await _context.TriggerCategories
                .Include(c => c.Topics)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            var userPreferences = await _context.UserTriggerPreferences
                .Where(p => p.UserId == userId)
                .ToDictionaryAsync(p => p.TopicId, p => p.Preference);

            var user = await _userManager.GetUserAsync(User);

            ViewBag.Categories = categories;
            ViewBag.UserPreferences = userPreferences;
            ViewBag.Birthday = user?.Birthday;
            ViewBag.CustomTrigger = user?.CustomTrigger;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveTriggerpunkte(
            DateTime? birthday,
            string? customTrigger,
            Dictionary<int, string>? preferences)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            user.Birthday = birthday;
            user.CustomTrigger = customTrigger;
            await _userManager.UpdateAsync(user);

            if (preferences != null)
            {
                foreach (var kvp in preferences)
                {
                    int topicId = kvp.Key;
                    string preferenceValue = kvp.Value;

                    TriggerPreferenceLevel? preferenceEnum = null;
                    if (!string.IsNullOrEmpty(preferenceValue))
                    {
                        preferenceEnum = Enum.Parse<TriggerPreferenceLevel>(preferenceValue);
                    }

                    var existing = await _context.UserTriggerPreferences
                        .FirstOrDefaultAsync(p => p.UserId == userId && p.TopicId == topicId);

                    if (existing != null)
                    {
                        existing.Preference = preferenceEnum;
                        existing.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        _context.UserTriggerPreferences.Add(new UserTriggerPreference
                        {
                            UserId = userId,
                            TopicId = topicId,
                            Preference = preferenceEnum,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Deine Triggerpunkte wurden erfolgreich gespeichert!";
            return RedirectToAction("Triggerpunkte");
        }
        public IActionResult Weihnachtenuebersicht()
        {
            return View();
        }
        public IActionResult Anleitung()
        {
            return View();
        }
        public IActionResult Weihnachtsabenteuer()
        {
            return View();
        }
    }
}