using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain.Hesparis;
using Suendenbock_App.Models.ViewModels.Hesparis;

namespace Suendenbock_App.Areas.Hesparis.Controllers
{
    /// <summary>
    /// Gruppe "Alltag" (/Hesparis/Alltag/…): Stundenplan, Kalender und Essensplan, später AGs und Nachsitzen.
    /// </summary>
    public class AlltagController : HesparisBaseController
    {
        public AlltagController(ApplicationDbContext context) : base(context)
        {
        }

        // =====================================================================
        // Stundenplan
        // =====================================================================

        /// <summary>Die Schulwoche (JS: renderStundenplan). Nur Anzeige, gespeichert wird hier nichts.</summary>
        [HttpGet]
        public async Task<IActionResult> Stundenplan()
        {
            var kampagne = await AkademieLadenAsync();
            var faecher = await _context.Faecher.OrderBy(f => f.Id).ToListAsync();

            // Lehrkräfte aus der Datenbank; solange dort keine Fächer zugeordnet sind, die aus der HTML
            var lehrerJeFach = (await _context.LehrerFaecher
                    .Where(lf => lf.Lehrer.KampagneId == kampagne.Id)
                    .Select(lf => new { lf.FachId, lf.Lehrer.Name })
                    .ToListAsync())
                .GroupBy(x => x.FachId)
                .ToDictionary(g => g.Key, g => string.Join(", ", g.Select(x => x.Name)));

            var (tag, zeile) = AlltagRegeln.Jetzt();
            var model = new StundenplanViewModel { HeuteTag = tag, JetztZeile = zeile };

            foreach (var f in faecher)
            {
                // Alle Räume, in denen das Fach laut Plan stattfindet
                var raeume = AlltagRegeln.Plan
                    .SelectMany(t => t)
                    .Where(s => s.Fach == f.Id)
                    .Select(s => AlltagRegeln.RaumText(s.Raum))
                    .Distinct();

                string? lehrkraft;
                if (lehrerJeFach.Count > 0)
                {
                    lehrkraft = lehrerJeFach.GetValueOrDefault(f.Id);
                }
                else
                {
                    lehrkraft = AlltagRegeln.LehrerStandard.GetValueOrDefault(f.Name);
                }

                var farbe = ("#a89568", "168,149,104");
                if (f.Id >= 0 && f.Id < AlltagRegeln.FachFarben.Length)
                {
                    farbe = AlltagRegeln.FachFarben[f.Id];
                }

                model.Faecher.Add(new StundenplanFach
                {
                    Id = f.Id,
                    Name = f.Name,
                    Kurzname = f.Kurzname ?? f.Name,
                    Lehrkraft = lehrkraft,
                    Farbe = farbe.Item1,
                    Rgb = farbe.Item2,
                    Orte = string.Join(" · ", raeume),
                });
            }

            return View(model);
        }

        // =====================================================================
        // Kalender
        // =====================================================================

        /// <summary>
        /// Schulkalender (JS: MODS.kalender): September bis Juli als Landschaft, Termine als Laternen.
        /// Das Panorama zeichnet hesparis-kalender.js; Termine und Spieltag verwaltet hier Gott.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Kalender(bool verwalten = false)
        {
            var kampagne = await AkademieLadenAsync();
            var termine = await TermineLadenAsync(kampagne);
            int jahr = AlltagRegeln.Schuljahresbeginn(kampagne.Spieltag);

            var model = new KalenderViewModel
            {
                Jahr = jahr,
                Heute = kampagne.Spieltag,
                IstSpielleitung = IsGott(),
                Verwalten = verwalten,
                Termine = termine.Select(t => new KalenderZeile
                {
                    Id = t.Id,
                    Datum = t.Datum,
                    Titel = t.Titel,
                    Art = AlltagRegeln.Art(t.Typ),
                }).ToList(),
            };

            model.PanoramaJson = JsonSerializer.Serialize(new
            {
                year = jahr,
                heute = kampagne.Spieltag.ToString("yyyy-MM-dd"),
                events = model.Termine.Select(t => new { date = t.Datum.ToString("yyyy-MM-dd"), title = t.Titel, type = t.Art.Code }),
                types = AlltagRegeln.TerminArten.ToDictionary(a => a.Code, a => new[] { a.Name, a.Farbe }),
                monate = AlltagRegeln.Monate,
            });
            return View(model);
        }

