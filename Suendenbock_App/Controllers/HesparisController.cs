using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain.Hesparis;
using Suendenbock_App.Models.ViewModels.Hesparis;

namespace Suendenbock_App.Controllers
{
    /// <summary>
    /// Hesparis-Akademie, Bereich "Figur": Startseite, Talente, Zeugnis, Tasche.
    /// Ansehen dürfen alle Figuren der eigenen Kampagne, bearbeiten nur die eigene Figur (oder Gott).
    /// </summary>
    [Authorize]
    public class HesparisController : BaseController
    {
        public HesparisController(ApplicationDbContext context) : base(context)
        {
        }

        // =====================================================================
        // Startseite
        // =====================================================================

        [HttpGet]
        public async Task<IActionResult> Index(int? kampagneId)
        {
            var kampagne = await KampagneFuerBenutzerAsync(kampagneId);
            if (kampagne == null)
                return View("KeineKampagne");

            var figuren = await _context.Spielfiguren
                .Where(f => f.KampagneId == kampagne.Id)
                .Include(f => f.Character)
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

        /// <summary>Lebenspunkte ändern (eigene Figur oder Gott).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lebenspunkte(int id, int aktuell, int max)
        {
            var figur = await _context.Spielfiguren.FirstOrDefaultAsync(f => f.Id == id);
            if (figur == null) return NotFound();
            if (!await DarfBearbeitenAsync(figur)) return Forbid();

            figur.LebenspunkteMax = Math.Max(1, max);
            figur.LebenspunkteAktuell = Math.Clamp(aktuell, 0, figur.LebenspunkteMax);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { kampagneId = figur.KampagneId });
        }

        // =====================================================================
        // Talente
        // =====================================================================

        [HttpGet]
        public async Task<IActionResult> Talente(int? id)
        {
            var figur = await FigurMitTalentenLadenAsync(id);
            if (figur == null) return KeineFigur();
            if (!await DarfAnsehenAsync(figur)) return Forbid();

            return View(await TalenteBerechnenAsync(figur));
        }

        /// <summary>Talentpunkte speichern. Erwartet Felder punkte[h0], punkte[w3] …</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Talente(int id, Dictionary<string, int> punkte)
        {
            var figur = await _context.Spielfiguren.Include(f => f.Talente).FirstOrDefaultAsync(f => f.Id == id);
            if (figur == null) return NotFound();
            if (!await DarfBearbeitenAsync(figur)) return Forbid();

            var gueltigeCodes = await _context.Talente.Select(t => t.Code).ToListAsync();
            foreach (var code in gueltigeCodes)
            {
                if (!punkte.TryGetValue(code, out var wert)) continue;
                wert = HesparisRegeln.PunkteNormalisieren(wert);

                var vorhanden = figur.Talente.FirstOrDefault(t => t.TalentCode == code);
                if (vorhanden == null)
                {
                    if (wert != 0)
                        figur.Talente.Add(new TalentWert { TalentCode = code, Wert = wert });
                }
                else if (wert == 0)
                {
                    _context.TalentWerte.Remove(vorhanden);
                }
                else
                {
                    vorhanden.Wert = wert;
                }
            }

            int verteilt = figur.Talente
                .Where(t => _context.Entry(t).State != EntityState.Deleted)
                .Sum(t => t.Wert);

            await _context.SaveChangesAsync();
            if (verteilt > HesparisRegeln.TalentBudget)
                TempData["Error"] = $"Gespeichert, aber {verteilt} von {HesparisRegeln.TalentBudget} Punkten verteilt. Irgendwo muss etwas runter.";
            else
                TempData["Success"] = "Talente gespeichert.";

            return RedirectToAction(nameof(Talente), new { id });
        }

