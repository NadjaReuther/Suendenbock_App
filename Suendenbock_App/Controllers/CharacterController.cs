using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Controllers
{
    public class CharacterController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CharacterController(ApplicationDbContext context)
        {
            _context = context;
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
        public IActionResult Form(int id)
        {
            // Load data for dropdowns
            ViewBag.MagicClasses = _context.MagicClasses.ToList();
            ViewBag.Guilds = _context.Guilds.ToList();
            ViewBag.Religions = _context.Religions.ToList();
            ViewBag.Specializations = _context.MagicClassSpecializations.Include(s => s.MagicClass).ToList();
            ViewBag.Rassen = _context.Rassen.ToList();
            ViewBag.Lebensstatus = _context.Lebensstati.ToList();
            ViewBag.Eindruecke = _context.Eindruecke.ToList();
            ViewBag.Berufe = _context.Berufe.ToList();
            ViewBag.Haeuser = _context.Haeuser.ToList();
            ViewBag.Herkunftslaender = _context.Herkunftslaender.ToList();
            ViewBag.Blutgruppen = _context.Blutgruppen.ToList();
            ViewBag.Regiment = _context.Regiments.ToList();
            ViewBag.Infanterieraenge = _context.Infanterieraenge.ToList();

            // Check if id is provided for editing
            if (id > 0)
            {
                // Load character data for editing
                var character = _context.Characters
                    .Include(ca => ca.Affiliation)
                        .ThenInclude(navigationPropertyPath: a => a.Guild)
                    .Include(ca => ca.Affiliation)
                        .ThenInclude(navigationPropertyPath: a => a.Religion)
                    .Include(ca => ca.Affiliation)
                        .ThenInclude(navigationPropertyPath: i => i.Regiment)
                    .Include(ca => ca.Affiliation)
                        .ThenInclude(navigationPropertyPath: i => i.Infanterierang)
                    .Include(cd => cd.Details)
                        .ThenInclude(navigationPropertyPath: d => d.Beruf)
                    .Include(cd => cd.Details)
                        .ThenInclude(navigationPropertyPath: d => d.Haus)
                    .Include(cd => cd.Details)
                        .ThenInclude(navigationPropertyPath: d => d.Herkunftsland)
                    .Include(cd => cd.Details)
                        .ThenInclude(navigationPropertyPath: d => d.Blutgruppe)
                    .Include(cd => cd.Details)
                        .ThenInclude(navigationPropertyPath: d => d.Stand)
                    .Include(c => c.Rasse)
                    .Include(c => c.Lebensstatus)
                    .Include(c => c.Eindruck)
                    .Include(c => c.CharacterMagicClasses)
                        .ThenInclude(cmc => cmc.MagicClassSpecialization)
                    .Include(c => c.Vater)     
                    .Include(c => c.Mutter)    
                    .FirstOrDefault(c => c.Id == id);


                if (character == null)
                {
                    return NotFound();
                }
                // Get selected magic classes
                var selectedIds = character.CharacterMagicClasses
                    .Select(cmc => cmc.MagicClassId)
                    .ToArray();

                ViewBag.SelectedMagicClasses = selectedIds;
                
                ViewBag.SelectedSpecializations = character.CharacterMagicClasses
                    .Where(cmc => cmc.MagicClassSpecializationId.HasValue)
                    .ToDictionary(
                        cmc => cmc.MagicClassId, 
                        cmc => cmc.MagicClassSpecializationId.Value
                    );
                // Return the view with the character data
                return View(character);
            }
            // Return the view for creating a new character
            // Initialize selected magic classes as an empty array
            ViewBag.SelectedMagicClasses = new int[0];
            ViewBag.SelectedSpecializations = new Dictionary<int, int>();
            return View();
        }
        public IActionResult CreateEdit(Character character, int[] selectedMagicClasses)
        {
            //Validierung der Pflichtfelder
            if(string.IsNullOrEmpty(character.Nachname) 
                || string.IsNullOrEmpty(character.Vorname) 
                || string.IsNullOrEmpty(character.Geschlecht)
                || character.RasseId == 0
                || character.LebensstatusId == 0
                || character.EindruckId == 0
                || selectedMagicClasses == null
                || selectedMagicClasses.Length == 0)
            {
                TempData["Error"] = "Pflichtfelder: Vor-/Nachname, Geschlecht, Rasse, Lebensstatus, Eindruck und mindestens eine Magieklasse sind erforderlich.";
                return RedirectToAction("Form", new { id = character.Id });
            }
            if (character.Id == 0)
            {               
                // Create new character
                character.CompletionLevel = CharacterCompleteness.BasicInfo; // Set default completion level
                _context.Characters.Add(character);
                _context.SaveChanges();

                //add MagicClass             
                foreach (var magicClassId in selectedMagicClasses)
                {
                    _context.CharacterMagicClasses.Add(new CharacterMagicClass
                    {
                        CharacterId = character.Id,
                        MagicClassId = magicClassId
                    });
                }
                _context.SaveChanges();
            }
            else
            {
                //Bestehenden Character bearbeiten
                var characterToUpdate = _context.Characters
                    .Include(c => c.CharacterMagicClasses)
                    .FirstOrDefault(c => c.Id == character.Id);

                if (characterToUpdate == null) 
                {
                    return NotFound();
                }
                // Update character properties
                characterToUpdate.Nachname = character.Nachname;
                characterToUpdate.Vorname = character.Vorname;
                characterToUpdate.Geschlecht = character.Geschlecht;
                characterToUpdate.RasseId = character.RasseId;
                characterToUpdate.LebensstatusId = character.LebensstatusId;
                characterToUpdate.EindruckId = character.EindruckId;
                characterToUpdate.Geburtsdatum = character.Geburtsdatum;
                // Optional: Update parent relationships if provided
                if (character.VaterId.HasValue && character.VaterId.Value > 0)
                {
                    characterToUpdate.VaterId = character.VaterId;
                }
                else
                {
                    characterToUpdate.VaterId = null; // Clear if not provided
                }
                if (character.MutterId.HasValue && character.MutterId.Value > 0)
                {
                    characterToUpdate.MutterId = character.MutterId;
                }
                else
                {
                    characterToUpdate.MutterId = null; // Clear if not provided
                }
                //characterToUpdate.ImagePath = character.ImagePath;

                //Update magic classes - first remove all existing
                _context.CharacterMagicClasses.RemoveRange(characterToUpdate.CharacterMagicClasses);
                //add MagicClass
                foreach (var magicClassId in selectedMagicClasses)
                {
                    _context.CharacterMagicClasses.Add(new CharacterMagicClass
                    {
                        CharacterId = character.Id,
                        MagicClassId = magicClassId,
                    });
                }
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "Admin");
        }

        public IActionResult EditDetails(int characterId)
        {
            // Load character details for editing
            var character = _context.Characters
                .Include(c => c.Details)
                .FirstOrDefault(c => c.Id == characterId);
            if (character == null)
            {
                return NotFound();
            }
            // ViewBag für DropDowns befüllen
            ViewBag.Staende = _context.Staende.ToList();
            ViewBag.Berufe = _context.Berufe.ToList();
            ViewBag.Blutgruppen = _context.Blutgruppen.ToList();
            ViewBag.Haeuser = _context.Haeuser.ToList();
            ViewBag.Herkunftslaender = _context.Herkunftslaender.ToList();

            return View(character);
        }

        public IActionResult EditAffiliation(int characterId)
        {
            // Load character affiliation for editing
            var character = _context.Characters
                .Include(c => c.Affiliation)
                .FirstOrDefault(c => c.Id == characterId);
            if (character == null)
            {
                return NotFound();
            }
            // ViewBag für DropDowns befüllen
            ViewBag.Guilds = _context.Guilds.ToList();
            ViewBag.Infanterien = _context.Infanterien.ToList();
            ViewBag.Infanterieraenge = _context.Infanterieraenge.ToList();
            ViewBag.Religions = _context.Religions.ToList();

            return View(character);
        }
        public IActionResult Delete(int id)
        {
            // Load character data based on id
            var character = _context.Characters.Find(id);
            if (character == null)
            {
                return NotFound();
            }
            // Remove character from the database
            _context.Characters.Remove(character);
            _context.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }

        private bool checkInDB(string search)
        {
            var character = _context.Characters.FirstOrDefault(c => c.Nachname == search || c.Vorname == search);
            if (character != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
