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

        protected BaseOrganizationController(
            ApplicationDbContext context,
            IImageUploadService imageUploadService)
        {
            _context = context;
            _imageUploadService = imageUploadService;
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

            if (!_imageUploadService.ValidateImageFile(logoFile))
            {
                TempData["Error"] = "Ungültiges Bild. Erlaubt sind nur JPG/PNG Dateien bis 5MB.";
                return null;
            }

            try
            {
                return await _imageUploadService.UploadImageAsync(
                    logoFile,
                    category,
                    organizationName
                );
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Hochladen des Bildes: {ex.Message}";
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