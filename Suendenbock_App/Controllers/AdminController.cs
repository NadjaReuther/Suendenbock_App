using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.ViewModels;

namespace Suendenbock_App.Controllers
{
    [Authorize(Roles = "Gott")]
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
                ToList();
            var allInfanteries = _context.Infanterien
                .Include(l => l.LeaderCharacter)
                .Include(l => l.VertreterCharacter)
                .ToList();
            var allRegiments = _context.Regiments
                .Include(i => i.Infanterie)
                .Include(r => r.Regimentsleiter)
                .Include(a => a.Adjutant)
                .ToList();
            var allCharacters = _context.Characters
                .Include(c => c.CharacterMagicClasses)
                    .ThenInclude(cmc => cmc.MagicClass)
                .Include(c => c.Details)
                    .ThenInclude(h => h.Haus)
                .Include(c => c.Details)
                    .ThenInclude(l => l.Herkunftsland)
                .Include(c => c.Details)
                    .ThenInclude(b => b.Blutgruppe)
                .ToList();
            var allMonsters = _context.Monsters
                .Include(m => m.Monstertyp)
                .ToList();
            var allMonstertypen = _context.MonsterTypes
                .Include(mt => mt.Monsterwuerfel)
                .Include(mt => mt.Monsterintelligenz)
                .Include(mt => mt.Monstergruppen)
                .ToList();

            var viewModel = new AdminViewModel
            {
                MagicClasses = allMagicClasses,
                Guilds = allGuilds,
                Infanteries = allInfanteries,
                Regiments = allRegiments,
                Characters = allCharacters,
                Monsters = allMonsters,
                Monstertypen = allMonstertypen
            };
            return View(viewModel);
        }

        public IActionResult QRGenerator()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ToggleCompanion(int characterId, bool setAsCompanion)
        {
            try
            {
                var character = _context.Characters.Find(characterId);
                if (character == null)
                {
                    return Json(new { success = false, message = "Character nicht gefunden." });
                }

                if (setAsCompanion)
                {
                    // Prüfe, ob bereits 2 Begleiter vorhanden sind
                    var currentCompanionCount = _context.Characters.Count(c => c.IsCompanion);

                    if (currentCompanionCount >= 2)
                    {
                        return Json(new { success = false, message = "Es können maximal 2 Begleitcharaktere gleichzeitig aktiv sein." });
                    }

                    // Den ausgewählten Character auf IsCompanion = true setzen
                    character.IsCompanion = true;
                    _context.SaveChanges();

                    return Json(new { success = true, message = $"{character.Vorname} {character.Nachname} wurde als Begleitcharakter gesetzt." });
                }
                else
                {
                    // Begleitcharakter-Status entfernen
                    character.IsCompanion = false;
                    _context.SaveChanges();

                    return Json(new { success = true, message = $"Begleitcharakter-Status von {character.Vorname} {character.Nachname} wurde entfernt." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Fehler: {ex.Message}" });
            }
        }
    }
}
