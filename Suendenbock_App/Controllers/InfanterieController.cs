using Microsoft.AspNetCore.Mvc;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Services;

namespace Suendenbock_App.Controllers
{
    public class InfanterieController : BaseOrganizationController
    {
        public InfanterieController(ApplicationDbContext context, IImageUploadService imageUploadService, IWebHostEnvironment environment, IMentionProcessorService mentionProcessor): base(context, imageUploadService, environment, mentionProcessor)
        {
        }
        
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Form(int id = 0)
        {
            LoadCommonViewBagData();
            ViewBag.InfanterieRang = _context.Infanterieraenge.ToList();

            if (id > 0)
            {
                var infanterie = _context.Infanterien.Find(id);
                if (infanterie == null)
                {
                    return NotFound();
                }
                return View(infanterie);
            }
            return View(new Infanterie());
        }

        [HttpPost]
        public async Task<IActionResult> CreateEdit(Infanterie infanterie, IFormFile? infanteriezeichen)
        {
            try
            {
                if (infanterie.Id == 0)
                {
                    // **NEUE INFANTERIE**
                    var uploadedImagePath = await ProcessImageUpload(infanteriezeichen, infanterie.Bezeichnung, "infanterie");
                    if (uploadedImagePath != null)
                    {
                        infanterie.ImagePath = uploadedImagePath;
                    }
                    _context.Infanterien.Add(infanterie);
                }
                else
                {
                    // **BESTEHENDE INFANTERIE BEARBEITEN**
                    var infanterieToUpdate = _context.Infanterien.Find(infanterie.Id);
                    if (infanterieToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Eigenschaften aktualisieren (OHNE ImagePath)
                    UpdateInfanterieProperties(infanterieToUpdate, infanterie);

                    // **NUR bei neuem Bild das alte löschen und ersetzen**
                    if (infanteriezeichen != null && infanteriezeichen.Length > 0)
                    {
                        // **ALTE DATEI ZUERST LÖSCHEN**
                        var oldImagePath = infanterieToUpdate.ImagePath;

                        // **EINDEUTIGEN NAMEN GENERIEREN**
                        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        var uniqueName = $"{infanterie.Bezeichnung}_{timestamp}";

                        var uploadedImagePath = await ProcessImageUpload(infanteriezeichen, uniqueName, "infanterie");
                        if (uploadedImagePath != null)
                        {
                            // Altes Bild löschen (NACH erfolgreichem Upload)
                            DeleteOldImage(oldImagePath);
                            // Neues Bild setzen
                            infanterieToUpdate.ImagePath = uploadedImagePath;
                        }
                    }
                }

                _context.SaveChanges();
                SetMessage(true, "Infanterie");
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Speichern der Infanterie: {ex.Message}";
                return RedirectToAction("Form", new { id = infanterie.Id });
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var infanterie = _context.Infanterien.Find(id);
            if (infanterie == null)
            {
                return NotFound();
            }

            try
            {
                var regimentCount = _context.Regiments.Count(r => r.InfanterieId == id);

                // Bild löschen falls vorhanden
                DeleteOldImage(infanterie.ImagePath);

                _context.Infanterien.Remove(infanterie);
                _context.SaveChanges();

                var message = regimentCount > 0
                    ? $"Infanterie und {regimentCount} Regiment(e) erfolgreich gelöscht"
                    : "Infanterie erfolgreich gelöscht";

                SetMessage(true, message);
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Löschen der Infanterie: {ex.Message}";
                return RedirectToAction("Index", "Admin");
            }       
        }
        
        /// <summary>
        /// Aktualisiert die Eigenschaften der Infanterie
        /// </summary>
        private void UpdateInfanterieProperties(Infanterie target, Infanterie source)
        {
            target.Bezeichnung = source.Bezeichnung;
            target.Sitz = source.Sitz;
            target.description = source.description;
            target.LightCardId = source.LightCardId;
            target.LeaderId = source.LeaderId;
            target.VertreterId = source.VertreterId;
        }
    }
}