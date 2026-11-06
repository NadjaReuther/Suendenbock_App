using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Controllers
{
    [Authorize(Roles = "Gott")]
    public class GameWorldController : BaseController
    {
        public GameWorldController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: GameWorld
        public async Task<IActionResult> Index()
        {
            var gameWorlds = await _context.GameWorlds
                .Include(gw => gw.CharacterGameWorlds)
                    .ThenInclude(cgw => cgw.Character)
                .OrderBy(gw => gw.SortOrder)
                .ToListAsync();

            return View(gameWorlds);
        }

        // GET: GameWorld/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GameWorld/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GameWorld gameWorld)
        {
            if (ModelState.IsValid)
            {
                gameWorld.CreatedAt = DateTime.UtcNow;
                _context.GameWorlds.Add(gameWorld);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Spielwelt '{gameWorld.Name}' wurde erfolgreich erstellt.";
                return RedirectToAction(nameof(Index));
            }
            return View(gameWorld);
        }

        // GET: GameWorld/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var gameWorld = await _context.GameWorlds.FindAsync(id);
            if (gameWorld == null)
                return NotFound();

            return View(gameWorld);
        }

        // POST: GameWorld/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GameWorld gameWorld)
        {
            if (id != gameWorld.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gameWorld);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Spielwelt '{gameWorld.Name}' wurde erfolgreich aktualisiert.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameWorldExists(gameWorld.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gameWorld);
        }

        // GET: GameWorld/ManageCharacters/5
        public async Task<IActionResult> ManageCharacters(int? id)
        {
            if (id == null)
                return NotFound();

            var gameWorld = await _context.GameWorlds
                .Include(gw => gw.CharacterGameWorlds)
                    .ThenInclude(cgw => cgw.Character)
                .FirstOrDefaultAsync(gw => gw.Id == id);

            if (gameWorld == null)
                return NotFound();

            // Alle Characters die NICHT in dieser Welt sind
            var assignedCharacterIds = gameWorld.CharacterGameWorlds.Select(cgw => cgw.CharacterId).ToList();
            var availableCharacters = await _context.Characters
                .Where(c => !assignedCharacterIds.Contains(c.Id))
                .OrderBy(c => c.Nachname)
                .ThenBy(c => c.Vorname)
                .ToListAsync();

            ViewBag.GameWorld = gameWorld;
            ViewBag.AvailableCharacters = availableCharacters;

            return View(gameWorld.CharacterGameWorlds.OrderBy(cgw => cgw.Character.Nachname).ToList());
        }

        // POST: GameWorld/AddCharacter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCharacter(int gameWorldId, int characterId)
        {
            // Prüfe ob bereits zugeordnet
            var existingMapping = await _context.CharacterGameWorlds
                .FirstOrDefaultAsync(cgw => cgw.GameWorldId == gameWorldId && cgw.CharacterId == characterId);

            if (existingMapping != null)
            {
                TempData["Error"] = "Character ist bereits dieser Spielwelt zugeordnet.";
                return RedirectToAction(nameof(ManageCharacters), new { id = gameWorldId });
            }

            var characterGameWorld = new CharacterGameWorld
            {
                GameWorldId = gameWorldId,
                CharacterId = characterId,
                AddedAt = DateTime.UtcNow
            };

            _context.CharacterGameWorlds.Add(characterGameWorld);
            await _context.SaveChangesAsync();

            var character = await _context.Characters.FindAsync(characterId);
            TempData["Success"] = $"Character '{character?.Vorname} {character?.Nachname}' wurde hinzugefügt.";

            return RedirectToAction(nameof(ManageCharacters), new { id = gameWorldId });
        }

        // POST: GameWorld/RemoveCharacter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCharacter(int id)
        {
            var characterGameWorld = await _context.CharacterGameWorlds
                .Include(cgw => cgw.Character)
                .FirstOrDefaultAsync(cgw => cgw.Id == id);

            if (characterGameWorld == null)
                return NotFound();

            var gameWorldId = characterGameWorld.GameWorldId;
            var characterName = $"{characterGameWorld.Character.Vorname} {characterGameWorld.Character.Nachname}";

            _context.CharacterGameWorlds.Remove(characterGameWorld);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Character '{characterName}' wurde aus der Spielwelt entfernt.";
            return RedirectToAction(nameof(ManageCharacters), new { id = gameWorldId });
        }

        // POST: GameWorld/RemoveCharacters
        // Mehrere Zuordnungen auf einmal entfernen (Checkboxen in ManageCharacters)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCharacters(int gameWorldId, List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                TempData["Error"] = "Keine Characters ausgewählt.";
                return RedirectToAction(nameof(ManageCharacters), new { id = gameWorldId });
            }

            // Nur Zuordnungen dieser Spielwelt, falls jemand fremde Ids mitschickt
            var zuordnungen = await _context.CharacterGameWorlds
                .Where(cgw => cgw.GameWorldId == gameWorldId && ids.Contains(cgw.Id))
                .ToListAsync();

            _context.CharacterGameWorlds.RemoveRange(zuordnungen);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{zuordnungen.Count} Character(s) wurden aus der Spielwelt entfernt.";
            return RedirectToAction(nameof(ManageCharacters), new { id = gameWorldId });
        }

        // POST: GameWorld/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gameWorld = await _context.GameWorlds.FindAsync(id);
            if (gameWorld != null)
            {
                // Prüfe ob Characters zugeordnet sind
                var hasCharacters = await _context.CharacterGameWorlds.AnyAsync(cgw => cgw.GameWorldId == id);
                if (hasCharacters)
                {
                    TempData["Error"] = "Spielwelt kann nicht gelöscht werden, da noch Characters zugeordnet sind.";
                    return RedirectToAction(nameof(Index));
                }

                _context.GameWorlds.Remove(gameWorld);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Spielwelt '{gameWorld.Name}' wurde gelöscht.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool GameWorldExists(int id)
        {
            return _context.GameWorlds.Any(e => e.Id == id);
        }
    }
}
