using Microsoft.AspNetCore.Mvc;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Controllers
{
    public class GuildController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuildController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Form(int id)
        {
            ViewBag.LightCards = _context.LightCards.ToList();
            ViewBag.Abenteuerrang = _context.Abenteuerraenge.ToList();
            ViewBag.Anmeldungsstatus = _context.Anmeldungsstati.ToList();
            ViewBag.Characters = _context.Characters.ToList();

            // Check if id is provided for editing
            if (id > 0)
            {
                // Load character data for editing
                var guild = _context.Guilds.Find(id);
                if (guild == null)
                {
                    return NotFound();
                }
                // Return the view with the guild data
                return View(guild);
            }
            // Return the view for creating a new guild
            return View();
        }
        public IActionResult CreateEdit(Guild guild)
        {
            if (guild.Id == 0)
            {
                // Create new character
                _context.Guilds.Add(guild);
            }
            else
            {
                // Edit existing character
                // Load character data based on id
                var guildToUpdate = _context.Guilds.Find(guild.Id);
                if (guildToUpdate == null)
                {
                    return NotFound();
                }
                // Update character properties
                guildToUpdate.Name = guild.Name;
                guildToUpdate.ImagePath = guild.ImagePath;
                guildToUpdate.LightCardId = guild.LightCardId;
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            // Load the guild to delete
            var guild = _context.Guilds.Find(id);
            if (guild == null)
            {
                return NotFound();
            }
            // Remove the guild from the context
            _context.Guilds.Remove(guild);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
