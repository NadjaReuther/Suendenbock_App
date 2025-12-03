using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Services;

namespace Suendenbock_App.Controllers
{
    [Authorize]
    public class GlossaryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageUploadService _imageUploadService;

        public GlossaryController(ApplicationDbContext context, IImageUploadService imageUploadService)
        {
            _context = context;
            _imageUploadService = imageUploadService;
        }

        // GET: Glossary/Browse - Öffentliche Übersicht
        [AllowAnonymous]
        public async Task<IActionResult> Browse(string searchTerm = "", string entityType = "")
        {
            var query = _context.GlossaryEntries.AsQueryable();

            // Suchfilter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(g => g.Title.ToLower().Contains(searchTerm.ToLower()) ||
                                        g.Description.ToLower().Contains(searchTerm.ToLower()));
            }

            // Entity-Type Filter
            if (!string.IsNullOrWhiteSpace(entityType))
            {
                query = query.Where(g => g.EntityType == entityType);
            }

            var entries = await query.OrderBy(g => g.Title).ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.EntityType = entityType;
            ViewBag.EntityTypes = GetAvailableEntityTypes();

            return View(entries);
        }

        // GET: Glossary/Index - Verwaltung (nur Gott)
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> Index(string searchTerm = "", string entityType = "")
        {
            var query = _context.GlossaryEntries.AsQueryable();

            // Suchfilter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(g => g.Title.ToLower().Contains(searchTerm.ToLower()) ||
                                        g.Description.ToLower().Contains(searchTerm.ToLower()));
            }

            // Entity-Type Filter
            if (!string.IsNullOrWhiteSpace(entityType))
            {
                query = query.Where(g => g.EntityType == entityType);
            }

            var entries = await query.OrderBy(g => g.Title).ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.EntityType = entityType;
            ViewBag.EntityTypes = GetAvailableEntityTypes();

            return View(entries);
        }

        // GET: Glossary/SearchEntities - Suche nach Entitäten (für AJAX, nur Gott)
        [HttpGet]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> SearchEntities(string entityType, string searchTerm = "")
        {
            if (string.IsNullOrWhiteSpace(entityType))
            {
                return Json(new List<object>());
            }

            var results = new List<object>();

            switch (entityType)
            {
                case "Rasse":
                    var rassen = await _context.Rassen
                        .Where(r => string.IsNullOrEmpty(searchTerm) || r.Name.ToLower().Contains(searchTerm.ToLower()))
                        .Select(r => new { r.Id, r.Name })
                        .ToListAsync();
                    results = rassen.Cast<object>().ToList();
                    break;

                case "MagicClass":
                    var magicClasses = await _context.MagicClasses
                        .Include(mc => mc.Obermagie)
                        .Where(mc => string.IsNullOrEmpty(searchTerm) || mc.Bezeichnung.ToLower().Contains(searchTerm.ToLower()))
                        .Select(mc => new { mc.Id, Name = mc.Bezeichnung + " (" + mc.Obermagie.Bezeichnung + ")" })
                        .ToListAsync();
                    results = magicClasses.Cast<object>().ToList();
                    break;

                case "Obermagie":
                    var obermagien = await _context.Obermagien
                        .Where(o => string.IsNullOrEmpty(searchTerm) || o.Bezeichnung.ToLower().Contains(searchTerm.ToLower()))
                        .Select(o => new { o.Id, Name = o.Bezeichnung })
                        .ToListAsync();
                    results = obermagien.Cast<object>().ToList();
                    break;

                case "Blutgruppe":
                    var blutgruppen = await _context.Blutgruppen
                        .Where(b => string.IsNullOrEmpty(searchTerm) || b.Name.ToLower().Contains(searchTerm.ToLower()))
                        .Select(b => new { b.Id, b.Name })
                        .ToListAsync();
                    results = blutgruppen.Cast<object>().ToList();
                    break;

                case "Haus":
                    var haeuser = await _context.Haeuser
                        .Where(h => string.IsNullOrEmpty(searchTerm) || h.Name.ToLower().Contains(searchTerm.ToLower()))
                        .Select(h => new { h.Id, h.Name })
                        .ToListAsync();
                    results = haeuser.Cast<object>().ToList();
                    break;

                case "Herkunftsland":
                    var laender = await _context.Herkunftslaender
                        .Where(h => string.IsNullOrEmpty(searchTerm) || h.Name.ToLower().Contains(searchTerm.ToLower()))
                        .Select(h => new { h.Id, h.Name })
                        .ToListAsync();
                    results = laender.Cast<object>().ToList();
                    break;

                case "Religion":
                    var religions = await _context.Religions
                        .Where(r => string.IsNullOrEmpty(searchTerm) || r.Type.ToLower().Contains(searchTerm.ToLower()))
                        .Select(r => new { r.Id, Name = r.Type })
                        .ToListAsync();
                    results = religions.Cast<object>().ToList();
                    break;

                case "Infanterierang":
                    var raenge = await _context.Infanterieraenge
                        .Where(i => string.IsNullOrEmpty(searchTerm) || i.Name.ToLower().Contains(searchTerm.ToLower()))
                        .Select(i => new { i.Id, i.Name })
                        .ToListAsync();
                    results = raenge.Cast<object>().ToList();
                    break;

                case "Stand":
                    var staende = await _context.Staende
                        .Where(s => string.IsNullOrEmpty(searchTerm) || s.Name.ToLower().Contains(searchTerm.ToLower()))
                        .Select(s => new { s.Id, s.Name })
                        .ToListAsync();
                    results = staende.Cast<object>().ToList();
                    break;
            }

            return Json(results);
        }

        // GET: Glossary/Create (nur Gott)
        [Authorize(Roles = "Gott")]
        public IActionResult Create(string? entityType = null, int? entityId = null)
        {
            ViewBag.EntityTypes = GetAvailableEntityTypes();
            ViewBag.SelectedEntityType = entityType;
            ViewBag.SelectedEntityId = entityId;

            return View(new GlossaryEntry());
        }

        // POST: Glossary/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GlossaryEntry entry, IFormFile? entryImage)
        {
            try
            {
                // Wenn EntityType und EntityId gesetzt sind, lade den Namen der Entität
                if (!string.IsNullOrEmpty(entry.EntityType) && entry.EntityId.HasValue)
                {
                    entry.Title = await GetEntityNameAsync(entry.EntityType, entry.EntityId.Value);
                }

                // Bild hochladen, falls vorhanden
                if (entryImage != null && entryImage.Length > 0)
                {
                    try
                    {
                        var uploadedImagePath = await _imageUploadService.UploadImageAsync(entryImage, "glossary", $"{entry.Title}_{DateTime.Now:yyyyMMddHHmmss}");
                        entry.ImagePath = uploadedImagePath;
                    }
                    catch (Exception ex)
                    {
                        TempData["Warning"] = $"Eintrag wurde erstellt, aber Bild-Upload fehlgeschlagen: {ex.Message}";
                    }
                }

                _context.GlossaryEntries.Add(entry);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Glossar-Eintrag erfolgreich erstellt!";
                return RedirectToAction("Show", new { id = entry.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Erstellen: {ex.Message}";
                ViewBag.EntityTypes = GetAvailableEntityTypes();
                return View(entry);
            }
        }

        // GET: Glossary/Edit/5 (nur Gott)
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> Edit(int id)
        {
            var entry = await _context.GlossaryEntries.FindAsync(id);
            if (entry == null)
            {
                return NotFound();
            }

            ViewBag.EntityTypes = GetAvailableEntityTypes();
            return View(entry);
        }

        // POST: Glossary/Edit/5 (nur Gott)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> Edit(int id, GlossaryEntry entry, IFormFile? entryImage)
        {
            if (id != entry.Id)
            {
                return NotFound();
            }

            try
            {
                var existingEntry = await _context.GlossaryEntries.FindAsync(id);
                if (existingEntry == null)
                {
                    return NotFound();
                }

                // Wenn EntityType und EntityId gesetzt sind, aktualisiere den Titel
                if (!string.IsNullOrEmpty(entry.EntityType) && entry.EntityId.HasValue)
                {
                    entry.Title = await GetEntityNameAsync(entry.EntityType, entry.EntityId.Value);
                }

                // Aktualisiere Felder
                existingEntry.Title = entry.Title;
                existingEntry.EntityType = entry.EntityType;
                existingEntry.EntityId = entry.EntityId;
                existingEntry.Description = entry.Description;

                // Bild hochladen, falls vorhanden
                if (entryImage != null && entryImage.Length > 0)
                {
                    try
                    {
                        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                        var uniqueName = $"{entry.Title}_{timestamp}";
                        var uploadedImagePath = await _imageUploadService.UploadImageAsync(entryImage, "glossary", uniqueName);
                        existingEntry.ImagePath = uploadedImagePath;
                    }
                    catch (Exception ex)
                    {
                        TempData["Warning"] = $"Eintrag wurde aktualisiert, aber Bild-Upload fehlgeschlagen: {ex.Message}";
                    }
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Glossar-Eintrag erfolgreich aktualisiert!";
                return RedirectToAction("Show", new { id = entry.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Aktualisieren: {ex.Message}";
                ViewBag.EntityTypes = GetAvailableEntityTypes();
                return View(entry);
            }
        }

        // GET: Glossary/Show/5 - Wiki-Anzeige
        [AllowAnonymous]
        public async Task<IActionResult> Show(int id)
        {
            var entry = await _context.GlossaryEntries.FindAsync(id);
            if (entry == null)
            {
                return NotFound();
            }

            return View(entry);
        }

        // POST: Glossary/Delete/5 (nur Gott)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> Delete(int id)
        {
            var entry = await _context.GlossaryEntries.FindAsync(id);
            if (entry == null)
            {
                return NotFound();
            }

            _context.GlossaryEntries.Remove(entry);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Glossar-Eintrag erfolgreich gelöscht!";
            return RedirectToAction(nameof(Index));
        }

        // Helper Methods
        private List<string> GetAvailableEntityTypes()
        {
            return new List<string>
            {
                "Rasse",
                "MagicClass",
                "Obermagie",
                "Blutgruppe",
                "Haus",
                "Herkunftsland",
                "Religion",
                "Infanterierang",
                "Stand"
            };
        }

        private async Task<string> GetEntityNameAsync(string entityType, int entityId)
        {
            return entityType switch
            {
                "Rasse" => (await _context.Rassen.FindAsync(entityId))?.Name ?? "",
                "MagicClass" => (await _context.MagicClasses.Include(mc => mc.Obermagie).FirstOrDefaultAsync(mc => mc.Id == entityId))?.Bezeichnung ?? "",
                "Obermagie" => (await _context.Obermagien.FindAsync(entityId))?.Bezeichnung ?? "",
                "Blutgruppe" => (await _context.Blutgruppen.FindAsync(entityId))?.Name ?? "",
                "Haus" => (await _context.Haeuser.FindAsync(entityId))?.Name ?? "",
                "Herkunftsland" => (await _context.Herkunftslaender.FindAsync(entityId))?.Name ?? "",
                "Religion" => (await _context.Religions.FindAsync(entityId))?.Type ?? "",
                "Infanterierang" => (await _context.Infanterieraenge.FindAsync(entityId))?.Name ?? "",
                "Stand" => (await _context.Staende.FindAsync(entityId))?.Name ?? "",
                _ => ""
            };
        }
    }
}
