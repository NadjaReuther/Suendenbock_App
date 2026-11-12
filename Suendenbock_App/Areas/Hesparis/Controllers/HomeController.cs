using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain.Hesparis;
using Suendenbock_App.Models.ViewModels.Hesparis;

namespace Suendenbock_App.Areas.Hesparis.Controllers
{
    /// <summary>
    /// Einstieg in Hesparis wie in der hesparis.html:
    /// /Hesparis = Startbild ("Zum Starten tippen"), danach /Hesparis/Home/Uebersicht = die Türme der Spielfiguren.
    /// </summary>
    public class HomeController : HesparisBaseController
    {
        public HomeController(ApplicationDbContext context) : base(context)
        {
        }

        // =====================================================================
        // Startbild
        // =====================================================================

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // =====================================================================
        // Übersicht: ein Turm je Spielfigur, darunter die Detailleiste
        // =====================================================================

        [HttpGet]
        public async Task<IActionResult> Uebersicht(int? figur)
        {
            var kampagne = await AkademieLadenAsync();

            var figuren = await _context.Spielfiguren
                .Where(f => f.KampagneId == kampagne.Id && f.HesparisCharakter != null)
                .Include(f => f.HesparisCharakter)
                .Include(f => f.Noten)
                .Include(f => f.FachStufen)
                .OrderBy(f => f.Position)
                .AsSplitQuery()
                .ToListAsync();

            if (figuren.Count == 0)
                return View("KeineKampagne");

            var aktiv = figuren.FirstOrDefault(f => f.Id == figur) ?? StandardFigur(figuren);
            var kopf = KopfErstellen(kampagne, figuren, aktiv, nameof(Uebersicht));

            // Zusatzdaten, die an anderen Tabellen hängen (je eine Abfrage für alle Figuren)
            var ids = figuren.Select(f => f.Id).ToList();
            var faecher = await _context.Faecher.ToDictionaryAsync(f => f.Id, f => f.Name);
            var sonderzauber = await _context.Sonderzauber.ToListAsync();
            var staende = await _context.SonderzauberStaende
                .Where(s => s.KampagneId == kampagne.Id && s.GewaehltePosition != null)
                .Include(s => s.Sonderzauber).ThenInclude(z => z.Optionen)
                .ToListAsync();
            var agMitglieder = await _context.AgMitglieder
                .Where(m => ids.Contains(m.SpielfigurId))
                .Include(m => m.AgEintrag).ThenInclude(e => e.Ag).ThenInclude(a => a.BonusTalent)
                .ToListAsync();
            var klassenEintraege = await _context.Mitschueler
                .Where(m => m.SpielfigurId != null && ids.Contains(m.SpielfigurId.Value))
                .Include(m => m.Lieblingsfach)
                .ToListAsync();

            var model = new UebersichtViewModel { Kopf = kopf, Ruhig = figur.HasValue };
            foreach (var f in figuren)
            {
                var charakter = f.HesparisCharakter!;
                var turm = new Turm
                {
                    Id = f.Id,
                    Vorname = charakter.Vorname,
                    VollerName = charakter.VollerName,
                    Eigene = kopf.Figuren.First(x => x.Id == f.Id).Eigene,
                    LpAktuell = f.LebenspunkteAktuell,
                    LpMax = f.LebenspunkteMax,
                };

                // Sonderzauber gehört zur Person (Vorname): daraus Element, Farbe und Symbol
                var zauber = sonderzauber.FirstOrDefault(z => z.Person == charakter.Vorname);
                if (zauber != null)
                {
                    turm.Aussehen = HesparisSigille.Fuer(zauber.Code);
                    var teile = zauber.Titel.Split("— ");
                    turm.Element = teile[teile.Length - 1];

                    var stand = staende.FirstOrDefault(s => s.SonderzauberCode == zauber.Code);
                    if (stand != null)
                    {
                        turm.Sonderzauber = stand.Sonderzauber.Optionen
                            .FirstOrDefault(o => o.Position == stand.GewaehltePosition)?.Name;
                    }
                }

                // Notenschnitt über die Fächer (JS: playerNotenSchnitt)
                var fachSchnitte = f.Noten
                    .GroupBy(n => n.FachId)
                    .Select(g => HesparisRegeln.Schnitt(g.Select(n => n.Note)))
                    .Where(s => s.HasValue)
                    .Select(s => s!.Value)
                    .ToList();
                if (fachSchnitte.Count > 0)
                {
                    turm.Notenschnitt = fachSchnitte.Average();
                    turm.BenoteteFaecher = fachSchnitte.Count;
                }

                var ag = agMitglieder.FirstOrDefault(m => m.SpielfigurId == f.Id)?.AgEintrag.Ag;
                if (ag != null)
                {
                    turm.AgName = ag.Name;
                    if (ag.BonusTalent != null && ag.BonusWert != 0)
                    {
                        turm.AgBonus = "+" + ag.BonusWert + " " + ag.BonusTalent.Name;
                    }
                }

                var eintrag = klassenEintraege.FirstOrDefault(m => m.SpielfigurId == f.Id);
                turm.Lieblingsfach = eintrag?.Lieblingsfach?.Name;
                turm.Rolle = eintrag?.Rolle;

                var beste = f.FachStufen.OrderByDescending(s => s.Stufe).FirstOrDefault(s => s.Stufe > 0);
                if (beste != null)
                {
                    turm.BestesFachId = beste.FachId;
                    turm.BestesFach = faecher.GetValueOrDefault(beste.FachId);
                    turm.BesteStufe = beste.Stufe;
                }

                model.Tuerme.Add(turm);
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
        public async Task<IActionResult> Akademie(int kampagneId, DateOnly spieltag, int schuljahr, int? figur)
        {
            var kampagne = await _context.Kampagnen.FirstOrDefaultAsync(k => k.Id == kampagneId);
            if (kampagne == null) return NotFound();

            kampagne.Spieltag = spieltag;
            kampagne.Kalenderjahr = spieltag.Month >= 8 ? spieltag.Year : spieltag.Year - 1;
            kampagne.Schuljahr = Math.Max(1, schuljahr);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Spieltag und Schuljahr gespeichert.";
            return RedirectToAction(nameof(Uebersicht), new { figur });
        }

        /// <summary>
        /// Lebenspunkte in der Detailleiste: Knöpfe −10 … +5 (aenderung), "voll" oder neues Maximum.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lebenspunkte(int id, int? aenderung, bool voll, int? max)
        {
            if (!DarfBearbeiten()) return Forbid();
            var figur = await _context.Spielfiguren.FirstOrDefaultAsync(f => f.Id == id);
            if (figur == null) return NotFound();

            if (max.HasValue)
            {
                figur.LebenspunkteMax = Math.Clamp(max.Value, 1, 999);
            }

            if (voll)
            {
                figur.LebenspunkteAktuell = figur.LebenspunkteMax;
            }
            else if (aenderung.HasValue)
            {
                figur.LebenspunkteAktuell += aenderung.Value;
            }

            figur.LebenspunkteAktuell = Math.Clamp(figur.LebenspunkteAktuell, 0, figur.LebenspunkteMax);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Uebersicht), new { figur = id });
        }
    }
}
