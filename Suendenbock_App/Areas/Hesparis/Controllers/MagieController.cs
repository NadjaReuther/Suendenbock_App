using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain.Hesparis;
using Suendenbock_App.Models.ViewModels.Hesparis;

namespace Suendenbock_App.Areas.Hesparis.Controllers
{
    /// <summary>
    /// Gruppe "Magie" (/Hesparis/Magie/…). Zuerst die Magiestufen ("Der Weg nach innen").
    /// Stufen wählen dürfen alle Spieler, Bonuspunkte und Abschlüsse vergibt Gott.
    /// </summary>
    public class MagieController : HesparisBaseController
    {
        public MagieController(ApplicationDbContext context) : base(context)
        {
        }

        // =====================================================================
        // Magiestufen
        // =====================================================================

        [HttpGet]
        public async Task<IActionResult> Stufen(int? id, int? fach)
        {
            var figur = await FigurLadenAsync(id, q => q
                .Include(f => f.FachStufen)
                .Include(f => f.StufenWahlen)
                .Include(f => f.Pruefungen));
            if (figur == null) return KeineFigur();

            var faecher = await _context.Faecher.Include(f => f.Stufen).OrderBy(f => f.Id).ToListAsync();
            var abschluesse = await _context.SchuljahrAbschnitte.Where(a => a.SpielfigurId == figur.Id).ToListAsync();

            int abschnitt = HesparisRegeln.Abschnitt(abschluesse);
            int maxStufe = HesparisRegeln.MaxStufe(abschnitt);
            int gesamt = HesparisRegeln.PunkteGesamt(figur, abschluesse);
            int verbraucht = HesparisRegeln.PunkteVerbraucht(figur, faecher.Select(f => f.Id));
            int frei = gesamt - verbraucht;
            bool fertig = HesparisRegeln.Bestanden(abschluesse.FirstOrDefault(a => a.Abschnitt == "dom")?.Wert);

            var model = new StufenViewModel
            {
                Kopf = await KopfFuerFigurAsync(figur, nameof(Stufen)),
                Abschnitt = abschnitt,
                AbschnittName = HesparisRegeln.Abschnitte[abschnitt - 1].Name,
                MaxStufe = maxStufe,
                PunkteGesamt = gesamt,
                PunkteVerbraucht = verbraucht,
                Bonuspunkte = figur.Bonuspunkte,
                AbschlussPunkte = HesparisRegeln.AbschlussZehntel(abschluesse) / 10,
                Faecher = faecher.Select(f => new StufenFach
                {
                    Id = f.Id,
                    Name = f.Name,
                    Stufe = HesparisRegeln.Stufe(figur, f.Id),
                    Fachpunkte = HesparisRegeln.Fachpunkte(figur, f.Id),
                }).ToList(),
            };
            foreach (var x in HesparisRegeln.Abschnitte)
            {
                model.Abschluesse[x.Schluessel] = abschluesse.FirstOrDefault(a => a.Abschnitt == x.Schluessel)?.Wert;
            }

            // Ausgewähltes Fach
            var gewaehlt = faecher.FirstOrDefault(f => f.Id == fach) ?? faecher[0];
            model.Auswahl = model.Faecher.First(f => f.Id == gewaehlt.Id);
            model.Beschreibung = gewaehlt.Beschreibung;
            int lv = model.Auswahl.Stufe;

            // Stufenliste: erreichte mit Text, die nächste markiert, darüber verdeckt (JS: rg-steps)
            int bis = Math.Max(3, Math.Min(maxStufe, Math.Max(lv + 1, 3)));
            for (int k = 1; k <= bis; k++)
            {
                bool erreicht = k <= lv;
                string status = "";
                if (erreicht)
                {
                    status = "got";
                }
                else if (HesparisRegeln.KannWaehlen(figur, gewaehlt.Id, k, maxStufe, frei))
                {
                    status = "next";
                }

                string text = "???";
                if (erreicht)
                {
                    text = figur.StufenWahlen.FirstOrDefault(w => w.FachId == gewaehlt.Id && w.Stufe == k)?.Wahl
                           ?? gewaehlt.Stufen.FirstOrDefault(s => s.Stufe == k)?.Beschreibung
                           ?? "";
                }
                model.Schritte.Add(new StufenSchritt { Stufe = k, Text = text, Status = status });
            }

            // Fähigkeiten an den Toren (Stufe 3, 7, 9, 10), nur bis zum aktuellen Abschnitt sichtbar
            var perkNamen = new[] { gewaehlt.PerkHafen, gewaehlt.PerkAnger, gewaehlt.PerkGarten, gewaehlt.PerkDom };
            for (int k = 0; k < abschnitt; k++)
            {
                var tor = HesparisRegeln.Abschnitte[k];
                bool erhalten = lv >= tor.Max && (tor.Schluessel != "dom" || fertig);
                string hinweis;
                if (erhalten)
                {
                    hinweis = "erhalten";
                }
                else if (tor.Schluessel == "dom")
                {
                    hinweis = "Stufe 10 und Abschluss";
                }
                else
                {
                    hinweis = "bei Stufe " + tor.Max;
                }

                string? text = null;
                if (erhalten && tor.Schluessel == "hafen")
                {
                    text = gewaehlt.HafenText;
                }
                model.Perks.Add(new StufenPerk { Tor = tor.Tor, Name = perkNamen[k] ?? "", Erhalten = erhalten, Hinweis = hinweis, Text = text });
            }

            // Nächste Stufe: Optionen und ob gewählt werden darf
            if (lv < 10)
            {
                model.KannWaehlen = HesparisRegeln.KannWaehlen(figur, gewaehlt.Id, lv + 1, maxStufe, frei);
                if (lv + 1 > maxStufe)
                {
                    model.Grund = "noch gesperrt";
                }
                else if (!model.KannWaehlen)
                {
                    model.Grund = "keine Punkte frei";
                }
                var naechste = gewaehlt.Stufen.FirstOrDefault(s => s.Stufe == lv + 1)?.Beschreibung ?? "";
                model.Optionen = naechste.Split(" ODER ").Select(o => o.Trim()).Where(o => o.Length > 0).ToList();
            }

            RingBerechnen(model, figur, frei, maxStufe);
            return View(model);
        }

