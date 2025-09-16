using System.Drawing;
using System.Drawing.Imaging;

namespace Suendenbock_App.Services
{
    public interface IImageUploadService
    {
        Task<string> UploadImageAsync(IFormFile file, string category, string filename);
        bool ValidateImageFile(IFormFile file);
    }

    public class ImageUploadService : IImageUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ImageUploadService> _logger;

        // Erlaubte Dateierweiterungen
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        // Maximale Dateigröße (5MB)
        private readonly long _maxFileSize = 5 * 1024 * 1024;

        public ImageUploadService(IWebHostEnvironment environment, ILogger<ImageUploadService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string category, string fileName)
        {
            var debugInfo = "";
            try
            {
                debugInfo += $"🚀 Upload gestartet: {file.FileName} → {category}/{fileName}; ";

                // Validierung der Datei
                if (!ValidateImageFile(file))
                {
                    debugInfo += "❌ Datei-Validierung fehlgeschlagen; ";
                    throw new ArgumentException("Ungültige Bilddatei");
                }
                debugInfo += "✅ Validierung OK; ";

                // Ordner-Pfad erstellen (z.B. wwwroot/images/infanterie/)
                var categoryPath = Path.Combine(_environment.WebRootPath, "images", category);
                debugInfo += $"📁 Ziel-Ordner: {categoryPath}; ";

                // Ordner erstellen falls er nicht existiert
                if (!Directory.Exists(categoryPath))
                {
                    debugInfo += "📁 Erstelle Ordner...; ";
                    Directory.CreateDirectory(categoryPath);
                }
                debugInfo += "📁 Ordner bereit; ";

                // Dateiname mit Erweiterung erstellen
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                var safeFileName = MakeSafeFileName(fileName) + fileExtension;
                var fullPath = Path.Combine(categoryPath, safeFileName);

                debugInfo += $"💾 Vollständiger Pfad: {fullPath}; ";

                // EINFACHER UPLOAD - Direkt speichern
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
                debugInfo += "💾 Datei kopiert; ";

                // Prüfen ob Datei wirklich existiert
                if (!File.Exists(fullPath))
                {
                    debugInfo += "❌ Datei wurde nicht gespeichert!; ";
                    throw new Exception("Datei konnte nicht gespeichert werden");
                }

                var fileInfo = new FileInfo(fullPath);
                debugInfo += $"✅ Datei gespeichert: {fileInfo.Length} bytes; ";

                // Relativen Pfad für die Datenbank zurückgeben
                var relativePath = $"/images/{category}/{safeFileName}";

                debugInfo += $"🎉 Upload erfolgreich: {relativePath}";

                // Debug-Info über TempData an Controller senden
                Console.WriteLine(debugInfo); // Auch in Console

                return relativePath;
            }
            catch (Exception ex)
            {
                debugInfo += $"💥 FEHLER: {ex.Message}";
                Console.WriteLine(debugInfo);
                throw;
            }
        }

        public bool ValidateImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            // Dateigröße prüfen
            if (file.Length > _maxFileSize)
            {
                return false;
            }

            // Dateierweiterung prüfen
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                return false;
            }

            // MIME-Type prüfen
            var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
            if (!allowedMimeTypes.Contains(file.ContentType.ToLower()))
            {
                return false;
            }

            return true;
        }

        private string MakeSafeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return "unnamed";

            // Ungültige Zeichen durch Unterstriche ersetzen
            var invalidChars = Path.GetInvalidFileNameChars();
            var safeFileName = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));

            // Leerzeichen und Umlaute ersetzen
            safeFileName = safeFileName.Replace(" ", "_")
                                     .Replace("ä", "ae")
                                     .Replace("ö", "oe")
                                     .Replace("ü", "ue")
                                     .Replace("ß", "ss");

            return safeFileName.ToLower();
        }
    }
}