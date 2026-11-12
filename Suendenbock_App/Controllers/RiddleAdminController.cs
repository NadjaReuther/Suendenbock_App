using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Models.ViewModels;

namespace Suendenbock_App.Controllers
{
    [Authorize(Roles = "Gott")]
    public class RiddleAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RiddleAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RiddleAdmin
        public async Task<IActionResult> Index()
        {
            var doors = await _context.RiddleDoors
                .OrderBy(d => d.DayNumber)
                .ToListAsync();
            return View(doors);
        }

        // GET: RiddleAdmin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RiddleAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RiddleDoor door)
        {
            if (ModelState.IsValid)
            {
                // Check if day number already exists
                var existingDoor = await _context.RiddleDoors
                    .FirstOrDefaultAsync(d => d.DayNumber == door.DayNumber);

                if (existingDoor != null)
                {
                    TempData["Error"] = $"Türchen {door.DayNumber} existiert bereits!";
                    return View(door);
                }

                // Automatisch ImagePath setzen basierend auf DayNumber
                door.ImagePath = $"/assets/raetsel/tueren/{door.DayNumber}.svg";

                _context.RiddleDoors.Add(door);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Türchen {door.DayNumber} wurde erfolgreich erstellt!";
                return RedirectToAction(nameof(Index));
            }
            return View(door);
        }

        // GET: RiddleAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var door = await _context.RiddleDoors.FindAsync(id);
            if (door == null)
            {
                return NotFound();
            }
            return View(door);
        }

        // POST: RiddleAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RiddleDoor door)
        {
            if (id != door.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Automatisch ImagePath setzen basierend auf DayNumber
                    door.ImagePath = $"/assets/raetsel/tueren/{door.DayNumber}.svg";

                    _context.Update(door);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Türchen {door.DayNumber} wurde erfolgreich aktualisiert!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoorExists(door.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(door);
        }

        // POST: RiddleAdmin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var door = await _context.RiddleDoors
                .Include(d => d.UserAttempts)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (door == null)
            {
                TempData["Error"] = "Türchen nicht gefunden!";
                return RedirectToAction(nameof(Index));
            }

            // Delete related attempts first
            _context.UserRiddleAttempts.RemoveRange(door.UserAttempts);
            _context.RiddleDoors.Remove(door);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Türchen {door.DayNumber} wurde erfolgreich gelöscht!";
            return RedirectToAction(nameof(Index));
        }

        // GET: RiddleAdmin/Statistics
        public async Task<IActionResult> Statistics()
        {
            var doors = await _context.RiddleDoors
                .Include(d => d.UserAttempts)
                .OrderBy(d => d.DayNumber)
                .ToListAsync();

            // Hole User-Informationen für die Statistik
            var allUserIds = doors.SelectMany(d => d.UserAttempts).Select(a => a.UserId).Distinct();

            var users = await _context.Users
                .Where(u => allUserIds.Contains(u.Id))
                .Include(u => u.CurrentCharacter)
                .ToDictionaryAsync(u => u.Id);

            ViewBag.Users = users;

            return View(doors);
        }

        // POST: RiddleAdmin/ResetAllProgress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAllProgress()
        {
            var allAttempts = await _context.UserRiddleAttempts.ToListAsync();
            _context.UserRiddleAttempts.RemoveRange(allAttempts);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Alle Spieler-Fortschritte wurden zurückgesetzt! ({allAttempts.Count} Einträge gelöscht)";
            return RedirectToAction(nameof(Index));
        }

        private bool DoorExists(int id)
        {
            return _context.RiddleDoors.Any(e => e.Id == id);
        }
    }
}
