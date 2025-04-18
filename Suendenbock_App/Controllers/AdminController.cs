using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suendenbock_App.Data;
using Suendenbock_App.Models;

namespace Suendenbock_App.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var allMagicClasses = _context.MagicClasses.ToList();
            var allGuilds = _context.Guilds.ToList();
            var allCharacters = _context.Characters.ToList();
            var viewModel = new AdminViewModel
            {
                MagicClasses = allMagicClasses,
                Guilds = allGuilds,
                Characters = allCharacters,
            };
            return View(viewModel);
        }
    }
}
