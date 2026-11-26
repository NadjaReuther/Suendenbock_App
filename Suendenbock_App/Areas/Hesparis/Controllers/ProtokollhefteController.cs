using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain.Hesparis;
using Suendenbock_App.Models.ViewModels.Hesparis;

namespace Suendenbock_App.Areas.Hesparis.Controllers
{
    /// <summary>
    /// Schuljahr · Protokollhefte (/Hesparis/Protokollhefte/…): je Außeneinsatz ein Heft (JS: MODS.hefte).
    /// Seiten wie in der HTML: Regal, Deckblatt, Heft, Abschnitt, Abgabe und das ganze Heft zum Lesen.
    /// Schreiben dürfen alle Spieler; Hefte anlegen und löschen sowie benoten nur Gott (in der HTML: sl-only).
    /// Textfelder speichern beim Tippen im Hintergrund (Aktion Feld), alles andere per Formular.
    /// </summary>
    public class ProtokollhefteController : HesparisBaseController
    {
        public ProtokollhefteController(ApplicationDbContext context) : base(context)
        {
        }

        // =====================================================================
        // Seiten
        // =====================================================================

        /// <summary>1 · Das Regal mit allen Heften, Inhaltsverzeichnis und Zetteln (JS: phRegal).</summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var akademie = await AkademieLadenAsync();
            var figuren = await FigurenAsync(akademie.Id);
            var hefte = await HefteLadenAsync(akademie.Id);
            var begleiter = await BegleiterAsync(hefte);

            var model = Grundmodell();
            model.Regal = hefte.Where(h => !h.Entwurf)
                .OrderByDescending(h => h.Nummer)
                .Select(h => Ansicht(h, figuren, begleiter, akademie.Spieltag))
                .ToList();

            var noten = model.Regal.Select(a => ProtokollheftRegeln.NoteAus(a.Heft.Note)).Where(n => n.HasValue).Select(n => n!.Value);
            model.Gruppenschnitt = HesparisRegeln.Schnitt(noten);
            model.NaechsteAbgabe = model.Regal
                .Where(a => !a.Heft.AbgegebenAm.HasValue && a.Heft.Abgabe.HasValue)
                .OrderBy(a => a.Heft.Abgabe)
                .FirstOrDefault();

            // Zettel "Schon mitgekommen": Mitschüler (ohne Spielfiguren) nach Anzahl Einsätze
            var klasse = await KlasseAsync(akademie.Id, model.Regal.Select(a => a.Heft).ToList(), null);
            model.SchonDabei = klasse.Where(k => k.WarDabei > 0)
                .OrderByDescending(k => k.WarDabei)
                .Take(5)
                .Select(k => (k.Name, k.WarDabei))
                .ToList();
            model.NochNieDabei = klasse.Count(k => k.WarDabei == 0);

            return View(model);
        }

        /// <summary>2 · Deckblatt mit Auftrag, Lehrkraft, Team und Schriftführer (JS: phDeck).</summary>
        [HttpGet]
        public async Task<IActionResult> Deckblatt(int id, string? frisch = null)
        {
            var (akademie, ansicht) = await AnsichtLadenAsync(id);
            if (ansicht == null) return NotFound();
            if (ansicht.Heft.Entwurf && !IsGott()) return NotFound();

            var model = Grundmodell(ansicht, frisch);
            model.Lehrkraefte = await LehrkraefteAsync(akademie.Id);

            var alle = await _context.Protokollhefte.Where(h => h.KampagneId == akademie.Id && !h.Entwurf).ToListAsync();
            model.Klasse = (await KlasseAsync(akademie.Id, alle, ansicht.Heft))
                .OrderBy(k => k.Unterwegs)
                .ThenBy(k => k.WarDabei)
                .ThenBy(k => ProtokollheftRegeln.Nachname(k.Name), StringComparer.Create(new System.Globalization.CultureInfo("de-DE"), false))
                .ToList();

            // Vorschlag: wer am seltensten geschrieben hat, Spielfiguren zuerst (JS: sugg)
            if (ansicht.Schreiber == null && ansicht.Team.Count > 0)
            {
                int Mal(HeftMitglied m) => alle.Count(h => h.Id != id &&
                    ((m.SpielfigurId.HasValue && h.SchreiberSpielfigurId == m.SpielfigurId) ||
                     (m.MitschuelerId.HasValue && h.SchreiberMitschuelerId == m.MitschuelerId)));
                model.Vorschlag = ansicht.Team.OrderBy(Mal).ThenBy(m => m.AusDerKlasse ? 1 : 0).First();
                model.VorschlagMal = Mal(model.Vorschlag);
            }
            return View(model);
        }

        /// <summary>3 · Heft aufgeschlagen: Arbeitsauftrag, Team, Notizen und Inhalt (JS: phHeft).</summary>
        [HttpGet]
        public async Task<IActionResult> Heft(int id, bool ruhig = false)
        {
            var (_, ansicht) = await AnsichtLadenAsync(id);
            if (ansicht == null || ansicht.Heft.Entwurf) return NotFound();
            var model = Grundmodell(ansicht, null);
            model.Animieren = !ruhig;
            return View(model);
        }

