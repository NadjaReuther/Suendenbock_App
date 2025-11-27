using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Controllers
{
    public class MagicClassController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MagicClassController(ApplicationDbContext context)
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
            ViewBag.Obermagies = _context.Obermagien.ToList();
            ViewBag.MagicClasses = _context.MagicClasses.ToList();
            ViewBag.LightCards = _context.LightCards.ToList();
            ViewBag.Specializations = _context.MagicClassSpecializations.Include(s => s.MagicClass).ToList();
            if (id > 0)
            {
                // Load data for editing
                var magicClass = _context.MagicClasses
                    .Include(mc => mc.MagicClassSpecializations)
                    .FirstOrDefault(mc => mc.Id == id);

                if (magicClass == null)
                {
                    return NotFound();
                }
                // Return the view with the magic class data
                ViewBag.Specializations = magicClass.MagicClassSpecializations.ToList();
                return View(magicClass);                
            }
            else
            {
                // Return the view for creating a new magic class
                ViewBag.Specializations = new List<MagicClassSpecialization>();
                return View(new MagicClass());
            }
        }

        public IActionResult CreateEdit(MagicClass magicClass, List<MagicClassSpecialization> specializations)
        {
            if (magicClass.Id == 0)
            {
                // Create new magic class
                _context.MagicClasses.Add(magicClass);
                _context.SaveChanges();

                // Add the specializations for the new magic class
                if (specializations != null)
                {
                    foreach (var specialization in specializations)
                    {
                        if (!string.IsNullOrEmpty(specialization.Name))
                        {
                            specialization.MagicClassId = magicClass.Id;
                            _context.MagicClassSpecializations.Add(specialization);
                        }
                    }
                }
            }
            else
            {
                // Edit existing magic class
                var magicClassToUpdate = _context.MagicClasses
                    .Include(mc => mc.MagicClassSpecializations)
                    .FirstOrDefault(mc => mc.Id == magicClass.Id);

                if (magicClassToUpdate == null)
                {
                    return NotFound();
                }
                magicClassToUpdate.Bezeichnung = magicClass.Bezeichnung;
                magicClassToUpdate.ImagePath = magicClass.ImagePath;

                // Update specializations
                _context.MagicClassSpecializations.RemoveRange(magicClassToUpdate.MagicClassSpecializations);

                // Add the new specializations
                if (specializations != null && specializations.Any())
                {
                    foreach (var specialization in specializations)
                    {
                        if (!string.IsNullOrEmpty(specialization.Name))
                        {
                            specialization.MagicClassId = magicClass.Id;
                            _context.MagicClassSpecializations.Add(specialization);
                        }
                    }
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }
        
        public IActionResult Delete(int id)
        {
            var magicClass = _context.MagicClasses
                .Include(mc => mc.MagicClassSpecializations)
                .FirstOrDefault(mc => mc.Id == id);
            if (magicClass == null)
            {
                return NotFound();
            }
            // Remove related specializations
            _context.MagicClassSpecializations.RemoveRange(magicClass.MagicClassSpecializations);
            // Remove the magic class
            _context.MagicClasses.Remove(magicClass);
            _context.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }

        public IActionResult Hierarchy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetHierarchyData()
        {
            // Lade alle Obermagien
            var obermagien = _context.Obermagien.ToList();

            // Lade alle MagicClasses mit ihren Beziehungen
            var magicClasses = _context.MagicClasses
                .Include(mc => mc.Obermagie)
                .Include(mc => mc.MagicClassSpecializations)
                .ToList();

            // Gruppiere MagicClasses nach Obermagie
            var magicClassesByObermagie = magicClasses
                .GroupBy(mc => mc.ObermagieId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Erstelle hierarchische Struktur für D3.js Collapsible Tree
            var hierarchyData = new
            {
                name = "Magie-System",
                children = obermagien.Select(obermagie =>
                {
                    var magicClassesForObermagie = magicClassesByObermagie.ContainsKey(obermagie.Id)
                        ? magicClassesByObermagie[obermagie.Id]
                        : new List<MagicClass>();

                    return new
                    {
                        name = obermagie.Bezeichnung,
                        children = magicClassesForObermagie.Select(magicClass => new
                        {
                            name = magicClass.Bezeichnung,
                            children = magicClass.MagicClassSpecializations.Select(spec => new
                            {
                                name = spec.Name
                            }).ToList()
                        }).ToList()
                    };
                }).ToList()
            };

            return Json(hierarchyData);
        }
    }
}
