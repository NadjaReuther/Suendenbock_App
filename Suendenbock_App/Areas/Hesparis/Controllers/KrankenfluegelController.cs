using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain.Hesparis;
using Suendenbock_App.Models.ViewModels.Hesparis;

namespace Suendenbock_App.Areas.Hesparis.Controllers
{
    /// <summary>
    /// Akademie · Krankenflügel (/Hesparis/Krankenfluegel): sechs Betten und das Aufnahmebuch (JS: MODS.kranken).
    /// Ansehen dürfen alle; aufnehmen, Tage ändern, entlassen und den Eintrag bearbeiten nur Gott.
    /// Verletzte aus Glimrack und Duellclub (JS: kfSyncGlim / kfAdmitAuto) kommen mit der Akademie-Wertung dazu.
    /// </summary>
    public class KrankenfluegelController : HesparisBaseController
    {
        public KrankenfluegelController(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Bettenreihe mit Krankenblatt unter dem gewählten Bett. buch/fall: Aufnahmebuch offen beim Eintrag,
        /// neu/neuBett: Aufnahmeformular (Gott), wenn möglich mit diesem Bett.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(int bett = 0, bool buch = false, int? fall = null, bool neu = false, int? neuBett = null)
        {
            var akademie = await AkademieLadenAsync();
            var heute = akademie.Spieltag;
            var figuren = await FigurChipsAsync(akademie.Id);
            var faelle = await _context.Krankheitsfaelle
                .Where(f => f.KampagneId == akademie.Id)
                .ToListAsync();

            KrankenZeile Zeile(Krankheitsfall f)
            {
                var chip = f.SpielfigurId.HasValue ? figuren.FirstOrDefault(c => c.Id == f.SpielfigurId) : null;
                return new KrankenZeile
                {
                    Fall = f,
                    Name = chip?.Name ?? (string.IsNullOrWhiteSpace(f.Name) ? "Unbekannt" : f.Name),
                    Aussehen = chip?.Aussehen ?? KrankenfluegelRegeln.Fremd,
                    Rest = KrankenfluegelRegeln.Rest(f, heute),
                };
            }

            var model = new KrankenfluegelViewModel
            {
                Heute = heute,
                Bett = Math.Clamp(bett, 0, KrankenfluegelRegeln.Betten - 1),
                Figuren = figuren,
                IstSpielleitung = IsGott(),
            };

            // Betten verteilen (JS: kfBeds): erst wer ein gültiges Bett hat, dann die übrigen in freie Betten
            var aktiv = faelle.Where(f => !f.Entlassung.HasValue).OrderBy(f => f.Aufnahme).ThenBy(f => f.Id).ToList();
            bool geaendert = false;
            foreach (var f in aktiv)
            {
                if (f.Bett is int b && b >= 0 && b < KrankenfluegelRegeln.Betten && model.Betten[b] == null)
                {
                    model.Betten[b] = Zeile(f);
                }
            }
            foreach (var f in aktiv.Where(f => model.Betten.All(z => z?.Fall != f)))
            {
                int frei = Array.FindIndex(model.Betten, z => z == null);
                if (frei < 0) break;
                model.Betten[frei] = Zeile(f);
                f.Bett = frei;
                geaendert = true;
            }
            if (geaendert)
            {
                await _context.SaveChangesAsync();
            }

            // Aufnahmebuch nach Aufnahmetag (JS: sort by in)
            model.Buch = faelle.OrderBy(f => f.Aufnahme).ThenBy(f => f.Id).Select(Zeile).ToList();
            model.Formular = neu && model.IstSpielleitung;
            model.BuchOffen = buch || model.Formular;
            model.AuswahlId = (model.Buch.FirstOrDefault(z => z.Fall.Id == fall) ?? model.Buch.LastOrDefault())?.Fall.Id;
            if (model.Formular)
            {
                model.FormularBett = neuBett is int nb && nb >= 0 && nb < KrankenfluegelRegeln.Betten && model.Betten[nb] == null
                    ? nb
                    : Array.FindIndex(model.Betten, z => z == null) is int i && i >= 0 ? i : (int?)null;
            }
            return View(model);
        }

        /// <summary>
        /// Neuaufnahme (JS: data-kfadd): eine Spielfigur oder "jemand anderes" mit Namen, Art, Diagnose,
        /// Behandlung, Folgen, Tage (0–60), Bett und ob die Schwester ihm glaubt.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aufnehmen(string? patient, string? name, string? art, string? diagnose, string? behandlung,
            string? folgen, int tage, int? bett, string? notiz, bool simuliert)
        {
            if (!IsGott()) return Forbid();
            var akademie = await AkademieLadenAsync();

            int? figurId = null;
            if (patient != "x" && int.TryParse(patient, out int fid)
                && await _context.Spielfiguren.AnyAsync(f => f.Id == fid && f.KampagneId == akademie.Id))
            {
                figurId = fid;
            }
            name = Kuerzen(name?.Trim(), 50);
            if (figurId == null && name.Length == 0)
            {
                TempData["Error"] = "Bei „jemand anderes“ bitte einen Namen eintragen.";
                return RedirectToAction(nameof(Index), new { neu = true, neuBett = bett });
            }

            // Nur freie Betten (JS: free)
            var belegt = await _context.Krankheitsfaelle
                .Where(f => f.KampagneId == akademie.Id && f.Entlassung == null && f.Bett != null)
                .Select(f => f.Bett!.Value)
                .ToListAsync();
            if (bett is int b && (b < 0 || b >= KrankenfluegelRegeln.Betten || belegt.Contains(b)))
            {
                bett = null;
            }
            if (bett == null)
            {
                int frei = Enumerable.Range(0, KrankenfluegelRegeln.Betten).FirstOrDefault(i => !belegt.Contains(i), -1);
                if (frei < 0)
                {
                    TempData["Error"] = "Alle sechs Betten sind belegt. Erst jemanden entlassen.";
                    return RedirectToAction(nameof(Index), new { buch = true });
                }
                bett = frei;
            }

            var fall = new Krankheitsfall
            {
                KampagneId = akademie.Id,
                SpielfigurId = figurId,
                Name = figurId == null ? name : "",
                Art = KrankenfluegelRegeln.Arten.Any(a => a.Art == art) ? art! : "Unfall",
                Diagnose = LeerZuNull(Kuerzen(diagnose?.Trim(), 80)),
                Behandlung = LeerZuNull(Kuerzen(behandlung?.Trim(), 80)),
                Malus = LeerZuNull(Kuerzen(folgen?.Trim(), 80)),
                Notiz = LeerZuNull(Kuerzen(notiz?.Trim(), 400)),
                Tage = Math.Clamp(tage, 0, 60),
                Aufnahme = akademie.Spieltag,
                Bett = bett,
                Simuliert = simuliert,
            };
            _context.Krankheitsfaelle.Add(fall);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { bett, buch = true, fall = fall.Id });
        }