        /// <summary>Gott: neuer Termin (JS: data-ksubmit).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> TerminHinzufuegen(DateOnly datum, string titel, string typ)
        {
            var kampagne = await AkademieLadenAsync();
            if (!string.IsNullOrWhiteSpace(titel))
            {
                var art = AlltagRegeln.TerminArten.FirstOrDefault(a => a.Code == typ) ?? AlltagRegeln.TerminArten[^1];
                _context.KalenderTermine.Add(new KalenderTermin
                {
                    KampagneId = kampagne.Id,
                    Datum = datum,
                    Titel = titel.Trim(),
                    Typ = art.Typ,
                });
                await _context.SaveChangesAsync();
                TempData["Success"] = "„" + titel.Trim() + "“ steht am " + AlltagRegeln.Datum(datum) + " im Kalender.";
            }
            return RedirectToAction(nameof(Kalender), new { verwalten = true });
        }

        /// <summary>Gott: Termin löschen (JS: data-kdel).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> TerminLoeschen(int id)
        {
            var kampagne = await AkademieLadenAsync();
            var termin = await _context.KalenderTermine.FirstOrDefaultAsync(t => t.Id == id && t.KampagneId == kampagne.Id);
            if (termin != null)
            {
                // Abgabetermin eines Protokollhefts: Verknüpfung lösen, sonst verweigert die Datenbank das Löschen
                foreach (var heft in await _context.Protokollhefte.Where(h => h.KalenderTerminId == termin.Id).ToListAsync())
                {
                    heft.KalenderTerminId = null;
                    heft.KalenderTermin = null;
                }
                _context.KalenderTermine.Remove(termin);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Kalender), new { verwalten = true });
        }

