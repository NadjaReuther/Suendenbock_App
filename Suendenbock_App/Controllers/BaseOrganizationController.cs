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
        protected readonly IMentionProcessorService _mentionProcessor;

        protected BaseOrganizationController(
            ApplicationDbContext context,
            IImageUploadService imageUploadService,
            IWebHostEnvironment environment,
            IMentionProcessorService mentionProcessor)
        {
            _context = context;
            _imageUploadService = imageUploadService;
            _environment = environment;
            _mentionProcessor = mentionProcessor;
        }

        /// <summary>
        /// Lädt die gemeinsamen ViewBag-Daten für beide Controller
        /// </summary>
        protected void LoadCommonViewBagData()
        {
            ViewBag.Lizenzen = _context.Lizenzen.ToList();
            ViewBag.Characters = _context.Characters.ToList();
        }

        /// <summary>
        /// Verarbeitet Bildupload - delegiert alles an den Service
        /// </summary>
        protected async Task<string?> ProcessImageUpload(IFormFile? logoFile, string organizationName, string category)
        {
            if (logoFile == null || logoFile.Length == 0)
                return null;

            try
            {
                var result = await _imageUploadService.UploadImageAsync(logoFile, category, organizationName);

                // Debug-Info für den Browser
                TempData["UploadDebug"] = $"✅ Upload erfolgreich: {result}";

                return result;
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = $"Ungültige Datei: {ex.Message}";
                TempData["UploadDebug"] = $"❌ Validation Error: {ex.Message}";
                return null;
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Upload-Fehler: {ex.Message}";
                TempData["UploadDebug"] = $"💥 Upload Error: {ex.Message}";
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