        /// <summary>Nächste Stufe in einem Fach wählen (JS: resolveChoice). Bei "… ODER …" mit der gewählten Option.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StufeWaehlen(int id, int fachId, int stufe, string wahl)
        {
            if (!DarfBearbeiten()) return Forbid();

            var figur = await _context.Spielfiguren
                .Include(f => f.FachStufen)
                .Include(f => f.StufenWahlen)
                .Include(f => f.Pruefungen)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (figur == null) return NotFound();

            var fach = await _context.Faecher.Include(f => f.Stufen).FirstOrDefaultAsync(f => f.Id == fachId);
            if (fach == null) return NotFound();

            var abschluesse = await _context.SchuljahrAbschnitte.Where(a => a.SpielfigurId == id).ToListAsync();
            var alleFaecher = await _context.Faecher.Select(f => f.Id).ToListAsync();
            int frei = HesparisRegeln.PunkteGesamt(figur, abschluesse) - HesparisRegeln.PunkteVerbraucht(figur, alleFaecher);
            int maxStufe = HesparisRegeln.MaxStufe(HesparisRegeln.Abschnitt(abschluesse));

            if (!HesparisRegeln.KannWaehlen(figur, fachId, stufe, maxStufe, frei))
            {
                TempData["Error"] = "Diese Stufe kann gerade nicht gewählt werden.";
                return RedirectToAction(nameof(Stufen), new { id, fach = fachId });
            }

            // Nur eine der angebotenen Optionen ist erlaubt
            var optionen = (fach.Stufen.FirstOrDefault(s => s.Stufe == stufe)?.Beschreibung ?? "")
                .Split(" ODER ").Select(o => o.Trim()).ToList();
            if (!optionen.Contains(wahl))
            {
                return BadRequest();
            }

            var fachStufe = figur.FachStufen.FirstOrDefault(s => s.FachId == fachId);
            if (fachStufe == null)
            {
                figur.FachStufen.Add(new SpielfigurFachStufe { FachId = fachId, Stufe = stufe });
            }
            else
            {
                fachStufe.Stufe = stufe;
            }

            var alteWahl = figur.StufenWahlen.FirstOrDefault(w => w.FachId == fachId && w.Stufe == stufe);
            if (alteWahl != null)
            {
                alteWahl.Wahl = wahl;
            }
            else
            {
                figur.StufenWahlen.Add(new StufenWahl { FachId = fachId, Stufe = stufe, Wahl = wahl });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"{fach.Name} Stufe {stufe}: {wahl}";
            return RedirectToAction(nameof(Stufen), new { id, fach = fachId });
        }

        /// <summary>Höchste Stufe eines Fachs zurücknehmen, samt gewählter Option.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StufeZuruecknehmen(int id, int fachId)
        {
            if (!DarfBearbeiten()) return Forbid();

            var figur = await _context.Spielfiguren
                .Include(f => f.FachStufen)
                .Include(f => f.StufenWahlen)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (figur == null) return NotFound();

            var fachStufe = figur.FachStufen.FirstOrDefault(s => s.FachId == fachId);
            if (fachStufe != null && fachStufe.Stufe > 0)
            {
                var wahl = figur.StufenWahlen.FirstOrDefault(w => w.FachId == fachId && w.Stufe == fachStufe.Stufe);
                if (wahl != null)
                {
                    _context.StufenWahlen.Remove(wahl);
                }

                if (fachStufe.Stufe == 1)
                {
                    _context.SpielfigurFachStufen.Remove(fachStufe);
                }
                else
                {
                    fachStufe.Stufe -= 1;
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Stufen), new { id, fach = fachId });
        }

        /// <summary>Gott: Zusatzpunkte für eine Figur (+1 / −1) oder +1 für alle.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> Bonuspunkte(int id, int aenderung, bool alle, int? fach)
        {
            if (alle)
            {
                var alleFiguren = await _context.Spielfiguren.ToListAsync();
                foreach (var f in alleFiguren)
                {
                    f.Bonuspunkte += 1;
                }
                TempData["Success"] = "Alle haben einen Punkt bekommen.";
            }
            else
            {
                var figur = await _context.Spielfiguren.FirstOrDefaultAsync(f => f.Id == id);
                if (figur == null) return NotFound();
                figur.Bonuspunkte = Math.Max(0, figur.Bonuspunkte + Math.Sign(aenderung));
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Stufen), new { id, fach });
        }