        /// <summary>
        /// Gott: Tag der Spielwelt setzen (JS: data-kheute) oder um Tage verschieben (JS: data-kday1).
        /// Davon hängen Wirkungen, Essensplan und das Schuljahr im Kalender ab.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> Spieltag(DateOnly? tag, int? tage, string? zurueck)
        {
            var kampagne = await AkademieLadenAsync();
            // Die Pfeile schicken "tage" mit, das Datumsfeld allein schickt nur "tag"
            if (tage.HasValue)
            {
                kampagne.Spieltag = kampagne.Spieltag.AddDays(tage.Value);
            }
            else if (tag.HasValue)
            {
                kampagne.Spieltag = tag.Value;
            }
            kampagne.Kalenderjahr = AlltagRegeln.Schuljahresbeginn(kampagne.Spieltag);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(zurueck) && Url.IsLocalUrl(zurueck))
            {
                return LocalRedirect(zurueck);
            }
            return RedirectToAction(nameof(Kalender), new { verwalten = true });
        }

        // =====================================================================
        // Essensplan
        // =====================================================================

        /// <summary>
        /// Speisesaal (JS: MODS.essen): drei Kessel für den gewählten Tag, darunter kaufen die Figuren.
        /// Gekauft werden kann nur am heutigen Spieltag. Die Spielleitung stellt unten den Wochenplan zusammen.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Essensplan(int woche = 0, int? tag = null, string? feld = null, string art = "alle", bool verwalten = false)
        {
            var kampagne = await AkademieLadenAsync();
            var heute = kampagne.Spieltag;
            woche = Math.Clamp(woche, AlltagRegeln.EssenWocheMin, AlltagRegeln.EssenWocheMax);
            var montag = AlltagRegeln.Montag(heute).AddDays(7 * woche);

            int gewaehlt = AlltagRegeln.Wochentag(heute);
            if (tag.HasValue)
            {
                gewaehlt = Math.Clamp(tag.Value, 0, 6);
            }

            var gerichte = await _context.Gerichte.ToListAsync();
            var plan = await WochenplanLadenAsync(kampagne, montag, gerichte);
            bool halberPreis = await HalberPreisAsync(kampagne);

            // Figuren mit Farbe, Geld und wem sie gehören
            var chips = await FigurChipsAsync(kampagne.Id);
            var figuren = await _context.Spielfiguren
                .Where(f => f.KampagneId == kampagne.Id && f.HesparisCharakter != null)
                .Include(f => f.HesparisCharakter)
                .ToListAsync();
            var kaeufe = await _context.Essenskaeufe
                .Where(k => k.Datum == heute && figuren.Select(f => f.Id).Contains(k.SpielfigurId))
                .ToListAsync();
            var userId = GetUserId();
            bool gott = IsGott();

            var model = new EssensplanViewModel
            {
                Woche = woche,
                Montag = montag,
                Heute = heute,
                Tag = gewaehlt,
                IstSpielleitung = gott,
                Verwalten = verwalten || feld != null,
                Plan = plan,
                HalberPreis = halberPreis,
                Art = art,
            };

            foreach (var (mahlzeit, name, _) in AlltagRegeln.Mahlzeiten)
            {
                var gericht = plan[gewaehlt][(int)mahlzeit];
                if (gericht == null) continue;
                int preis = AlltagRegeln.Preis(gericht.Preis, halberPreis);

                var kessel = new EssenKessel
                {
                    Mahlzeit = (int)mahlzeit,
                    Name = name,
                    Gericht = gericht,
                    Farbe = AlltagRegeln.GerichtFarben.GetValueOrDefault(gericht.Code, "#e8c98a"),
                    Preis = preis,
                };
                foreach (var chip in chips)
                {
                    var figur = figuren.First(f => f.Id == chip.Id);
                    bool gegessen = kaeufe.Any(k => k.SpielfigurId == figur.Id && k.Mahlzeit == mahlzeit);
                    kessel.Gaeste.Add(new EssenGast
                    {
                        FigurId = figur.Id,
                        Name = chip.Name,
                        Farbe = chip.Aussehen.Farbe,
                        Geld = figur.Geld,
                        Gegessen = gegessen,
                        ZuWenigGeld = !gegessen && figur.Geld < preis,
                        Darf = gott || figur.HesparisCharakter!.UserId == userId,
                    });
                }
                model.Kessel.Add(kessel);
            }

            // Theke: was heute noch wirkt
            var wirkungen = await _context.Essenswirkungen
                .Where(w => w.Datum == heute && figuren.Select(f => f.Id).Contains(w.SpielfigurId))
                .OrderBy(w => w.Id)
                .ToListAsync();
            foreach (var w in wirkungen)
            {
                var chip = chips.FirstOrDefault(c => c.Id == w.SpielfigurId);
                model.Wirkungen.Add(new EssenWirkungZeile
                {
                    Name = chip?.Name ?? "?",
                    Farbe = chip?.Aussehen.Farbe ?? "#e7d6a3",
                    Text = w.Wirkung ?? w.Gericht,
                });
            }

            // Spielleitung: angetipptes Feld und passende Gerichte
            if (gott && FeldLesen(feld) is (int ft, int fm))
            {
                model.Feld = (ft, fm);
                model.Auswahl = gerichte
                    .Where(g => (int)g.Mahlzeit == fm && (art == "alle" || g.Art == art))
                    .OrderBy(g => g.Preis)
                    .ToList();
            }
            return View(model);
        }

        /// <summary>Essen kaufen (JS: essBuy): Geld abziehen, LP sofort heilen, sonst Wirkung bis morgen.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EssenKaufen(int figurId, int mahlzeit, int woche, int tag)
        {
            var kampagne = await AkademieLadenAsync();
            var heute = kampagne.Spieltag;
            var figur = await _context.Spielfiguren
                .Include(f => f.HesparisCharakter)
                .Include(f => f.Geldbuchungen)
                .FirstOrDefaultAsync(f => f.Id == figurId && f.KampagneId == kampagne.Id);
            var zurueck = RedirectToAction(nameof(Essensplan), new { woche, tag });
            if (figur == null || mahlzeit < 0 || mahlzeit > 2) return zurueck;

            // Spielerinnen kaufen nur für die eigene Figur
            if (!IsGott() && figur.HesparisCharakter?.UserId != GetUserId())
            {
                return Forbid();
            }

            var gerichte = await _context.Gerichte.ToListAsync();
            var plan = await WochenplanLadenAsync(kampagne, AlltagRegeln.Montag(heute), gerichte);
            var gericht = plan[AlltagRegeln.Wochentag(heute)][mahlzeit];
            if (gericht == null) return zurueck;

            var art = (Mahlzeit)mahlzeit;
            bool schonGegessen = await _context.Essenskaeufe
                .AnyAsync(k => k.SpielfigurId == figur.Id && k.Datum == heute && k.Mahlzeit == art);
            int preis = AlltagRegeln.Preis(gericht.Preis, await HalberPreisAsync(kampagne));
            var name = figur.HesparisCharakter?.Anzeigename ?? "";
            if (schonGegessen)
            {
                TempData["Error"] = name + " hat schon " + AlltagRegeln.Mahlzeiten[mahlzeit].Name + " gegessen.";
                return zurueck;
            }
            if (figur.Geld < preis)
            {
                TempData["Error"] = name + " hat zu wenig Geld (" + HesparisRegeln.GeldText(figur.Geld) + ").";
                return zurueck;
            }

            figur.Geld -= preis;
            BuchungEintragen(figur, -preis, "Mensa: " + gericht.Name);
            _context.Essenskaeufe.Add(new Essenskauf { SpielfigurId = figur.Id, Datum = heute, Mahlzeit = art, GerichtCode = gericht.Code });

            if ((gericht.Lebenspunkte ?? 0) > 0)
            {
                figur.LebenspunkteAktuell = Math.Min(figur.LebenspunkteMax, figur.LebenspunkteAktuell + gericht.Lebenspunkte!.Value);
            }
            if (AlltagRegeln.WirktWeiter(gericht))
            {
                _context.Essenswirkungen.Add(new Essenswirkung { SpielfigurId = figur.Id, Datum = heute, Gericht = gericht.Name, Wirkung = gericht.Wirkung });
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = name + " isst " + gericht.Name + ".";
            return zurueck;
        }

        /// <summary>Gott: ein Feld des Speiseplans mit einem Gericht belegen (JS: data-essset).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> GerichtSetzen(int woche, int tag, int mahlzeit, string code, string art = "alle")
        {
            var kampagne = await AkademieLadenAsync();
            var montag = AlltagRegeln.Montag(kampagne.Spieltag).AddDays(7 * Math.Clamp(woche, AlltagRegeln.EssenWocheMin, AlltagRegeln.EssenWocheMax));
            var gerichte = await _context.Gerichte.ToListAsync();
            await WochenplanLadenAsync(kampagne, montag, gerichte);

            var gericht = gerichte.FirstOrDefault(g => g.Code == code && (int)g.Mahlzeit == mahlzeit);
            var eintrag = await _context.SpeiseplanEintraege.FirstOrDefaultAsync(e =>
                e.KampagneId == kampagne.Id && e.Woche == montag && e.Wochentag == tag && e.Mahlzeit == (Mahlzeit)mahlzeit);
            if (gericht != null && eintrag != null)
            {
                eintrag.GerichtCode = gericht.Code;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Essensplan), null, new { woche, feld = tag + "-" + mahlzeit, art }, "es-sl");
        }

        /// <summary>Gott: die ganze Woche oder ein einzelnes Feld neu würfeln (JS: data-essroll).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> SpeiseplanWuerfeln(int woche, string? feld, string art = "alle")
        {
            var kampagne = await AkademieLadenAsync();
            var montag = AlltagRegeln.Montag(kampagne.Spieltag).AddDays(7 * Math.Clamp(woche, AlltagRegeln.EssenWocheMin, AlltagRegeln.EssenWocheMax));
            var gerichte = await _context.Gerichte.ToListAsync();
            await WochenplanLadenAsync(kampagne, montag, gerichte);

            var eintraege = await _context.SpeiseplanEintraege
                .Where(e => e.KampagneId == kampagne.Id && e.Woche == montag)
                .ToListAsync();
            if (FeldLesen(feld) is (int ft, int fm))
            {
                // nur dieses Feld
                eintraege = eintraege.Where(e => e.Wochentag == ft && (int)e.Mahlzeit == fm).ToList();
            }
            foreach (var e in eintraege)
            {
                e.GerichtCode = ZufallsGericht(gerichte, e.Mahlzeit, e.GerichtCode);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Essensplan), null, new { woche, feld, art, verwalten = true }, "es-sl");
        }

        /// <summary>
        /// Plan einer Woche als [Wochentag][Mahlzeit]. Fehlt er, wird er angelegt:
        /// ganz am Anfang mit dem Plan aus der HTML, danach ausgewürfelt (JS: essWeek).
        /// </summary>
        private async Task<Gericht?[][]> WochenplanLadenAsync(Kampagne kampagne, DateOnly montag, List<Gericht> gerichte)
        {
            var eintraege = await _context.SpeiseplanEintraege
                .Where(e => e.KampagneId == kampagne.Id && e.Woche == montag)
                .ToListAsync();

            if (eintraege.Count == 0)
            {
                bool ersterPlan = !await _context.SpeiseplanEintraege.AnyAsync(e => e.KampagneId == kampagne.Id)
                                  && montag == AlltagRegeln.Montag(kampagne.Spieltag);
                for (int t = 0; t < 7; t++)
                {
                    foreach (var (mahlzeit, _, _) in AlltagRegeln.Mahlzeiten)
                    {
                        string code;
                        if (ersterPlan)
                        {
                            code = AlltagRegeln.PlanStandard[t][(int)mahlzeit];
                        }
                        else
                        {
                            code = ZufallsGericht(gerichte, mahlzeit, null);
                        }
                        var e = new SpeiseplanEintrag { KampagneId = kampagne.Id, Woche = montag, Wochentag = t, Mahlzeit = mahlzeit, GerichtCode = code };
                        _context.SpeiseplanEintraege.Add(e);
                        eintraege.Add(e);
                    }
                }
                await _context.SaveChangesAsync();
            }

            var plan = new Gericht?[7][];
            for (int t = 0; t < 7; t++)
            {
                plan[t] = new Gericht?[3];
                foreach (var e in eintraege.Where(e => e.Wochentag == t))
                {
                    plan[t][(int)e.Mahlzeit] = gerichte.FirstOrDefault(g => g.Code == e.GerichtCode);
                }
            }
            return plan;
        }

        /// <summary>Zufälliges Gericht dieser Mahlzeit, möglichst nicht das bisherige (JS: essRandom).</summary>
        private static string ZufallsGericht(List<Gericht> gerichte, Mahlzeit mahlzeit, string? nicht)
        {
            var liste = gerichte.Where(g => g.Mahlzeit == mahlzeit && g.Code != nicht).ToList();
            if (liste.Count == 0) return nicht ?? "";
            return liste[Random.Shared.Next(liste.Count)].Code;
        }

        /// <summary>Gilt die Ratsregel "Mensa zum halben Preis"? (JS: ratRegel('mensa50'))</summary>
        private Task<bool> HalberPreisAsync(Kampagne kampagne) =>
            _context.RatRegeln.AnyAsync(r => r.KampagneId == kampagne.Id && r.Schluessel == "mensa50" && r.AufgehobenAm == null);

        /// <summary>"2-1" → (Mittwoch, Mittag); sonst null.</summary>
        private static (int, int)? FeldLesen(string? feld)
        {
            var teile = (feld ?? "").Split('-');
            if (teile.Length == 2 && int.TryParse(teile[0], out int t) && int.TryParse(teile[1], out int m)
                && t >= 0 && t < 7 && m >= 0 && m < 3)
            {
                return (t, m);
            }
            return null;
        }

        /// <summary>Neue Geldbuchung vorne einreihen, ältere ab Platz 5 fallen weg (wie im FigurController).</summary>
        private void BuchungEintragen(Spielfigur figur, int betrag, string grund)
        {
            foreach (var alt in figur.Geldbuchungen.OrderByDescending(b => b.Position).ToList())
            {
                alt.Position += 1;
                if (alt.Position >= HesparisRegeln.GeldbuchungenMax)
                {
                    _context.Geldbuchungen.Remove(alt);
                }
            }
            figur.Geldbuchungen.Add(new Geldbuchung { Position = 0, Betrag = betrag, Grund = grund });
        }
    }
}
