using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Models.ViewModels;

namespace Suendenbock_App.Controllers
{
    [Authorize(Roles = "Gott")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var allMagicClasses = _context.MagicClasses.ToList();
            var allGuilds = _context.Guilds.
                Include(l => l.LeaderCharacter).
                Include(v => v.VertreterCharacter).
                Include(ar => ar.AbenteuerrangNavigation).
                Include(am => am.AnmeldungsstatusNavigation).
                ToList();
            var allInfanteries = _context.Infanterien
                .Include(l => l.LeaderCharacter)
                .Include(l => l.VertreterCharacter)
                .ToList();
            var allRegiments = _context.Regiments
                .Include(i => i.Infanterie)
                .Include(r => r.Regimentsleiter)
                .Include(a => a.Adjutant)
                .ToList();
            var allCharacters = _context.Characters
                .Include(c => c.CharacterMagicClasses)
                    .ThenInclude(cmc => cmc.MagicClass)
                .Include(c => c.Details)
                    .ThenInclude(h => h.Haus)
                .Include(c => c.Details)
                    .ThenInclude(l => l.Herkunftsland)
                .Include(c => c.Details)
                    .ThenInclude(b => b.Blutgruppe)
                .ToList();
            var allMonsters = _context.Monsters
                .Include(m => m.Monstertyp)
                .ToList();
            var allMonstertypen = _context.MonsterTypes
                .Include(mt => mt.Monsterwuerfel)
                .Include(mt => mt.Monsterintelligenz)
                .Include(mt => mt.Monstergruppen)
                .ToList();

            var viewModel = new AdminViewModel
            {
                MagicClasses = allMagicClasses,
                Guilds = allGuilds,
                Infanteries = allInfanteries,
                Regiments = allRegiments,
                Characters = allCharacters,
                Monsters = allMonsters,
                Monstertypen = allMonstertypen
            };
            return View(viewModel);
        }

