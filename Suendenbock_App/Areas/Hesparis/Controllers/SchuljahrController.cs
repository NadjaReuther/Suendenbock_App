using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain.Hesparis;
using Suendenbock_App.Models.ViewModels.Hesparis;

namespace Suendenbock_App.Areas.Hesparis.Controllers
{
    /// <summary>
    /// Gruppe "Schuljahr" (/Hesparis/Schuljahr/…): Questbrett und Tagebuch. Weitere Seiten (Protokollhefte,
    /// Akademie-Wertung, Jahresabschluss) folgen.
    /// Questbrett: Notizen, Hinweise, Hauptquest wählen und Versiegeln dürfen alle Spieler;
    /// neue Quests anschlagen nur Gott (in der HTML: sl-only).
    /// Tagebuch: Ausgaben und Kleinanzeigen schreiben dürfen alle Spieler und Gott.
    /// </summary>
    public class SchuljahrController : HesparisBaseController
    {
        private const string Aktiv = "aktiv";
        private const string Erledigt = "erledigt";

        public SchuljahrController(ApplicationDbContext context) : base(context)
        {
        }

        // =====================================================================
        // Questbrett
        // =====================================================================

        /// <summary>
        /// Das Brett im Ostflügel mit der Hauptquest und das Ermittlungsbuch mit allen Fällen (JS: MODS.missionen).
        /// buch/fall: Buch gleich aufgeschlagen beim gewählten Fall (nach Aktionen im Buch).
        /// frisch: gerade geklärter Hinweis, der rechts mit Schwung angepinnt wird.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Questbrett(bool buch = false, int? fall = null, int? frisch = null)
        {
            var akademie = await AkademieLadenAsync();
            var liste = await QuestsLadenAsync(akademie.Id);

            var aktive = liste.Where(m => m.Status == Aktiv).ToList();
            var haupt = aktive.FirstOrDefault();

            var model = new QuestbrettViewModel
            {
                Nebenquests = Math.Max(0, aktive.Count - 1),
                BuchOffen = buch,
                FrischId = frisch,
                DarfBearbeiten = DarfBearbeiten(),
                IstSpielleitung = IsGott(),
            };

            // Buch: erst die laufenden in Brett-Reihenfolge, dann die erledigten (JS: act.concat(…))
            foreach (var m in aktive.Concat(liste.Where(m => m.Status != Aktiv)))
            {
                var zeile = new QuestZeile
                {
                    Id = m.Id,
                    Nummer = m.Nummer,
                    Roemisch = HesparisRegeln.Roemisch(m.Nummer),
                    Titel = m.Titel,
                    Ziel = m.Ziel,
                    Notiz = m.Notiz,
                    StartText = m.StartText,
                    EndeText = m.EndeText,
                    VerfallenText = m.VerfallenText,
                    Erledigt = m.Status != Aktiv,
                    IstHaupt = m == haupt,
                    Hinweise = m.Hinweise.OrderBy(h => h.Position).ThenBy(h => h.Id).Select(h => new QuestHinweisZeile
                    {
                        Id = h.Id,
                        Text = h.Text,
                        Erledigt = h.Erledigt,
                    }).ToList(),
                };
                model.Buch.Add(zeile);
                if (zeile.IstHaupt)
                {
                    model.Haupt = zeile;
                }
            }

            // Aufgeschlagener Fall: der gewählte, sonst die Hauptquest, sonst der erste (JS: MS.sel || main || list[0])
            var auswahl = model.Buch.FirstOrDefault(q => q.Id == fall) ?? model.Haupt ?? model.Buch.FirstOrDefault();
            model.AuswahlId = auswahl?.Id;

            return View(model);
        }