        /// <summary>Bei einem Bonus mit mehreren möglichen Talenten das Ziel wählen (JS: talentZiel).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TalentZiel(int id, string schluessel, string? talentCode)
        {
            var figur = await _context.Spielfiguren
                .Include(f => f.TalentZiele)
                .Include(f => f.FachStufen)
                .Include(f => f.StufenWahlen)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (figur == null) return NotFound();
            if (!await DarfBearbeitenAsync(figur)) return Forbid();

            // Nur Ziele erlauben, die der Bonus tatsächlich anbietet
            var faecher = await _context.Faecher.Include(f => f.Stufen).ToListAsync();
            var bonus = HesparisRegeln.FachBoni(figur, faecher).FirstOrDefault(b => b.Schluessel == schluessel);
            if (bonus == null) return BadRequest();

            var vorhanden = figur.TalentZiele.FirstOrDefault(z => z.Schluessel == schluessel);
            if (string.IsNullOrEmpty(talentCode))
            {
                if (vorhanden != null) _context.TalentZiele.Remove(vorhanden);
            }
            else
            {
                if (!bonus.Kandidaten.Contains(talentCode)) return BadRequest();
                if (vorhanden == null)
                    figur.TalentZiele.Add(new TalentZiel { Schluessel = schluessel, TalentCode = talentCode });
                else
                    vorhanden.TalentCode = talentCode;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Talente), new { id });
        }

