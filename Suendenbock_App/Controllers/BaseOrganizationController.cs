using Microsoft.AspNetCore.Mvc;
using Suendenbock_App.Data;
using Suendenbock_App.Services;

namespace Suendenbock_App.Controllers
{
    /// <summary>
    /// Basis-Controller für Organisationen (Gilden und Infanterie)
    /// Enthält gemeinsame Funktionalitäten
    /// </summary>
    public abstract class BaseOrganizationController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IImageUploadService _imageUploadService;
        protected readonly IWebHostEnvironment _environment;
        protected BaseOrganizationController(
            ApplicationDbContext context,
            IImageUploadService imageUploadService,
            IWebHostEnvironment environment)
        {
            _context = context;
            _imageUploadService = imageUploadService;
            _environment = environment;
        }

        /// <summary>
        /// Lädt die gemeinsamen ViewBag-Daten für beide Controller
        /// </summary>
        protected void LoadCommonViewBagData()
        {
            ViewBag.LightCards = _context.LightCards.ToList();
            ViewBag.Characters = _context.Characters.ToList();
        }

        /// <summary>
        /// Verarbeitet Bildupload für Organisationslogos
        /// </summary>
        protected async Task<string?> ProcessImageUpload(IFormFile? logoFile, string organizationName, string category)
        {
            if (logoFile == null || logoFile.Length == 0)
                return null;

            try
            {
                // Validierung der Datei ZUERST
                if (!_imageUploadService.ValidateImageFile(logoFile))
                {
                    TempData["Error"] = "Ungültige Bilddatei. Nur JPG/PNG bis 5MB erlaubt.";
                    return null;
                }

                // Ordner-Pfad sicherstellen
                var categoryPath = Path.Combine(_environment.WebRootPath, "images", category);

                // Ordner erstellen falls nötig
                if (!Directory.Exists(categoryPath))
                {
                    Directory.CreateDirectory(categoryPath);
                }

                // Upload durchführen
                var result = await _imageUploadService.UploadImageAsync(logoFile, category, organizationName);

                TempData["Success"] = "Bild erfolgreich hochgeladen!";
                return result;
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Upload-Fehler: {ex.Message}";
                return null;
            }
        }

        /// <summary>
        /// Löscht ein altes Bild vom Server
        /// </summary>
        protected void DeleteOldImage(string? imagePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(imagePath))
                {
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot", imagePath.TrimStart('/'));
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
        /// Setzt Erfolgs- oder Fehlermeldungen
        /// </summary>
        protected void SetMessage(bool success, string entityName)
        {
            if (success)
            {
                TempData["Success"] = $"{entityName} erfolgreich gespeichert!";
            }
            else
            {
                TempData["Error"] = $"Fehler beim Speichern von {entityName}!";
            }
        }
    }
}