        public IActionResult QRGenerator()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ToggleCompanion(int characterId, bool setAsCompanion)
        {
            try
            {
                var character = _context.Characters.Find(characterId);
                if (character == null)
                {
                    return Json(new { success = false, message = "Character nicht gefunden." });
                }

                if (setAsCompanion)
                {
                    // Prüfe, ob bereits 2 Begleiter vorhanden sind
                    var currentCompanionCount = _context.Characters.Count(c => c.IsCompanion);

                    if (currentCompanionCount >= 2)
                    {
                        return Json(new { success = false, message = "Es können maximal 2 Begleitcharaktere gleichzeitig aktiv sein." });
                    }

                    // Den ausgewählten Character auf IsCompanion = true setzen
                    character.IsCompanion = true;
                    _context.SaveChanges();

                    return Json(new { success = true, message = $"{character.Vorname} {character.Nachname} wurde als Begleitcharakter gesetzt." });
                }
                else
                {
                    // Begleitcharakter-Status entfernen
                    character.IsCompanion = false;
                    _context.SaveChanges();

                    return Json(new { success = true, message = $"Begleitcharakter-Status von {character.Vorname} {character.Nachname} wurde entfernt." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Fehler: {ex.Message}" });
            }
        }

        /// <summary>
        /// Achievement-Verwaltung nur für Gott - Übersicht aller Achievements
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ManageAchievements()
        {
            var achievements = await _context.Achievements
                .OrderBy(a => a.Category)
                .ThenBy(a => a.Scope)
                .ThenBy(a => a.Points)
                .ToListAsync();

            return View(achievements);
        }

        /// <summary>
        /// Formular zum Erstellen eines neuen Achievements
        /// </summary>
        [HttpGet]
        public IActionResult CreateAchievement()
        {
            return View(new Achievement());
        }

        /// <summary>
        /// Speichert ein neues Achievement
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAchievement(Achievement achievement)
        {
            if (ModelState.IsValid)
            {
                _context.Achievements.Add(achievement);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Achievement '{achievement.Name}' wurde erfolgreich erstellt!";
                return RedirectToAction(nameof(ManageAchievements));
            }

            return View(achievement);
        }

        /// <summary>
        /// Formular zum Bearbeiten eines Achievements
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditAchievement(int id)
        {
            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement == null)
            {
                TempData["Error"] = "Achievement nicht gefunden.";
                return RedirectToAction(nameof(ManageAchievements));
            }

            return View(achievement);
        }

        /// <summary>
        /// Speichert ein bearbeitetes Achievement
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAchievement(Achievement achievement)
        {
            if (ModelState.IsValid)
            {
                _context.Achievements.Update(achievement);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Achievement '{achievement.Name}' wurde erfolgreich aktualisiert!";
                return RedirectToAction(nameof(ManageAchievements));
            }

            return View(achievement);
        }

        /// <summary>
        /// Löscht ein Achievement
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAchievement(int id)
        {
            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement != null)
            {
                // Prüfe ob Achievement bereits vergeben wurde
                var hasUserAchievements = await _context.UserAchievements.AnyAsync(ua => ua.AchievementId == id);
                var hasGuildAchievements = await _context.GuildAchievements.AnyAsync(ga => ga.AchievementId == id);

                if (hasUserAchievements || hasGuildAchievements)
                {
                    TempData["Error"] = $"Achievement '{achievement.Name}' kann nicht gelöscht werden, da es bereits an Spieler oder Gilden vergeben wurde.";
                }
                else
                {
                    _context.Achievements.Remove(achievement);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = $"Achievement '{achievement.Name}' wurde erfolgreich gelöscht!";
                }
            }
            else
            {
                TempData["Error"] = "Achievement nicht gefunden.";
            }

            return RedirectToAction(nameof(ManageAchievements));
        }

        // ===== FELDEFFEKTE-VERWALTUNG =====

        /// <summary>
        /// Feldeffekt-Verwaltung nur für Gott - Übersicht aller Feldeffekte
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ManageFeldEffekte()
        {
            var feldEffekte = await _context.FeldEffekte
                .Include(fe => fe.LightCard)
                .OrderBy(fe => fe.Name)
                .ToListAsync();

            return View(feldEffekte);
        }

        /// <summary>
        /// Formular zum Erstellen eines neuen Feldeffekts
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateFeldEffekt()
        {
            ViewBag.LightCards = await _context.LightCards
                .OrderBy(lc => lc.Bezeichnung)
                .ToListAsync();

            return View(new FeldEffekt());
        }

        /// <summary>
        /// Speichert einen neuen Feldeffekt
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFeldEffekt(FeldEffekt feldEffekt)
        {
            if (ModelState.IsValid)
            {
                _context.FeldEffekte.Add(feldEffekt);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Feldeffekt '{feldEffekt.Name}' wurde erfolgreich erstellt!";
                return RedirectToAction(nameof(ManageFeldEffekte));
            }

            // Reload LightCards wenn Validierung fehlschlägt
            ViewBag.LightCards = await _context.LightCards
                .OrderBy(lc => lc.Bezeichnung)
                .ToListAsync();

            return View(feldEffekt);
        }

        /// <summary>
        /// Formular zum Bearbeiten eines Feldeffekts
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditFeldEffekt(int id)
        {
            var feldEffekt = await _context.FeldEffekte
                .Include(fe => fe.LightCard)
                .FirstOrDefaultAsync(fe => fe.Id == id);

            if (feldEffekt == null)
            {
                TempData["Error"] = "Feldeffekt nicht gefunden.";
                return RedirectToAction(nameof(ManageFeldEffekte));
            }

            ViewBag.LightCards = await _context.LightCards
                .OrderBy(lc => lc.Bezeichnung)
                .ToListAsync();

            return View(feldEffekt);
        }

        /// <summary>
        /// Speichert einen bearbeiteten Feldeffekt
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFeldEffekt(FeldEffekt feldEffekt)
        {
            if (ModelState.IsValid)
            {
                _context.FeldEffekte.Update(feldEffekt);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Feldeffekt '{feldEffekt.Name}' wurde erfolgreich aktualisiert!";
                return RedirectToAction(nameof(ManageFeldEffekte));
            }

            // Reload LightCards wenn Validierung fehlschlägt
            ViewBag.LightCards = await _context.LightCards
                .OrderBy(lc => lc.Bezeichnung)
                .ToListAsync();

            return View(feldEffekt);
        }

        /// <summary>
        /// Löscht einen Feldeffekt
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFeldEffekt(int id)
        {
            var feldEffekt = await _context.FeldEffekte.FindAsync(id);
            if (feldEffekt != null)
            {
                _context.FeldEffekte.Remove(feldEffekt);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Feldeffekt '{feldEffekt.Name}' wurde erfolgreich gelöscht!";
            }
            else
            {
                TempData["Error"] = "Feldeffekt nicht gefunden.";
            }

            return RedirectToAction(nameof(ManageFeldEffekte));
        }

        // ===== SESSION VORBEREITEN (GenerateSession) =====

        /// <summary>
        /// Session vorbereiten - Akte verwalten und neue Sessions anlegen
        /// Route: /Admin/GenerateSession
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GenerateSession()
        {
            // Lade alle vorhandenen Acts für die Verwaltung
            var acts = await _context.Acts
                .OrderBy(a => a.ActNumber)
                .ToListAsync();

            // Lade Weltkarten für jeden Act separat (Act.Map ist nicht mehr gemappt)
            var worldMaps = await _context.Maps
                .Where(m => m.IsWorldMap)
                .ToListAsync();

            foreach (var act in acts)
            {
                act.Map = worldMaps.FirstOrDefault(m => m.ActId == act.Id);
            }

            // Lade verfügbare Begleiter alle aus der Gilde "Wolkenbruch"
            var companions = await _context.Characters
                .Include(c => c.Affiliation)
                .Where(c => c.Affiliation.Guild.Name == "Wolkenbruch")
                .Select(c => $"{c.Vorname} {c.Nachname}")
                .ToListAsync();

            // Lade alle Länder aus der Datenbank
            var countries = await _context.Herkunftslaender
                .OrderBy(h => h.Name)
                .Select(h => h.Name)
                .ToListAsync();

            ViewBag.Acts = acts;
            ViewBag.Companions = companions;
            ViewBag.Countries = countries;

            return View();
        }

        /// <summary>
        /// Session starten - Lädt automatisch den aktiven Act und springt zur Wetter-Auswahl
        /// Route: /Admin/StartSession
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> StartSession()
        {
            // Finde den aktiven Act
            var activeAct = await _context.Acts
                .FirstOrDefaultAsync(a => a.IsActive);

            // Lade Weltkarte des aktiven Acts
            if (activeAct != null)
            {
                activeAct.Map = await _context.Maps
                    .Where(m => m.ActId == activeAct.Id && m.IsWorldMap)
                    .FirstOrDefaultAsync();
            }

            if (activeAct == null)
            {
                TempData["Error"] = "Kein aktiver Akt gefunden! Bitte aktiviere zuerst einen Akt.";
                return RedirectToAction("Index");
            }

            // Lade die gleichen Daten wie bei GenerateSession
            var companions = await _context.Characters
                .Include(c => c.Affiliation)
                .Where(c => c.Affiliation.Guild.Name == "Wolkenbruch")
                .Select(c => $"{c.Vorname} {c.Nachname}")
                .ToListAsync();

            var countries = await _context.Herkunftslaender
                .OrderBy(h => h.Name)
                .Select(h => h.Name)
                .ToListAsync();

            ViewBag.Companions = companions;
            ViewBag.Countries = countries;
            ViewBag.ActiveActId = activeAct.Id; // Dieser Flag sagt der View, dass sie direkt den Act laden soll

            // Verwende die gleiche View wie GenerateSession
            return View("GenerateSession");
        }

        // ===== MAP MANAGEMENT =====

        public async Task<IActionResult> ManageMaps()
        {
            var maps = await _context.Maps
                .Include(m => m.Act)
                .Include(m => m.ParentMap)
                .Include(m => m.ChildMaps)
                .Include(m => m.Markers)
                .OrderBy(m => m.Act.ActNumber)
                .ThenBy(m => m.IsWorldMap ? 0 : 1)
                .ThenBy(m => m.RegionName)
                .Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.ImageUrl,
                    m.IsWorldMap,
                    m.RegionName,
                    m.ActId,
                    ActNumber = m.Act.ActNumber,
                    ActName = m.Act.Name,
                    ParentMapId = m.ParentMapId,
                    ParentMapName = m.ParentMap != null ? m.ParentMap.Name : null,
                    ChildMapCount = m.ChildMaps.Count,
                    MarkerCount = m.Markers.Count
                })
                .ToListAsync();

