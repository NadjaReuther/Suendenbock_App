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

        /// <summary>
        /// Löscht einen Benutzer - funktioniert auch bei Benutzern ohne E-Mail/Namen
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "Benutzer nicht gefunden.";
                    return RedirectToAction("Index");
                }

                var displayName = !string.IsNullOrEmpty(user.Email) ? user.Email :
                                 !string.IsNullOrEmpty(user.UserName) ? user.UserName :
                                 $"Benutzer (ID: {id.Substring(0, 8)}...)";

                // Setze UserId bei allen Characters auf NULL (verwaist sie)
                var characters = await _context.Characters.Where(c => c.UserId == id).ToListAsync();
                if (characters.Any())
                {
                    foreach (var character in characters)
                    {
                        character.UserId = null;
                    }
                    await _context.SaveChangesAsync();
                }

                // Lösche alle UserAchievements des Users
                var userAchievements = await _context.UserAchievements.Where(ua => ua.UserId == id).ToListAsync();
                if (userAchievements.Any())
                {
                    _context.UserAchievements.RemoveRange(userAchievements);
                }

                // Lösche alle TriggerPreferences des Users
                var triggerPrefs = await _context.UserTriggerPreferences.Where(tp => tp.UserId == id).ToListAsync();
                if (triggerPrefs.Any())
                {
                    _context.UserTriggerPreferences.RemoveRange(triggerPrefs);
                }

                // Lösche alle UserAdventChoices des Users
                var adventChoices = await _context.UserAdventChoices.Where(uac => uac.UserId == id).ToListAsync();
                if (adventChoices.Any())
                {
                    _context.UserAdventChoices.RemoveRange(adventChoices);
                }

                // Lösche alle PollVotes des Users
                var pollVotes = await _context.PollVotes.Where(pv => pv.UserId == id).ToListAsync();
                if (pollVotes.Any())
                {
                    _context.PollVotes.RemoveRange(pollVotes);
                }

                // Lösche alle EventRSVPs des Users
                var eventRsvps = await _context.EventRSVPs.Where(er => er.UserId == id).ToListAsync();
                if (eventRsvps.Any())
                {
                    _context.EventRSVPs.RemoveRange(eventRsvps);
                }

                // Setze CreatedByUserId bei MonthlyEvents auf NULL
                // WICHTIG: Nutze direkt SQL, da die Datenbank möglicherweise noch CreatedById statt CreatedByUserId hat
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE MonthlyEvents SET CreatedById = NULL WHERE CreatedById = {0}", id);

                // Falls der Code CreatedByUserId verwendet:
                var monthlyEvents = await _context.MonthlyEvents.Where(me => me.CreatedByUserId == id).ToListAsync();
                if (monthlyEvents.Any())
                {
                    foreach (var evt in monthlyEvents)
                    {
                        evt.CreatedByUserId = null;
                    }
                }

                // Setze AuthorUserId bei ForumThreads auf NULL
                var forumThreads = await _context.ForumThreads.Where(ft => ft.AuthorUserId == id).ToListAsync();
                if (forumThreads.Any())
                {
                    foreach (var thread in forumThreads)
                    {
                        thread.AuthorUserId = null;
                    }
                }

                // Setze AuthorUserId bei ForumReplies auf NULL
                var forumReplies = await _context.forumReplies.Where(fr => fr.AuthorUserId == id).ToListAsync();
                if (forumReplies.Any())
                {
                    foreach (var reply in forumReplies)
                    {
                        reply.AuthorUserId = null;
                    }
                }

                // Setze ReporterUserId und ResolvedByUserId bei Tickets auf NULL
                var ticketsReporter = await _context.Tickets.Where(t => t.ReporterUserId == id).ToListAsync();
                if (ticketsReporter.Any())
                {
                    foreach (var ticket in ticketsReporter)
                    {
                        ticket.ReporterUserId = null;
                    }
                }

                var ticketsResolver = await _context.Tickets.Where(t => t.ResolvedByUserId == id).ToListAsync();
                if (ticketsResolver.Any())
                {
                    foreach (var ticket in ticketsResolver)
                    {
                        ticket.ResolvedByUserId = null;
                    }
                }

                // NewsComment hat keinen UserId - ignorieren

                // Speichere alle Änderungen
                await _context.SaveChangesAsync();

                // Lösche den Benutzer
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = $"Benutzer '{displayName}' wurde erfolgreich gelöscht.";
                }
                else
                {
                    TempData["Error"] = $"Fehler beim Löschen: {string.Join(", ", result.Errors.Select(e => e.Description))}";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Löschen: {ex.Message} | InnerException: {ex.InnerException?.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
