using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Controllers
{
    [Authorize(Roles = "Gott")]
    public class MonstertypController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MonstertypController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        public IActionResult Overview()
        {
            var monstertyp = _context.MonsterTypes
                .Include(mt => mt.Monster)
                .OrderBy(mt => mt.MonsterwuerfelId)
                .ToList();
            return View(monstertyp);
        }

        // GET: Monstertyp/Form (Create & Edit)
        public IActionResult Form(int? id)
        {
            Monstertyp monstertyp;

            if (id.HasValue)
            {
                // Bearbeiten: Lade existierenden Monstertyp
                monstertyp = _context.MonsterTypes
                    .Include(mt => mt.Monsterwuerfel)
                    .Include(mt => mt.Monsterintelligenz)
                    .Include(mt => mt.Monstergruppen)
                    .Include(mt => mt.MonstertypImmunitaeten)
                    .Include(mt => mt.MonstertypenVorkommen)
                    .Include(mt => mt.MonstertypAnfaelligkeiten)
                    .FirstOrDefault(mt => mt.Id == id.Value);

                if (monstertyp == null)
                {
                    return NotFound();
                }

                // Lade existierende Selections für die Multi-Selects
                ViewBag.SelectedImmunitaeten = monstertyp.MonstertypImmunitaeten?
                    .Select(mi => mi.MonsterimmunitaetenId).ToList() ?? new List<int>();
                ViewBag.SelectedVorkommen = monstertyp.MonstertypenVorkommen?
                    .Select(mv => mv.MonstervorkommenId).ToList() ?? new List<int>();
                ViewBag.SelectedAnfaelligkeiten = monstertyp.MonstertypAnfaelligkeiten?
                    .Select(ma => ma.MonsteranfaelligkeitenId).ToList() ?? new List<int>();
            }
            else
            {
                // Neu erstellen
                monstertyp = new Monstertyp();
                ViewBag.SelectedImmunitaeten = new List<int>();
                ViewBag.SelectedVorkommen = new List<int>();
                ViewBag.SelectedAnfaelligkeiten = new List<int>();
            }

            // Lade Dropdown-Daten
            ViewBag.Monsterwuerfel = _context.Monsterwuerfel.ToList();
            ViewBag.Monsterintelligenz = _context.Monsterintelligenzen.ToList();
            ViewBag.Monstergruppen = _context.Monstergruppen.ToList();

            // Lade Stammdaten für Multi-Selects
            ViewBag.Monsterimmunitaeten = _context.Monsterimmunitaeten.OrderBy(m => m.Name).ToList();
            ViewBag.Monstervorkommen = _context.Monstervorkommen.OrderBy(m => m.Name).ToList();
            ViewBag.Monsteranfaelligkeiten = _context.Monsteranfaelligkeiten.OrderBy(m => m.Name).ToList();

            return View(monstertyp);
        }

        // POST: Monstertyp/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(Monstertyp monstertyp, int[] immunitaetenIds, int[] vorkommenIds, int[] anfaelligkeitenIds)
        {
            // Description verarbeiten (CKEditor → ProcessedDescription)
            if (!string.IsNullOrEmpty(monstertyp.Description))
            {
                monstertyp.ProcessedDescription = monstertyp.Description;
            }

            if (monstertyp.Id == 0)
            {
                // Neu erstellen
                _context.MonsterTypes.Add(monstertyp);
                _context.SaveChanges(); // Speichern, um Id zu erhalten
            }
            else
            {
                // Aktualisieren
                _context.MonsterTypes.Update(monstertyp);

                // Lösche alte Join-Table Einträge
                var oldImmunitaeten = _context.Monstertypimmunitaeten.Where(mi => mi.MonstertypId == monstertyp.Id);
                _context.Monstertypimmunitaeten.RemoveRange(oldImmunitaeten);

                var oldVorkommen = _context.Monstertypvorkommen.Where(mv => mv.MonstertypId == monstertyp.Id);
                _context.Monstertypvorkommen.RemoveRange(oldVorkommen);

                var oldAnfaelligkeiten = _context.Monstertypanfaelligkeiten.Where(ma => ma.MonstertypId == monstertyp.Id);
                _context.Monstertypanfaelligkeiten.RemoveRange(oldAnfaelligkeiten);

                _context.SaveChanges();
            }

            // Füge neue Immunitäten hinzu
            if (immunitaetenIds != null && immunitaetenIds.Length > 0)
            {
                foreach (var immunitaetId in immunitaetenIds)
                {
                    _context.Monstertypimmunitaeten.Add(new Monstertypimmunitaeten
                    {
                        MonstertypId = monstertyp.Id,
                        MonsterimmunitaetenId = immunitaetId
                    });
                }
            }

            // Füge neue Vorkommen hinzu
            if (vorkommenIds != null && vorkommenIds.Length > 0)
            {
                foreach (var vorkommenId in vorkommenIds)
                {
                    _context.Monstertypvorkommen.Add(new Monstertypvorkommen
                    {
                        MonstertypId = monstertyp.Id,
                        MonstervorkommenId = vorkommenId
                    });
                }
            }

            // Füge neue Anfälligkeiten hinzu
            if (anfaelligkeitenIds != null && anfaelligkeitenIds.Length > 0)
            {
                foreach (var anfaelligkeitId in anfaelligkeitenIds)
                {
                    _context.Monstertypanfaelligkeiten.Add(new Monstertypanfaelligkeiten
                    {
                        MonstertypId = monstertyp.Id,
                        MonsteranfaelligkeitenId = anfaelligkeitId
                    });
                }
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = monstertyp.Id == 0
                ? "Monstertyp erfolgreich erstellt!"
                : "Monstertyp erfolgreich aktualisiert!";

            return RedirectToAction("Index", "Admin");
        }

        // POST: Monstertyp/Delete
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var monstertyp = _context.MonsterTypes.Find(id);

            if (monstertyp == null)
            {
                return Json(new { success = false, message = "Monstertyp nicht gefunden." });
            }

            // Prüfe ob Monster mit diesem Typ existieren
            var hasMonsters = _context.Monsters.Any(m => m.MonstertypId == id);
            if (hasMonsters)
            {
                return Json(new { success = false, message = "Monstertyp kann nicht gelöscht werden, da noch Monster diesem Typ zugeordnet sind." });
            }

            _context.MonsterTypes.Remove(monstertyp);
            _context.SaveChanges();

            return Json(new { success = true, message = "Monstertyp erfolgreich gelöscht!" });
        }
    }
}