            return View(maps);
        }

        [HttpGet]
        public async Task<IActionResult> CreateMap()
        {
            var viewModel = new CreateMapViewModel
            {
                Acts = await _context.Acts.OrderBy(a => a.ActNumber).ToListAsync(),
                WorldMaps = await _context.Maps
                    .Where(m => m.IsWorldMap)
                    .OrderBy(m => m.Act.ActNumber)
                    .Select(m => new WorldMapOption
                    {
                        Id = m.Id,
                        DisplayName = "Act " + m.Act.ActNumber + ": " + m.Name
                    })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMap(CreateMapViewModel viewModel, IFormFile? imageFile)
        {
            try
            {
                // Validierung
                if (string.IsNullOrEmpty(viewModel.Name))
                {
                    ModelState.AddModelError("Name", "Kartenname ist erforderlich!");
                }

                // Wenn Detail-Karte (nicht IsWorldMap), dann muss RegionName vorhanden sein
                if (!viewModel.IsWorldMap && string.IsNullOrEmpty(viewModel.RegionName))
                {
                    ModelState.AddModelError("RegionName", "Regionsname ist bei Detail-Karten erforderlich!");
                }

                // Bild-Upload Validierung
                if (imageFile == null || imageFile.Length == 0)
                {
                    ModelState.AddModelError("imageFile", "Kartenbild ist erforderlich!");
                }

                // Wenn Validierung fehlgeschlagen, Dropdowns neu laden und View zurückgeben
                if (!ModelState.IsValid)
                {
                    viewModel.Acts = await _context.Acts.OrderBy(a => a.ActNumber).ToListAsync();
                    viewModel.WorldMaps = await _context.Maps
                        .Where(m => m.IsWorldMap)
                        .OrderBy(m => m.Act.ActNumber)
                        .Select(m => new WorldMapOption
                        {
                            Id = m.Id,
                            DisplayName = "Act " + m.Act.ActNumber + ": " + m.Name
                        })
                        .ToListAsync();
                    return View(viewModel);
                }

                // Bild-Upload
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "maps");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                var imageUrl = $"/images/maps/{uniqueFileName}";

                // Create Map entity from ViewModel
                var map = new Models.Map
                {
                    Name = viewModel.Name,
                    ImageUrl = imageUrl,
                    ActId = viewModel.ActId,
                    IsWorldMap = viewModel.IsWorldMap,
                    RegionName = viewModel.RegionName,
                    ParentMapId = viewModel.ParentMapId,
                    CreatedAt = DateTime.Now
                };

                _context.Maps.Add(map);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Karte '{map.Name}' erfolgreich erstellt!";
                return RedirectToAction("ManageMaps");
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                var fullMessage = $"Fehler beim Erstellen der Karte: {ex.Message}";
                if (ex.InnerException != null)
                {
                    fullMessage += $" | Inner Exception: {innerMessage}";
                }
                ModelState.AddModelError("", fullMessage);
                viewModel.Acts = await _context.Acts.OrderBy(a => a.ActNumber).ToListAsync();
                viewModel.WorldMaps = await _context.Maps
                    .Where(m => m.IsWorldMap)
                    .OrderBy(m => m.Act.ActNumber)
                    .Select(m => new WorldMapOption
                    {
                        Id = m.Id,
                        DisplayName = "Act " + m.Act.ActNumber + ": " + m.Name
                    })
                    .ToListAsync();
                return View(viewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditMap(int id)
        {
            var map = await _context.Maps
                .Include(m => m.Act)
                .Include(m => m.ParentMap)
                .Include(m => m.ChildMaps)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (map == null)
            {
                TempData["Error"] = "Karte nicht gefunden!";
                return RedirectToAction("ManageMaps");
            }

            var viewModel = new EditMapViewModel
            {
                Id = map.Id,
                Name = map.Name,
                ImageUrl = map.ImageUrl,
                ActId = map.ActId,
                IsWorldMap = map.IsWorldMap,
                RegionName = map.RegionName,
                ParentMapId = map.ParentMapId,
                CreatedAt = map.CreatedAt,
                ChildMaps = map.ChildMaps,
                ChildMapsCount = map.ChildMaps.Count,
                Acts = await _context.Acts.OrderBy(a => a.ActNumber).ToListAsync(),
                WorldMaps = await _context.Maps
                    .Where(m => m.IsWorldMap && m.Id != id)
                    .OrderBy(m => m.Act.ActNumber)
                    .Select(m => new WorldMapOption
                    {
                        Id = m.Id,
                        DisplayName = "Act " + m.Act.ActNumber + ": " + m.Name
                    })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditMap(EditMapViewModel viewModel, IFormFile? imageFile)
        {
            try
            {
                var existingMap = await _context.Maps.FindAsync(viewModel.Id);
                if (existingMap == null)
                {
                    TempData["Error"] = "Karte nicht gefunden!";
                    return RedirectToAction("ManageMaps");
                }

                // Validierung
                if (string.IsNullOrEmpty(viewModel.Name))
                {
                    TempData["Error"] = "Kartenname ist erforderlich!";
                    return RedirectToAction("EditMap", new { id = viewModel.Id });
                }

                if (!viewModel.IsWorldMap && string.IsNullOrEmpty(viewModel.RegionName))
                {
                    TempData["Error"] = "Regionsname ist bei Detail-Karten erforderlich!";
                    return RedirectToAction("EditMap", new { id = viewModel.Id });
                }

                // Update properties
                existingMap.Name = viewModel.Name;
                existingMap.ActId = viewModel.ActId;
                existingMap.IsWorldMap = viewModel.IsWorldMap;
                existingMap.RegionName = viewModel.RegionName;
                existingMap.ParentMapId = viewModel.ParentMapId;

                // Bild-Upload (optional bei Edit)
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "maps");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    existingMap.ImageUrl = $"/images/maps/{uniqueFileName}";
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = $"Karte '{existingMap.Name}' erfolgreich aktualisiert!";
                return RedirectToAction("ManageMaps");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Aktualisieren der Karte: {ex.Message}";
                return RedirectToAction("EditMap", new { id = viewModel.Id });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMap(int id)
        {
            try
            {
                var map = await _context.Maps
                    .Include(m => m.ChildMaps)
                    .Include(m => m.Markers)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (map == null)
                {
                    return Json(new { success = false, message = "Karte nicht gefunden!" });
                }

                // Prüfe ob Child-Maps vorhanden sind
                if (map.ChildMaps.Any())
                {
                    return Json(new { success = false, message = $"Karte kann nicht gelöscht werden! Es sind noch {map.ChildMaps.Count} Region(en) zugeordnet." });
                }

                // Lösche Marker
                if (map.Markers.Any())
                {
                    _context.MapMarkers.RemoveRange(map.Markers);
                }

                _context.Maps.Remove(map);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"Karte '{map.Name}' erfolgreich gelöscht!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Fehler: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConvertToWorldMap(int id)
        {
            try
            {
                var map = await _context.Maps.FindAsync(id);
                if (map == null)
                {
                    return Json(new { success = false, message = "Karte nicht gefunden!" });
                }

                map.IsWorldMap = true;
                map.ParentMapId = null; // World Maps haben keinen Parent
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"'{map.Name}' wurde zur Weltkarte konvertiert!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Fehler: {ex.Message}" });
            }
        }
    }
}
