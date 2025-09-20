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
            var allMagicClasses = _context.MagicClasses.ToList();
            var allGuilds = _context.Guilds.
                Include(l => l.LeaderCharacter).
                Include(v => v.VertreterCharacter).
                Include(ar => ar.AbenteuerrangNavigation).
                Include(am => am.AnmeldungsstatusNavigation).
                Include(c => c.Characters).ToList();
            var allInfanteries = _context.Infanterien
                .Include(l => l.LeaderCharacter)
                .Include(a => a.VertreterCharacter)
                .ToList();
            var allRegiments = _context.Regiments
                .Include(i => i.Infanterie)
                .Include(r => r.RegimentsCharacter)
                .Include(a => a.AdjutantCharacter)
                .ToList();
            var allCharacters = _context.Characters
                .Include(c => c.CharacterMagicClasses)
                .Include(c => c.Details)
                .Include(c => c.Affiliation)
                .ToList();

            var viewModel = new AdminViewModel
            {
                MagicClasses = allMagicClasses,
                Guilds = allGuilds,
                Infanteries = allInfanteries,
                Regiments = allRegiments,
                Characters = allCharacters,
            };
            return View(viewModel);
        }
    }
}