        /// <summary>Einen Tag mehr oder weniger im Bett, 0–60 (JS: data-kft).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tage(int id, int aenderung)
        {
            if (!IsGott()) return Forbid();
            var fall = await FallLadenAsync(id);
            if (fall == null) return NotFound();
            fall.Tage = Math.Clamp((fall.Tage ?? 0) + Math.Sign(aenderung), 0, 60);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { bett = fall.Bett ?? 0 });
        }

        /// <summary>Entlassen: das Bett wird frei, im Buch steht "entlassen" (JS: data-kfout).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Entlassen(int id, bool buch = false)
        {
            if (!IsGott()) return Forbid();
            var akademie = await AkademieLadenAsync();
            var fall = await FallLadenAsync(id);
            if (fall == null) return NotFound();
            int bett = fall.Bett ?? 0;
            fall.Entlassung = akademie.Spieltag;
            fall.Bett = null;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { bett, buch, fall = buch ? id : (int?)null });
        }

        /// <summary>Eintrag ganz löschen (JS: data-kfdel).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Loeschen(int id)
        {
            if (!IsGott()) return Forbid();
            var fall = await FallLadenAsync(id);
            if (fall != null)
            {
                _context.Krankheitsfaelle.Remove(fall);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { buch = true });
        }

        /// <summary>
        /// Diagnose, Behandlung, Folgen oder Notiz im Buch ändern, beim Tippen im Hintergrund (JS: data-kfe);
        /// "simuliert" an/aus (JS: data-kffake).
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Feld(int id, string feld, string? wert)
        {
            if (!IsGott()) return Forbid();
            var fall = await FallLadenAsync(id);
            if (fall == null) return NotFound();
            switch (feld)
            {
                case "diagnose": fall.Diagnose = LeerZuNull(Kuerzen(wert, 80)); break;
                case "behandlung": fall.Behandlung = LeerZuNull(Kuerzen(wert, 80)); break;
                case "folgen": fall.Malus = LeerZuNull(Kuerzen(wert, 80)); break;
                case "notiz": fall.Notiz = LeerZuNull(Kuerzen(wert, 400)); break;
                case "simuliert": fall.Simuliert = wert == "true"; break;
                default: return BadRequest();
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<Krankheitsfall?> FallLadenAsync(int id)
        {
            var akademie = await AkademieLadenAsync();
            return await _context.Krankheitsfaelle.FirstOrDefaultAsync(f => f.Id == id && f.KampagneId == akademie.Id);
        }

        private static string Kuerzen(string? text, int max)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Length > max ? text[..max] : text;
        }

        private static string? LeerZuNull(string? text) => string.IsNullOrWhiteSpace(text) ? null : text;
    }
}
