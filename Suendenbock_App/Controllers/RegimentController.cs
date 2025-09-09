using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Services;

namespace Suendenbock_App.Controllers
{
    public class RegimentController : BaseOrganizationController
    {
        private readonly ICachedDataService _cachedData;
        public RegimentController(ApplicationDbContext context, IImageUploadService imageUploadService, ICachedDataService cachedDataService, IWebHostEnvironment environment) : base(context, imageUploadService, environment)
        {
            _cachedData = cachedDataService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Form(int id = 0)
        {
            ViewBag.Infanterie = await _cachedData.GetInfanterieAsync();

            if (id > 0)
            {
                var regiment = _context.Regiments
                    .Include(r => r.Infanterie)
                    .Include(r => r.RegimentsCharacter)
                    .Include(r => r.AdjutantCharacter)
                    .FirstOrDefault(r => r.Id == id);

                if (regiment == null)
                {
                    return NotFound();
                }
                return View(regiment);
            }
            return View(new Regiment());
        }

        [HttpPost]
        public IActionResult CreateEdit(Regiment regiment)
        {
            try
            {
                if (regiment.Id == 0)
                {
                    _context.Regiments.Add(regiment);
                }
                else
                {
                    var regimentToUpdate = _context.Regiments.Find(regiment.Id);
                    if (regimentToUpdate == null)
                    {
                        return NotFound();
                    }
                    RegimentregimentProperties(regimentToUpdate, regiment);
                }
                _context.SaveChanges();
                SetMessage(true, "Regiment");
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Speichern: {ex.Message}";
                return RedirectToAction("Form", new { id = regiment.Id });
            }
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var regiment = _context.Regiments.Find(id);
            if (regiment == null)
            {
                return NotFound();
            }
            _context.Regiments.Remove(regiment);
            _context.SaveChanges();

            SetMessage(true, "Regiment gelöscht");
            return RedirectToAction("Index", "Admin");
        }
        public void RegimentregimentProperties(Regiment regimentToUpdate, Regiment regiment)
        {
            regimentToUpdate.Name = regiment.Name;
            regimentToUpdate.Description = regiment.Description;
            regimentToUpdate.InfanterieId = regiment.InfanterieId;
            regimentToUpdate.Regimentsleiter = regiment.Regimentsleiter;
            regimentToUpdate.Adjutant = regiment.Adjutant;
        }
    }
}
