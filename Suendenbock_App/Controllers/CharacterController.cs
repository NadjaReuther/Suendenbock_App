using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Services;
using System.Threading.Tasks;

namespace Suendenbock_App.Controllers
{
    public class CharacterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageUploadService _imageUploadService;
        private readonly IWebHostEnvironment _environment;
        public CharacterController(ApplicationDbContext context, IImageUploadService imageUploadService, IWebHostEnvironment environment)
        {
            _context = context;
            _imageUploadService = imageUploadService;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CharacterSheet(int id)
        {
            var character = _context.Characters
                .Include(ca => ca.Affiliation)
                .Include(cd => cd.Details)
                .Include(c => c.CharacterMagicClasses)
                    .ThenInclude(cmc => cmc.MagicClass)
                        .ThenInclude(mc => mc.Obermagie)
                .Include(c => c.CharacterMagicClasses)
                    .ThenInclude(cmc => cmc.MagicClassSpecialization)
                .Include(c => c.Rasse)
                .Include(c => c.Lebensstatus)
                .Include(c => c.Eindruck)
                .Include(c => c.Vater)
                .Include(c => c.Mutter)
                .FirstOrDefault(c => c.Id == id);
            if (character == null)
            {
                return NotFound();
            }
            return View(character);
        }
        public IActionResult Form(int id = 0, int step = 1)
        {
            //Lade Basis-Daten für das Formular
            LoadFormViewBagData();

            ViewBag.Step = step;
            
            if (id > 0)
            {
                // Load character data for editing
                var character = LoadCharacterForEditing(id);
                if (character == null)
                {
                    return NotFound();
                }
                return View(character);
            }
            // neuen leeren Charakter erstellen
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveStep1(Character character, 
                                                   int[] selectedMagicClasses, 
                                                   Dictionary<int, int> selectedSpecializations, 
                                                   IFormFile? characterImage)
        {
            try
            {
                if(!ValidateStep1(character, selectedMagicClasses))
                {                    
                    LoadFormViewBagData();
                    SetSelectedMagicClassesViewBag(selectedMagicClasses, selectedSpecializations);
                    TempData["Error"] = "Bitte füllen Sie alle Pflichtfelder aus und wählen Sie 1-2 Magieklassen.";
                    return View("Form", character);
                }
                if (character.Id == 0)
                {
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
                    _context.Characters.Add(character);
                    _context.SaveChanges();

                }
                else
                {
                    var existingCharacter = _context.Characters.Find(character.Id);
                    if (existingCharacter != null)
                    {
                        if (characterImage != null && characterImage.Length > 0)
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(existingCharacter.ImagePath))
                                {
                                    DeleteOldImage(existingCharacter.ImagePath);
                                }

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
        public async Task<IActionResult> SaveStep2(int id, int? standId, int? berufId, int? blutgruppeId, int? hausId, int? herkunftslandId, int? bodyHeight)
        {
            try
            {
                var character = await _context.Characters.Include(c => c.Details).FirstOrDefaultAsync(c => c.Id == id);
                if(character == null)
                {
                    return NotFound();
                }
                // CharacterDetails erstellen oder aktualisieren
                if (character.Details == null)
                {
                    character.Details = new CharacterDetails { CharacterId = id };
                    _context.CharacterDetails.Add(character.Details);
                }
                character.Details.StandId = standId;
                character.Details.BerufId = berufId;
                character.Details.BlutgruppeId = blutgruppeId;
                character.Details.HausId = hausId;
                character.Details.HerkunftslandId = herkunftslandId;
                character.Details.BodyHeight = bodyHeight;

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
                    character.Affiliation = new CharacterAffiliation { CharacterId = id };
                    _context.CharacterAffiliations.Add(character.Affiliation);
                }
                character.Affiliation.GuildId = guildId;
                character.Affiliation.ReligionId = religionId;
                character.Affiliation.RegimentsId = regimentId;
                character.Affiliation.InfanterierangId = infanterierangId;
                
                character.CompletionLevel = CharacterCompleteness.Complete;
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
            ViewBag.MagicClasses = _context.MagicClasses.Include(mc => mc.Obermagie).ThenInclude(o => o.LightCard).ToList();
            ViewBag.Specializations = _context.MagicClassSpecializations.Include(s => s.MagicClass).ToList();
            ViewBag.Rassen = _context.Rassen.ToList();
            ViewBag.Lebensstatus = _context.Lebensstati.ToList();
            ViewBag.Eindruecke = _context.Eindruecke.ToList();
            ViewBag.Characters = _context.Characters.ToList();
            ViewBag.Staende = _context.Staende.ToList();
            ViewBag.Blutgruppen = _context.Blutgruppen.ToList();
            ViewBag.Haeuser = _context.Haeuser.ToList();
            ViewBag.Herkunftslaender = _context.Herkunftslaender.ToList();
            ViewBag.Guilds = _context.Guilds.ToList();
            ViewBag.Religions = _context.Religions.ToList();
            ViewBag.Regiment = _context.Regiments.ToList();
            ViewBag.Infanterieraenge = _context.Infanterieraenge.ToList();
        }
        private Character LoadCharacterForEditing(int id)
        {
            var character = _context.Characters
                    .Include(c => c.Details)
                    .Include(c => c.Affiliation)
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
                // Ausgewählte Magieklassen und Spezialisierungen in ViewBag speichern
                var selectedMagicClasses = character.CharacterMagicClasses.Select(cmc => cmc.MagicClassId).ToArray();
                var selectedSpecializations = character.CharacterMagicClasses
                    .Where(cmc => cmc.MagicClassSpecializationId.HasValue)
                    .ToDictionary(cmc => cmc.MagicClassId, cmc => cmc.MagicClassSpecializationId.Value);

                SetSelectedMagicClassesViewBag(selectedMagicClasses, selectedSpecializations);
            }
            return character;
        }
        private void SetSelectedMagicClassesViewBag(int[] selectedMagicClasses, Dictionary<int, int> selectedSpecializations)
        {
            ViewBag.SelectedMagicClasses = selectedMagicClasses ?? new int[0];
            ViewBag.SelectedSpecializations = selectedSpecializations ?? new Dictionary<int, int>();
        }
        private bool ValidateStep1(Character character, int[] selectedMagicClasses)
        {
            return !string.IsNullOrEmpty(character.Vorname) &&
                   !string.IsNullOrEmpty(character.Nachname) &&
                   !string.IsNullOrEmpty(character.Rufname) &&
                   !string.IsNullOrEmpty(character.Geschlecht) &&
                   character.RasseId > 0 &&
                   character.LebensstatusId > 0 &&
                   character.EindruckId > 0 &&
                   selectedMagicClasses != null &&
                   selectedMagicClasses.Length >= 1 && 
                   selectedMagicClasses.Length <= 2;
        }
        private void UpdateCharacterStep1(Character existingCharacter, Character newCharacter)
        {
            // Der existingCharacter ist bereits geladen, keine neue Datenbankabfrage nötig
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
        }
        private async Task SaveCharacterMagicClasses(int characterId, int[] selectedMagicClasses, Dictionary<int, int> selectedSpecializations)
        {
            // Alle bestehenden MagicClasses für diesen Charakter entfernen
            var existingMagicClasses = _context.CharacterMagicClasses.Where(cmc => cmc.CharacterId == characterId);
            _context.CharacterMagicClasses.RemoveRange(existingMagicClasses);

            if(selectedMagicClasses != null)
            {
                foreach (var magicClassId in selectedMagicClasses)
                {
                    var characterMagicCLass = new CharacterMagicClass
                    {
                        CharacterId = characterId,
                        MagicClassId = magicClassId
                    };

                    // Spezialisierung hinzufügen, falls ausgewählt
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