        /// <summary>
        /// Gott: Abschlussnote eintragen (hafen, anger, garten, dom). Ein bestandener Abschluss öffnet
        /// den nächsten Abschnitt und bringt Punkte. Leer = kein Abschluss.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> Abschluss(int id, string abschnitt, string? wert, int? fach)
        {
            if (!HesparisRegeln.Abschnitte.Any(a => a.Schluessel == abschnitt)) return BadRequest();
            var erlaubt = new[] { "S", "A", "B", "C", "D", "E", "alt" };
            if (!string.IsNullOrEmpty(wert) && !erlaubt.Contains(wert)) return BadRequest();

            var eintrag = await _context.SchuljahrAbschnitte.FirstOrDefaultAsync(a => a.SpielfigurId == id && a.Abschnitt == abschnitt);
            if (string.IsNullOrEmpty(wert))
            {
                if (eintrag != null)
                {
                    _context.SchuljahrAbschnitte.Remove(eintrag);
                }
            }
            else if (eintrag == null)
            {
                _context.SchuljahrAbschnitte.Add(new SchuljahrAbschnitt { SpielfigurId = id, Abschnitt = abschnitt, Wert = wert });
            }
            else
            {
                eintrag.Wert = wert;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Stufen), new { id, fach });
        }

        // =====================================================================
        // Hilfsmethoden
        // =====================================================================

