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

        // Maximale Bildabmessungen
        private readonly int _maxWidth = 1920;
        private readonly int _maxHeight = 1080;

        public ImageUploadService(IWebHostEnvironment environment, ILogger<ImageUploadService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string category, string fileName)
        {
            try
            {
                // Validierung der Datei
                if (!ValidateImageFile(file))
                {
                    throw new ArgumentException("Ungültige Bilddatei");
                }

                // Ordner-Pfad erstellen (z.B. wwwroot/images/guild/)
                var categoryPath = Path.Combine(_environment.WebRootPath, "images", category);


                // Ordner erstellen falls er nicht existiert
                if (!Directory.Exists(categoryPath))
                {
                    Directory.CreateDirectory(categoryPath);
                }

                // Dateiname mit Erweiterung erstellen
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                var safeFileName = MakeSafeFileName(fileName) + fileExtension;
                var fullPath = Path.Combine(categoryPath, safeFileName);

                // Bild verarbeiten und speichern
                using (var imageStream = file.OpenReadStream())
                {
                    using (var image = Image.FromStream(imageStream))
                    {
                        // Bild verkleinern falls nötig
                        var resizedImage = ResizeImageIfNeeded(image);

                        // Als JPEG mit guter Qualität speichern
                        var encoder = ImageCodecInfo.GetImageDecoders()
                            .FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);

                        var encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 85L);

                        resizedImage.Save(fullPath, encoder, encoderParams);
                        resizedImage.Dispose();
                    }
                }

                // Relativen Pfad für die Datenbank zurückgeben
                var relativePath = $"/images/{category}/{safeFileName}";

                _logger.LogInformation($"Bild erfolgreich hochgeladen: {relativePath}");
                return relativePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fehler beim Hochladen des Bildes: {fileName}");
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

        private Image ResizeImageIfNeeded(Image originalImage)
        {
            if (originalImage.Width <= _maxWidth && originalImage.Height <= _maxHeight)
            {
                return new Bitmap(originalImage);
            }

            // Seitenverhältnis beibehalten
            var ratioX = (double)_maxWidth / originalImage.Width;
            var ratioY = (double)_maxHeight / originalImage.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(originalImage.Width * ratio);
            var newHeight = (int)(originalImage.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        private string MakeSafeFileName(string fileName)
        {
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