        /// <summary>4/5 · Ein Abschnitt: links schreiben, rechts die Anleitung (JS: phSeite).</summary>
        [HttpGet]
        public async Task<IActionResult> Abschnitt(int id, int k = 0, string? frisch = null, bool ruhig = false)
        {
            var (_, ansicht) = await AnsichtLadenAsync(id);
            if (ansicht == null || ansicht.Heft.Entwurf) return NotFound();
            var model = Grundmodell(ansicht, frisch);
            model.Abschnitt = Math.Clamp(k, 0, ProtokollheftRegeln.Abschnitte.Length - 1);
            model.Animieren = !ruhig;
            return View(model);
        }

        /// <summary>6 · Abgabe und Bewertung (JS: phAbgabe).</summary>
        [HttpGet]
        public async Task<IActionResult> Abgabe(int id, string? frisch = null, bool ruhig = false)
        {
            var (_, ansicht) = await AnsichtLadenAsync(id);
            if (ansicht == null || ansicht.Heft.Entwurf) return NotFound();
            var model = Grundmodell(ansicht, frisch);
            model.Animieren = !ruhig;
            model.NotenZehntel = HesparisRegeln.StaffelZehntel(ansicht.Heft.Note);
            model.AlsText = AlsText(ansicht);
            return View(model);
        }

        /// <summary>Das ganze Heft am Stück, zum Lesen, Kopieren und Drucken (JS: phLesen).</summary>
        [HttpGet]
        public async Task<IActionResult> Lesen(int id)
        {
            var (_, ansicht) = await AnsichtLadenAsync(id);
            if (ansicht == null || ansicht.Heft.Entwurf) return NotFound();
            var model = Grundmodell(ansicht, null);
            model.AlsText = AlsText(ansicht);
            return View(model);
        }

        // =====================================================================
        // Anlegen, Verwerfen, Löschen (Gott)
        // =====================================================================

