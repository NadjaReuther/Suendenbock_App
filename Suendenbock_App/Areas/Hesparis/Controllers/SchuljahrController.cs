using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain.Hesparis;
using Suendenbock_App.Models.ViewModels.Hesparis;

namespace Suendenbock_App.Areas.Hesparis.Controllers
{
    /// <summary>
    /// Gruppe "Schuljahr" (/Hesparis/Schuljahr/…): Questbrett. Weitere Seiten (Protokollhefte, Tagebuch,
    /// Akademie-Wertung, Jahresabschluss) folgen.
    /// Questbrett: Notizen, Hinweise, Hauptquest wählen und Versiegeln dürfen alle Spieler;
    /// neue Quests anschlagen nur Gott (in der HTML: sl-only).
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
