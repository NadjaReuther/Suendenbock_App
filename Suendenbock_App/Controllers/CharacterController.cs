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
        public IActionResult Form(int id)
        {
            // Load data for dropdowns
            ViewBag.MagicClasses = _context.MagicClasses.ToList();
            ViewBag.Guilds = _context.Guilds.ToList();
            ViewBag.Religions = _context.Religions.ToList();
            ViewBag.Specializations = _context.Specializations.Include(s => s.MagicClass).ToList();


            // Check if id is provided for editing
            if (id > 0)
            {
                // Load character data for editing
                var character = _context.Characters
                    .Include(c => c.CharacterMagicClasses)
                    .FirstOrDefault(c => c.Id == id);


                if (character == null)
                {
                    return NotFound();
                }
                // Get selected magic classes
                var selectedIds = character.CharacterMagicClasses
                    .Select(cmc => cmc.MagicClassId)
                    .ToArray();

                System.Diagnostics.Debug.WriteLine($"Type of SelectedMagicClasses: {selectedIds.GetType().FullName}");

                ViewBag.SelectedMagicClasses = selectedIds;
                // Return the view with the character data
                return View(character);
            }
            // Return the view for creating a new character
            // Initialize selected magic classes as an empty array
            ViewBag.SelectedMagicClasses = new int[0];
            return View();
        }
        [HttpPost]
        public IActionResult CreateEdit(Character character, int[] selectedMagicClasses)
        {

            if (character.Id == 0)
            {
                //check if Vorname and Nachname already exists
                if (checkInDB(character.Nachname)
                     && checkInDB(character.Vorname))
                {
                    return NotFound();
                }
                // Create new character
                _context.Characters.Add(character);
                _context.SaveChanges();

                //add MagicClass
                if(selectedMagicClasses != null && selectedMagicClasses.Length > 0)
                {
                    foreach (var magicClassId in selectedMagicClasses)
                    {
                        _context.CharacterMagicClasses.Add(new CharacterMagicClass
                        {
                            CharacterId = character.Id,
                            MagicClassId = magicClassId
                        });
                    }
                }
            }
            else
            {
                // Edit existing character
                // Load character data based on id
                //check if Vorname and Nachname already exists
                var characterToUpdate = _context.Characters.Find(character.Id);
                if (characterToUpdate == null 
                    || (checkInDB(character.Nachname)
                    && checkInDB(character.Vorname)))
                {
                    return NotFound();
                }
                // Update character properties
                characterToUpdate.Nachname = character.Nachname;
                characterToUpdate.Vorname = character.Vorname;
                characterToUpdate.Geschlecht = character.Geschlecht;
                //characterToUpdate.Geburtsdatum = character.Geburtsdatum;
                //characterToUpdate.ImagePath = character.ImagePath;

                //Update magic classes - first remove all existing
                _context.CharacterMagicClasses.RemoveRange(characterToUpdate.CharacterMagicClasses);
                //add MagicClass
                if (selectedMagicClasses != null && selectedMagicClasses.Length > 0)
                {
                    foreach (var magicClassId in selectedMagicClasses)
                    {
                        string specializationKey = $"Specialization_{magicClassId}";
                        int? specializationId = null;

                        if(Request.Form.ContainsKey(specializationKey) && !string.IsNullOrEmpty(Request.Form[specializationKey]))
                        {
                            specializationId = int.Parse(Request.Form[specializationKey]);
                        }
                        _context.CharacterMagicClasses.Add(new CharacterMagicClass
                        {
                            CharacterId = character.Id,
                            MagicClassId = magicClassId,
                            SpecializationId = specializationId
                        });
                    }
                }

                characterToUpdate.GuildId = character.GuildId;
                characterToUpdate.ReligionId = character.ReligionId;
            }
            // Save changes to the database
            _context.SaveChanges();
            return RedirectToAction("Index", "Admin");
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
