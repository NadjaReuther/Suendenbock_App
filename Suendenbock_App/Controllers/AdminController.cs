using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.ViewModels;

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
            var allMagicClasses = _context.MagicClasses.Include(mc => mc.Specializations).ToList();
            var allGuilds = _context.Guilds.ToList();
            var allCharacters = _context.Characters.Include(c => c.CharacterMagicClasses)
                .ThenInclude(cmc => cmc.Specialization).ToList();
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
