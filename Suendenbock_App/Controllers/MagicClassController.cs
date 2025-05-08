using Microsoft.AspNetCore.Mvc;
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
            if (id > 0)
            {
                // Load data for editing
                var magicClass = _context.MagicClasses.Find(id);
                if (magicClass == null)
                {
                    return NotFound();
                }
                else
                {
                    // Return the view with the magic class data
                    return View(magicClass);
                }
            }
            else
            {
                // Return the view for creating a new magic class
                return View();
            }
        }

        public IActionResult CreateEdit(MagicClass magicClass)
        {
            if (magicClass.Id == 0)
            {
                // Create new magic class
                _context.MagicClasses.Add(magicClass);
            }
            else
            {
                // Edit existing magic class
                var magicClassToUpdate = _context.MagicClasses.Find(magicClass.Id);
                if (magicClassToUpdate == null)
                {
                    return NotFound();
                }
                magicClassToUpdate.Bezeichnung = magicClass.Bezeichnung;
                magicClassToUpdate.ImagePath = magicClass.ImagePath;
                magicClassToUpdate.LightCardId = magicClass.LightCardId;
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
