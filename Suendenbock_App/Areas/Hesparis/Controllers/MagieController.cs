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
    /// Gruppe "Magie" (/Hesparis/Magie/…): Magiestufen ("Der Weg nach innen"), Sonderzauber ("Vier Karten, eine Wahl")
    /// , Ätherwaffen und Trankküche.
    /// Stufen wählen und Karten aufdecken dürfen alle Spieler; Bonuspunkte, Abschlüsse und Zurücksetzen nur Gott.
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
        // Sonderzauber ("Vier Karten, eine Wahl")
        // =====================================================================

        private static readonly string[] Roemisch = { "I", "II", "III", "IV", "V", "VI" };

        /// <summary>Höchstens so viele Karten dürfen aufgedeckt werden (JS: revealed.length >= 2).</summary>
        private const int MaxAufgedeckt = 2;

        /// <summary>
        /// Kartentisch. Welcher Zauber offen ist: ?zauber=Code, sonst der der Figur (id), sonst der eigene.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Sonderzauber(int? id, string? zauber)
        {
            var kampagne = await AkademieLadenAsync();
            var alleZauber = await _context.Sonderzauber
                .Include(z => z.Optionen)
                .OrderBy(z => z.Code)
                .ToListAsync();
            if (alleZauber.Count == 0) return View("KeineKampagne");

            var staende = await _context.SonderzauberStaende
                .Where(s => s.KampagneId == kampagne.Id)
                .Include(s => s.Aufgedeckt)
                .ToListAsync();
            var figuren = await _context.Spielfiguren
                .Where(f => f.KampagneId == kampagne.Id && f.HesparisCharakter != null)
                .Include(f => f.HesparisCharakter)
                .OrderBy(f => f.Position)
                .ToListAsync();

            // Reihenfolge der Leiste wie die Türme: nach der Position der Spielfigur, Zauber ohne Figur ans Ende
            int Reihenfolge(Sonderzauber z)
            {
                var f = figuren.FirstOrDefault(x => x.HesparisCharakter!.ErsterVorname == z.Person);
                if (f == null)
                {
                    return int.MaxValue;
                }
                return f.Position;
            }
            alleZauber = alleZauber.OrderBy(Reihenfolge).ToList();

            // Welcher Zauber ist ausgewählt?
            var gewaehlt = alleZauber.FirstOrDefault(z => z.Code == zauber);
            if (gewaehlt == null)
            {
                var figur = figuren.FirstOrDefault(f => f.Id == id) ?? figuren.FirstOrDefault(f => f.HesparisCharakter!.UserId == GetUserId());
                if (figur != null)
                {
                    gewaehlt = alleZauber.FirstOrDefault(z => z.Person == figur.HesparisCharakter!.ErsterVorname);
                }
            }
            gewaehlt ??= alleZauber[0];

            var model = new SonderzauberViewModel
            {
                DarfBearbeiten = DarfBearbeiten(),
                IstSpielleitung = IsGott(),
            };

            foreach (var z in alleZauber)
            {
                var stand = staende.FirstOrDefault(s => s.SonderzauberCode == z.Code);
                var eintrag = new SonderzauberFigur
                {
                    Code = z.Code,
                    Person = z.Person,
                    Titel = z.Titel,
                    Element = z.Titel.Split("— ").Last(),
                    Aussehen = HesparisSigille.Fuer(z.Code),
                    FigurId = figuren.FirstOrDefault(f => f.HesparisCharakter!.ErsterVorname == z.Person)?.Id,
                };

                // Status unter dem Namen (JS: sonderStatus)
                var gewaehlteOption = z.Optionen.FirstOrDefault(o => stand != null && o.Position == stand.GewaehltePosition);
                if (gewaehlteOption != null)
                {
                    eintrag.StatusKlasse = "done";
                    eintrag.StatusText = "✦ " + gewaehlteOption.Name;
                }
                else if (stand != null && stand.Aufgedeckt.Count > 0)
                {
                    eintrag.StatusKlasse = "progress";
                    eintrag.StatusText = stand.Aufgedeckt.Count + " von " + MaxAufgedeckt + " aufgedeckt";
                }
                else
                {
                    eintrag.StatusKlasse = "open";
                    eintrag.StatusText = z.Optionen.Count + " verdeckte Karten";
                }
                model.Figuren.Add(eintrag);
            }
            model.Auswahl = model.Figuren.First(f => f.Code == gewaehlt.Code);
            ViewData["FigurId"] = model.Auswahl.FigurId;

            // Karten des ausgewählten Zaubers
            var aktStand = staende.FirstOrDefault(s => s.SonderzauberCode == gewaehlt.Code);
            var offen = new List<int>();
            int? gewaehltePos = null;
            if (aktStand != null)
            {
                offen = aktStand.Aufgedeckt.Select(a => a.Position).ToList();
                gewaehltePos = aktStand.GewaehltePosition;
            }
            bool fertig = gewaehltePos.HasValue;
            model.Aufgedeckt = offen.Count;
            model.Gewaehlt = fertig;

            // Gerade passiert (für die Animation): "reveal:2" oder "choose:1", kommt aus TempData
            string? justArt = null;
            int? justPos = null;
            if (TempData["SzJust"] is string just && just.Contains(':'))
            {
                var teile = just.Split(':');
                justArt = teile[0];
                justPos = int.Parse(teile[1]);
            }

            foreach (var opt in gewaehlt.Optionen.OrderBy(o => o.Position))
            {
                bool istOffen = offen.Contains(opt.Position);
                bool istGewaehlt = gewaehltePos == opt.Position;
                bool kannAufdecken = !fertig && offen.Count < MaxAufgedeckt && !istOffen && model.DarfBearbeiten;

                var klasse = "sz-card";
                if (istOffen)
                {
                    klasse += " is-revealed";
                }
                else if (fertig)
                {
                    klasse += " is-sealed";
                }
                else if (kannAufdecken)
                {
                    klasse += " can-reveal";
                }
                else
                {
                    klasse += " is-locked";
                }

                if (fertig && istOffen)
                {
                    if (istGewaehlt)
                    {
                        klasse += " is-chosen";
                    }
                    else
                    {
                        klasse += " not-chosen";
                    }
                }

                if (justArt != null && justPos == opt.Position)
                {
                    klasse += " just-" + justArt;
                }
                else if (justArt == "choose")
                {
                    klasse += " just-fade";
                }

                string hinweis;
                if (fertig)
                {
                    hinweis = "für immer verborgen";
                }
                else if (kannAufdecken)
                {
                    hinweis = "Zum Aufdecken antippen";
                }
                else
                {
                    hinweis = "nicht mehr aufdeckbar";
                }

                model.Karten.Add(new SonderzauberKarte
                {
                    Position = opt.Position,
                    Roemisch = opt.Position < Roemisch.Length ? Roemisch[opt.Position] : (opt.Position + 1).ToString(),
                    Option = opt,
                    Klasse = klasse,
                    IstAufgedeckt = istOffen,
                    IstGewaehlt = istGewaehlt,
                    KannAufdecken = kannAufdecken,
                    Versiegelt = fertig,
                    Hinweis = hinweis,
                });
            }

            // Schritte oben rechts (JS: s1, s2, s3)
            if (fertig || offen.Count >= MaxAufgedeckt)
            {
                model.Schritt1 = "done";
            }
            else
            {
                model.Schritt1 = "active";
            }

            if (fertig)
            {
                model.Schritt2 = "done";
                model.Schritt3 = "done";
            }
            else if (offen.Count >= 1)
            {
                model.Schritt2 = "active";
            }

            return View(model);
        }

        /// <summary>Eine Karte aufdecken (JS: revealSonderCard). Höchstens zwei, und nur solange nichts gewählt ist.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SonderzauberAufdecken(string zauber, int position)
        {
            if (!DarfBearbeiten()) return Forbid();
            var stand = await SonderzauberStandAsync(zauber);
            if (stand == null) return NotFound();

            bool erlaubt = stand.GewaehltePosition == null
                           && stand.Aufgedeckt.Count < MaxAufgedeckt
                           && stand.Aufgedeckt.All(a => a.Position != position)
                           && stand.Sonderzauber.Optionen.Any(o => o.Position == position);
            if (erlaubt)
            {
                stand.Aufgedeckt.Add(new SonderzauberAufgedeckt { Position = position });
                await _context.SaveChangesAsync();
                TempData["SzJust"] = "reveal:" + position;
            }
            return RedirectToAction(nameof(Sonderzauber), new { zauber });
        }

        /// <summary>Eine aufgedeckte Karte endgültig wählen (JS: chooseSonderCard). Danach ist nichts mehr änderbar.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SonderzauberWaehlen(string zauber, int position)
        {
            if (!DarfBearbeiten()) return Forbid();
            var stand = await SonderzauberStandAsync(zauber);
            if (stand == null) return NotFound();

            if (stand.GewaehltePosition == null && stand.Aufgedeckt.Any(a => a.Position == position))
            {
                stand.GewaehltePosition = position;
                await _context.SaveChangesAsync();
                var name = stand.Sonderzauber.Optionen.First(o => o.Position == position).Name;
                TempData["Success"] = $"{stand.Sonderzauber.Person} hat „{name}“ gewählt.";
                TempData["SzJust"] = "choose:" + position;
            }
            return RedirectToAction(nameof(Sonderzauber), new { zauber });
        }

        /// <summary>Gott: Kartentisch einer Figur zurücksetzen (alles wieder verdeckt, keine Wahl).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> SonderzauberZuruecksetzen(string zauber)
        {
            var kampagne = await AkademieLadenAsync();
            var stand = await _context.SonderzauberStaende
                .Include(s => s.Aufgedeckt)
                .FirstOrDefaultAsync(s => s.KampagneId == kampagne.Id && s.SonderzauberCode == zauber);
            if (stand != null)
            {
                _context.SonderzauberStaende.Remove(stand);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Die Karten liegen wieder verdeckt.";
            }
            return RedirectToAction(nameof(Sonderzauber), new { zauber });
        }

        /// <summary>Stand eines Sonderzaubers in der Akademie laden, beim ersten Mal anlegen.</summary>
        private async Task<SonderzauberStand?> SonderzauberStandAsync(string zauber)
        {
            var kampagne = await AkademieLadenAsync();
            var sonderzauber = await _context.Sonderzauber
                .Include(z => z.Optionen)
                .FirstOrDefaultAsync(z => z.Code == zauber);
            if (sonderzauber == null) return null;

            var stand = await _context.SonderzauberStaende
                .Include(s => s.Aufgedeckt)
                .FirstOrDefaultAsync(s => s.KampagneId == kampagne.Id && s.SonderzauberCode == zauber);
            if (stand == null)
            {
                stand = new SonderzauberStand { KampagneId = kampagne.Id, SonderzauberCode = zauber };
                _context.SonderzauberStaende.Add(stand);
            }
            stand.Sonderzauber = sonderzauber;
            return stand;
        }

        // =====================================================================
        // Ätherwaffen
        // =====================================================================

        /// <summary>
        /// Ätherwaffen (JS: renderArmory). art = nah, fern oder vert.
        /// Gezogen wird auf dem Server; die Seite spielt danach nur die Glücksrad-Animation ab.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Waffen(string? art)
        {
            var kampagne = await AkademieLadenAsync();
            var kategorie = WaffenRegeln.KategorieAus(art);
            var chronik = await _context.Waffenwuerfe
                .Where(w => w.KampagneId == kampagne.Id)
                .OrderByDescending(w => w.Position)
                .ToListAsync();

            var model = new WaffenViewModel
            {
                Kategorie = kategorie,
                Chronik = chronik,
                DarfBeschwoeren = DarfBearbeiten(),
            };

            // Zuletzt beschworene Waffe dieser Kategorie steht auf dem Altar (JS: lastOf)
            var letzte = chronik.FirstOrDefault(w => w.Waffenart == kategorie.Art);
            if (letzte != null)
            {
                model.Letzte = model.Arsenal.FirstOrDefault(w => w.Name == letzte.Name);
            }

            if (TempData["WaffeNeu"] is int neu && neu >= 0 && neu < model.Arsenal.Length)
            {
                model.Neu = neu;
            }
            return View(model);
        }

        /// <summary>Eine Waffe beschwören (JS: rollWeapon): zufällige Form der Kategorie, oben in die Chronik.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WaffeBeschwoeren(string art)
        {
            if (!DarfBearbeiten()) return Forbid();
            var kampagne = await AkademieLadenAsync();
            var kategorie = WaffenRegeln.KategorieAus(art);
            var liste = WaffenRegeln.Waffen[kategorie.Art];

            int index = Random.Shared.Next(liste.Length);
            var waffe = liste[index];

            var alle = await _context.Waffenwuerfe
                .Where(w => w.KampagneId == kampagne.Id)
                .OrderByDescending(w => w.Position)
                .ToListAsync();
            int position = 0;
            if (alle.Count > 0)
            {
                position = alle[0].Position + 1;
            }
            // Nur die letzten 8 behalten
            _context.Waffenwuerfe.RemoveRange(alle.Skip(WaffenRegeln.ChronikMax - 1));
            _context.Waffenwuerfe.Add(new Waffenwurf
            {
                KampagneId = kampagne.Id,
                Position = position,
                Waffenart = kategorie.Art,
                Name = waffe.Name,
                Schaden = waffe.Schaden,
            });
            await _context.SaveChangesAsync();

            TempData["WaffeNeu"] = index;
            return RedirectToAction(nameof(Waffen), new { art = kategorie.Code });
        }

        /// <summary>Chronik leeren (JS: chronikClear).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChronikLeeren(string? art)
        {
            if (!DarfBearbeiten()) return Forbid();
            var kampagne = await AkademieLadenAsync();
            var alle = await _context.Waffenwuerfe.Where(w => w.KampagneId == kampagne.Id).ToListAsync();
            _context.Waffenwuerfe.RemoveRange(alle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Waffen), new { art });
        }

        // =====================================================================
        // Trankküche
        // =====================================================================

        /// <summary>
        /// Rezeptbuch, Zielwert und Brauen (JS: MODS.brauen). Gebraut wird am Tisch mit echten Würfeln,
        /// die Seite rechnet nur den Zielwert aus und packt die Tränke in die Tasche.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Trankkueche(int? id, int? rezept)
        {
            var figur = await FigurMitTalentenLadenAsync(id);
            if (figur == null) return KeineFigur();

            var kampagne = figur.Kampagne;
            var model = new TrankkuecheViewModel { Gesperrt = kampagne.BrauenGesperrt };

            // Gesperrt: Spieler sehen nur die verschlossene Tür, Gott sieht alles plus Schalter
            if (model.Gesperrt && !IsGott())
            {
                ViewData["FigurId"] = figur.Id;
                return View("TrankkuecheGesperrt");
            }

            // Talentbogen ausrechnen: daraus Metallurgie und die Meisterschaften
            var bogen = await TalenteBerechnenAsync(figur, nameof(Trankkueche));
            model.Kopf = bogen.Kopf;
            var metallurgie = bogen.Begabungen.SelectMany(b => b.Talente).FirstOrDefault(t => t.Name == "Metallurgie");
            if (metallurgie != null)
            {
                model.Metallurgie = metallurgie.AufDemBogen;
                model.SaubererSud = metallurgie.Meisterschaften.Any(m => m.Gewaehlt && m.Name == "Sauberer Sud");
                model.DoppelterKessel = metallurgie.Meisterschaften.Any(m => m.Gewaehlt && m.Name == "Doppelter Kessel");
                model.Meisterbrau = metallurgie.Meisterschaften.Any(m => m.Gewaehlt && m.Name == "Meisterbrau");
            }
            if (GrenzchemieStufe(figur) >= 3)
            {
                model.Schluck = 2;
            }

            // Figurenleiste in den Farben der Figuren
            model.Figuren = await FigurChipsAsync(kampagne.Id, figur.Id);

            // Rezeptbuch (beim ersten Öffnen mit den Standardrezepten füllen)
            var rezepte = await RezepteLadenAsync(kampagne.Id);
            foreach (var r in rezepte)
            {
                model.Rezepte.Add(new RezeptZeile
                {
                    Id = r.Id,
                    Name = r.Name,
                    Wirkung = r.Wirkung,
                    Modifikator = r.Modifikator,
                    Zutaten = r.Zutaten.OrderBy(z => z.Position).Select(z => new ZutatZeile
                    {
                        Name = z.Name,
                        Menge = z.Menge,
                        Vorhanden = AnzahlInTasche(figur, z.Name),
                    }).ToList(),
                });
            }
            model.Auswahl = model.Rezepte.FirstOrDefault(r => r.Id == rezept) ?? model.Rezepte.FirstOrDefault();

            if (model.Auswahl != null)
            {
                int sud = 0;
                if (model.SaubererSud)
                {
                    sud = 20;
                }
                // Ein W100 unter 1 gibt es nicht: mindestens 1 (JS: Math.max(1, t.val))
                model.Zielwert = Math.Max(1, model.Metallurgie + sud + model.Auswahl.Modifikator);
            }

            // Braubuch: die letzten 8 Einträge
            var eintraege = await _context.Braueintraege
                .Where(b => b.KampagneId == kampagne.Id)
                .Include(b => b.Spielfigur).ThenInclude(f => f!.HesparisCharakter)
                .OrderByDescending(b => b.Position)
                .Take(8)
                .ToListAsync();
            model.Braubuch = eintraege.Select(b => new BrauZeile
            {
                Wer = b.Spielfigur?.HesparisCharakter?.Anzeigename ?? "Jemand",
                Ergebnis = b.Ergebnis ?? "ok",
                Text = b.Text,
            }).ToList();

            return View(model);
        }

        /// <summary>
        /// Ergebnis vom Tisch eintragen (JS: data-brew): ok = gelungen, krit = kritisch, fail = misslungen.
        /// Die Zutaten werden in jedem Fall verbraucht.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Brauen(int id, int rezeptId, string ergebnis)
        {
            if (!DarfBearbeiten()) return Forbid();
            var figur = await FigurMitTalentenLadenAsync(id);
            if (figur == null) return NotFound();
            if (figur.Kampagne.BrauenGesperrt && !IsGott()) return Forbid();
            if (ergebnis != "ok" && ergebnis != "krit" && ergebnis != "fail") return BadRequest();

            var rezept = await _context.Rezepte
                .Include(r => r.Zutaten)
                .FirstOrDefaultAsync(r => r.Id == rezeptId && r.KampagneId == figur.KampagneId);
            if (rezept == null) return NotFound();

            // Erst prüfen, ob alles da ist; sonst nichts verbrauchen
            if (rezept.Zutaten.Any(z => AnzahlInTasche(figur, z.Name) < z.Menge))
            {
                TempData["Error"] = "Es fehlen Zutaten.";
                return RedirectToAction(nameof(Trankkueche), new { id, rezept = rezeptId });
            }
            foreach (var z in rezept.Zutaten)
            {
                AusTascheNehmen(figur, z.Name, z.Menge);
            }

            // Meisterschaften für Doppelter Kessel und Meisterbrau
            var bogen = await TalenteBerechnenAsync(figur, nameof(Trankkueche));
            var metallurgie = bogen.Begabungen.SelectMany(b => b.Talente).FirstOrDefault(t => t.Name == "Metallurgie");
            bool kessel = metallurgie?.Meisterschaften.Any(m => m.Gewaehlt && m.Name == "Doppelter Kessel") ?? false;
            bool meisterbrau = metallurgie?.Meisterschaften.Any(m => m.Gewaehlt && m.Name == "Meisterbrau") ?? false;

            int anzahl = 0;
            if (ergebnis != "fail")
            {
                anzahl = 1;
                if (kessel)
                {
                    anzahl = 2;
                }
            }
            bool doppelt = ergebnis == "krit" && meisterbrau;

            string text;
            if (anzahl > 0)
            {
                string notiz = "1 Schluck";
                if (GrenzchemieStufe(figur) >= 3)
                {
                    notiz = "2 Schluck";
                }
                if (doppelt)
                {
                    notiz += " · wirkt doppelt";
                }
                InTascheLegen(figur, rezept.Name, anzahl, notiz);

                text = "braut " + anzahl + "× " + rezept.Name;
                if (doppelt)
                {
                    text += ", doppelt wirksam";
                }
                text += ".";
                TempData["Success"] = $"{figur.HesparisCharakter!.Anzeigename}: {anzahl}× {rezept.Name} liegt in der Tasche.";
            }
            else
            {
                text = "hat den " + rezept.Name + " verpatzt.";
                TempData["Error"] = "Misslungen. Die Zutaten sind verbraucht.";
            }

            await BraubuchEintragenAsync(figur, ergebnis, text);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Trankkueche), new { id, rezept = rezeptId });
        }

        /// <summary>Gott: neues Rezept ins Rezeptbuch (JS: data-recipeadd).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> RezeptHinzufuegen(int id, string name, string wirkung, int modifikator, string? zutaten)
        {
            var kampagne = await AkademieLadenAsync();
            if (string.IsNullOrWhiteSpace(name)) return RedirectToAction(nameof(Trankkueche), new { id });

            var rezepte = await RezepteLadenAsync(kampagne.Id);
            var rezept = new Rezept
            {
                KampagneId = kampagne.Id,
                Position = rezepte.Count,
                Name = name.Trim(),
                Wirkung = wirkung?.Trim(),
                Modifikator = Math.Clamp(modifikator, -30, 0),
            };
            int pos = 0;
            foreach (var (zutat, menge) in HesparisRegeln.ZutatenLesen(zutaten))
            {
                rezept.Zutaten.Add(new RezeptZutat { Position = pos++, Name = zutat, Menge = menge });
            }
            _context.Rezepte.Add(rezept);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"„{rezept.Name}“ steht im Rezeptbuch.";
            return RedirectToAction(nameof(Trankkueche), new { id, rezept = rezept.Id });
        }

        /// <summary>Gott: Rezept wieder entfernen.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> RezeptLoeschen(int id, int rezeptId)
        {
            var kampagne = await AkademieLadenAsync();
            var rezept = await _context.Rezepte.FirstOrDefaultAsync(r => r.Id == rezeptId && r.KampagneId == kampagne.Id);
            if (rezept != null)
            {
                _context.Rezepte.Remove(rezept);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"„{rezept.Name}“ ist aus dem Rezeptbuch entfernt.";
            }
            return RedirectToAction(nameof(Trankkueche), new { id });
        }

        /// <summary>Gott: Trankküche für Spieler sperren oder öffnen (JS: brewLock).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> TrankkuecheSperren(int? id)
        {
            var kampagne = await AkademieLadenAsync();
            kampagne.BrauenGesperrt = !kampagne.BrauenGesperrt;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Trankkueche), new { id });
        }

        /// <summary>Rezepte der Akademie; beim ersten Mal die Standardrezepte anlegen.</summary>
        private async Task<List<Rezept>> RezepteLadenAsync(int kampagneId)
        {
            var rezepte = await _context.Rezepte
                .Where(r => r.KampagneId == kampagneId)
                .Include(r => r.Zutaten)
                .OrderBy(r => r.Position)
                .ToListAsync();
            if (rezepte.Count > 0) return rezepte;

            int pos = 0;
            foreach (var (name, wirkung, mod, zutaten) in HesparisRegeln.RezepteStandard)
            {
                var r = new Rezept { KampagneId = kampagneId, Position = pos++, Name = name, Wirkung = wirkung, Modifikator = mod };
                int zp = 0;
                foreach (var (zutat, menge) in zutaten)
                {
                    r.Zutaten.Add(new RezeptZutat { Position = zp++, Name = zutat, Menge = menge });
                }
                _context.Rezepte.Add(r);
                rezepte.Add(r);
            }
            await _context.SaveChangesAsync();
            return rezepte;
        }

        /// <summary>Figurenknöpfe mit Farbe aus dem Sonderzauber (JS: mdPlayers / plCol).</summary>
        private async Task<List<FigurChip>> FigurChipsAsync(int kampagneId, int aktivId)
        {
            var figuren = await _context.Spielfiguren
                .Where(f => f.KampagneId == kampagneId && f.HesparisCharakter != null)
                .Include(f => f.HesparisCharakter)
                .OrderBy(f => f.Position)
                .ToListAsync();
            var sonderzauber = await _context.Sonderzauber.ToListAsync();

            return figuren.Select(f =>
            {
                var name = f.HesparisCharakter!.Anzeigename;
                var zauber = sonderzauber.FirstOrDefault(z => z.Person == name);
                return new FigurChip
                {
                    Id = f.Id,
                    Name = name,
                    Aussehen = HesparisSigille.Fuer(zauber?.Code),
                    Aktiv = f.Id == aktivId,
                };
            }).ToList();
        }

        private static int GrenzchemieStufe(Spielfigur figur) =>
            figur.FachStufen.FirstOrDefault(s => s.FachId == HesparisRegeln.FachGrenzchemie)?.Stufe ?? 0;

        /// <summary>Wie viele Stück mit diesem Namen in der Tasche liegen (JS: itemCount).</summary>
        private static int AnzahlInTasche(Spielfigur figur, string name) =>
            figur.Gegenstaende.Where(g => g.Name == name).Sum(g => g.Menge);

        /// <summary>Menge aus der Tasche nehmen, leere Stapel verschwinden (JS: takeItem).</summary>
        private void AusTascheNehmen(Spielfigur figur, string name, int menge)
        {
            int rest = menge;
            foreach (var g in figur.Gegenstaende.Where(g => g.Name == name).OrderBy(g => g.Id).ToList())
            {
                if (rest <= 0) break;
                int weg = Math.Min(rest, g.Menge);
                g.Menge -= weg;
                rest -= weg;
                if (g.Menge <= 0)
                {
                    figur.Gegenstaende.Remove(g);
                    _context.Gegenstaende.Remove(g);
                }
            }
        }

        /// <summary>
        /// Trank in die Tasche legen; gleicher Trank mit gleicher Notiz und ohne eigene Wirkung
        /// kommt auf denselben Stapel (JS: addItem).
        /// </summary>
        private static void InTascheLegen(Spielfigur figur, string name, int menge, string notiz)
        {
            var stapel = figur.Gegenstaende.FirstOrDefault(g =>
                g.Name == name && g.Kategorie == "Trank" && (g.Notiz ?? "") == notiz && !HesparisRegeln.EffektGueltig(g.Effekt));
            if (stapel != null)
            {
                stapel.Menge += menge;
                stapel.Neu = true;
                return;
            }
            figur.Gegenstaende.Add(new Gegenstand
            {
                Name = name,
                Kategorie = "Trank",
                Menge = menge,
                Notiz = notiz,
                Quelle = "brau",
                Neu = true,
            });
        }

        /// <summary>Neuer Eintrag oben ins Braubuch, ältere über 30 fallen weg.</summary>
        private async Task BraubuchEintragenAsync(Spielfigur figur, string ergebnis, string text)
        {
            var alle = await _context.Braueintraege
                .Where(b => b.KampagneId == figur.KampagneId)
                .OrderByDescending(b => b.Position)
                .ToListAsync();
            int neuePosition = 0;
            if (alle.Count > 0)
            {
                neuePosition = alle[0].Position + 1;
            }
            foreach (var alt in alle.Skip(HesparisRegeln.BraubuchMax - 1))
            {
                _context.Braueintraege.Remove(alt);
            }
            _context.Braueintraege.Add(new Braueintrag
            {
                KampagneId = figur.KampagneId,
                Position = neuePosition,
                SpielfigurId = figur.Id,
                Ergebnis = ergebnis,
                Text = text,
            });
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
