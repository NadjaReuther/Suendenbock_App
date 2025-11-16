using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Services;

namespace Suendenbock_App.Controllers
{
    [Authorize]
    public class MonsterController : BaseOrganizationController
    {
        public MonsterController(ApplicationDbContext context, IImageUploadService imageUploadService, IWebHostEnvironment environment) : base(context, imageUploadService, environment)
        {
        }
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Overview()
        {
            var monstertyp = _context.MonsterTypes
                .OrderBy(mt => mt.MonsterwuerfelId)
                .ToList();
            return View(monstertyp);
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
        [AllowAnonymous]
        public IActionResult MonstertypSheet(int id)
        {
            var monstertyp = _context.MonsterTypes
                .Include(mt => mt.Monster)
                .Include(mt => mt.Monstergruppen)
                .Include(mt => mt.Monsterintelligenz)
                .Include(mt => mt.Monsterwuerfel)
                .Include(mt => mt.MonstertypImmunitaeten)
                    .ThenInclude(im => im.Monsterimmunitaeten)
                .Include(mt => mt.MonstertypenVorkommen)
                    .ThenInclude(vo => vo.Monstervorkommen)
                .Include(mt => mt.MonstertypAnfaelligkeiten)
                    .ThenInclude(an => an.Monsteranfaelligkeiten)
                .FirstOrDefault(mt => mt.Id == id);
            if (monstertyp == null)
            {
                return NotFound();
            }
            return View(monstertyp);
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
                    var uploadedImagePath = await ProcessImageUpload(monsterzeichen, monster.Name, "monster");
                    if (uploadedImagePath != null)
                    {
                        monster.ImagePath = uploadedImagePath;
                    }
                    _context.Monsters.Add(monster);
                }
                else
                {
                    var monsterToUpdate = _context.Monsters.Find(monster);
                    if (monsterToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Eigenschaften aktualisieren (OHNE ImagePath)
                    MonsterInfanterieProperties(monsterToUpdate, monster);

                    // **NUR bei neuem Bild das alte löschen und ersetzen**
                    if (monsterzeichen != null && monsterzeichen.Length > 0)
                    {
                        // **ALTE DATEI ZUERST LÖSCHEN**
                        var oldImagePath = monsterToUpdate.ImagePath;

                        // **EINDEUTIGEN NAMEN GENERIEREN**
                        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        var uniqueName = $"{monster.Name}_{timestamp}";

                        var uploadedImagePath = await ProcessImageUpload(monsterzeichen, uniqueName, "monster");
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
                SetMessage(true, "Monster");
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Speichern des Monsters: {ex.Message}";
                return RedirectToAction("Form", new { id = monster.Id });
            }
        }
        /// <summary>
        /// Aktualisiert die Eigenschaften des Monsters außer dem ImagePath
        /// </summary>
        private void MonsterInfanterieProperties(Monster target, Monster source)
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
