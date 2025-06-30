using Microsoft.AspNetCore.Mvc;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Services;

namespace Suendenbock_App.Controllers
{
    public class GuildController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageUploadService _imageUploadService;

        public GuildController(ApplicationDbContext context, IImageUploadService imageUploadService)
        {
            _context = context;
            _imageUploadService = imageUploadService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Form(int id)
        {
            ViewBag.LightCards = _context.LightCards.ToList();
            ViewBag.Abenteuerrang = _context.Abenteuerraenge.ToList();
            ViewBag.Anmeldungsstatus = _context.Anmeldungsstati.ToList();
            ViewBag.Characters = _context.Characters.ToList();

            // Check if id is provided for editing
            if (id > 0)
            {
                // Load character data for editing
                var guild = _context.Guilds.Find(id);
                if (guild == null)
                {
                    return NotFound();
                }
                // Return the view with the guild data
                return View(guild);
            }
            // Return the view for creating a new guild
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEdit(Guild guild, IFormFile? gildenlogo)
        {
            try
            {
                // Bildupload verarbeiten, falls vorhanden
                if (gildenlogo != null && gildenlogo.Length > 0)
                {
                    if (!_imageUploadService.ValidateImageFile(gildenlogo))
                    {
                        TempData["Error"] = "Ungültiges Bild. Erlaubt sind nur JPG/PNG Dateien bis 5MB.";
                        return RedirectToAction("Form", new { id = guild.Id });
                    }

                    try
                    {
                        // Bild hochladen -- guild als Kategorie, Gildenname als Dateiname
                        guild.ImagePath = await _imageUploadService.UploadImageAsync(
                            gildenlogo,
                            "guild",
                            guild.Name
                            );
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = $"Fehler beim Hochladen des Bildes: {ex.Message}";
                        return RedirectToAction("Form", new { id = guild.Id });
                    }
                }

                if (guild.Id == 0)
                {
                    // Create new character
                    _context.Guilds.Add(guild);
                }
                else
                {
                    // Edit existing guild
                    // Load guild data based on id
                    var guildToUpdate = _context.Guilds.Find(guild.Id);
                    if (guildToUpdate == null)
                    {
                        return NotFound();
                    }
                    // Update guild properties
                    guildToUpdate.Name = guild.Name;
                    guildToUpdate.Description = guild.Description;
                    guildToUpdate.AbenteuerrangId = guild.AbenteuerrangId;
                    guildToUpdate.AnmeldungsstatusId = guild.AnmeldungsstatusId;
                    guildToUpdate.LightCardId = guild.LightCardId;
                    guildToUpdate.leader = guild.leader;
                    guildToUpdate.vertreter = guild.vertreter;

                    // ImagePath nur aktualisieren wenn auch ein neues Bild hochgeladen wurde
                    if (!string.IsNullOrEmpty(guild.ImagePath))
                    {
                        // altes Bild löschen, falls vorhanden
                        if (!string.IsNullOrEmpty(guildToUpdate.ImagePath))
                        {
                            DeleteOldImage(guildToUpdate.ImagePath);
                        }
                        guildToUpdate.ImagePath = guild.ImagePath;
                    }
                }
                _context.SaveChanges();
                TempData["Success"] = "Gilde erfolgreich gespeichert!";
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Speichern: {ex.Message}";
                return RedirectToAction("Form", new { id = guild.Id });
            }
        }
        public IActionResult Delete(int id)
        {
            // Load the guild to delete
            var guild = _context.Guilds.Find(id);
            if (guild == null)
            {
                return NotFound();
            }
            // Remove the guild from the context
            _context.Guilds.Remove(guild);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private void DeleteOldImage(string imagePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(imagePath))
                {
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
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
    }
}
