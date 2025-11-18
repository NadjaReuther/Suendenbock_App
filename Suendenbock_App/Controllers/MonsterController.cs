using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Services;

namespace Suendenbock_App.Controllers
{
    [Authorize]
    public class MonsterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageUploadService _imageUploadService;
        private readonly IWebHostEnvironment _environment;
        public MonsterController(
            ApplicationDbContext context,
            IImageUploadService imageUploadService,
            IWebHostEnvironment environment)
        {
            _context = context;
            _imageUploadService = imageUploadService;
            _environment = environment;
        }
      
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult PlayerOverview()
        {
            // Lade alle Monster gruppiert nach Monstertyp
            // Nur Monster mit encounter=true werden angezeigt
            var monsterTypes = _context.MonsterTypes
                .Include(mt => mt.Monster.Where(m => m.encounter))
                .OrderBy(mt => mt.Name)
                .ToList();

            return View(monsterTypes);
        }
        public IActionResult Form(int id = 0)
        {
            ViewBag.Monstertyp = new SelectList(_context.MonsterTypes.ToList(), "Id", "Name");

            if (id > 0)
            {
                var monster = _context.Monsters.Find(id);
                if (monster == null)
                {
                    return NotFound();
                }
                return View(monster);
            }
            return View(new Monster());
        }
        [HttpPost]
        public async Task<IActionResult> CreateEdit(Monster monster, IFormFile? monsterzeichen)
        {
            try
            {
                if (monster.Id == 0)
                {
                    var uploadedImagePath = await _imageUploadService.UploadImageAsync(monsterzeichen, monster.Name, "monster");
                    if (uploadedImagePath != null)
                    {
                        monster.ImagePath = uploadedImagePath;
                    }
                    _context.Monsters.Add(monster);
                }
                else
                {
                    var monsterToUpdate = _context.Monsters.Find(monster.Id);
                    if (monsterToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Eigenschaften aktualisieren (OHNE ImagePath)
                    UpdateMonsterProperties(monsterToUpdate, monster);

                    // **NUR bei neuem Bild das alte löschen und ersetzen**
                    if (monsterzeichen != null && monsterzeichen.Length > 0)
                    {
                        // **ALTE DATEI ZUERST LÖSCHEN**
                        var oldImagePath = monsterToUpdate.ImagePath;

                        // **EINDEUTIGEN NAMEN GENERIEREN**
                        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        var uniqueName = $"{monster.Name}_{timestamp}";

                        var uploadedImagePath = await _imageUploadService.UploadImageAsync(monsterzeichen, uniqueName, "monster");
                        if (uploadedImagePath != null)
                        {
                            // Altes Bild löschen (NACH erfolgreichem Upload)
                            DeleteOldImage(oldImagePath);
                            // Neues Bild setzen
                            monsterToUpdate.ImagePath = uploadedImagePath;
                        }
                    }
                }

                _context.SaveChanges();
                TempData["Success"] = "Monster erfolgreich erstellt/aktualisiert!";
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Speichern des Monsters: {ex.Message}";
                return RedirectToAction("Form", new { id = monster.Id });
            }
        }
        public IActionResult Delete(int id)
        {
            var monster = _context.Monsters.Find(id);
            if (monster == null)
            {
                return NotFound();
            }

            DeleteOldImage(monster.ImagePath);

            _context.Monsters.Remove(monster);
            _context.SaveChanges();

            TempData["Success"] = "Monster gelöscht!";
            return RedirectToAction("Index", "Admin");
        }
        private void DeleteOldImage(string? imagePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(imagePath))
                {
                    var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
            catch (Exception ex)
            {
                // Logging aber nicht den ganzen Prozess stoppen
                Console.WriteLine($"Fehler beim Löschen des alten Bildes: {ex.Message}");
            }
        }
        /// <summary>
        /// Aktualisiert die Eigenschaften des Monsters außer dem ImagePath
        /// </summary>
        private void UpdateMonsterProperties(Monster target, Monster source)
        {
            target.Name = source.Name;
            target.Description = source.Description;
            target.ProcessedDescription = source.ProcessedDescription;
            target.encounter = source.encounter;
            target.perfected = source.perfected;
            target.MonstertypId = source.MonstertypId;
        }

        [HttpPost]
        [Route("api/monster/toggle")]
        public IActionResult ToggleMonsterField([FromBody] MonsterToggleRequest request)
        {
            try
            {
                var monster = _context.Monsters.Find(request.Id);
                if (monster == null)
                {
                    return NotFound(new { message = "Monster nicht gefunden" });
                }

                // Feld basierend auf dem Request aktualisieren
                if (request.Field.ToLower() == "encounter")
                {
                    monster.encounter = request.Value;
                }
                else if (request.Field.ToLower() == "perfected")
                {
                    monster.perfected = request.Value;
                }
                else
                {
                    return BadRequest(new { message = "Ungültiges Feld" });
                }

                _context.SaveChanges();

                return Ok(new
                {
                    success = true,
                    message = $"{request.Field} erfolgreich aktualisiert",
                    id = monster.Id,
                    encounter = monster.encounter,
                    perfected = monster.perfected
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Fehler: {ex.Message}" });
            }
        }

        // DTO für Toggle-Request
        public class MonsterToggleRequest
        {
            public int Id { get; set; }
            public string Field { get; set; } = string.Empty;
            public bool Value { get; set; }
        }
    }
}