        /// <summary>Neues Heft als Entwurf anlegen und zum Deckblatt (JS: data-pha="new"). Alte Entwürfe fliegen raus.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Neu()
        {
            if (!IsGott()) return Forbid();
            var akademie = await AkademieLadenAsync();

            var entwuerfe = await _context.Protokollhefte.Where(h => h.KampagneId == akademie.Id && h.Entwurf).ToListAsync();
            _context.Protokollhefte.RemoveRange(entwuerfe);

            int nummer = await _context.Protokollhefte
                .Where(h => h.KampagneId == akademie.Id && !h.Entwurf)
                .Select(h => (int?)h.Nummer).MaxAsync() ?? 0;
            var heute = akademie.Spieltag;
            var heft = new Protokollheft
            {
                KampagneId = akademie.Id,
                Nummer = nummer + 1,
                Schuljahr = akademie.Schuljahr,
                Entwurf = true,
                Von = heute,
                Bis = heute,
                Abgabe = heute.AddDays(7),
            };
            _context.Protokollhefte.Add(heft);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Deckblatt), new { id = heft.Id });
        }

        /// <summary>Entwurf verwerfen (JS: data-pha="cancel").</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verwerfen(int id)
        {
            if (!IsGott()) return Forbid();
            var heft = await HeftLadenAsync(id);
            if (heft != null && heft.Entwurf)
            {
                _context.Protokollhefte.Remove(heft);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>Entwurf ins Regal stellen (JS: data-pha="create"). Ohne Titel geht es nicht.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Anlegen(int id)
        {
            if (!IsGott()) return Forbid();
            var heft = await HeftLadenAsync(id);
            if (heft == null) return NotFound();
            if (string.IsNullOrWhiteSpace(heft.Titel))
            {
                TempData["Error"] = "Bitte zuerst einen Titel eintragen!";
                return RedirectToAction(nameof(Deckblatt), new { id });
            }
            heft.Entwurf = false;
            await KalenderAbgleichenAsync(heft);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Heft), new { id });
        }

        /// <summary>Heft löschen (JS: data-pha="del"). Der Abgabetermin im Kalender verschwindet mit.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Loeschen(int id)
        {
            if (!IsGott()) return Forbid();
            var heft = await HeftLadenAsync(id);
            if (heft != null)
            {
                heft.Entwurf = true;
                await KalenderAbgleichenAsync(heft);
                _context.Protokollhefte.Remove(heft);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Heft Nr. " + heft.Nummer + " gelöscht.";
            }
            return RedirectToAction(nameof(Index));
        }

        // =====================================================================
        // Textfelder (beim Tippen im Hintergrund)
        // =====================================================================

        /// <summary>
        /// Speichert ein Feld (JS: data-phf, data-pht, data-phj, data-phab, data-php). feld:
        /// titel, lehrer, ort, von, bis, abgabe, auftrag, rueckmeldung, txt:abschnitt, jeder:schlüssel,
        /// ab:zeile:uhrzeit|text|wer, p:person:name|rolle|sagt|eindruck.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Feld(int id, string feld, string? wert)
        {
            if (!DarfBearbeiten()) return Forbid();
            var heft = await HeftLadenAsync(id);
            if (heft == null) return NotFound();
            if (heft.Entwurf && !IsGott()) return Forbid();

            wert ??= "";
            var teile = (feld ?? "").Split(':');
            bool kalender = false;
            switch (teile[0])
            {
                case "titel": heft.Titel = Kuerzen(wert.Trim(), 80); kalender = true; break;
                case "lehrer": heft.Lehrer = LeerZuNull(Kuerzen(wert.Trim(), 200)); break;
                case "ort": heft.Ort = LeerZuNull(Kuerzen(wert.Trim(), 80)); break;
                case "auftrag": heft.Auftrag = LeerZuNull(Kuerzen(wert, 800)); break;
                case "von":
                    heft.Von = DatumLesen(wert);
                    if (heft.Von.HasValue && heft.Bis.HasValue && heft.Bis < heft.Von) heft.Bis = heft.Von;
                    break;
                case "bis":
                    heft.Bis = DatumLesen(wert);
                    if (heft.Von.HasValue && heft.Bis.HasValue && heft.Bis < heft.Von) heft.Bis = heft.Von;
                    break;
                case "abgabe": heft.Abgabe = DatumLesen(wert); kalender = true; break;
                case "rueckmeldung":
                    if (!IsGott()) return Forbid();
                    heft.Rueckmeldung = LeerZuNull(Kuerzen(wert, 600));
                    break;
                case "txt" when teile.Length == 2 && ProtokollheftRegeln.Abschnitte.Any(s => s.Schluessel == teile[1] && s.Art == "text"):
                    {
                        var a = heft.Abschnitte.FirstOrDefault(x => x.Abschnitt == teile[1]);
                        if (a == null) heft.Abschnitte.Add(a = new HeftAbschnitt { Abschnitt = teile[1] });
                        a.Text = Kuerzen(wert, 8000);
                        break;
                    }
                case "jeder" when teile.Length == 2:
                    {
                        var j = heft.JederEintraege.FirstOrDefault(x => x.Schluessel == teile[1]);
                        if (j == null) heft.JederEintraege.Add(j = new HeftJederEintrag { Schluessel = Kuerzen(teile[1], 40) });
                        j.Text = Kuerzen(wert, 2000);
                        break;
                    }
                case "ab" when teile.Length == 3 && int.TryParse(teile[1], out int zeileId):
                    {
                        var r = heft.Ablauf.FirstOrDefault(x => x.Id == zeileId);
                        if (r == null) return NotFound();
                        if (teile[2] == "uhrzeit") r.Uhrzeit = LeerZuNull(Kuerzen(wert.Trim(), 10));
                        else if (teile[2] == "text") r.Text = Kuerzen(wert, 200);
                        else if (teile[2] == "wer") r.Wer = LeerZuNull(Kuerzen(wert, 80));
                        break;
                    }
                case "p" when teile.Length == 3 && int.TryParse(teile[1], out int personId):
                    {
                        var p = heft.Personen.FirstOrDefault(x => x.Id == personId);
                        if (p == null) return NotFound();
                        if (teile[2] == "name") p.Name = Kuerzen(wert, 80);
                        else if (teile[2] == "rolle") p.Rolle = LeerZuNull(Kuerzen(wert, 80));
                        else if (teile[2] == "sagt") p.Sagt = LeerZuNull(Kuerzen(wert, 600));
                        else if (teile[2] == "eindruck") p.Eindruck = LeerZuNull(Kuerzen(wert, 400));
                        break;
                    }
                default:
                    return BadRequest();
            }
            if (kalender) await KalenderAbgleichenAsync(heft);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // =====================================================================
        // Deckblatt: Team, Schriftführer, worauf die Lehrkraft achtet
        // =====================================================================

        /// <summary>Spielfigur fehlt / ist wieder dabei (JS: data-pha="fehlt"). Wer fehlt, kann nicht schreiben.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Fehlt(int id, int figur)
        {
            if (!DarfBearbeiten()) return Forbid();
            var heft = await HeftLadenAsync(id);
            if (heft == null) return NotFound();

            var eintrag = heft.Fehlend.FirstOrDefault(f => f.SpielfigurId == figur);
            if (eintrag != null)
            {
                heft.Fehlend.Remove(eintrag);
            }
            else if (await _context.Spielfiguren.AnyAsync(f => f.Id == figur && f.KampagneId == heft.KampagneId))
            {
                heft.Fehlend.Add(new HeftFehlend { SpielfigurId = figur });
                if (heft.SchreiberSpielfigurId == figur) heft.SchreiberSpielfigurId = null;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Deckblatt), new { id, frisch = "fehlt|" + figur });
        }

        /// <summary>
        /// Mitschüler als +1 wählen oder austragen (JS: data-pha="plus"). wert: "m7" (Mitschüler-Eintrag) oder
        /// "c12" (Charakter der Art Mitschüler, bekommt dabei seinen Klassen-Eintrag).
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Begleiter(int id, string wert)
        {
            if (!DarfBearbeiten()) return Forbid();
            var heft = await HeftLadenAsync(id);
            if (heft == null) return NotFound();

            var mitschueler = await MitschuelerAusWertAsync(heft.KampagneId, wert);
            if (mitschueler == null) return NotFound();

            string? frisch = null;
            if (heft.BegleiterId == mitschueler.Id)
            {
                if (heft.SchreiberMitschuelerId == mitschueler.Id) heft.SchreiberMitschuelerId = null;
                heft.BegleiterId = null;
            }
            else
            {
                if (heft.SchreiberMitschuelerId.HasValue) heft.SchreiberMitschuelerId = null;
                heft.BegleiterId = mitschueler.Id;
                frisch = "plus";
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Deckblatt), new { id, frisch });
        }

        /// <summary>Schriftführer/in festlegen oder austragen (JS: data-pha="schreib"). wer: "f12" oder "m7".</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Schreiber(int id, string wer, string? zurueck = null)
        {
            if (!DarfBearbeiten()) return Forbid();
            var heft = await HeftLadenAsync(id);
            if (heft == null) return NotFound();

            bool istAn = (wer.StartsWith('f') && int.TryParse(wer[1..], out int f) && heft.SchreiberSpielfigurId == f)
                         || (wer.StartsWith('m') && int.TryParse(wer[1..], out int m) && heft.SchreiberMitschuelerId == m);
            heft.SchreiberSpielfigurId = null;
            heft.SchreiberMitschuelerId = null;
            if (!istAn)
            {
                if (wer.StartsWith('f') && int.TryParse(wer[1..], out int fid) && heft.Fehlend.All(x => x.SpielfigurId != fid)
                    && await _context.Spielfiguren.AnyAsync(x => x.Id == fid && x.KampagneId == heft.KampagneId))
                {
                    heft.SchreiberSpielfigurId = fid;
                }
                else if (wer.StartsWith('m') && int.TryParse(wer[1..], out int mid) && heft.BegleiterId == mid)
                {
                    heft.SchreiberMitschuelerId = mid;
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Deckblatt), new { id, frisch = "schreib" });
        }

        /// <summary>Worauf die Lehrkraft achtet, an/aus (JS: data-pha="achtet").</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Achtet(int id, string text)
        {
            if (!DarfBearbeiten()) return Forbid();
            var heft = await HeftLadenAsync(id);
            if (heft == null) return NotFound();
            if (!ProtokollheftRegeln.Achtet.Contains(text)) return BadRequest();

            var s = heft.Schwerpunkte.FirstOrDefault(x => x.Text == text);
            if (s != null) heft.Schwerpunkte.Remove(s);
            else heft.Schwerpunkte.Add(new HeftSchwerpunkt { Text = text });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Deckblatt), new { id });
        }

        // =====================================================================
        // Haken: Leitfragen und Abgabe-Checkliste
        // =====================================================================

        /// <summary>Leitfrage als beantwortet abhaken (JS: data-pha="q").</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Frage(int id, int k, int n)
        {
            if (!DarfBearbeiten()) return Forbid();
            var heft = await HeftLadenAsync(id);
            if (heft == null) return NotFound();
            if (k < 0 || k >= ProtokollheftRegeln.Abschnitte.Length) return BadRequest();
            var s = ProtokollheftRegeln.Abschnitte[k];
            if (n < 0 || n >= s.Fragen.Length) return BadRequest();

            var f = heft.Fragen.FirstOrDefault(x => x.Abschnitt == s.Schluessel && x.Position == n);
            if (f == null) heft.Fragen.Add(f = new HeftFrage { Abschnitt = s.Schluessel, Position = n });
            f.Beantwortet = !f.Beantwortet;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Abschnitt), new { id, k, ruhig = true, frisch = f.Beantwortet ? "q|" + s.Schluessel + "|" + n : null });
        }

        /// <summary>Haken vor der Abgabe (JS: data-pha="ck").</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Check(int id, string schluessel)
        {
            if (!DarfBearbeiten()) return Forbid();
            var heft = await HeftLadenAsync(id);
            if (heft == null) return NotFound();
            if (ProtokollheftRegeln.Checks.All(c => c.Schluessel != schluessel)) return BadRequest();

            var c = heft.Checks.FirstOrDefault(x => x.Schluessel == schluessel);
            if (c == null) heft.Checks.Add(c = new HeftCheck { Schluessel = schluessel });
            c.Erledigt = !c.Erledigt;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Abgabe), new { id, ruhig = true, frisch = c.Erledigt ? "ck|" + schluessel : null });
        }

        // =====================================================================
        // Notizen von unterwegs
        // =====================================================================

        /// <summary>Schnell etwas notieren, mit Spieltag und Uhrzeit (JS: data-phform="note").</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NotizNeu(int id, string? uhrzeit, string? text)
        {
            if (!DarfBearbeiten()) return Forbid();
            var akademie = await AkademieLadenAsync();
            var heft = await HeftLadenAsync(id);
            if (heft == null) return NotFound();
            text = Kuerzen(text?.Trim(), 200);
            if (text.Length > 0)
            {
                heft.Notizen.Add(new HeftNotiz
                {
                    Position = heft.Notizen.Select(x => x.Position).DefaultIfEmpty(-1).Max() + 1,
                    Datum = akademie.Spieltag,
                    Uhrzeit = LeerZuNull(Kuerzen(uhrzeit?.Trim(), 10)),
                    Text = text,
                });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Heft), new { id, ruhig = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NotizLoeschen(int id, int notizId)
        {
            if (!DarfBearbeiten()) return Forbid();
            var heft = await HeftLadenAsync(id);
            if (heft == null) return NotFound();
            var n = heft.Notizen.FirstOrDefault(x => x.Id == notizId);
            if (n != null)
            {
                heft.Notizen.Remove(n);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Heft), new { id, ruhig = true });
        }

        /// <summary>
        /// Notiz in den offenen Abschnitt übernehmen (JS: data-pha="nimp"): beim Ablauf als neue Zeile,
        /// sonst ans Ende des Textes.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NotizUebernehmen(int id, int notizId, int k)
        {
            if (!DarfBearbeiten()) return Forbid();
            var heft = await HeftLadenAsync(id);
            if (heft == null) return NotFound();
            if (k < 0 || k >= ProtokollheftRegeln.Abschnitte.Length) return BadRequest();
            var s = ProtokollheftRegeln.Abschnitte[k];
            var n = heft.Notizen.FirstOrDefault(x => x.Id == notizId);
            if (n == null) return NotFound();

            if (n.Uebernommen.All(u => u.Abschnitt != s.Schluessel))
            {
                if (s.Art == "ablauf")
                {
                    AblaufAusNotiz(heft, n);
                }
                else if (s.Art == "text")
                {
                    var a = heft.Abschnitte.FirstOrDefault(x => x.Abschnitt == s.Schluessel);
                    if (a == null) heft.Abschnitte.Add(a = new HeftAbschnitt { Abschnitt = s.Schluessel });
                    var alt = (a.Text ?? "").TrimEnd();
                    a.Text = Kuerzen((alt.Length > 0 ? alt + "\n" : "") + n.Text, 8000);
                }
                n.Uebernommen.Add(new HeftNotizUebernahme { Abschnitt = s.Schluessel });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Abschnitt), new { id, k, ruhig = true });
        }

        /// <summary>Alle noch nicht übernommenen Notizen als Ablauf-Zeilen (JS: data-pha="nimpall").</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NotizenInAblauf(int id)
        {
            if (!DarfBearbeiten()) return Forbid();
            var heft = await HeftLadenAsync(id);
            if (heft == null) return NotFound();
            foreach (var n in heft.Notizen.OrderBy(x => x.Position).Where(x => x.Uebernommen.All(u => u.Abschnitt != "ablauf")))
            {
                AblaufAusNotiz(heft, n);
                n.Uebernommen.Add(new HeftNotizUebernahme { Abschnitt = "ablauf" });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Abschnitt), new { id, k = AbschnittIndex("ablauf"), ruhig = true });
        }

        // =====================================================================
        // Ablauf und Steckbriefe
        // =====================================================================

        /// <summary>Neue Ablauf-Zeile (JS: data-phform="ab").</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AblaufNeu(int id, string? datum, string? uhrzeit, string? text, string? wer)
        {
            if (!DarfBearbeiten()) return Forbid();
            var akademie = await AkademieLadenAsync();
            var heft = await HeftLadenAsync(id);
            if (heft == null) return NotFound();
            text = Kuerzen(text?.Trim(), 200);
            if (text.Length > 0)
            {
                heft.Ablauf.Add(new HeftAblauf
                {
                    Position = heft.Ablauf.Select(x => x.Position).DefaultIfEmpty(-1).Max() + 1,
                    Datum = DatumLesen(datum) ?? akademie.Spieltag,
                    Uhrzeit = LeerZuNull(Kuerzen(uhrzeit?.Trim(), 10)),
                    Text = text,
                    Wer = LeerZuNull(Kuerzen(wer?.Trim(), 80)),
                });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Abschnitt), new { id, k = AbschnittIndex("ablauf"), ruhig = true });
        }

        /// <summary>Wichtiger Moment ★ an/aus (JS: data-pha="abstar").</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AblaufStern(int id, int zeileId)
        {
            if (!DarfBearbeiten()) return Forbid();
            var heft = await HeftLadenAsync(id);
            var r = heft?.Ablauf.FirstOrDefault(x => x.Id == zeileId);
            if (r == null) return NotFound();
            r.Wichtig = !r.Wichtig;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Abschnitt), new { id, k = AbschnittIndex("ablauf"), ruhig = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AblaufLoeschen(int id, int zeileId)
        {
            if (!DarfBearbeiten()) return Forbid();
            var heft = await HeftLadenAsync(id);
            var r = heft?.Ablauf.FirstOrDefault(x => x.Id == zeileId);
            if (r != null)
            {
                heft!.Ablauf.Remove(r);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Abschnitt), new { id, k = AbschnittIndex("ablauf"), ruhig = true });
        }

        /// <summary>Neuer, leerer Steckbrief (JS: data-pha="padd").</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PersonNeu(int id)
        {
            if (!DarfBearbeiten()) return Forbid();
            var heft = await HeftLadenAsync(id);
            if (heft == null) return NotFound();
            heft.Personen.Add(new HeftPerson { Position = heft.Personen.Select(x => x.Position).DefaultIfEmpty(-1).Max() + 1 });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Abschnitt), new { id, k = AbschnittIndex("personen"), ruhig = true, frisch = "padd" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PersonLoeschen(int id, int personId)
        {
            if (!DarfBearbeiten()) return Forbid();
            var heft = await HeftLadenAsync(id);
            var p = heft?.Personen.FirstOrDefault(x => x.Id == personId);
            if (p != null)
            {
                heft!.Personen.Remove(p);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Abschnitt), new { id, k = AbschnittIndex("personen"), ruhig = true });
        }

        /// <summary>Wie wichtig, 1–3 Sterne; derselbe Stern nochmal = 0 (JS: data-pha="pwich").</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PersonSterne(int id, int personId, int n)
        {
            if (!DarfBearbeiten()) return Forbid();
            var heft = await HeftLadenAsync(id);
            var p = heft?.Personen.FirstOrDefault(x => x.Id == personId);
            if (p == null) return NotFound();
            n = Math.Clamp(n, 0, 3);
            p.Wichtig = p.Wichtig == n ? 0 : n;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Abschnitt), new { id, k = AbschnittIndex("personen"), ruhig = true });
        }

        // =====================================================================
        // Abgabe und Note
        // =====================================================================

        /// <summary>
        /// Heft abgeben (JS: data-pha="abgeben"): nur wenn alle Abschnitte erfüllt, alle Haken gesetzt und
        /// ein Schriftführer feststeht. Der Abgabetermin verschwindet aus dem Kalender.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Abgeben(int id)
        {
            if (!DarfBearbeiten()) return Forbid();
            var (akademie, ansicht) = await AnsichtLadenAsync(id);
            if (ansicht == null || ansicht.Heft.Entwurf) return NotFound();
            var heft = ansicht.Heft;

            bool bereit = ansicht.Fertig == ProtokollheftRegeln.Abschnitte.Length
                          && ProtokollheftRegeln.Checks.All(c => ansicht.Check(c.Schluessel))
                          && ansicht.Schreiber != null;
            if (!bereit || heft.AbgegebenAm.HasValue)
            {
                return RedirectToAction(nameof(Abgabe), new { id, ruhig = true });
            }
            heft.AbgegebenAm = akademie.Spieltag.ToDateTime(TimeOnly.MinValue);
            await KalenderAbgleichenAsync(heft);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Abgabe), new { id, ruhig = true, frisch = "abgabe" });
        }

        /// <summary>Abgabe zurücknehmen, solange noch keine Note da ist (JS: data-pha="zurueck").</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Zuruecknehmen(int id)
        {
            if (!DarfBearbeiten()) return Forbid();
            var heft = await HeftLadenAsync(id);
            if (heft == null) return NotFound();
            if (string.IsNullOrEmpty(heft.Note))
            {
                heft.AbgegebenAm = null;
                await KalenderAbgleichenAsync(heft);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Abgabe), new { id, ruhig = true });
        }

        /// <summary>
        /// Note eintragen, dieselbe nochmal = wieder weg (JS: data-pha="note"). Sie gilt für alle, die dabei waren:
        /// steht im Zeugnis und bringt Freipunkte für die Magiestufen.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Note(int id, string note)
        {
            if (!IsGott()) return Forbid();
            var heft = await HeftLadenAsync(id);
            if (heft == null) return NotFound();
            if (!ProtokollheftRegeln.Noten.Contains(note) || !heft.AbgegebenAm.HasValue) return BadRequest();
            heft.Note = heft.Note == note ? null : note;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Abgabe), new { id, ruhig = true, frisch = heft.Note != null ? "note" : null });
        }

        // =====================================================================
        // Hilfen
        // =====================================================================

        private ProtokollheftViewModel Grundmodell(HeftAnsicht? ansicht = null, string? frisch = null) => new()
        {
            Heft = ansicht,
            Frisch = frisch,
            DarfBearbeiten = DarfBearbeiten(),
            IstSpielleitung = IsGott(),
        };

        private Task<List<Protokollheft>> HefteLadenAsync(int kampagneId) => MitAllem(_context.Protokollhefte)
            .Where(h => h.KampagneId == kampagneId)
            .ToListAsync();

        private async Task<Protokollheft?> HeftLadenAsync(int id)
        {
            var akademie = await AkademieLadenAsync();
            return await MitAllem(_context.Protokollhefte).FirstOrDefaultAsync(h => h.Id == id && h.KampagneId == akademie.Id);
        }

        private static IQueryable<Protokollheft> MitAllem(IQueryable<Protokollheft> q) => q
            .Include(h => h.Fehlend)
            .Include(h => h.Schwerpunkte)
            .Include(h => h.Notizen).ThenInclude(n => n.Uebernommen)
            .Include(h => h.Abschnitte)
            .Include(h => h.Fragen)
            .Include(h => h.Ablauf)
            .Include(h => h.Personen)
            .Include(h => h.JederEintraege)
            .Include(h => h.Checks)
            .AsSplitQuery();

        private async Task<(Kampagne Akademie, HeftAnsicht? Ansicht)> AnsichtLadenAsync(int id)
        {
            var akademie = await AkademieLadenAsync();
            var heft = await HeftLadenAsync(id);
            if (heft == null) return (akademie, null);
            var figuren = await FigurenAsync(akademie.Id);
            var begleiter = await BegleiterAsync(new List<Protokollheft> { heft });
            return (akademie, Ansicht(heft, figuren, begleiter, akademie.Spieltag));
        }

        private static HeftAnsicht Ansicht(Protokollheft heft, List<HeftMitglied> figuren, Dictionary<int, HeftMitglied> begleiter, DateOnly heute) =>
            new(heft, figuren, heft.BegleiterId.HasValue ? begleiter.GetValueOrDefault(heft.BegleiterId.Value) : null, heute);

        /// <summary>Alle Spielfiguren als Teammitglieder, in ihren Farben.</summary>
        private async Task<List<HeftMitglied>> FigurenAsync(int kampagneId)
        {
            var chips = await FigurChipsAsync(kampagneId);
            var namen = await _context.Spielfiguren
                .Where(f => f.KampagneId == kampagneId && f.HesparisCharakter != null)
                .Select(f => new { f.Id, f.HesparisCharakter!.Vorname, f.HesparisCharakter.Nachname })
                .ToListAsync();
            return chips.Select(c =>
            {
                var n = namen.FirstOrDefault(x => x.Id == c.Id);
                return new HeftMitglied
                {
                    Schluessel = "f" + c.Id,
                    SpielfigurId = c.Id,
                    Name = n != null ? (n.Vorname + " " + n.Nachname).Trim() : c.Name,
                    Kurz = c.Name,
                    Farbe = c.Aussehen.Farbe,
                };
            }).ToList();
        }

        /// <summary>Die Mitschüler, die in diesen Heften mitkommen, als Teammitglieder.</summary>
        private async Task<Dictionary<int, HeftMitglied>> BegleiterAsync(List<Protokollheft> hefte)
        {
            var ids = hefte.Where(h => h.BegleiterId.HasValue).Select(h => h.BegleiterId!.Value).Distinct().ToList();
            if (ids.Count == 0) return new();
            var liste = await _context.Mitschueler.Where(m => ids.Contains(m.Id)).ToListAsync();
            return liste.ToDictionary(m => m.Id, MitgliedAus);
        }

        private static HeftMitglied MitgliedAus(Mitschueler m) => new()
        {
            Schluessel = "m" + m.Id,
            MitschuelerId = m.Id,
            Name = m.Name,
            Kurz = m.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? m.Name,
            Farbe = ProtokollheftRegeln.MitschuelerFarbe(m.Farbe, m.Magie),
        };

        /// <summary>
        /// Klassenliste für das +1 (JS: mates): Mitschüler-Einträge ohne Spielfigur und Charaktere der Art
        /// Mitschüler, die noch keinen Eintrag haben. Mit Anzahl Einsätzen, "gerade unterwegs" und Herzen.
        /// </summary>
        private async Task<List<HeftKlassenZeile>> KlasseAsync(int kampagneId, List<Protokollheft> hefte, Protokollheft? aktuell)
        {
            var eintraege = await _context.Mitschueler
                .Where(m => m.KampagneId == kampagneId && m.SpielfigurId == null)
                .Include(m => m.Beziehungen)
                .ToListAsync();
            var verknuepft = eintraege.Where(m => m.HesparisCharakterId.HasValue).Select(m => m.HesparisCharakterId!.Value).ToHashSet();
            var nurCharakter = await _context.HesparisCharaktere
                .Where(c => c.CharakterartId == Charakterart.Mitschueler)
                .ToListAsync();

            var andere = hefte.Where(h => aktuell == null || h.Id != aktuell.Id).ToList();
            var zeilen = eintraege.Select(m => new HeftKlassenZeile
            {
                Wert = "m" + m.Id,
                Name = m.Name,
                WarDabei = andere.Count(h => h.BegleiterId == m.Id),
                Unterwegs = andere.Any(h => h.BegleiterId == m.Id && !h.AbgegebenAm.HasValue),
                // Herzen der Gruppe (JS: phHerz): Freundschaft minus Abneigung aller Spielfiguren zu ihm
                Herzen = m.Beziehungen.Sum(b => b.SpielfigurZuMitschueler.Freundschaft - b.SpielfigurZuMitschueler.Abneigung),
                Gewaehlt = aktuell?.BegleiterId == m.Id,
            }).ToList();
            zeilen.AddRange(nurCharakter.Where(c => !verknuepft.Contains(c.Id)).Select(c => new HeftKlassenZeile
            {
                Wert = "c" + c.Id,
                Name = c.VollerName,
            }));
            return zeilen;
        }

        /// <summary>"m7" → Mitschüler 7; "c12" → Klassen-Eintrag zu Charakter 12 (wird bei Bedarf angelegt).</summary>
        private async Task<Mitschueler?> MitschuelerAusWertAsync(int kampagneId, string wert)
        {
            if (string.IsNullOrEmpty(wert) || wert.Length < 2 || !int.TryParse(wert[1..], out int nr)) return null;
            if (wert[0] == 'm')
            {
                return await _context.Mitschueler.FirstOrDefaultAsync(m => m.Id == nr && m.KampagneId == kampagneId && m.SpielfigurId == null);
            }
            if (wert[0] != 'c') return null;

            var vorhanden = await _context.Mitschueler.FirstOrDefaultAsync(m => m.KampagneId == kampagneId && m.HesparisCharakterId == nr);
            if (vorhanden != null) return vorhanden;
            var charakter = await _context.HesparisCharaktere.FirstOrDefaultAsync(c => c.Id == nr && c.CharakterartId == Charakterart.Mitschueler);
            if (charakter == null) return null;

            var neu = new Mitschueler
            {
                KampagneId = kampagneId,
                HesparisCharakterId = charakter.Id,
                Code = "hc" + charakter.Id,
                Name = charakter.VollerName,
            };
            _context.Mitschueler.Add(neu);
            await _context.SaveChangesAsync();
            return neu;
        }

        /// <summary>Lehrkräfte zur Auswahl (JS: lehrerList): Lehrer-Einträge mit Fächern und Charaktere der Art Lehrkraft.</summary>
        private async Task<List<HeftLehrkraft>> LehrkraefteAsync(int kampagneId)
        {
            var eintraege = await _context.Lehrer
                .Where(l => l.KampagneId == kampagneId)
                .Include(l => l.Faecher).ThenInclude(f => f.Fach)
                .ToListAsync();
            var liste = eintraege.Select(l => new HeftLehrkraft
            {
                Name = l.Name,
                Faecher = l.Faecher.Count > 0 ? string.Join(", ", l.Faecher.Select(f => f.Fach.Name)) : l.Aufgaben,
            }).ToList();

            var verknuepft = eintraege.Where(l => l.HesparisCharakterId.HasValue).Select(l => l.HesparisCharakterId!.Value).ToHashSet();
            var charaktere = await _context.HesparisCharaktere.Where(c => c.CharakterartId == Charakterart.Lehrkraft).ToListAsync();
            foreach (var c in charaktere.Where(c => !verknuepft.Contains(c.Id)))
            {
                if (liste.All(l => l.Name != c.VollerName)) liste.Add(new HeftLehrkraft { Name = c.VollerName });
            }
            return liste.OrderBy(l => l.Name, StringComparer.Create(new System.Globalization.CultureInfo("de-DE"), false)).ToList();
        }

        /// <summary>
        /// Abgabetermin im Schulkalender führen (JS: phKalSync): solange das Heft ausgegeben, mit Abgabedatum
        /// und noch nicht abgegeben ist; sonst weg damit.
        /// </summary>
        private async Task KalenderAbgleichenAsync(Protokollheft heft)
        {
            KalenderTermin? termin = heft.KalenderTerminId.HasValue
                ? await _context.KalenderTermine.FirstOrDefaultAsync(t => t.Id == heft.KalenderTerminId)
                : null;
            if (heft.Entwurf || !heft.Abgabe.HasValue || heft.AbgegebenAm.HasValue)
            {
                if (termin != null)
                {
                    heft.KalenderTermin = null;
                    heft.KalenderTerminId = null;
                    _context.KalenderTermine.Remove(termin);
                }
                return;
            }
            if (termin == null)
            {
                // Erst die Standardtermine anlegen lassen, falls der Kalender noch nie geöffnet wurde
                await TermineLadenAsync(await AkademieLadenAsync());
                termin = new KalenderTermin { KampagneId = heft.KampagneId, Typ = Termintyp.Schule };
                _context.KalenderTermine.Add(termin);
                heft.KalenderTermin = termin;
            }
            termin.Datum = heft.Abgabe.Value;
            termin.Titel = Kuerzen("Abgabe Protokollheft Nr. " + heft.Nummer + (string.IsNullOrWhiteSpace(heft.Titel) ? "" : ": " + heft.Titel), 200);
        }

        private static void AblaufAusNotiz(Protokollheft heft, HeftNotiz n)
        {
            heft.Ablauf.Add(new HeftAblauf
            {
                Position = heft.Ablauf.Select(x => x.Position).DefaultIfEmpty(-1).Max() + 1,
                Datum = n.Datum,
                Uhrzeit = n.Uhrzeit,
                Text = n.Text,
            });
        }

        private static int AbschnittIndex(string schluessel) =>
            Array.FindIndex(ProtokollheftRegeln.Abschnitte, s => s.Schluessel == schluessel);

        /// <summary>Das ganze Heft als Text (JS: phText), für "Als Text kopieren".</summary>
        private static string AlsText(HeftAnsicht a)
        {
            var h = a.Heft;
            var L = new List<string>
            {
                "PROTOKOLLHEFT NR. " + h.Nummer + ": " + h.Titel, "Akademie Hesparis · Außeneinsatz", "",
                "Lehrkraft: " + (h.Lehrer ?? "–"), "Ort: " + (h.Ort ?? "–"), "Unterwegs: " + a.Zeitraum,
                "Abgabe bis: " + HeftAnsicht.Datum(h.Abgabe), "Team: " + string.Join(", ", a.Team.Select(m => m.Name)),
                "Schriftführer/in: " + (a.Schreiber?.Name ?? "–"), "",
            };
            if (!string.IsNullOrEmpty(h.Auftrag)) L.AddRange(new[] { "Arbeitsauftrag:", h.Auftrag, "" });
            for (int i = 0; i < ProtokollheftRegeln.Abschnitte.Length; i++)
            {
                var s = ProtokollheftRegeln.Abschnitte[i];
                if (s.Art == "kopf") continue;
                L.Add((i + 1) + ". " + s.Titel.ToUpperInvariant());
                if (s.Art == "ablauf")
                {
                    foreach (var r in a.AblaufSortiert)
                        L.Add((r.Datum.HasValue ? HeftAnsicht.Datum(r.Datum) + " " : "") + (r.Uhrzeit ?? "--:--") + " · " + r.Text
                              + (string.IsNullOrEmpty(r.Wer) ? "" : " (" + r.Wer + ")") + (r.Wichtig ? " ★" : ""));
                }
                else if (s.Art == "personen")
                {
                    foreach (var p in a.PersonenSortiert)
                    {
                        L.Add("– " + (string.IsNullOrEmpty(p.Name) ? "?" : p.Name) + (string.IsNullOrEmpty(p.Rolle) ? "" : ", " + p.Rolle)
                              + (p.Wichtig > 0 ? " " + new string('★', p.Wichtig) : ""));
                        if (!string.IsNullOrEmpty(p.Sagt)) L.Add("  " + p.Sagt);
                        if (!string.IsNullOrEmpty(p.Eindruck)) L.Add("  Eindruck: " + p.Eindruck);
                    }
                }
                else if (s.Art == "jeder")
                {
                    foreach (var m in a.Team) L.Add(m.Name + ": " + (a.Jeder(m.Schluessel) is { Length: > 0 } t ? t : "–"));
                }
                else
                {
                    L.Add(a.Text(s.Schluessel) is { Length: > 0 } t ? t : "–");
                }
                L.Add("");
            }
            if (h.AbgegebenAm.HasValue) L.Add("Abgegeben am " + a.AbgegebenText);
            if (!string.IsNullOrEmpty(h.Note)) L.Add("Note: " + h.Note + (string.IsNullOrEmpty(h.Rueckmeldung) ? "" : " · " + h.Rueckmeldung));
            return string.Join("\n", L);
        }

        private static DateOnly? DatumLesen(string? wert) =>
            DateOnly.TryParseExact(wert, "yyyy-MM-dd", out var d) ? d : null;

        private static string Kuerzen(string? text, int max)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Length > max ? text[..max] : text;
        }

        private static string? LeerZuNull(string? text) => string.IsNullOrWhiteSpace(text) ? null : text;
    }
}