        /// <summary>
        /// Geometrie des Rings wie in der HTML: 720er Raster, Stufe 0 außen (Radius 330), je Stufe 27 nach innen.
        /// Positionen als Prozent, damit der Ring mit der Breite mitwächst.
        /// </summary>
        private static void RingBerechnen(StufenViewModel model, Spielfigur figur, int frei, int maxStufe)
        {
            int n = model.Faecher.Count;
            double Radius(int k) => 330 - k * 27;
            (double X, double Y) Punkt(int i, double r)
            {
                double a = -Math.PI / 2 + i * Math.PI * 2 / n;
                return (360 + Math.Cos(a) * r, 360 + Math.Sin(a) * r);
            }
            string Prozent(double v) => (v / 720 * 100).ToString("0.###", CultureInfo.InvariantCulture) + "%";
            string Zahl(double v) => v.ToString("0.#", CultureInfo.InvariantCulture);

            model.Polygon = string.Join(" ", model.Faecher.Select((f, i) =>
            {
                var p = Punkt(i, Radius(f.Stufe));
                return Zahl(p.X) + "," + Zahl(p.Y);
            }));

            // Speichen von der Mitte nach außen und leuchtende Pfade bis zur erreichten Stufe
            for (int i = 0; i < n; i++)
            {
                var aussen = Punkt(i, Radius(0));
                model.Speichen.Add(new RingLinie { X1 = "360", Y1 = "360", X2 = Zahl(aussen.X), Y2 = Zahl(aussen.Y) });

                int lv = model.Faecher[i].Stufe;
                if (lv > 0)
                {
                    var innen = Punkt(i, Radius(lv));
                    model.Pfade.Add(new RingLinie { X1 = Zahl(aussen.X), Y1 = Zahl(aussen.Y), X2 = Zahl(innen.X), Y2 = Zahl(innen.Y) });
                }
            }

            // Tore zwischen den Abschnitten, mittig zwischen zwei Speichen (der Dom ist die Mitte selbst)
            double torWinkel = -Math.PI / 2 + Math.PI / n;
            for (int k = 0; k < 3; k++)
            {
                var abschnitt = HesparisRegeln.Abschnitte[k];
                double r = (Radius(abschnitt.Max) + Radius(abschnitt.Max + 1)) / 2;
                model.Tore.Add(new RingTor
                {
                    Name = abschnitt.Tor,
                    Links = Prozent(360 + Math.Cos(torWinkel) * r),
                    Oben = Prozent(360 + Math.Sin(torWinkel) * r),
                    Offen = model.Abschnitt > k + 1,
                });
            }

            for (int i = 0; i < n; i++)
            {
                var fach = model.Faecher[i];
                for (int k = 1; k <= 10; k++)
                {
                    var p = Punkt(i, Radius(k));
                    bool erreicht = k <= fach.Stufe;
                    bool tor = k == 3 || k == 7 || k == 9 || k == 10;
                    bool gesperrt = k > maxStufe;
                    bool naechste = !erreicht && HesparisRegeln.KannWaehlen(figur, fach.Id, k, maxStufe, frei);

                    var klasse = "rg-dot";
                    if (tor) klasse += " gate";
                    if (erreicht) klasse += " lit";
                    if (naechste) klasse += " next";
                    if (gesperrt) klasse += " locked";

                    string zustand = "";
                    if (erreicht) zustand = ", erreicht";
                    else if (naechste) zustand = ", jetzt wählbar";
                    else if (gesperrt) zustand = ", gesperrt";

                    model.Punkte.Add(new RingPunkt
                    {
                        FachId = fach.Id,
                        Stufe = k,
                        Links = Prozent(p.X),
                        Oben = Prozent(p.Y),
                        Klasse = klasse,
                        Beschriftung = fach.Name + ", Stufe " + k + zustand,
                    });
                }

                var l = Punkt(i, 372);
                model.Beschriftungen.Add(new RingBeschriftung
                {
                    FachId = fach.Id,
                    Name = fach.Name,
                    Stufe = fach.Stufe,
                    Links = Prozent(l.X),
                    Oben = Prozent(l.Y),
                    Aktiv = fach.Id == model.Auswahl.Id,
                });
            }
        }
    }
}
