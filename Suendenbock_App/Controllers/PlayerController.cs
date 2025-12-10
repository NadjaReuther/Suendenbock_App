using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Models.ViewModels;
using Suendenbock_App.Services;

namespace Suendenbock_App.Controllers
{
    [Authorize(Roles = "Spieler")]
    public class PlayerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAchievementService _achievementService;

        /// <summary>
        /// Konstruktor ruft die DbContext- und UserManager-Instanzen ab
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="achievementService"></param>
        public PlayerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IAchievementService achievementService)
        {
            _context = context;
            _userManager = userManager;
            _achievementService = achievementService;
        }

        /// <summary>
        /// Displays the main view for the current user's character.
        /// </summary>
        /// <remarks>This method retrieves the character associated with the currently logged-in user,
        /// including related magic classes and their associated details, and returns a view displaying this
        /// information.</remarks>
        /// <returns>An <see cref="IActionResult"/> that renders the view for the user's character.</returns>
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

            // Adventskalender-Statistik laden
            var userChoices = await _context.UserAdventChoices
                .Where(c => c.UserId == userId)
                .Include(c => c.AdventDoor)
                .OrderBy(c => c.AdventDoor.DayNumber)
                .ToListAsync();

            var adventStats = new
            {
                TotalOpened = userChoices.Count,
                EmmaChoices = userChoices.Count(c => c.ChoiceIndex == 0),
                KasimirChoices = userChoices.Count(c => c.ChoiceIndex == 1),
                Doors = userChoices.Select(c => new
                {
                    DayNumber = c.AdventDoor.DayNumber,
                    DoorType = c.AdventDoor.DoorType,
                    OpenedAt = c.ChosenAt,
                    Choice = c.ChoiceIndex.HasValue ? (c.ChoiceIndex == 0 ? "Emma" : "Kasimir") : null
                }).ToList()
            };

            ViewBag.AdventStats = adventStats;

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

        /// <summary>
        /// Zeigt die Achievements-Übersicht des Spielers an
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Achievements()
        {
            var userId = _userManager.GetUserId(User);

            // Hole alle User-Achievements des Spielers
            var userAchievements = await _achievementService.GetUserAchievements(userId);

            // Hole alle verfügbaren Achievements
            var allAchievements = await _context.Achievements
                .Where(a => a.Scope == AchievementScope.User)
                .OrderBy(a => a.Category)
                .ThenBy(a => a.Points)
                .ToListAsync();

            // Berechne Statistiken
            var unlockedCount = userAchievements.Count;
            var totalCount = allAchievements.Count(a => !a.IsSecret);
            var totalPoints = userAchievements.Sum(ua => ua.Achievement.Points);
            var maxPoints = allAchievements.Where(a => !a.IsSecret).Sum(a => a.Points);

            // Gruppiere nach Kategorien
            var achievementsByCategory = allAchievements
                .GroupBy(a => a.Category)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(a => new AchievementItem
                    {
                        Achievement = a,
                        IsUnlocked = userAchievements.Any(ua => ua.AchievementId == a.Id),
                        UnlockedAt = userAchievements.FirstOrDefault(ua => ua.AchievementId == a.Id)?.UnlockedAt
                    }).ToList()
                );

            var viewModel = new PlayerAchievementsViewModel
            {
                UnlockedCount = unlockedCount,
                TotalCount = totalCount,
                TotalPoints = totalPoints,
                MaxPoints = maxPoints,
                AchievementsByCategory = achievementsByCategory
            };

            return View(viewModel);
        }

        [AllowAnonymous]
        [Authorize]
        public async Task<IActionResult> Weihnachtsabenteuer()
        {
            // Prüfen ob User in Spieler oder Gott Rolle ist
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var isSpieler = await _userManager.IsInRoleAsync(user, "Spieler");
            var isGott = await _userManager.IsInRoleAsync(user, "Gott");

            if (!isSpieler && !isGott)
            {
                return Forbid();
            }

            return View();
        }

        [AllowAnonymous]
        [Authorize]
        public async Task<IActionResult> WeihnachtstuerDetail(int day)
        {
            // Prüfen ob User in Spieler oder Gott Rolle ist
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var isSpieler = await _userManager.IsInRoleAsync(user, "Spieler");
            var isGott = await _userManager.IsInRoleAsync(user, "Gott");

            if (!isSpieler && !isGott)
            {
                return Forbid();
            }

            ViewBag.Day = day;
            ViewBag.IsGott = isGott;

            return View();
        }
    }
}