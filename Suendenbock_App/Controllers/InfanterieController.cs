using Microsoft.AspNetCore.Mvc;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Services;

namespace Suendenbock_App.Controllers
{
    public class InfanterieController : BaseOrganizationController
    {
        public InfanterieController(ApplicationDbContext context, IImageUploadService imageUploadService, IWebHostEnvironment environment): base(context, imageUploadService, environment)
        {
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Form(int id = 0)
        {
            // Load common ViewBag data
            LoadCommonViewBagData();

            //specific ViewBag data for Infanterie
            ViewBag.InfanterieRang = _context.Infanterieraenge.ToList();

            // Check if id is provided for editing
            if (id > 0)
            {
                var infanterie = _context.Infanterien.Find(id);
                if (infanterie == null)
                {
                    return NotFound();
                }
                return View(infanterie);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateEdit(Infanterie infanterie, IFormFile? infanteriezeichen)
        {
            try
            {
                if (infanterie.Id == 0)
                {
                    var uploadedImagePath = await ProcessImageUpload(infanteriezeichen, infanterie.Bezeichnung, "infanterie");
                    if (uploadedImagePath != null)
                    {
                        infanterie.ImagePath = uploadedImagePath;
                    }
                    _context.Infanterien.Add(infanterie);
                }
                else
                {
                    var infanterieToUpdate = _context.Infanterien.Find(infanterie.Id);
                    if (infanterieToUpdate == null)
                    {
                        return NotFound();
                    }
                    UpdateInfanterieProperties(infanterieToUpdate, infanterie);

                    // update image only if a new one was uploaded
                    if (infanteriezeichen != null && infanteriezeichen.Length > 0)
                    {
                        var uploadedImagePath = await ProcessImageUpload(infanteriezeichen, infanterie.Bezeichnung, "infanterie");
                        if (uploadedImagePath != null)
                        {
                            // Altes Bild löschen
                            DeleteOldImage(infanterieToUpdate.ImagePath);
                            // Neues Bild setzen
                            infanterieToUpdate.ImagePath = uploadedImagePath;
                        }
                        // Falls Upload fehlschlägt, bleibt das alte Bild erhalten
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

                // Delete associated image if exists
                DeleteOldImage(infanterie.ImagePath);

                _context.Infanterien.Remove(infanterie); //delete Infanterie and his regiments automatically through cascade in DBContext
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
        public void UpdateInfanterieProperties(Infanterie target, Infanterie source)
        {
            target.Bezeichnung = source.Bezeichnung;
            target.Sitz = source.Sitz;
            target.description = source.description;
            target.LightCardId = source.LightCardId;
            target.leader = source.leader;
            target.vertreter = source.vertreter;
        }
    }
}
