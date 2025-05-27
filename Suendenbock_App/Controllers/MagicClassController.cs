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
            ViewBag.MagicClasses = _context.MagicClasses.ToList();
            ViewBag.LightCards = _context.LightCards.ToList();
            ViewBag.Specializations = _context.Specializations.Include(s => s.MagicClass).ToList();
            if (id > 0)
            {
                // Load data for editing
                var magicClass = _context.MagicClasses
                    .Include(mc => mc.Specializations)
                    .FirstOrDefault(mc => mc.Id == id);

                if (magicClass == null)
                {
                    return NotFound();
                }
                else
                {
                    // Return the view with the magic class data
                    ViewBag.Specializations = magicClass.Specializations.ToList();
                    return View(magicClass);
                }
            }
            else
            {
                // Return the view for creating a new magic class
                ViewBag.Specializations = new List<Specialization>();
                return View(new MagicClass());
            }
        }

        public IActionResult CreateEdit(MagicClass magicClass, List<Specialization> specializations)
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
                            _context.Specializations.Add(specialization);
                        }
                    }
                }
            }
            else
            {
                // Edit existing magic class
                var magicClassToUpdate = _context.MagicClasses
                    .Include(mc => mc.Specializations)
                    .FirstOrDefault(mc => mc.Id == magicClass.Id);

                if (magicClassToUpdate == null)
                {
                    return NotFound();
                }
                magicClassToUpdate.Bezeichnung = magicClass.Bezeichnung;
                magicClassToUpdate.ImagePath = magicClass.ImagePath;
                magicClassToUpdate.LightCardId = magicClass.LightCardId;

                // Update specializations
                _context.Specializations.RemoveRange(magicClassToUpdate.Specializations);

                // Add the new specializations
                if (specializations != null && specializations.Any())
                {
                    foreach (var specialization in specializations)
                    {
                        if (!string.IsNullOrEmpty(specialization.Name))
                        {
                            specialization.MagicClassId = magicClass.Id;
                            _context.Specializations.Add(specialization);
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
                .Include(mc => mc.Specializations)
                .FirstOrDefault(mc => mc.Id == id);
            if (magicClass == null)
            {
                return NotFound();
            }
            // Remove related specializations
            _context.Specializations.RemoveRange(magicClass.Specializations);
            // Remove the magic class
            _context.MagicClasses.Remove(magicClass);
            _context.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }
    }
}