        /// <summary>Neue Quest anschlagen (JS: data-madd). Sie kommt direkt hinter die Hauptquest, ohne Hauptquest an den Anfang.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuestAnschlagen(string titel, string? ziel)
        {
            if (!IsGott()) return Forbid();
            titel = (titel ?? "").Trim();
            if (titel.Length == 0)
            {
                TempData["Error"] = "Die Quest braucht einen Titel.";
                return RedirectToAction(nameof(Questbrett));
            }

            var akademie = await AkademieLadenAsync();
            var liste = await QuestsLadenAsync(akademie.Id);

            var neu = new Mission
            {
                KampagneId = akademie.Id,
                Nummer = liste.Select(m => m.Nummer).DefaultIfEmpty(0).Max() + 1,
                Titel = Kuerzen(titel, 80),
                Ziel = LeerZuNull(Kuerzen(ziel?.Trim(), 160)),
                Status = Aktiv,
                StartText = HesparisRegeln.TagUndMonat(akademie.Spieltag),
            };

            var haupt = liste.FirstOrDefault(m => m.Status == Aktiv);
            if (haupt != null)
            {
                liste.Insert(liste.IndexOf(haupt) + 1, neu);
            }
            else
            {
                liste.Insert(0, neu);
            }
            _context.Missionen.Add(neu);
            ReihenfolgeSetzen(liste);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Questbrett));
        }

        /// <summary>Notizen der Gruppe zur Hauptquest (JS: data-mnote). Wird beim Tippen im Hintergrund gespeichert.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuestNotiz(int id, string? notiz)
        {
            if (!DarfBearbeiten()) return Forbid();
            var mission = await QuestLadenAsync(id);
            if (mission == null) return NotFound();

            mission.Notiz = LeerZuNull(Kuerzen(notiz, 3000));
            await _context.SaveChangesAsync();

            // Aus dem Hintergrund (fetch) kein Neuladen, sonst zurück zum Brett
            if (Request.Headers["X-Requested-With"] == "fetch")
            {
                return NoContent();
            }
            return RedirectToAction(nameof(Questbrett));
        }

        /// <summary>Neuen Hinweis an die Hauptquest heften (JS: data-hadd).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HinweisAnheften(int id, string text)
        {
            if (!DarfBearbeiten()) return Forbid();
            var mission = await QuestLadenAsync(id);
            if (mission == null) return NotFound();

            text = Kuerzen((text ?? "").Trim(), 160);
            if (text.Length > 0 && mission.Status == Aktiv)
            {
                mission.Hinweise.Add(new MissionHinweis
                {
                    Position = mission.Hinweise.Select(h => h.Position).DefaultIfEmpty(-1).Max() + 1,
                    Text = text,
                });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Questbrett));
        }

        /// <summary>
        /// Hinweis abreißen oder wieder anheften (JS: data-hdone). Im Buch heißt das abhaken.
        /// buch: kam aus dem Ermittlungsbuch, dann bleibt es beim selben Fall aufgeschlagen.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HinweisUmschalten(int id, int hinweisId, bool buch = false)
        {
            if (!DarfBearbeiten()) return Forbid();
            var mission = await QuestLadenAsync(id);
            if (mission == null) return NotFound();

            var hinweis = mission.Hinweise.FirstOrDefault(h => h.Id == hinweisId);
            int? frisch = null;
            if (hinweis != null && mission.Status == Aktiv)
            {
                hinweis.Erledigt = !hinweis.Erledigt;
                if (hinweis.Erledigt)
                {
                    frisch = hinweis.Id;
                }
                await _context.SaveChangesAsync();
            }
            return ZurueckZumBrett(buch, mission.Id, frisch);
        }

        /// <summary>Hinweis ganz entfernen (JS: data-hdel, nur im Buch bei laufenden Fällen).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HinweisEntfernen(int id, int hinweisId)
        {
            if (!DarfBearbeiten()) return Forbid();
            var mission = await QuestLadenAsync(id);
            if (mission == null) return NotFound();

            var hinweis = mission.Hinweise.FirstOrDefault(h => h.Id == hinweisId);
            if (hinweis != null && mission.Status == Aktiv)
            {
                _context.MissionHinweise.Remove(hinweis);
                await _context.SaveChangesAsync();
            }
            return ZurueckZumBrett(true, mission.Id, null);
        }

        /// <summary>Laufende Nebenquest als Hauptquest ans Brett hängen (JS: data-mmain): sie rückt an den Anfang.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlsHauptquest(int id)
        {
            if (!DarfBearbeiten()) return Forbid();
            var akademie = await AkademieLadenAsync();
            var liste = await QuestsLadenAsync(akademie.Id);

            var mission = liste.FirstOrDefault(m => m.Id == id);
            if (mission == null) return NotFound();

            liste.Remove(mission);
            liste.Insert(0, mission);
            ReihenfolgeSetzen(liste);
            await _context.SaveChangesAsync();

            return ZurueckZumBrett(true, mission.Id, null);
        }

        /// <summary>
        /// Quest versiegeln und abschließen (JS: data-mdone). Wer noch Nachsitzen offen hat, das nicht in
        /// dieser Quest abgeleistet wurde, verliert sein ältestes offenes Nachsitzen (JS: nsDecay).
        /// Danach ist das Buch beim abgeschlossenen Fall aufgeschlagen.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuestAbschliessen(int id)
        {
            if (!DarfBearbeiten()) return Forbid();
            var akademie = await AkademieLadenAsync();
            var mission = await QuestLadenAsync(id);
            if (mission == null) return NotFound();
            if (mission.Status != Aktiv) return ZurueckZumBrett(true, mission.Id, null);

            mission.Status = Erledigt;
            mission.EndeText = HesparisRegeln.TagUndMonat(akademie.Spieltag);

            var verfallen = await NachsitzenVerfallenLassenAsync(akademie.Id, mission.Id);
            mission.VerfallenText = verfallen.Count > 0
                ? Kuerzen("Nachsitzen verfallen: " + string.Join(", ", verfallen), 200)
                : null;

            await _context.SaveChangesAsync();
            return ZurueckZumBrett(true, mission.Id, null);
        }

        // =====================================================================
        // Tagebuch: „Der Hesparis-Bote“
        // =====================================================================

        /// <summary>
        /// Jede Spielsitzung ist eine Ausgabe der Zeitung, rechts die Kleinanzeigen (= To-dos) (JS: renderProtokoll).
        /// ausgabe: aufgeschlagene Ausgabe (sonst die neueste). neu: neue Ausgabe setzen. bearbeiten: Ausgabe überarbeiten.
        /// frisch/frischAnzeige: gerade gedruckt, kommt mit Schwung herein. erledigt: Liste „Erledigt“ aufgeklappt.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Tagebuch(int? ausgabe, bool neu = false, bool bearbeiten = false,
                                                  int? frisch = null, int? frischAnzeige = null, bool erledigt = false)
        {
            var akademie = await AkademieLadenAsync();
            var eintraege = await _context.ProtokollEintraege
                .Where(p => p.KampagneId == akademie.Id)
                .Include(p => p.Beteiligte)
                .OrderByDescending(p => p.ErstelltAm)
                .ThenByDescending(p => p.Id)
                .ToListAsync();

            var model = new TagebuchViewModel
            {
                HeuteDatum = HesparisRegeln.TagebuchDatum(HesparisRegeln.Ortszeit(DateTime.UtcNow), akademie),
                Spieltag = HesparisRegeln.TagUndMonat(akademie.Spieltag),
                Figuren = await FigurChipsAsync(akademie.Id),
                FrischAusgabeId = frisch,
                FrischAnzeigeId = frischAnzeige,
                ErledigtOffen = erledigt,
                DarfBearbeiten = DarfBearbeiten(),
            };

            int n = eintraege.Count;
            for (int i = 0; i < n; i++)
            {
                var e = eintraege[i];
                var ortszeit = HesparisRegeln.Ortszeit(e.ErstelltAm);
                model.Ausgaben.Add(new AusgabeZeile
                {
                    Id = e.Id,
                    Nummer = n - i,
                    Titel = e.Titel,
                    Text = e.Text,
                    Datum = HesparisRegeln.TagebuchDatum(ortszeit, akademie),
                    Uhrzeit = ortszeit.ToString("HH:mm"),
                    DatumKurz = HesparisRegeln.DatumKurz(ortszeit),
                    Spieltag = e.Spieltag.HasValue ? HesparisRegeln.TagUndMonat(e.Spieltag.Value) : null,
                    Ueberarbeitet = e.BearbeitetAm.HasValue,
                    Dabei = e.Beteiligte.Select(b => b.SpielfigurId).ToHashSet(),
                });
            }

            // Ohne Ausgaben oder auf Wunsch: neue Ausgabe setzen (JS: boSel = 'new')
            if (!neu && n > 0)
            {
                model.Auswahl = model.Ausgaben.FirstOrDefault(a => a.Id == ausgabe) ?? model.Ausgaben[0];
                model.Bearbeiten = bearbeiten && model.DarfBearbeiten;
            }

            // Kleinanzeigen: offene nach Dringlichkeit, dann Alter; erledigte zuletzt abgehakt zuerst
            var aufgaben = await _context.Aufgaben.Where(a => a.KampagneId == akademie.Id).ToListAsync();
            model.Offen = aufgaben.Where(a => !a.Erledigt)
                .OrderBy(a => a.Prioritaet).ThenBy(a => a.ErstelltAm).ThenBy(a => a.Id)
                .Select(AnzeigeZeileAus).ToList();
            model.Erledigt = aufgaben.Where(a => a.Erledigt)
                .OrderByDescending(a => a.ErledigtAm ?? DateTime.MinValue).ThenByDescending(a => a.Id)
                .Select(AnzeigeZeileAus).ToList();

            return View(model);
        }

        /// <summary>Neue Ausgabe in Druck geben (JS: boForm, mode new). Schlagzeile oder Bericht muss da sein.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AusgabeDrucken(string? titel, string? text, int[]? wer)
        {
            if (!DarfBearbeiten()) return Forbid();
            titel = Kuerzen(titel?.Trim(), 80);
            text = Kuerzen(text?.Trim(), 4000);
            if (titel.Length == 0 && text.Length == 0)
            {
                return RedirectToAction(nameof(Tagebuch), new { neu = true });
            }

            var akademie = await AkademieLadenAsync();
            var eintrag = new ProtokollEintrag
            {
                KampagneId = akademie.Id,
                Titel = titel,
                Text = LeerZuNull(text),
                ErstelltAm = DateTime.UtcNow,
                Spieltag = akademie.Spieltag,
            };
            foreach (var figurId in await GueltigeFigurenAsync(akademie.Id, wer))
            {
                eintrag.Beteiligte.Add(new ProtokollBeteiligter { SpielfigurId = figurId });
            }
            _context.ProtokollEintraege.Add(eintrag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Tagebuch), new { ausgabe = eintrag.Id, frisch = eintrag.Id });
        }

        /// <summary>Ausgabe überarbeiten (JS: boForm, mode edit): Schlagzeile, Bericht und wer dabei war.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AusgabeSpeichern(int id, string? titel, string? text, int[]? wer)
        {
            if (!DarfBearbeiten()) return Forbid();
            var akademie = await AkademieLadenAsync();
            var eintrag = await _context.ProtokollEintraege
                .Include(p => p.Beteiligte)
                .FirstOrDefaultAsync(p => p.Id == id && p.KampagneId == akademie.Id);
            if (eintrag == null) return NotFound();

            titel = Kuerzen(titel?.Trim(), 80);
            text = Kuerzen(text?.Trim(), 4000);
            if (titel.Length == 0 && text.Length == 0)
            {
                return RedirectToAction(nameof(Tagebuch), new { ausgabe = id, bearbeiten = true });
            }

            eintrag.Titel = titel;
            eintrag.Text = LeerZuNull(text);
            eintrag.BearbeitetAm = DateTime.UtcNow;

            var neu = (await GueltigeFigurenAsync(akademie.Id, wer)).ToHashSet();
            foreach (var b in eintrag.Beteiligte.Where(b => !neu.Contains(b.SpielfigurId)).ToList())
            {
                eintrag.Beteiligte.Remove(b);
            }
            foreach (var figurId in neu.Where(f => eintrag.Beteiligte.All(b => b.SpielfigurId != f)))
            {
                eintrag.Beteiligte.Add(new ProtokollBeteiligter { SpielfigurId = figurId });
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Tagebuch), new { ausgabe = id });
        }

        /// <summary>Ausgabe einstampfen (JS: data-bodel e:…). Danach ist die neueste aufgeschlagen.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AusgabeEinstampfen(int id)
        {
            if (!DarfBearbeiten()) return Forbid();
            var akademie = await AkademieLadenAsync();
            var eintrag = await _context.ProtokollEintraege
                .FirstOrDefaultAsync(p => p.Id == id && p.KampagneId == akademie.Id);
            if (eintrag != null)
            {
                _context.ProtokollEintraege.Remove(eintrag);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Tagebuch));
        }

        /// <summary>Kleinanzeige aufgeben (JS: boAdForm).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnzeigeAufgeben(string? text, string? prio, int? ausgabe, bool erledigt = false)
        {
            if (!DarfBearbeiten()) return Forbid();
            text = Kuerzen(text?.Trim(), 140);
            if (text.Length == 0)
            {
                return RedirectToAction(nameof(Tagebuch), new { ausgabe, erledigt });
            }

            var akademie = await AkademieLadenAsync();
            var aufgabe = new Aufgabe
            {
                KampagneId = akademie.Id,
                Text = text,
                Prioritaet = HesparisRegeln.PrioAus(prio),
                ErstelltAm = DateTime.UtcNow,
            };
            _context.Aufgaben.Add(aufgabe);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Tagebuch), new { ausgabe, erledigt, frischAnzeige = aufgabe.Id });
        }

        /// <summary>Kleinanzeige abhaken oder wieder öffnen (JS: data-bocheck).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnzeigeAbhaken(int id, int? ausgabe, bool erledigt = false)
        {
            if (!DarfBearbeiten()) return Forbid();
            var aufgabe = await AufgabeLadenAsync(id);
            if (aufgabe == null) return NotFound();

            aufgabe.Erledigt = !aufgabe.Erledigt;
            aufgabe.ErledigtAm = aufgabe.Erledigt ? DateTime.UtcNow : null;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Tagebuch), new { ausgabe, erledigt });
        }

        /// <summary>Dringlichkeit weiterschalten: Dringend → Gesucht → Irgendwann → Dringend (JS: data-boprio).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnzeigePrio(int id, int? ausgabe, bool erledigt = false)
        {
            if (!DarfBearbeiten()) return Forbid();
            var aufgabe = await AufgabeLadenAsync(id);
            if (aufgabe == null) return NotFound();

            if (!aufgabe.Erledigt)
            {
                aufgabe.Prioritaet = HesparisRegeln.PrioNaechste(aufgabe.Prioritaet);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Tagebuch), new { ausgabe, erledigt });
        }

        /// <summary>Kleinanzeige entfernen (JS: data-bodel t:…).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnzeigeEntfernen(int id, int? ausgabe, bool erledigt = false)
        {
            if (!DarfBearbeiten()) return Forbid();
            var aufgabe = await AufgabeLadenAsync(id);
            if (aufgabe != null)
            {
                _context.Aufgaben.Remove(aufgabe);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Tagebuch), new { ausgabe, erledigt });
        }

        private async Task<Aufgabe?> AufgabeLadenAsync(int id)
        {
            var akademie = await AkademieLadenAsync();
            return await _context.Aufgaben.FirstOrDefaultAsync(a => a.Id == id && a.KampagneId == akademie.Id);
        }

        /// <summary>Nur Spielfiguren dieser Akademie, jede einmal.</summary>
        private async Task<List<int>> GueltigeFigurenAsync(int kampagneId, int[]? wer)
        {
            if (wer == null || wer.Length == 0) return new List<int>();
            var erlaubt = await _context.Spielfiguren
                .Where(f => f.KampagneId == kampagneId && wer.Contains(f.Id))
                .Select(f => f.Id)
                .ToListAsync();
            return erlaubt.Distinct().ToList();
        }

        private static AnzeigeZeile AnzeigeZeileAus(Aufgabe a) => new()
        {
            Id = a.Id,
            Text = a.Text,
            Prio = HesparisRegeln.PrioSchluessel(a.Prioritaet),
            PrioText = HesparisRegeln.PrioText(a.Prioritaet),
            Erledigt = a.Erledigt,
        };

        // ---------------------------------------------------------------- Hilfen

        /// <summary>Alle Quests der Akademie in Brett-Reihenfolge, samt Hinweisen.</summary>
        private Task<List<Mission>> QuestsLadenAsync(int kampagneId) => _context.Missionen
            .Where(m => m.KampagneId == kampagneId)
            .Include(m => m.Hinweise)
            .OrderBy(m => m.Position)
            .ThenByDescending(m => m.Nummer)
            .ToListAsync();

        private async Task<Mission?> QuestLadenAsync(int id)
        {
            var akademie = await AkademieLadenAsync();
            return await _context.Missionen
                .Include(m => m.Hinweise)
                .FirstOrDefaultAsync(m => m.Id == id && m.KampagneId == akademie.Id);
        }

        /// <summary>Positionen lückenlos nach der Listenreihenfolge vergeben.</summary>
        private static void ReihenfolgeSetzen(List<Mission> liste)
        {
            for (int i = 0; i < liste.Count; i++)
            {
                liste[i].Position = i;
            }
        }

        /// <summary>
        /// JS: nsDecay. Für jede Spielfigur mit offenem Nachsitzen: hat sie keins, das in dieser Quest abgeleistet
        /// wird, verfällt das älteste. Liefert die Namen der Betroffenen.
        /// </summary>
        private async Task<List<string>> NachsitzenVerfallenLassenAsync(int kampagneId, int missionId)
        {
            var offen = await _context.Nachsitzen
                .Where(n => n.KampagneId == kampagneId && !n.Verfallen)
                .Include(n => n.Spielfigur).ThenInclude(f => f.HesparisCharakter)
                .ToListAsync();

            var namen = new List<string>();
            foreach (var gruppe in offen.GroupBy(n => n.Spielfigur).OrderBy(g => g.Key.Position))
            {
                if (gruppe.Any(n => n.MissionId == missionId)) continue;

                var aeltestes = gruppe.OrderBy(n => n.ErstelltAm).ThenBy(n => n.Id).First();
                aeltestes.Verfallen = true;
                namen.Add(gruppe.Key.HesparisCharakter?.Anzeigename ?? "Jemand");
            }
            return namen;
        }

        private IActionResult ZurueckZumBrett(bool buch, int fall, int? frisch)
        {
            if (buch)
            {
                return RedirectToAction(nameof(Questbrett), new { buch = true, fall });
            }
            return RedirectToAction(nameof(Questbrett), new { frisch });
        }

        private static string Kuerzen(string? text, int max)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Length > max ? text[..max] : text;
        }

        private static string? LeerZuNull(string? text) => string.IsNullOrWhiteSpace(text) ? null : text;
    }
}
