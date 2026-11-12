using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain.Hesparis;
using Suendenbock_App.Models.ViewModels.Hesparis;

namespace Suendenbock_App.Areas.Hesparis.Controllers
{
    /// <summary>Startseite der Akademie (/Hesparis): alle Spielercharaktere, Spieltag für Gott.</summary>
    public class HomeController : HesparisBaseController
    {
        public HomeController(ApplicationDbContext context) : base(context)
        {
        }

        // =====================================================================
        // Startseite
        // =====================================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var kampagne = await AkademieLadenAsync();

            var figuren = await _context.Spielfiguren
                .Where(f => f.KampagneId == kampagne.Id && f.HesparisCharakter != null)
                .Include(f => f.HesparisCharakter)
                .Include(f => f.Noten)
                .Include(f => f.Talente)
                .Include(f => f.Gegenstaende)
                .Include(f => f.Wirkungen)
                .OrderBy(f => f.Position)
                .AsSplitQuery()
                .ToListAsync();

            if (figuren.Count == 0)
                return View("KeineKampagne");

            var start = StandardFigur(figuren);
            var kopf = KopfErstellen(kampagne, figuren, start, "Index");

            var model = new HesparisIndexViewModel { Kopf = kopf };
            foreach (var f in figuren)
            {
                var fachSchnitte = f.Noten
                    .GroupBy(n => n.FachId)
                    .Select(g => HesparisRegeln.Schnitt(g.Select(n => n.Note)))
                    .Where(s => s.HasValue)
                    .Select(s => s!.Value)
                    .ToList();

                var kopfwerte = new Dictionary<string, int>();
                foreach (var code in HesparisRegeln.BegabungFarbe.Keys)
                {
                    int summe = f.Talente.Where(t => t.TalentCode.StartsWith(code)).Sum(t => t.Wert);
                    kopfwerte[code] = HesparisRegeln.Kopfwert(summe);
                }

                model.Karten.Add(new FigurUebersicht
                {
                    Figur = kopf.Figuren.First(x => x.Id == f.Id),
                    LebenspunkteAktuell = f.LebenspunkteAktuell,
                    LebenspunkteMax = f.LebenspunkteMax,
                    Geld = f.Geld,
                    Notenschnitt = fachSchnitte.Count > 0 ? fachSchnitte.Average() : null,
                    Kopfwerte = kopfwerte,
                    Gegenstaende = f.Gegenstaende.Sum(g => g.Menge),
                    AktiveWirkungen = f.Wirkungen.Count(w => HesparisRegeln.WirkungAktiv(w, kampagne.Spieltag)),
                });
            }

            return View(model);
        }

        /// <summary>
        /// Spieltag und Schuljahr der Akademie. Nur Gott. Vom Spieltag hängt ab, wann Wirkungen ablaufen.
        /// (Später übernimmt das der Kalender aus dem Bereich "Alltag".)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> Akademie(int kampagneId, DateOnly spieltag, int schuljahr)
        {
            var kampagne = await _context.Kampagnen.FirstOrDefaultAsync(k => k.Id == kampagneId);
            if (kampagne == null) return NotFound();

            kampagne.Spieltag = spieltag;
            kampagne.Kalenderjahr = spieltag.Month >= 8 ? spieltag.Year : spieltag.Year - 1;
            kampagne.Schuljahr = Math.Max(1, schuljahr);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Spieltag und Schuljahr gespeichert.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>Lebenspunkte ändern (eigene Figur oder Gott).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lebenspunkte(int id, int aktuell, int max)
        {
            var figur = await _context.Spielfiguren.FirstOrDefaultAsync(f => f.Id == id);
            if (figur == null) return NotFound();
            if (!DarfBearbeiten()) return Forbid();

            figur.LebenspunkteMax = Math.Max(1, max);
            figur.LebenspunkteAktuell = Math.Clamp(aktuell, 0, figur.LebenspunkteMax);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
