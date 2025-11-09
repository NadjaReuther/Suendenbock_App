using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Controllers
{
    [Authorize(Roles = "Gott")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserManagementController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Include(u => u.TriggerPreferences)
                .ToListAsync();

            var userList = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // Anzahl der Characters für diesen User zählen
                var characterCount = await _context.Characters
                    .Where(c => c.UserId == user.Id)
                    .CountAsync();

                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "keine",
                    CharacterCount = characterCount,
                    Birthday = user.Birthday,
                    CustomTrigger = user.CustomTrigger,
                    TriggerPreferencesCount = user.TriggerPreferences.Count(p => p.Preference != null)
                });
            }

            return View(userList);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(string email, string role, string initialPassword)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                TempData["Error"] = "Ein Benutzer mit dieser E-Mail-Adresse existiert bereits.";
                return View();
            }

            var user = new ApplicationUser { 
                UserName = email, 
                Email = email 
            };

            var result = await _userManager.CreateAsync(user, initialPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Fehler beim Erstellen : " + string.Join(", ", result.Errors.Select(e => e.Description));
        
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            ViewBag.UserEmail = user.Email;
            ViewBag.UserId = user.Id;
            
            return View();
        }

        public async Task<IActionResult> ResetPassword(string id, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                TempData["Success"] = "Passwort für {user.Email} erfolgreich zurückgesetzt.";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Fehler: " + string.Join(", ", result.Errors.Select(e => e.Description));
            return View();
        }

        // ===============================
        // TRIGGERPUNKTE VERWALTUNG
        // ===============================

        /// <summary>
        /// Zeigt eine Übersicht aller Spieler mit ihren Triggerpunkten (nur für Spielleiter)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TriggerOverview()
        {
            var users = await _context.Users
                .Include(u => u.TriggerPreferences)
                    .ThenInclude(p => p.Topic)
                        .ThenInclude(t => t.Category)
                .OrderBy(u => u.Email)
                .ToListAsync();

            // Gesamtanzahl der Topics für Fortschrittsberechnung
            var totalTopicsCount = await _context.TriggerTopics.CountAsync();
            ViewBag.TotalTopicsCount = totalTopicsCount;

            return View(users);
        }

        /// <summary>
        /// Zeigt die Triggerpunkte eines bestimmten Spielers im Detail
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TriggerDetails(string id)
        {
            var user = await _context.Users
                .Include(u => u.TriggerPreferences)
                    .ThenInclude(p => p.Topic)
                        .ThenInclude(t => t.Category)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            // Alle Kategorien mit Topics laden
            var categories = await _context.TriggerCategories
                .Include(c => c.Topics)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            ViewBag.Categories = categories;

            return View(user);
        }
    }
}
