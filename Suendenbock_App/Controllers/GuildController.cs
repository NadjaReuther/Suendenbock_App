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
            // Check if id is provided for editing
            if (id > 0)
            {
                // Load character data for editing
                var character = _context.Characters.Find(id);
                if (character == null)
                {
                    return NotFound();
                }
                // Return the view with the character data
                return View(character);
            }
            // Return the view for creating a new character
            return View();
        }
        public IActionResult CreateEdit(Guild guild)
        {
            if(guild.Id == 0)
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
    }
}
