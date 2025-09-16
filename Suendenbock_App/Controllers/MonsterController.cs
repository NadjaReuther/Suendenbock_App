using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;

namespace Suendenbock_App.Controllers
{
    public class MonsterController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MonsterController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MonstertypSheet(int id)
        {
            var monstertyp = _context.MonsterTypes
                .Include(mt => mt.Monster)
                .Include(mt => mt.Monstergruppen)
                .Include(mt => mt.Monsterintelligenz)
                .Include(mt => mt.Monsterwuerfel)
                .Include(mt => mt.MonstertypImmunitaeten)
                    .ThenInclude(im => im.Monsterimmunitaeten)
                .Include(mt => mt.MonstertypenVorkommen)
                    .ThenInclude(vo => vo.Monstervorkommen)
                .Include(mt => mt.MonstertypAnfaelligkeiten)
                    .ThenInclude(an => an.Monsteranfaelligkeiten)
                .FirstOrDefault(mt => mt.Id == id);
            if (monstertyp == null)
            {
                return NotFound();
            }
            return View(monstertyp);
        }
    }
}