        /// <summary>Meisterschaft wählen oder abwählen (JS: openMst → data-mston / data-mstoff).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Meisterschaft(int id, int meisterschaftId, bool waehlen)
        {
            var figur = await _context.Spielfiguren
                .Include(f => f.Meisterschaften)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (figur == null) return NotFound();
            if (!await DarfBearbeitenAsync(figur)) return Forbid();

            var vorhanden = figur.Meisterschaften.FirstOrDefault(m => m.MeisterschaftDefinitionId == meisterschaftId);

            if (!waehlen)
            {
                if (vorhanden != null) _context.SpielfigurMeisterschaften.Remove(vorhanden);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Talente), new { id });
            }

            if (vorhanden != null)
                return RedirectToAction(nameof(Talente), new { id });

            // Ob noch ein Platz frei ist, hängt vom Talentwert inklusive aller Boni ab
            var mitTalenten = await FigurMitTalentenLadenAsync(id);
            var model = await TalenteBerechnenAsync(mitTalenten!);
            var talent = model.Begabungen.SelectMany(b => b.Talente)
                .FirstOrDefault(t => t.Meisterschaften.Any(m => m.Id == meisterschaftId));

            if (talent == null) return BadRequest();
            if (!model.MeisterschaftenFreigeschaltet)
            {
                TempData["Error"] = "Meisterschaften werden erst mit 3 Punkten in Chronotaktik freigeschaltet.";
            }
            else if (talent.FreieMeisterschaften <= 0)
            {
                TempData["Error"] = $"Für {talent.Name} ist gerade keine Meisterschaft frei.";
            }
            else
            {
                _context.SpielfigurMeisterschaften.Add(new SpielfigurMeisterschaft
                {
                    SpielfigurId = figur.Id,
                    MeisterschaftDefinitionId = meisterschaftId,
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Talente), new { id });
        }

        // =====================================================================
        // Zeugnis
        // =====================================================================

        [HttpGet]
        public async Task<IActionResult> Zeugnis(int? id)
        {
            var figur = await FigurLadenAsync(id, q => q
                .Include(f => f.Noten)
                .Include(f => f.FachStufen)
                .Include(f => f.Pruefungen));
            if (figur == null) return KeineFigur();
            if (!await DarfAnsehenAsync(figur)) return Forbid();

            var faecher = await _context.Faecher.OrderBy(f => f.Id).ToListAsync();
            var model = new ZeugnisViewModel { Kopf = await KopfFuerFigurAsync(figur, nameof(Zeugnis)) };

            foreach (var fach in faecher)
            {
                var noten = figur.Noten.Where(n => n.FachId == fach.Id).OrderBy(n => n.Position).ToList();

                // Fachpunkte: je A oder S in den Abschlussprüfungen ein Punkt (JS: fachPunkte).
                // Der Anteil aus Klassenarbeiten (JS: sjArbZ) kommt mit dem Bereich "Alltag" dazu.
                int fachpunkte = figur.Pruefungen.Count(p =>
                    HesparisRegeln.FachpunktAbschnitte.Contains(p.Abschnitt) &&
                    p.Teil == fach.Id.ToString() &&
                    (p.Wert == "A" || p.Wert == "S"));

                model.Faecher.Add(new ZeugnisFach
                {
                    FachId = fach.Id,
                    Name = fach.Name,
                    Noten = noten,
                    Schnitt = HesparisRegeln.Schnitt(noten.Select(n => n.Note)),
                    Stufe = figur.FachStufen.FirstOrDefault(s => s.FachId == fach.Id)?.Stufe ?? 0,
                    Fachpunkte = fachpunkte,
                });
            }

            var schnitte = model.Faecher.Where(f => f.Schnitt.HasValue).Select(f => f.Schnitt!.Value).ToList();
            model.Gesamtschnitt = schnitte.Count > 0 ? schnitte.Average() : null;
            model.FachpunkteGesamt = model.Faecher.Sum(f => f.Fachpunkte);
            model.FachpunkteFrei = model.Faecher.Sum(f => f.FachpunkteFrei);

            return View(model);
        }

        /// <summary>Note eintragen. Nur die Spielleitung (wie in der HTML).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> NoteHinzufuegen(int id, int fachId, Note note)
        {
            var figur = await _context.Spielfiguren.Include(f => f.Noten).FirstOrDefaultAsync(f => f.Id == id);
            if (figur == null) return NotFound();
            if (!await _context.Faecher.AnyAsync(f => f.Id == fachId)) return BadRequest();

            int position = figur.Noten.Where(n => n.FachId == fachId).Select(n => n.Position).DefaultIfEmpty(-1).Max() + 1;
            figur.Noten.Add(new Schulnote { FachId = fachId, Note = note, Position = position });
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Zeugnis), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> NoteEntfernen(int id, int noteId)
        {
            var note = await _context.Schulnoten.FirstOrDefaultAsync(n => n.Id == noteId && n.SpielfigurId == id);
            if (note != null)
            {
                _context.Schulnoten.Remove(note);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Zeugnis), new { id });
        }

        // =====================================================================
        // Tasche
        // =====================================================================

        [HttpGet]
        public async Task<IActionResult> Tasche(int? id, int? bearbeiten)
        {
            var figur = await FigurLadenAsync(id, q => q
                .Include(f => f.Gegenstaende)
                .Include(f => f.Geldbuchungen)
                .Include(f => f.Wirkungen));
            if (figur == null) return KeineFigur();
            if (!await DarfAnsehenAsync(figur)) return Forbid();

            var spieltag = figur.Kampagne.Spieltag;
            var model = new TascheViewModel
            {
                Kopf = await KopfFuerFigurAsync(figur, nameof(Tasche)),
                Geld = figur.Geld,
                Muenzen = HesparisRegeln.MuenzenAufteilen(figur.Geld),
                Buchungen = figur.Geldbuchungen.OrderBy(b => b.Position).ToList(),
                Wirkungen = figur.Wirkungen
                    .Where(w => HesparisRegeln.WirkungAktiv(w, spieltag))
                    .Select(w => (w, HesparisRegeln.WirkungRest(w, spieltag)))
                    .ToList(),
                TalentNamen = await _context.Talente.ToDictionaryAsync(t => t.Code, t => t.Name),
            };

            foreach (var kategorie in HesparisRegeln.Kategorien)
            {
                var liste = figur.Gegenstaende.Where(g => g.Kategorie == kategorie).OrderBy(g => g.Name).ToList();
                if (liste.Count > 0) model.Gruppen.Add((kategorie, liste));
            }
            // Ältere Einträge mit unbekannter Art landen unter "Sonstiges"
            var unbekannt = figur.Gegenstaende.Where(g => !HesparisRegeln.Kategorien.Contains(g.Kategorie)).ToList();
            if (unbekannt.Count > 0) model.Gruppen.Add(("Weitere", unbekannt));

            var zuBearbeiten = figur.Gegenstaende.FirstOrDefault(g => g.Id == bearbeiten);
            if (zuBearbeiten != null)
            {
                model.Formular = new GegenstandFormular
                {
                    Id = zuBearbeiten.Id,
                    Name = zuBearbeiten.Name,
                    Kategorie = zuBearbeiten.Kategorie,
                    Menge = zuBearbeiten.Menge,
                    Notiz = zuBearbeiten.Notiz,
                    Quelle = zuBearbeiten.Quelle,
                    Preis = zuBearbeiten.Preis,
                    EffektArt = zuBearbeiten.Effekt.Art,
                    EffektTalentCode = zuBearbeiten.Effekt.TalentCode,
                    EffektWert = zuBearbeiten.Effekt.Wert,
                    EffektVorteilAuf = zuBearbeiten.Effekt.VorteilAuf,
                    EffektDauerArt = zuBearbeiten.Effekt.DauerArt,
                    EffektDauerAnzahl = zuBearbeiten.Effekt.DauerAnzahl,
                };
            }

            return View(model);
        }

        /// <summary>Geld einnehmen oder ausgeben. Die letzten fünf Buchungen bleiben stehen (JS: tLog).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Geld(int id, GeldFormular formular)
        {
            var figur = await _context.Spielfiguren.Include(f => f.Geldbuchungen).FirstOrDefaultAsync(f => f.Id == id);
            if (figur == null) return NotFound();
            if (!await DarfBearbeitenAsync(figur)) return Forbid();

            int betrag = Math.Abs(formular.Kerzen) * (formular.Einnahme ? 1 : -1);
            if (betrag == 0)
            {
                TempData["Error"] = "Kein Betrag angegeben.";
                return RedirectToAction(nameof(Tasche), new { id });
            }
            if (figur.Geld + betrag < 0)
            {
                TempData["Error"] = "So viel ist nicht im Geldbeutel.";
                return RedirectToAction(nameof(Tasche), new { id });
            }

            figur.Geld += betrag;
            BuchungEintragen(figur, betrag, string.IsNullOrWhiteSpace(formular.Grund) ? (betrag > 0 ? "Einnahme" : "Ausgabe") : formular.Grund.Trim());

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Tasche), new { id });
        }

        /// <summary>Gegenstand anlegen oder ändern.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GegenstandSpeichern(int id, GegenstandFormular formular)
        {
            var figur = await _context.Spielfiguren.Include(f => f.Gegenstaende).FirstOrDefaultAsync(f => f.Id == id);
            if (figur == null) return NotFound();
            if (!await DarfBearbeitenAsync(figur)) return Forbid();

            if (string.IsNullOrWhiteSpace(formular.Name))
            {
                TempData["Error"] = "Der Gegenstand braucht einen Namen.";
                return RedirectToAction(nameof(Tasche), new { id });
            }

            Gegenstand gegenstand;
            if (formular.Id.HasValue)
            {
                var vorhanden = figur.Gegenstaende.FirstOrDefault(g => g.Id == formular.Id.Value);
                if (vorhanden == null) return NotFound();
                gegenstand = vorhanden;
            }
            else
            {
                gegenstand = new Gegenstand { Neu = true };
                figur.Gegenstaende.Add(gegenstand);
            }

            gegenstand.Name = formular.Name.Trim();
            gegenstand.Kategorie = HesparisRegeln.Kategorien.Contains(formular.Kategorie) ? formular.Kategorie : "Sonstiges";
            gegenstand.Menge = Math.Max(1, formular.Menge);
            gegenstand.Notiz = formular.Notiz;
            gegenstand.Quelle = formular.Quelle;
            gegenstand.Preis = formular.Preis;
            gegenstand.Effekt = EffektAusFormular(formular);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Tasche), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GegenstandEntfernen(int id, int gegenstandId)
        {
            var figur = await _context.Spielfiguren.FirstOrDefaultAsync(f => f.Id == id);
            if (figur == null) return NotFound();
            if (!await DarfBearbeitenAsync(figur)) return Forbid();

            var gegenstand = await _context.Gegenstaende.FirstOrDefaultAsync(g => g.Id == gegenstandId && g.SpielfigurId == id);
            if (gegenstand != null)
            {
                _context.Gegenstaende.Remove(gegenstand);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Tasche), new { id });
        }

        /// <summary>
        /// Trank, Zutat oder Süßes benutzen: einer weniger in der Tasche,
        /// die Wirkung startet am aktuellen Spieltag (JS: fxUse).
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GegenstandBenutzen(int id, int gegenstandId)
        {
            var figur = await _context.Spielfiguren
                .Include(f => f.Kampagne)
                .Include(f => f.Gegenstaende)
                .Include(f => f.Wirkungen)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (figur == null) return NotFound();
            if (!await DarfBearbeitenAsync(figur)) return Forbid();

            var gegenstand = figur.Gegenstaende.FirstOrDefault(g => g.Id == gegenstandId);
            if (gegenstand == null) return NotFound();

            var fx = await EffektMitLadenAsync(gegenstand, figur.KampagneId);

            gegenstand.Menge -= 1;
            gegenstand.Neu = false;
            if (gegenstand.Menge <= 0)
                _context.Gegenstaende.Remove(gegenstand);

            if (fx != null)
            {
                figur.Wirkungen.Add(new Wirkung
                {
                    Name = gegenstand.Name,
                    Effekt = EffektKopie(fx),
                    Ab = figur.Kampagne.Spieltag,
                    Restanwendungen = fx.DauerArt == EffektDauerArt.Anwendungen ? (fx.DauerAnzahl ?? 1) : null,
                });
                TempData["Success"] = $"{gegenstand.Name} benutzt, die Wirkung läuft.";
            }
            else
            {
                TempData["Success"] = $"{gegenstand.Name} benutzt.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Tasche), new { id });
        }

        /// <summary>Eine Anwendung einer Wirkung verbrauchen.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WirkungAnwenden(int id, int wirkungId, string? zurueck)
        {
            var figur = await _context.Spielfiguren.FirstOrDefaultAsync(f => f.Id == id);
            if (figur == null) return NotFound();
            if (!await DarfBearbeitenAsync(figur)) return Forbid();

            var wirkung = await _context.Wirkungen.FirstOrDefaultAsync(w => w.Id == wirkungId && w.SpielfigurId == id);
            if (wirkung != null)
            {
                wirkung.Restanwendungen = (wirkung.Restanwendungen ?? 1) - 1;
                if (wirkung.Restanwendungen <= 0)
                    _context.Wirkungen.Remove(wirkung);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(zurueck == nameof(Talente) ? nameof(Talente) : nameof(Tasche), new { id });
        }

        /// <summary>Wirkung ist vorbei.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WirkungBeenden(int id, int wirkungId, string? zurueck)
        {
            var figur = await _context.Spielfiguren.FirstOrDefaultAsync(f => f.Id == id);
            if (figur == null) return NotFound();
            if (!await DarfBearbeitenAsync(figur)) return Forbid();

            var wirkung = await _context.Wirkungen.FirstOrDefaultAsync(w => w.Id == wirkungId && w.SpielfigurId == id);
            if (wirkung != null)
            {
                _context.Wirkungen.Remove(wirkung);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(zurueck == nameof(Talente) ? nameof(Talente) : nameof(Tasche), new { id });
        }

        // =====================================================================
        // Hilfsmethoden
        // =====================================================================

        /// <summary>
        /// Welche Kampagne ein Benutzer sieht:
        /// Gott: die gewünschte oder die neueste. Spieler: eine Kampagne, in der einer seiner Characters mitspielt,
        /// bevorzugt die mit seinem aktiven Character.
        /// (Nicht über GetCurrentGameWorldAsync, weil das bei Characters in mehreren Welten immer 1618 liefert.)
        /// </summary>
        private async Task<Kampagne?> KampagneFuerBenutzerAsync(int? kampagneId)
        {
            if (IsGott())
            {
                var alle = _context.Kampagnen.Include(k => k.GameWorld);
                if (kampagneId.HasValue)
                    return await alle.FirstOrDefaultAsync(k => k.Id == kampagneId.Value);
                return await alle.OrderByDescending(k => k.Id).FirstOrDefaultAsync();
            }

            var userId = GetUserId();
            var eigene = await _context.Spielfiguren
                .Where(f => f.Character.UserId == userId)
                .Select(f => new { f.KampagneId, f.CharacterId })
                .ToListAsync();
            if (eigene.Count == 0) return null;

            int gewaehlt;
            if (kampagneId.HasValue && eigene.Any(e => e.KampagneId == kampagneId.Value))
            {
                gewaehlt = kampagneId.Value;
            }
            else
            {
                var aktiv = await GetCurrentCharacterAsync();
                gewaehlt = eigene.FirstOrDefault(e => aktiv != null && e.CharacterId == aktiv.Id)?.KampagneId
                           ?? eigene.Max(e => e.KampagneId);
            }

            return await _context.Kampagnen.Include(k => k.GameWorld).FirstOrDefaultAsync(k => k.Id == gewaehlt);
        }

        /// <summary>
        /// Lädt eine Figur samt Kampagne und Character. Ohne Id: die eigene Figur
        /// (bzw. bei Gott die erste Figur der neuesten Kampagne).
        /// </summary>
        private async Task<Spielfigur?> FigurLadenAsync(int? id, Func<IQueryable<Spielfigur>, IQueryable<Spielfigur>> includes)
        {
            if (!id.HasValue)
            {
                var kampagne = await KampagneFuerBenutzerAsync(null);
                if (kampagne == null) return null;

                var userId = GetUserId();
                id = await _context.Spielfiguren
                    .Where(f => f.KampagneId == kampagne.Id)
                    .OrderByDescending(f => f.Character.UserId == userId)
                    .ThenBy(f => f.Position)
                    .Select(f => (int?)f.Id)
                    .FirstOrDefaultAsync();
                if (!id.HasValue) return null;
            }

            var query = _context.Spielfiguren
                .Include(f => f.Kampagne).ThenInclude(k => k.GameWorld)
                .Include(f => f.Character)
                .AsSplitQuery();
            return await includes(query).FirstOrDefaultAsync(f => f.Id == id.Value);
        }

        private IActionResult KeineFigur() => View("KeineKampagne");

        private Task<Spielfigur?> FigurMitTalentenLadenAsync(int? id) => FigurLadenAsync(id, q => q
            .Include(f => f.Talente)
            .Include(f => f.TalentZiele)
            .Include(f => f.FachStufen)
            .Include(f => f.StufenWahlen)
            .Include(f => f.Meisterschaften)
            .Include(f => f.Gegenstaende)
            .Include(f => f.Wirkungen));

        /// <summary>
        /// Rechnet den kompletten Talentbogen aus (JS: talCalc): Punkte, Kopfwerte und alle Boni.
        /// Wird von der Talente-Seite und beim Wählen einer Meisterschaft benutzt.
        /// </summary>
        private async Task<TalenteViewModel> TalenteBerechnenAsync(Spielfigur figur)
        {
            var kampagne = figur.Kampagne;
            var begabungen = await _context.Begabungen
                .Include(b => b.Talente).ThenInclude(t => t.Meisterschaften)
                .AsSplitQuery()
                .ToListAsync();
            var faecher = await _context.Faecher.Include(f => f.Stufen).ToListAsync();

            // Alle Boni sammeln: Magiestufen, AGs, Tasche, Wirkungen (JS: talCalc)
            var boni = HesparisRegeln.FachBoni(figur, faecher);
            boni.AddRange(await AgBoniAsync(figur));
            var (tascheBoni, vorteile) = await TascheBoniAsync(figur, kampagne);
            boni.AddRange(tascheBoni);

            var talentNamen = begabungen.SelectMany(b => b.Talente).ToDictionary(t => t.Code, t => t.Name);
            var punkte = figur.Talente.ToDictionary(t => t.TalentCode, t => t.Wert);
            var gewaehlt = figur.Meisterschaften.Select(m => m.MeisterschaftDefinitionId).ToHashSet();

            var model = new TalenteViewModel
            {
                Kopf = await KopfFuerFigurAsync(figur, nameof(Talente)),
                Boni = boni,
                Vorteile = vorteile,
                TalentNamen = talentNamen,
                MeisterschaftenFreigeschaltet =
                    (figur.FachStufen.FirstOrDefault(s => s.FachId == HesparisRegeln.FachChronotaktik)?.Stufe ?? 0) >= 3,
            };

            foreach (var b in begabungen.OrderBy(b => "hws".IndexOf(b.Code)))
            {
                var talente = b.Talente.OrderBy(t => t.Position).ToList();
                int summe = talente.Sum(t => punkte.GetValueOrDefault(t.Code));
                int kopfwert = HesparisRegeln.Kopfwert(summe);

                var zeile = new BegabungZeile
                {
                    Code = b.Code,
                    Name = b.Name,
                    Farbe = HesparisRegeln.BegabungFarbe.GetValueOrDefault(b.Code, "#e7d6a3"),
                    Summe = summe,
                    Kopfwert = kopfwert,
                    Geistesblitze = HesparisRegeln.Geistesblitze(kopfwert),
                };

                foreach (var t in talente)
                {
                    var talentBoni = boni.Where(x => x.Ziel == t.Code).ToList();
                    int bonus = talentBoni.Sum(x => x.Betrag);
                    int p = punkte.GetValueOrDefault(t.Code);
                    zeile.Talente.Add(new TalentZeile
                    {
                        Code = t.Code,
                        Name = t.Name,
                        Punkte = p,
                        Bonus = bonus,
                        Boni = talentBoni,
                        Gesamt = p + kopfwert + bonus,
                        Meisterschaften = t.Meisterschaften.OrderBy(m => m.Position).Select(m => new MeisterschaftZeile
                        {
                            Id = m.Id,
                            Name = m.Name,
                            Beschreibung = m.Beschreibung,
                            Gewaehlt = gewaehlt.Contains(m.Id),
                        }).ToList(),
                    });
                }

                model.Verteilt += summe;
                model.Begabungen.Add(zeile);
            }

            return model;
        }

        /// <summary>Ansehen: Gott oder jemand, der in derselben Kampagne mitspielt.</summary>
        private async Task<bool> DarfAnsehenAsync(Spielfigur figur)
        {
            if (IsGott()) return true;
            var userId = GetUserId();
            return await _context.Spielfiguren.AnyAsync(f => f.KampagneId == figur.KampagneId && f.Character.UserId == userId);
        }

        /// <summary>Bearbeiten: eigene Figur (Character.UserId) oder Gott. Nutzt die Regel aus dem BaseController.</summary>
        private async Task<bool> DarfBearbeitenAsync(Spielfigur figur) => await CanEditCharacterAsync(figur.CharacterId);

        private Spielfigur StandardFigur(List<Spielfigur> figuren)
        {
            var userId = GetUserId();
            return figuren.FirstOrDefault(f => f.Character.UserId == userId) ?? figuren[0];
        }

        private FigurKopfViewModel KopfErstellen(Kampagne kampagne, List<Spielfigur> figuren, Spielfigur aktiv, string aktion)
        {
            var userId = GetUserId();
            return new FigurKopfViewModel
            {
                Kampagne = kampagne,
                AktiveFigurId = aktiv.Id,
                Aktion = aktion,
                IstSpielleitung = IsGott(),
                DarfBearbeiten = IsGott() || aktiv.Character.UserId == userId,
                Figuren = figuren.Select(f => new FigurKurz
                {
                    Id = f.Id,
                    CharacterId = f.CharacterId,
                    Name = FigurKurz.NameVon(f.Character),
                    Bild = f.Character.ImagePath,
                    Farbe = f.Character.UserColor,
                    Eigene = f.Character.UserId == userId,
                }).ToList(),
            };
        }

        private async Task<FigurKopfViewModel> KopfFuerFigurAsync(Spielfigur figur, string aktion)
        {
            var figuren = await _context.Spielfiguren
                .Where(f => f.KampagneId == figur.KampagneId)
                .Include(f => f.Character)
                .OrderBy(f => f.Position)
                .ToListAsync();
            return KopfErstellen(figur.Kampagne, figuren, figuren.First(f => f.Id == figur.Id), aktion);
        }

        /// <summary>Boni aus AGs, in denen die Figur Mitglied ist (JS: agBoni).</summary>
        private async Task<List<TalentBonus>> AgBoniAsync(Spielfigur figur)
        {
            var ags = await _context.AgMitglieder
                .Where(m => m.SpielfigurId == figur.Id && m.AgEintrag.KampagneId == figur.KampagneId)
                .Select(m => m.AgEintrag.Ag)
                .Where(a => a.BonusTalentCode != null && a.BonusWert != 0)
                .ToListAsync();

            return ags.Select(a => new TalentBonus
            {
                Quelle = a.Name,
                Herkunft = "AG",
                Betrag = a.BonusWert,
                Beschriftung = "+" + a.BonusWert,
                Kandidaten = new List<string> { a.BonusTalentCode! },
                Ziel = a.BonusTalentCode,
            }).ToList();
        }

        /// <summary>
        /// Boni und Vorteile aus der Tasche (Bücher, Ausrüstung, Sonstiges) und aus laufenden Wirkungen
        /// (JS: fxActive / itemBoni). Gegenstände ohne eigene Wirkung übernehmen die Wirkung der
        /// gleichnamigen Ware im Schulladen.
        /// </summary>
        private async Task<(List<TalentBonus> Boni, List<TalentBonus> Vorteile)> TascheBoniAsync(Spielfigur figur, Kampagne kampagne)
        {
            var laden = await _context.LadenArtikel.Where(l => l.KampagneId == figur.KampagneId).ToListAsync();
            var aktiv = new List<(string Name, Effekt Fx, Wirkung? Wirkung, string Herkunft)>();

            foreach (var g in figur.Gegenstaende.Where(g => g.Menge > 0 && HesparisRegeln.PassiveKategorien.Contains(g.Kategorie)))
            {
                var fx = HesparisRegeln.EffektGueltig(g.Effekt) ? g.Effekt : laden.FirstOrDefault(l => l.Name == g.Name)?.Effekt;
                if (HesparisRegeln.EffektGueltig(fx)) aktiv.Add((g.Name, fx!, null, "in der Tasche"));
            }
            foreach (var w in figur.Wirkungen.Where(w => HesparisRegeln.WirkungAktiv(w, kampagne.Spieltag)))
                aktiv.Add((w.Name, w.Effekt, w, "wirkt gerade"));

            var boni = new List<TalentBonus>();
            var vorteile = new List<TalentBonus>();
            foreach (var (name, fx, wirkung, herkunft) in aktiv)
            {
                var bonus = new TalentBonus
                {
                    Quelle = name,
                    Herkunft = herkunft,
                    WirkungId = wirkung?.Id,
                    Anwendungen = fx.DauerArt == EffektDauerArt.Anwendungen,
                    Dauer = wirkung != null ? HesparisRegeln.WirkungRest(wirkung, kampagne.Spieltag) : null,
                };
                if (fx.Art == EffektArt.Talent)
                {
                    bonus.Betrag = fx.Wert ?? 0;
                    bonus.Kandidaten = new List<string> { fx.TalentCode! };
                    bonus.Ziel = fx.TalentCode;
                    boni.Add(bonus);
                }
                else
                {
                    bonus.Beschriftung = "Vorteil auf " + fx.VorteilAuf;
                    vorteile.Add(bonus);
                }
            }
            return (boni, vorteile);
        }

        private async Task<Effekt?> EffektMitLadenAsync(Gegenstand g, int kampagneId)
        {
            if (HesparisRegeln.EffektGueltig(g.Effekt)) return g.Effekt;
            var ware = await _context.LadenArtikel.FirstOrDefaultAsync(l => l.KampagneId == kampagneId && l.Name == g.Name);
            return ware != null && HesparisRegeln.EffektGueltig(ware.Effekt) ? ware.Effekt : null;
        }

        private static Effekt EffektKopie(Effekt fx) => new()
        {
            Art = fx.Art,
            TalentCode = fx.TalentCode,
            Wert = fx.Wert,
            VorteilAuf = fx.VorteilAuf,
            DauerArt = fx.DauerArt,
            DauerAnzahl = fx.DauerAnzahl,
            DauerText = fx.DauerText,
        };

        private static Effekt EffektAusFormular(GegenstandFormular f)
        {
            if (f.EffektArt == EffektArt.Talent && !string.IsNullOrEmpty(f.EffektTalentCode) && f.EffektWert.HasValue)
            {
                return new Effekt
                {
                    Art = EffektArt.Talent,
                    TalentCode = f.EffektTalentCode,
                    Wert = f.EffektWert,
                    DauerArt = f.EffektDauerArt,
                    DauerAnzahl = f.EffektDauerArt.HasValue ? Math.Max(1, f.EffektDauerAnzahl ?? 1) : null,
                };
            }
            if (f.EffektArt == EffektArt.Vorteil && !string.IsNullOrWhiteSpace(f.EffektVorteilAuf))
            {
                return new Effekt
                {
                    Art = EffektArt.Vorteil,
                    VorteilAuf = f.EffektVorteilAuf.Trim(),
                    DauerArt = f.EffektDauerArt,
                    DauerAnzahl = f.EffektDauerArt.HasValue ? Math.Max(1, f.EffektDauerAnzahl ?? 1) : null,
                };
            }
            return new Effekt();
        }

        /// <summary>Neue Buchung vorne einreihen, ältere ab Platz 5 fallen weg.</summary>
        private void BuchungEintragen(Spielfigur figur, int betrag, string grund)
        {
            foreach (var alt in figur.Geldbuchungen.OrderByDescending(b => b.Position).ToList())
            {
                alt.Position += 1;
                if (alt.Position >= HesparisRegeln.GeldbuchungenMax)
                    _context.Geldbuchungen.Remove(alt);
            }
            figur.Geldbuchungen.Add(new Geldbuchung { Position = 0, Betrag = betrag, Grund = grund });
        }
    }
}
