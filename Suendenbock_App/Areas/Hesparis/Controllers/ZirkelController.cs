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
    /// Zirkel der Priore (/Hesparis/Zirkel). Zwölf Magien, je ein Prior und ein Sekundant.
    /// Die Spielleitung hinterlegt, wer dahintersteckt; die Spieler raten auf der Rückseite des Bretts.
    /// Ein falscher Versuch sperrt das Feld für 5 Minuten. Dazu kommt der Mordfall.
    /// </summary>
    public class ZirkelController : HesparisBaseController
    {
        public ZirkelController(ApplicationDbContext context) : base(context)
        {
        }

        // =====================================================================
        // Seite
        // =====================================================================

        [HttpGet]
        public async Task<IActionResult> Index(int? nr, bool wenden = false)
        {
            var kampagne = await AkademieLadenAsync();
            var daten = await DatenLadenAsync(kampagne.Id);
            bool sl = IsGott();

            int auswahl = 7;
            if (nr is >= 1 and <= 12)
            {
                auswahl = nr.Value;
            }

            // Rückmeldung zum letzten Versuch (für das Aufblinken der Zeile)
            string? rmSchluessel = null;
            string? rmErgebnis = null;
            if (TempData["ZkFb"] is string fb)
            {
                var teile = fb.Split('#');
                rmSchluessel = teile[0];
                rmErgebnis = teile[1];
            }

            var model = new ZirkelViewModel
            {
                Nr = auswahl,
                IstSpielleitung = sl,
                DarfRaten = DarfBearbeiten() && !sl,
                Wenden = wenden,
                PriorPerson = daten.Person(auswahl, ZirkelSeite.Prior),
                SekundantPerson = daten.Person(auswahl, ZirkelSeite.Sekundant),
            };

            model.Prior = SpalteBauen(daten, auswahl, ZirkelSeite.Prior, sl, rmSchluessel, rmErgebnis);
            model.Sekundant = SpalteBauen(daten, auswahl, ZirkelSeite.Sekundant, sl, rmSchluessel, rmErgebnis);

            // Auswahl aller Personen (für "dient wirklich" und den Mordfall)
            foreach (var m in ZirkelRegeln.Magien)
            {
                foreach (var seite in new[] { ZirkelSeite.Prior, ZirkelSeite.Sekundant })
                {
                    model.Personen.Add((m.Nr + "|" + ZirkelRegeln.Kurz(seite), daten.PersonText(m.Nr, seite, sl)));
                }
            }

            // Notizen zur gewählten Magie
            model.Notizen = daten.Notizen
                .Where(n => n.Nr == auswahl)
                .OrderByDescending(n => n.ErstelltAm)
                .Select(n =>
                {
                    var zu = "Prior";
                    if (n.Seite == ZirkelSeite.Sekundant)
                    {
                        zu = "Sekundant";
                    }
                    return new ZirkelNotizZeile
                    {
                        Id = n.Id,
                        Typ = n.Typ ?? "Notiz",
                        Zu = zu,
                        Von = n.Spielfigur?.HesparisCharakter?.Anzeigename ?? "",
                        Text = n.Text,
                        Bestaetigt = n.Bestaetigt,
                    };
                }).ToList();
            model.Figuren = await FigurenAsync(kampagne.Id);

            model.Fall = FallBauen(daten, sl, rmSchluessel, rmErgebnis);

            if (sl)
            {
                var jetzt = DateTime.UtcNow;
                model.AktiveSperren = daten.Felder.Count(f => f.GesperrtBis > jetzt);
                model.Versuche = daten.Versuche
                    .OrderByDescending(v => v.Zeitpunkt)
                    .Select(v => new ZirkelVersuchZeile
                    {
                        Tag = v.SpieltagText ?? "",
                        Was = VersuchBeschriftung(v),
                        Wert = v.Wert,
                    }).ToList();
            }

            model.BrettJson = BrettJson(daten, auswahl, sl);
            ViewData["FigurId"] = model.Figuren.FirstOrDefault(f => f.Aktiv)?.Id;
            return View(model);
        }

        // =====================================================================
        // Spieler: raten, Mordfall, Notizen
        // =====================================================================

        /// <summary>Ein Feld einritzen (JS: data-zchk). Richtig = gefunden, falsch = 5 Minuten gesperrt.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Raten(int nr, string seite, string feld, string? wert)
        {
            if (!DarfBearbeiten()) return Forbid();
            var s = ZirkelRegeln.SeiteAus(seite);
            var info = ZirkelRegeln.Felder(s).FirstOrDefault(f => f.Code == feld);
            if (info == null || nr < 1 || nr > 12 || string.IsNullOrWhiteSpace(wert))
                return RedirectToAction(nameof(Index), new { nr, wenden = true });

            var kampagne = await AkademieLadenAsync();
            var daten = await DatenLadenAsync(kampagne.Id);
            var person = daten.Person(nr, s);
            var wahrheit = ZirkelRegeln.Wert(person, feld);
            var stand = daten.Feld(nr, s, feld, _context);

            // Schon gefunden, gesperrt oder nichts hinterlegt: nichts tun
            if (string.IsNullOrEmpty(wahrheit) || stand.Gefunden || stand.GesperrtBis > DateTime.UtcNow)
                return RedirectToAction(nameof(Index), new { nr, wenden = true });

            // Text mit Tippfehler-Toleranz, Auswahlfelder müssen genau stimmen
            bool richtig;
            if (info.Typ == "text")
            {
                richtig = ZirkelRegeln.TextPasst(wahrheit, wert);
            }
            else
            {
                richtig = wahrheit == wert.Trim();
            }

            if (richtig)
            {
                stand.Gefunden = true;
                stand.GesperrtBis = null;
            }
            else
            {
                stand.GesperrtBis = DateTime.UtcNow + ZirkelRegeln.Sperre;
                // Vor- und Nachnamen so speichern, wie getippt; sonst lesbar (z. B. "König" statt "koenig")
                var gezeigt = wert.Trim();
                if (feld != "vn" && feld != "nn")
                {
                    gezeigt = ZirkelRegeln.Anzeige(feld, gezeigt);
                }
                await VersuchEintragenAsync(kampagne, nr, s, feld, gezeigt);
            }
            await _context.SaveChangesAsync();

            TempData["ZkFb"] = nr + "|" + seite + "|" + feld + "#" + (richtig ? "ok" : "bad");
            return RedirectToAction(nameof(Index), new { nr, wenden = true });
        }

        /// <summary>Mordfall: "Wer ist tot?" (frage = tot) bzw. "Wer war es?" (frage = moe).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FallRaten(int nr, string frage, string? wert)
        {
            if (!DarfBearbeiten()) return Forbid();
            if (frage != "tot" && frage != "moe") return BadRequest();
            var kampagne = await AkademieLadenAsync();
            var daten = await DatenLadenAsync(kampagne.Id);
            var fall = daten.Fall;
            var sperre = daten.Feld(0, ZirkelSeite.Prior, frage, _context);

            int rest = fall.RestVersucheOpfer;
            if (frage == "moe")
            {
                rest = fall.RestVersucheTaeter;
            }
            if (string.IsNullOrEmpty(wert) || rest <= 0 || sperre.GesperrtBis > DateTime.UtcNow)
                return RedirectToAction(nameof(Index), new { nr });

            string loesung = fall.OpferNr?.ToString() ?? "";
            if (frage == "moe")
            {
                loesung = TaeterWert(fall) ?? "";
            }
            bool richtig = loesung != "" && loesung == wert;

            if (richtig)
            {
                if (frage == "tot")
                {
                    fall.OpferGeloest = true;
                    // Wie im Original: das Brett dreht sich zum Toten
                    nr = fall.OpferNr!.Value;
                }
                else
                {
                    fall.TaeterGeloest = true;
                }
            }
            else
            {
                rest = Math.Max(0, rest - 1);
                if (frage == "tot")
                {
                    fall.RestVersucheOpfer = rest;
                }
                else
                {
                    fall.RestVersucheTaeter = rest;
                }
                if (rest > 0)
                {
                    sperre.GesperrtBis = DateTime.UtcNow + ZirkelRegeln.Sperre;
                }

                string gezeigt = wert;
                var teile = wert.Split('|');
                if (int.TryParse(teile[0], out var wNr) && wNr >= 1 && wNr <= 12)
                {
                    var wSeite = ZirkelSeite.Prior;
                    if (teile.Length > 1)
                    {
                        wSeite = ZirkelRegeln.SeiteAus(teile[1]);
                    }
                    gezeigt = ZirkelRegeln.Rolle(wNr, wSeite);
                }
                await VersuchEintragenAsync(kampagne, 0, ZirkelSeite.Prior, frage, gezeigt);
            }
            await _context.SaveChangesAsync();

            TempData["ZkFb"] = "m|" + frage + "#" + (richtig ? "ok" : "bad");
            return RedirectToAction(nameof(Index), new { nr });
        }

        /// <summary>Notiz oder Gerücht zur gewählten Magie (JS: data-znote).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NotizSchreiben(int nr, string seite, string typ, int? spielfigurId, string text)
        {
            if (!DarfBearbeiten()) return Forbid();
            if (string.IsNullOrWhiteSpace(text) || nr < 1 || nr > 12) return RedirectToAction(nameof(Index), new { nr });
            var kampagne = await AkademieLadenAsync();

            if (typ != "Gerücht")
            {
                typ = "Notiz";
            }
            _context.ZirkelNotizen.Add(new ZirkelNotiz
            {
                KampagneId = kampagne.Id,
                Nr = nr,
                Seite = ZirkelRegeln.SeiteAus(seite),
                Typ = typ,
                SpielfigurId = spielfigurId,
                Text = text.Trim(),
                ErstelltAm = DateTime.UtcNow,
            });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { nr });
        }

        // =====================================================================
        // Spielleitung
        // =====================================================================

        /// <summary>
        /// Wahrheit einer Person hinterlegen (Unterseite des Bretts). werte[vn], werte[nn] …;
        /// offiziell = Felder, die die Spieler schon sehen dürfen.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> WahrheitSpeichern(int nr, string seite, Dictionary<string, string?> werte, List<string> offiziell)
        {
            var s = ZirkelRegeln.SeiteAus(seite);
            var kampagne = await AkademieLadenAsync();
            var daten = await DatenLadenAsync(kampagne.Id);
            var person = daten.Person(nr, s);

            foreach (var info in ZirkelRegeln.Felder(s))
            {
                ZirkelRegeln.WertSetzen(person, info.Code, werte.GetValueOrDefault(info.Code));
                var stand = daten.Feld(nr, s, info.Code, _context);
                stand.Gefunden = offiziell.Contains(info.Code) && !string.IsNullOrEmpty(ZirkelRegeln.Wert(person, info.Code));
            }
            await _context.SaveChangesAsync();

            TempData["Success"] = ZirkelRegeln.Rolle(nr, s) + " gespeichert.";
            return RedirectToAction(nameof(Index), new { nr, wenden = true });
        }

        /// <summary>Treue, wem jemand wirklich dient, Status und geheime Notiz (JS: zkSlSecret).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> GeheimesSpeichern(int nr,
            string? priorTreue, string? priorDient, string? priorStatus, string? priorNotiz,
            string? sekundantTreue, string? sekundantDient, string? sekundantNotiz)
        {
            var kampagne = await AkademieLadenAsync();
            var daten = await DatenLadenAsync(kampagne.Id);

            var prior = daten.Person(nr, ZirkelSeite.Prior);
            prior.Treue = Leer(priorTreue);
            prior.Status = Leer(priorStatus);
            prior.GeheimeNotiz = Leer(priorNotiz);
            DientSetzen(prior, priorDient);

            var sekundant = daten.Person(nr, ZirkelSeite.Sekundant);
            sekundant.Treue = Leer(sekundantTreue);
            sekundant.GeheimeNotiz = Leer(sekundantNotiz);
            DientSetzen(sekundant, sekundantDient);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Geheimes zu " + ZirkelRegeln.MagieVon(nr).Name + " gespeichert.";
            return RedirectToAction(nameof(Index), new { nr });
        }

        /// <summary>Lösung des Mordfalls (JS: data-zm). Ein neuer Toter oder Täter macht die Lösung wieder offen.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> FallSpeichern(int nr, int? tot, string? moe, int max, string? motiv)
        {
            var kampagne = await AkademieLadenAsync();
            var daten = await DatenLadenAsync(kampagne.Id);
            var fall = daten.Fall;

            if (tot != fall.OpferNr)
            {
                fall.OpferNr = tot;
                fall.OpferGeloest = false;
                fall.TaeterGeloest = false;
                fall.MotivAufgedeckt = false;
            }

            int? taeterNr = null;
            ZirkelSeite? taeterSeite = null;
            if (!string.IsNullOrEmpty(moe))
            {
                var teile = moe.Split('|');
                taeterNr = int.Parse(teile[0]);
                taeterSeite = ZirkelRegeln.SeiteAus(teile[1]);
            }
            if (taeterNr != fall.TaeterNr || taeterSeite != fall.TaeterSeite)
            {
                fall.TaeterNr = taeterNr;
                fall.TaeterSeite = taeterSeite;
                fall.TaeterGeloest = false;
                fall.MotivAufgedeckt = false;
            }

            max = Math.Clamp(max, 1, 9);
            if (max != fall.MaxVersuche)
            {
                // Eine neue Versuchszahl füllt beide Stufen neu auf
                fall.MaxVersuche = max;
                fall.RestVersucheOpfer = max;
                fall.RestVersucheTaeter = max;
            }
            fall.Motiv = Leer(motiv);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Lösung des Falls gespeichert.";
            return RedirectToAction(nameof(Index), new { nr });
        }

        /// <summary>Knöpfe im Fall (JS: data-zma): +1 Versuch, Sperren aufheben, Motiv, zurücksetzen.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> FallAktion(int nr, string aktion)
        {
            var kampagne = await AkademieLadenAsync();
            var daten = await DatenLadenAsync(kampagne.Id);
            var fall = daten.Fall;
            var sperreTot = daten.Feld(0, ZirkelSeite.Prior, "tot", _context);
            var sperreMoe = daten.Feld(0, ZirkelSeite.Prior, "moe", _context);

            switch (aktion)
            {
                case "tot":
                    fall.RestVersucheOpfer = Math.Min(9, fall.RestVersucheOpfer + 1);
                    sperreTot.GesperrtBis = null;
                    break;
                case "moe":
                    fall.RestVersucheTaeter = Math.Min(9, fall.RestVersucheTaeter + 1);
                    sperreMoe.GesperrtBis = null;
                    break;
                case "entsperren":
                    sperreTot.GesperrtBis = null;
                    sperreMoe.GesperrtBis = null;
                    break;
                case "motiv":
                    fall.MotivAufgedeckt = !fall.MotivAufgedeckt;
                    break;
                case "zuruecksetzen":
                    fall.OpferGeloest = false;
                    fall.TaeterGeloest = false;
                    fall.MotivAufgedeckt = false;
                    fall.RestVersucheOpfer = fall.MaxVersuche;
                    fall.RestVersucheTaeter = fall.MaxVersuche;
                    sperreTot.GesperrtBis = null;
                    sperreMoe.GesperrtBis = null;
                    break;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { nr });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> NotizBestaetigen(int id, int nr)
        {
            var notiz = await _context.ZirkelNotizen.FirstOrDefaultAsync(n => n.Id == id);
            if (notiz != null)
            {
                notiz.Bestaetigt = !notiz.Bestaetigt;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { nr });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> NotizLoeschen(int id, int nr)
        {
            var notiz = await _context.ZirkelNotizen.FirstOrDefaultAsync(n => n.Id == id);
            if (notiz != null)
            {
                _context.ZirkelNotizen.Remove(notiz);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { nr });
        }

        /// <summary>Alle Sperren aufheben (JS: data-zunlockall).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> SperrenAufheben(int nr)
        {
            var kampagne = await AkademieLadenAsync();
            var gesperrt = await _context.ZirkelFelder
                .Where(f => f.KampagneId == kampagne.Id && f.GesperrtBis != null)
                .ToListAsync();
            foreach (var f in gesperrt)
            {
                f.GesperrtBis = null;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { nr });
        }

        /// <summary>Liste der Fehlversuche leeren (JS: data-zlogclr).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> VersucheLeeren(int nr)
        {
            var kampagne = await AkademieLadenAsync();
            var alle = await _context.ZirkelVersuche.Where(v => v.KampagneId == kampagne.Id).ToListAsync();
            _context.ZirkelVersuche.RemoveRange(alle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { nr });
        }

        // =====================================================================
        // Daten
        // =====================================================================

        /// <summary>Alles, was der Zirkel einer Akademie hat, einmal geladen.</summary>
        private class ZirkelDaten
        {
            public int KampagneId { get; init; }
            public List<ZirkelPerson> Personen { get; init; } = new();
            public List<ZirkelFeld> Felder { get; init; } = new();
            public List<ZirkelVersuch> Versuche { get; init; } = new();
            public List<ZirkelNotiz> Notizen { get; init; } = new();
            public ZirkelMordfall Fall { get; init; } = null!;

            public ZirkelPerson Person(int nr, ZirkelSeite seite) =>
                Personen.First(p => p.Nr == nr && p.Seite == seite);

            /// <summary>Stand eines Feldes; fehlt er, wird er angelegt.</summary>
            public ZirkelFeld Feld(int nr, ZirkelSeite seite, string feld, ApplicationDbContext context)
            {
                var f = Felder.FirstOrDefault(x => x.Nr == nr && x.Seite == seite && x.Feld == feld);
                if (f == null)
                {
                    f = new ZirkelFeld { KampagneId = KampagneId, Nr = nr, Seite = seite, Feld = feld };
                    context.ZirkelFelder.Add(f);
                    Felder.Add(f);
                }
                return f;
            }

            /// <summary>Darf der Betrachter den Wert sehen? Beim Prior ist die Magie immer bekannt.</summary>
            public bool Gefunden(int nr, ZirkelSeite seite, string feld)
            {
                if (seite == ZirkelSeite.Prior && feld == "mag") return true;
                return Felder.Any(x => x.Nr == nr && x.Seite == seite && x.Feld == feld && x.Gefunden);
            }

            public string? Sichtbar(int nr, ZirkelSeite seite, string feld, bool sl)
            {
                if (seite == ZirkelSeite.Prior && feld == "mag") return nr.ToString();
                if (sl || Gefunden(nr, seite, feld)) return ZirkelRegeln.Wert(Person(nr, seite), feld);
                return null;
            }

            public string Name(int nr, ZirkelSeite seite, bool sl)
            {
                var teile = new[]
                {
                    ZirkelRegeln.Anzeige("vn", Sichtbar(nr, seite, "vn", sl)),
                    ZirkelRegeln.Anzeige("nn", Sichtbar(nr, seite, "nn", sl)),
                };
                return string.Join(" ", teile.Where(t => t != ""));
            }

            /// <summary>"Prior des Lichts · Aldric Morn" (JS: zkPerson).</summary>
            public string PersonText(int nr, ZirkelSeite seite, bool sl)
            {
                var name = Name(nr, seite, sl);
                if (name == "") return ZirkelRegeln.Rolle(nr, seite);
                return ZirkelRegeln.Rolle(nr, seite) + " · " + name;
            }
        }

        private async Task<ZirkelDaten> DatenLadenAsync(int kampagneId)
        {
            var daten = new ZirkelDaten
            {
                KampagneId = kampagneId,
                Personen = await _context.ZirkelPersonen.Where(p => p.KampagneId == kampagneId).ToListAsync(),
                Felder = await _context.ZirkelFelder.Where(f => f.KampagneId == kampagneId).ToListAsync(),
                Versuche = await _context.ZirkelVersuche.Where(v => v.KampagneId == kampagneId).ToListAsync(),
                Notizen = await _context.ZirkelNotizen
                    .Where(n => n.KampagneId == kampagneId)
                    .Include(n => n.Spielfigur).ThenInclude(f => f!.HesparisCharakter)
                    .ToListAsync(),
                Fall = await _context.ZirkelMordfaelle.FirstOrDefaultAsync(m => m.KampagneId == kampagneId)
                       ?? new ZirkelMordfall { KampagneId = kampagneId },
            };

            // Beim ersten Öffnen: 24 leere Personen und den (leeren) Fall anlegen
            bool neu = false;
            foreach (var m in ZirkelRegeln.Magien)
            {
                foreach (var seite in new[] { ZirkelSeite.Prior, ZirkelSeite.Sekundant })
                {
                    if (!daten.Personen.Any(p => p.Nr == m.Nr && p.Seite == seite))
                    {
                        var p = new ZirkelPerson { KampagneId = kampagneId, Nr = m.Nr, Seite = seite };
                        _context.ZirkelPersonen.Add(p);
                        daten.Personen.Add(p);
                        neu = true;
                    }
                }
            }
            if (daten.Fall.Id == 0)
            {
                _context.ZirkelMordfaelle.Add(daten.Fall);
                neu = true;
            }
            if (neu)
            {
                await _context.SaveChangesAsync();
            }
            return daten;
        }

        private ZirkelSpalte SpalteBauen(ZirkelDaten daten, int nr, ZirkelSeite seite, bool sl, string? rmSchluessel, string? rmErgebnis)
        {
            var magie = ZirkelRegeln.MagieVon(nr);
            var person = daten.Person(nr, seite);
            var spalte = new ZirkelSpalte
            {
                Seite = seite,
                Titel = seite == ZirkelSeite.Prior ? "Prior" : "Sekundant",
                Genitiv = magie.Genitiv,
                Name = daten.Name(nr, seite, sl),
            };

            var jetzt = DateTime.UtcNow;
            foreach (var info in ZirkelRegeln.Felder(seite))
            {
                var schluessel = nr + "|" + ZirkelRegeln.Kurz(seite) + "|" + info.Code;
                var wahrheit = ZirkelRegeln.Wert(person, info.Code);
                var stand = daten.Felder.FirstOrDefault(f => f.Nr == nr && f.Seite == seite && f.Feld == info.Code);
                var zeile = new ZirkelZeile
                {
                    Feld = info.Code,
                    Name = info.Name,
                    Typ = info.Typ,
                    Schluessel = schluessel,
                    Wahrheit = wahrheit,
                    Gefunden = stand?.Gefunden ?? false,
                };
                if (zeile.Gefunden || sl)
                {
                    zeile.Anzeige = ZirkelRegeln.Anzeige(info.Code, wahrheit);
                }
                if (stand?.GesperrtBis > jetzt)
                {
                    zeile.SperrSekunden = (int)Math.Ceiling((stand.GesperrtBis.Value - jetzt).TotalSeconds);
                    zeile.LetzterVersuch = daten.Versuche
                        .Where(v => v.Nr == nr && v.Seite == seite && v.Feld == info.Code)
                        .OrderByDescending(v => v.Zeitpunkt)
                        .FirstOrDefault()?.Wert;
                }
                if (rmSchluessel == schluessel)
                {
                    zeile.Rueckmeldung = rmErgebnis;
                }
                spalte.Zeilen.Add(zeile);
            }
            return spalte;
        }

        /// <summary>Der Mordfall (JS: zkMordCard). Spieler sehen ihn erst, wenn ein Toter hinterlegt ist.</summary>
        private ZirkelFallViewModel? FallBauen(ZirkelDaten daten, bool sl, string? rmSchluessel, string? rmErgebnis)
        {
            var fall = daten.Fall;
            if (!sl && fall.OpferNr == null) return null;

            var jetzt = DateTime.UtcNow;
            int Sperre(string frage)
            {
                var f = daten.Felder.FirstOrDefault(x => x.Nr == 0 && x.Feld == frage);
                if (f?.GesperrtBis > jetzt)
                {
                    return (int)Math.Ceiling((f.GesperrtBis.Value - jetzt).TotalSeconds);
                }
                return 0;
            }
            string? Rueck(string frage)
            {
                if (rmSchluessel == "m|" + frage) return rmErgebnis;
                return null;
            }

            var priore = ZirkelRegeln.Magien
                .Select(m => (m.Nr.ToString(), daten.PersonText(m.Nr, ZirkelSeite.Prior, sl)))
                .ToList();
            var alle = new List<(string, string)>();
            foreach (var m in ZirkelRegeln.Magien)
            {
                alle.Add((m.Nr + "|p", daten.PersonText(m.Nr, ZirkelSeite.Prior, sl)));
                alle.Add((m.Nr + "|s", daten.PersonText(m.Nr, ZirkelSeite.Sekundant, sl)));
            }

            var vm = new ZirkelFallViewModel
            {
                HatToten = fall.OpferNr != null,
                TotNr = fall.OpferNr,
                MoerderWert = TaeterWert(fall),
                MaxVersuche = fall.MaxVersuche,
                MoerderGeloest = fall.TaeterGeloest,
                MotivOffen = fall.MotivAufgedeckt,
                Motiv = fall.Motiv,
                Tot = new ZirkelFallStufe
                {
                    Frage = "tot",
                    Titel = "1 · Wer ist tot?",
                    Geloest = fall.OpferGeloest,
                    Offen = true,
                    HatLoesung = fall.OpferNr != null,
                    Rest = fall.RestVersucheOpfer,
                    Max = fall.MaxVersuche,
                    SperrSekunden = Sperre("tot"),
                    Rueckmeldung = Rueck("tot"),
                    Optionen = priore,
                },
                Moerder = new ZirkelFallStufe
                {
                    Frage = "moe",
                    Titel = "2 · Wer war es?",
                    Geloest = fall.TaeterGeloest,
                    Offen = fall.OpferGeloest,
                    HatLoesung = fall.TaeterNr != null,
                    Rest = fall.RestVersucheTaeter,
                    Max = fall.MaxVersuche,
                    SperrSekunden = Sperre("moe"),
                    Rueckmeldung = Rueck("moe"),
                    Optionen = alle,
                },
            };
            if (fall.OpferNr != null)
            {
                vm.Tot.Antwort = daten.PersonText(fall.OpferNr.Value, ZirkelSeite.Prior, sl);
            }
            if (fall.TaeterNr != null && fall.TaeterSeite != null)
            {
                vm.Moerder.Antwort = daten.PersonText(fall.TaeterNr.Value, fall.TaeterSeite.Value, sl);
            }
            return vm;
        }

        /// <summary>
        /// Daten für das gezeichnete Brett (zirkel.js): welche Figur wo steht, in welcher Farbe,
        /// wer tot ist und (nur für Gott) wem jemand wirklich dient.
        /// </summary>
        private static string BrettJson(ZirkelDaten daten, int auswahl, bool sl)
        {
            int tot = 0;
            if (daten.Fall.OpferNr != null && (sl || daten.Fall.OpferGeloest))
            {
                tot = daten.Fall.OpferNr.Value;
            }

            var figuren = new Dictionary<string, object>();
            var pfeile = new List<object>();
            foreach (var m in ZirkelRegeln.Magien)
            {
                foreach (var seite in new[] { ZirkelSeite.Prior, ZirkelSeite.Sekundant })
                {
                    var key = m.Nr + "|" + ZirkelRegeln.Kurz(seite);
                    var farbe = m.Farbe;
                    if (seite == ZirkelSeite.Sekundant)
                    {
                        // Die Farbe des Sekundanten verrät seine Magie, also nur, wenn sie bekannt ist
                        farbe = "#a89568";
                        var mag = daten.Sichtbar(m.Nr, seite, "mag", sl);
                        if (int.TryParse(mag, out var magNr) && magNr >= 1 && magNr <= 12)
                        {
                            farbe = ZirkelRegeln.MagieVon(magNr).Farbe;
                        }
                    }
                    figuren[key] = new
                    {
                        fig = daten.Sichtbar(m.Nr, seite, "fig", sl) ?? "",
                        col = farbe,
                        title = daten.PersonText(m.Nr, seite, sl),
                    };

                    var person = daten.Person(m.Nr, seite);
                    if (sl && m.Nr == auswahl && person.DientNr != null && person.DientSeite != null)
                    {
                        pfeile.Add(new
                        {
                            from = key,
                            to = person.DientNr + "|" + ZirkelRegeln.Kurz(person.DientSeite.Value),
                            bad = person.Treue == "korrumpiert" || person.Treue == "Verräter",
                        });
                    }
                }
            }

            return JsonSerializer.Serialize(new
            {
                sel = auswahl,
                dead = tot,
                magien = ZirkelRegeln.Magien.Select(x => new { n = x.Nr, name = x.Name, col = x.Farbe }),
                figuren,
                pfeile,
            });
        }

        private async Task VersuchEintragenAsync(Kampagne kampagne, int nr, ZirkelSeite seite, string feld, string wert)
        {
            _context.ZirkelVersuche.Add(new ZirkelVersuch
            {
                KampagneId = kampagne.Id,
                Nr = nr,
                Seite = seite,
                Feld = feld,
                Wert = wert,
                Zeitpunkt = DateTime.UtcNow,
                // "Do, 3. September" → "3. September"
                SpieltagText = HesparisMenue.Spieltag(kampagne.Spieltag).Split(", ")[1],
            });

            // Nur die letzten 60 behalten
            var alt = await _context.ZirkelVersuche
                .Where(v => v.KampagneId == kampagne.Id)
                .OrderByDescending(v => v.Zeitpunkt)
                .Skip(ZirkelRegeln.VersucheMax - 1)
                .ToListAsync();
            _context.ZirkelVersuche.RemoveRange(alt);
        }

        /// <summary>"Prior der Seele · Beruf" bzw. "Fall · Wer ist tot?" (JS: zkKeyLabel).</summary>
        private static string VersuchBeschriftung(ZirkelVersuch v)
        {
            if (v.Nr == 0 && v.Feld == "tot") return "Fall · Wer ist tot?";
            if (v.Nr == 0 && v.Feld == "moe") return "Fall · Wer war es?";
            var feldName = ZirkelRegeln.Felder(v.Seite).FirstOrDefault(f => f.Code == v.Feld)?.Name ?? v.Feld;
            return ZirkelRegeln.Rolle(v.Nr, v.Seite) + " · " + feldName;
        }

        private static string? TaeterWert(ZirkelMordfall fall)
        {
            if (fall.TaeterNr == null || fall.TaeterSeite == null) return null;
            return fall.TaeterNr + "|" + ZirkelRegeln.Kurz(fall.TaeterSeite.Value);
        }

        private static void DientSetzen(ZirkelPerson person, string? wert)
        {
            if (string.IsNullOrEmpty(wert))
            {
                person.DientNr = null;
                person.DientSeite = null;
                return;
            }
            var teile = wert.Split('|');
            person.DientNr = int.Parse(teile[0]);
            person.DientSeite = ZirkelRegeln.SeiteAus(teile[1]);
        }

        private static string? Leer(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            return text.Trim();
        }

        /// <summary>Spielfiguren für "Von wem" bei den Notizen; die eigene ist vorausgewählt.</summary>
        private async Task<List<FigurChip>> FigurenAsync(int kampagneId)
        {
            var userId = GetUserId();
            var figuren = await _context.Spielfiguren
                .Where(f => f.KampagneId == kampagneId && f.HesparisCharakter != null)
                .Include(f => f.HesparisCharakter)
                .OrderBy(f => f.Position)
                .ToListAsync();
            return figuren.Select(f => new FigurChip
            {
                Id = f.Id,
                Name = f.HesparisCharakter!.Anzeigename,
                Aktiv = f.HesparisCharakter.UserId == userId,
            }).ToList();
        }
    }
}
