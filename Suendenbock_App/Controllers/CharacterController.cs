using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Migrations;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Services;
using System.Threading.Tasks;

namespace Suendenbock_App.Controllers
{
    [Authorize]
    public class CharacterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageUploadService _imageUploadService;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;
        public CharacterController(
            ApplicationDbContext context, 
            IImageUploadService imageUploadService, 
            IWebHostEnvironment environment,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _imageUploadService = imageUploadService;
            _environment = environment;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult CharacterSheet(int id)
        {
            var character = _context.Characters
                // Basis-Character Daten
                .Include(c => c.Rasse)
                .Include(c => c.Lebensstatus)
                .Include(c => c.Eindruck)
                .Include(c => c.Vater)
                .Include(c => c.Mutter)

                // CharacterDetails mit allen verknüpften Tabellen
                .Include(c => c.Details)
                    .ThenInclude(d => d.Stand)
                .Include(c => c.Details)
                    .ThenInclude(d => d.Blutgruppe)
                .Include(c => c.Details)
                    .ThenInclude(d => d.Haus)
                .Include(c => c.Details)
                    .ThenInclude(d => d.Herkunftsland)

                // CharacterAffiliation (Zugehörigkeiten)
                .Include(c => c.Affiliation)
                    .ThenInclude(a => a.Guild)
                .Include(c => c.Affiliation)
                    .ThenInclude(a => a.Religion)
                .Include(c => c.Affiliation)
                    .ThenInclude(a => a.Regiment)
                .Include(c => c.Affiliation)
                    .ThenInclude(a => a.Infanterierang)

                // Magie-Informationen komplett
                .Include(c => c.CharacterMagicClasses)
                    .ThenInclude(cmc => cmc.MagicClass)
                        .ThenInclude(mc => mc.Obermagie)
                            .ThenInclude(o => o.LightCard)
                .Include(c => c.CharacterMagicClasses)
                    .ThenInclude(cmc => cmc.MagicClassSpecialization)
                .FirstOrDefault(c => c.Id == id);
            if (character == null)
            {
                return NotFound();
            }
            return View(character);
        }
        public IActionResult Form(int id = 0, int step = 1)
        {
            var userId = _userManager.GetUserId(User);
            var isGod = User.IsInRole("Gott");
            
            if (id > 0)
            {
                // Load character data for editing
                var character = LoadCharacterForEditing(id);
                if (character == null)
                {
                    return NotFound();
                }
                if (!isGod && character.UserId != userId)
                {
                    TempData["Error"] = "Du darfst nur deine eigenen Character bearbeiten";
                    return RedirectToAction("Index", "Player");
                }
                //Lade Basis-Daten für das Formular
                LoadFormViewBagData();
                ViewBag.Step = step;
                return View(character);
            }
            if (!isGod)
            {
                TempData["Error"] = "Nur Gott darf neue Character erstellen!";
                return RedirectToAction("Index", "Player");
            }
            
            LoadFormViewBagData();
            ViewBag.Step = step;
            return View(new Character());
        }

        [HttpPost]
        public async Task<IActionResult> SaveStep1(Character character, 
                                                   int[] selectedMagicClasses,
                                                   int[] selectedObermagien,
                                                   Dictionary<int, int> selectedSpecializations, 
                                                   IFormFile? characterImage)
        {
            var userId = _userManager?.GetUserId(User);
            var isGod = User.IsInRole("Gott");

            // Validierung: Prüfe Pflichtfelder und Magie-Auswahl
            if (!ValidateStep1(character, selectedMagicClasses, selectedObermagien))
            {
                LoadFormViewBagData();
                SetSelectedMagicClassesViewBag(selectedMagicClasses, selectedSpecializations);
                TempData["Error"] = "Bitte fülle alle Pflichtfelder aus und wähle 1-2 Magieklassen oder Unbegabt.";
                return View("Form", character);
            }

            try
            {
                if (character.Id == 0)
                {
                    // Neuen Charakter erstellen
                    if (!isGod)
                    {
                        TempData["Error"] = "Nur Gott darf neue Characters erstellen!";
                        return RedirectToAction("Index", "Player");
                    }

                    // Bild hochladen (falls vorhanden)
                    if (characterImage != null && characterImage.Length > 0)
                    {
                        try
                        {
                            var uploadedImagePath = await _imageUploadService.UploadImageAsync(characterImage, "characters", $"{character.Vorname}_{character.Nachname}");
                            character.ImagePath = uploadedImagePath;
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = $"Bild-Upload fehlgeschlagen: {ex.Message}";
                        }
                    }
                    character.CompletionLevel = CharacterCompleteness.BasicInfo;
                    
                    // Unbegabt-Status setzen
                    const int unbegabtId = 16;
                    character.IsUnbegabt = selectedObermagien != null &&
                                            selectedObermagien.Contains(unbegabtId);
                    _context.Characters.Add(character);
                    _context.SaveChanges();
                }
                else
                {
                    // Bestehenden Charakter aktualisieren
                    var existingCharacter = _context.Characters.Find(character.Id);

                    if (existingCharacter != null)
                    {
                        // Berechtigungsprüfung
                        if (!isGod && existingCharacter?.UserId != userId)
                        {
                            TempData["Error"] = "Du darfst nur deine eigenen Character bearbeiten!";
                            return RedirectToAction("Index", "Player");
                        }

                        // Neues Bild hochladen (falls vorhanden)
                        if (characterImage != null && characterImage.Length > 0)
                        {
                            try
                            {
                                // Altes Bild löschen
                                if (!string.IsNullOrEmpty(existingCharacter.ImagePath))
                                {
                                    DeleteOldImage(existingCharacter.ImagePath);
                                }

                                // Neues Bild mit Timestamp speichern
                                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                                var uniqueName = $"{character.Vorname}_{character.Nachname}_{timestamp}";
                                var uploadedImagePath = await _imageUploadService.UploadImageAsync(characterImage, "characters", uniqueName);
                                existingCharacter.ImagePath = uploadedImagePath;
                            }
                            catch (Exception ex)
                            {
                                TempData["Error"] = $"Bild-Upload fehlgeschlagen: {ex.Message}";
                            }
                        }

                        // Unbegabt-Status aktualisieren
                        const int unbegabtId = 16;
                        existingCharacter.IsUnbegabt = selectedObermagien != null &&
                                                selectedObermagien.Contains(unbegabtId);
                        
                        //Gott kann User zuweisen
                        if (isGod)
                        {
                            existingCharacter.UserId = character.UserId;
                            existingCharacter.UserColor = character.UserColor;
                        }
                        UpdateCharacterStep1(existingCharacter, character);
                    }
                }

                //Magieklassen und Spezialisierung speichern
                await SaveCharacterMagicClasses(character.Id, selectedMagicClasses, selectedSpecializations);
                TempData["Success"] = "Schritt 1 erfolgreich gespeichert!";
                return RedirectToAction("Form", new { id = character.Id, step = 2 });                
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Speichern: {ex.Message}";
                LoadFormViewBagData();
                return View("Form", character);
            }            
        }
        [HttpPost]
        public async Task<IActionResult> SaveStep2(int id, int? standId, string? beruf, int? blutgruppeId, int? hausId, int? herkunftslandId, int? bodyHeight, string? description)
        {
            var userId = _userManager?.GetUserId(User);
            var isGod = User.IsInRole("Gott");

            try
            {
                var character = await _context.Characters.Include(c => c.Details).FirstOrDefaultAsync(c => c.Id == id);
                if (character == null)
                {
                    return NotFound();
                }

                // CharacterDetails erstellen oder aktualisieren
                if (character.Details == null)
                {
                    if(!isGod)
                    {
                        TempData["Error"] = "Nur Gott darf neue Characters erstellen!";
                        return RedirectToAction("Index", "Player");
                    }
                    character.Details = new CharacterDetails { CharacterId = id };

                    _context.CharacterDetails.Add(character.Details);
                }
                if (!isGod && character?.UserId != userId)
                {
                    TempData["Error"] = "Du darfst nur deine eigenen Character bearbeiten!";
                    return RedirectToAction("Index", "Player");
                }
                character.Details.StandId = standId;
                character.Details.Beruf = beruf;
                character.Details.BlutgruppeId = blutgruppeId;
                character.Details.HausId = hausId;
                character.Details.HerkunftslandId = herkunftslandId;
                character.Details.BodyHeight = bodyHeight;
                character.Details.Description = description;
                character.CompletionLevel = CharacterCompleteness.WithDetails;
                
                await _context.SaveChangesAsync();

                TempData["Success"] = "Schritt 2 erfolgreich gespeichert!";
                return RedirectToAction("Form", new { id = character.Id, step = 3 });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Speichern: {ex.Message}";
                return RedirectToAction("Form", new { id = id, step = 2 });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveStep3(int id, int? guildId, int? religionId, int? regimentId, int? infanterierangId)
        {
            var userId = _userManager?.GetUserId(User);
            var isGod = User.IsInRole("Gott");
            try
            {
                var character = await _context.Characters.Include(c => c.Affiliation).FirstOrDefaultAsync(c => c.Id == id);
                if (character == null)
                {
                    return NotFound();
                }
                // CharacterAffiliation erstellen oder aktualisieren
                if (character.Affiliation == null)
                {
                    if (!isGod)
                    {
                        TempData["Error"] = "Nur Gott darf neue Characters erstellen!";
                        return RedirectToAction("Index", "Player");
                    }
                    character.Affiliation = new CharacterAffiliation { CharacterId = id };
                    _context.CharacterAffiliations.Add(character.Affiliation);
                }
                if (!isGod && character?.UserId != userId)
                {
                    TempData["Error"] = "Du darfst nur deine eigenen Character bearbeiten!";
                    return RedirectToAction("Index", "Player");
                }
                character.Affiliation.GuildId = guildId;
                character.Affiliation.ReligionId = religionId;
                character.Affiliation.RegimentId = regimentId;
                character.Affiliation.InfanterierangId = infanterierangId;
                
                character.CompletionLevel = CharacterCompleteness.Complete;
                if(isGod && !string.IsNullOrEmpty(Request.Form["userId"]))
                {
                    character.UserId = Request.Form["userId"];
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "Charakter erfolgreich erfolgreich erstellt/aktualisiert!";
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Speichern: {ex.Message}";
                return RedirectToAction("Form", new { id = id, step = 3 });
            }
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var character = _context.Characters
                    .Include(c => c.Details)
                    .Include(c => c.Affiliation)
                    .Include(c => c.CharacterMagicClasses)
                    .FirstOrDefault(c => c.Id == id);

                if (character == null)
                {
                    return NotFound();
                }

                // Alte Bilddatei löschen, falls vorhanden
                if (!string.IsNullOrEmpty(character.ImagePath))
                {
                    DeleteOldImage(character.ImagePath);
                }
                
                _context.Characters.Remove(character);
                _context.SaveChanges();

                TempData["Success"] = "Charakter erfolgreich gelöscht!";
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Löschen: {ex.Message}";
                return RedirectToAction("Index", "Admin");
            }
        }
        // Hilfsmethoden
        private void DeleteOldImage(string imagePath)
        {
            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                // Log error but do not interrupt the main flow
                Console.WriteLine($"Fehler beim Löschen des alten Bildes: {ex.Message}");
            }
        }
        private void LoadFormViewBagData()
        {

            // Load data for dropdowns
            ViewBag.Obermagien = _context.Obermagien.ToList();
            ViewBag.MagicClasses = _context.MagicClasses
                .Include(mc => mc.Obermagie)
                .ThenInclude(o => o.LightCard).ToList();
            ViewBag.Specializations = _context.MagicClassSpecializations
                .Include(s => s.MagicClass).ToList();

            ViewBag.Rassen = new SelectList(_context.Rassen.ToList(),"Id","Name");
            ViewBag.Lebensstatus = new SelectList(_context.Lebensstati.ToList(),"Id","Name");
            ViewBag.Eindruecke = new SelectList(_context.Eindruecke.ToList(), "Id", "Name");
            ViewBag.Geschlechter = new SelectList(new[] { "männlich", "weiblich" });

            ViewBag.Characters = _context.Characters.ToList();
            
            ViewBag.Staende = new SelectList(_context.Staende.ToList(), "Id", "Name");
            ViewBag.Blutgruppen = new SelectList(_context.Blutgruppen.ToList(), "Id", "Name");
            ViewBag.Haeuser = new SelectList(_context.Haeuser.ToList(), "Id", "Name");
            ViewBag.Herkunftslaender = new SelectList(_context.Herkunftslaender.ToList(), "Id", "Name");
            ViewBag.Guilds = new SelectList(_context.Guilds.ToList(), "Id", "Name");
            ViewBag.Religions = new SelectList(_context.Religions.ToList(), "Id", "Type");
            ViewBag.Regiment = new SelectList(_context.Regiments.ToList(), "Id", "Name");
            ViewBag.Infanterieraenge = new SelectList(_context.Infanterieraenge.ToList(), "Id", "Name");

            ViewBag.Players = _userManager.Users
                .ToList()
                .Where(u => _userManager.IsInRoleAsync(u, "Spieler").Result)
                .Select(u => new { u.Id, u.Email })
                .ToList();

            ViewBag.MagicClassesJson = _context.MagicClasses
                .Include(mc => mc.Obermagie)
                .Select(mc => new {
                    Id = mc.Id,
                    Bezeichnung = mc.Bezeichnung,
                    ObermagieId = mc.ObermagieId,
                    ImagePath = mc.ImagePath
                }).ToList();

            ViewBag.SpecializationsJson = _context.MagicClassSpecializations
                .Select(s => new {
                    Id = s.Id,
                    Name = s.Name,
                    MagicClassId = s.MagicClassId
                }).ToList();
            ViewBag.Lightcards = _context.LightCards.ToList();
        }
        /// <summary>
        /// Lädt einen Charakter zum Bearbeiten mit allen Magie-Daten
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Character LoadCharacterForEditing(int id)
        {
            var character = _context.Characters
                    .Include(c => c.Details)
                    .Include(c => c.Affiliation)
                    .Include(c => c.CharacterMagicClasses)
                        .ThenInclude(cmc => cmc.MagicClass)
                            .ThenInclude(om => om.Obermagie)
                    .Include(c => c.CharacterMagicClasses)
                        .ThenInclude(cmc => cmc.MagicClassSpecialization)                     
                    .Include(c => c.Rasse)
                    .Include(c => c.Lebensstatus)
                    .Include(c => c.Eindruck)
                    .Include(c => c.Vater)
                    .Include(c => c.Mutter)
                    .FirstOrDefault(c => c.Id == id);

            if (character != null)
            {
                // Unbegabt: Setze nur Obermagie-ID
                const int unbegabtId = 16;
                if(character.IsUnbegabt)
                {
                    ViewBag.SelectedObermagie1 = unbegabtId;
                    ViewBag.SelectedMagicClass1 = null;
                    ViewBag.SelectedSpecialization1 = null;
                    SetSelectedMagicClassesViewBag(new int[0], new Dictionary<int, int>());
                    return character;
                }

                // Normale Magie: Lade alle Daten
                var selectedMagicClasses = character.CharacterMagicClasses
                    .Select(cmc => cmc.MagicClassId).ToArray();
                var selectedSpecializations = character.CharacterMagicClasses
                    .Where(cmc => cmc.MagicClassSpecializationId.HasValue)
                    .ToDictionary(cmc => cmc.MagicClassId, cmc => cmc.MagicClassSpecializationId.Value);

                var magicClassesList = character.CharacterMagicClasses.ToList();

                // Erste Magie
                if (magicClassesList.Count > 0)
                {
                    var firstMagic = magicClassesList[0];
                    ViewBag.SelectedObermagie1 = firstMagic.MagicClass?.ObermagieId;
                    ViewBag.SelectedMagicClass1 = firstMagic.MagicClassId;
                    ViewBag.SelectedSpecialization1 = firstMagic.MagicClassSpecializationId;
                }

                // Zweite Magie
                if (magicClassesList.Count > 1)
                {
                    var secondMagic = magicClassesList[1];
                    ViewBag.SelectedObermagie2 = secondMagic.MagicClass?.ObermagieId;
                    ViewBag.SelectedMagicClass2 = secondMagic.MagicClassId;
                    ViewBag.SelectedSpecialization2 = secondMagic.MagicClassSpecializationId;
                }

                SetSelectedMagicClassesViewBag(selectedMagicClasses, selectedSpecializations);
            }
            return character;
        }
        private void SetSelectedMagicClassesViewBag(int[] selectedMagicClasses, Dictionary<int, int> selectedSpecializations)
        {
            ViewBag.SelectedMagicClasses = selectedMagicClasses ?? new int[0];
            ViewBag.SelectedSpecializations = selectedSpecializations ?? new Dictionary<int, int>();
        }

        /// <summary>
        /// Validiert die Pflichtfelder und Magie-Auswahl für Schritt 1
        /// </summary>
        /// <param name="character"></param>
        /// <param name="selectedMagicClasses"></param>
        /// <param name="selectedObermagien"></param>
        /// <returns></returns>
        private bool ValidateStep1(Character character, int[] selectedMagicClasses, int[] selectedObermagien)
        {
            // Basisfelder prüfen
            if (string.IsNullOrWhiteSpace(character.Vorname) ||
                string.IsNullOrWhiteSpace(character.Nachname) ||
                string.IsNullOrWhiteSpace(character.Rufname) ||
                string.IsNullOrWhiteSpace(character.Geschlecht) ||
                character.RasseId <= 0 ||
                character.LebensstatusId <= 0 ||
                character.EindruckId <= 0)
            {
                return false;
            }

            // Magie-Validierung
            const int unbegabtId = 16;
            bool isUnbegabt = selectedObermagien != null &&
                              selectedObermagien.Contains(unbegabtId);

            // Unbegabt: keine MagicClasses erforderlich
            if (isUnbegabt)
                return true;

            // Normal: 1-2 MagicClasses erforderlich
            return selectedMagicClasses != null &&
                   selectedMagicClasses.Length >= 1 &&
                   selectedMagicClasses.Length <= 2;
        }
        /// <summary>
        /// Aktualisiert die Basis-Felder eines bestehenden Characters
        /// </summary>
        /// <param name="existingCharacter"></param>
        /// <param name="newCharacter"></param>
        private void UpdateCharacterStep1(Character existingCharacter, Character newCharacter)
        {
            existingCharacter.Vorname = newCharacter.Vorname;
            existingCharacter.Nachname = newCharacter.Nachname;
            existingCharacter.Rufname = newCharacter.Rufname;
            existingCharacter.Geschlecht = newCharacter.Geschlecht;
            existingCharacter.Geburtsdatum = newCharacter.Geburtsdatum;
            existingCharacter.RasseId = newCharacter.RasseId;
            existingCharacter.LebensstatusId = newCharacter.LebensstatusId;
            existingCharacter.EindruckId = newCharacter.EindruckId;
            existingCharacter.VaterId = newCharacter.VaterId;
            existingCharacter.MutterId = newCharacter.MutterId;
            existingCharacter.Profan = newCharacter.Profan;
            existingCharacter.Beschraenkt = newCharacter.Beschraenkt;
        }
        /// <summary>
        /// Speichert die Magieklassen und Spezialisierungen eines Characters
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="selectedMagicClasses"></param>
        /// <param name="selectedSpecializations"></param>
        /// <returns></returns>
        private async Task SaveCharacterMagicClasses(int characterId, int[] selectedMagicClasses, Dictionary<int, int> selectedSpecializations)
        {
            // Alte Magieklassen entfernen
            var existingMagicClasses = _context.CharacterMagicClasses.Where(cmc => cmc.CharacterId == characterId);
            _context.CharacterMagicClasses.RemoveRange(existingMagicClasses);

            // Neue Magieklassen hinzufügen
            if(selectedMagicClasses != null)
            {
                foreach (var magicClassId in selectedMagicClasses)
                {
                    var characterMagicCLass = new CharacterMagicClass
                    {
                        CharacterId = characterId,
                        MagicClassId = magicClassId
                    };

                    // Spezialisierung hinzufügen (falls vorhanden)
                    if (selectedSpecializations.ContainsKey(magicClassId))
                    {
                        characterMagicCLass.MagicClassSpecializationId = selectedSpecializations[magicClassId];
                    }

                    _context.CharacterMagicClasses.Add(characterMagicCLass);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}