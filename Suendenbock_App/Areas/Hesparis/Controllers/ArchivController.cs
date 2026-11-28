using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain.Hesparis;
using Suendenbock_App.Models.ViewModels.Hesparis;

namespace Suendenbock_App.Areas.Hesparis.Controllers
{
    /// <summary>
    /// Akademie · Archiv (/Hesparis/Archiv): das Aktenregal hinter einer verschlossenen Tür (JS: MODS.archiv).
    /// Hinein kommt, wer das Schlüsselwort kennt; die Tür bleibt offen, bis jemand sie abschließt oder der
    /// Browser geschlossen wird (Sitzungs-Cookie). Dokumente und Kisten verwaltet nur Gott.
    /// </summary>
    public class ArchivController : HesparisBaseController
    {
        private const string TuerCookie = "hes-archiv";

        /// <summary>Der Cookie gilt nur fürs Archiv.</summary>
        private const string TuerPfad = "/Hesparis/Archiv";

        public ArchivController(ApplicationDbContext context) : base(context)
        {
        }

        // =====================================================================
        // Seiten
        // =====================================================================

        /// <summary>
        /// Tür oder Aktenregal mit Lesepult. kiste: Code der herausgezogenen Kiste, dok: Dokument auf dem Pult,
        /// bearbeiten: Gott bearbeitet es, tuer: Gott stellt Tür und Schlüssel ein, kisten: Verwaltung offen.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(string? kiste, int? dok, bool bearbeiten = false, bool tuer = false, bool kisten = false)
        {
            var akademie = await AkademieLadenAsync();
            await StartinhalteAnlegenAsync(akademie);
            bool sl = IsGott();

            var model = new ArchivViewModel
            {
                IstSpielleitung = sl,
                Hinweis = akademie.ArchivHinweis,
                Schluessel = sl ? Schluessel(akademie) : null,
            };

            // Ohne passenden Schlüssel (oder wenn Gott die Tür einstellt) steht man vor der Tür
            if (!TuerOffen(akademie) || (tuer && sl))
            {
                model.TuerSeite = true;
                return View("Tuer", model);
            }

            var kategorien = await _context.ArchivKategorien
                .Where(k => k.KampagneId == akademie.Id)
                .OrderBy(k => k.Position).ThenBy(k => k.Id)
                .ToListAsync();
            var dokumente = await _context.ArchivDokumente
                .Where(d => d.KampagneId == akademie.Id)
                .Include(d => d.Kategorien)
                .OrderBy(d => d.Id)
                .ToListAsync();
            var sichtbar = dokumente.Where(d => sl || !d.Verborgen).ToList();
            var ids = kategorien.Select(k => k.Id).ToHashSet();

            // Regal: je Kategorie eine Kiste, dazu "Ohne Kategorie", wenn dort etwas liegt (JS: arBoxes)
            for (int i = 0; i < kategorien.Count; i++)
            {
                var k = kategorien[i];
                model.Kisten.Add(new ArchivKiste
                {
                    Id = k.Id,
                    Code = k.Code,
                    Name = k.Name,
                    Signatur = k.Signatur ?? "",
                    Farbe = ArchivRegeln.KistenFarbe(k.Code, i),
                    Anzahl = sichtbar.Count(d => d.Kategorien.Any(x => x.ArchivKategorieId == k.Id)),
                });
            }
            var ohne = sichtbar.Where(d => !d.Kategorien.Any(x => ids.Contains(x.ArchivKategorieId))).ToList();
            if (ohne.Count > 0)
            {
                model.Kisten.Add(new ArchivKiste
                {
                    Code = ArchivRegeln.OhneKategorie,
                    Name = "Ohne Kategorie",
                    Signatur = "—",
                    Farbe = ArchivRegeln.KistenFarbe(ArchivRegeln.OhneKategorie, 0),
                    Anzahl = ohne.Count,
                });
            }

            model.Kiste = model.Kisten.FirstOrDefault(k => k.Code == kiste) ?? model.Kisten.FirstOrDefault();
            if (model.Kiste != null)
            {
                model.Dokumente = model.Kiste.Id.HasValue
                    ? sichtbar.Where(d => d.Kategorien.Any(x => x.ArchivKategorieId == model.Kiste.Id)).ToList()
                    : ohne;
            }
            model.Dokument = model.Dokumente.FirstOrDefault(d => d.Id == dok) ?? model.Dokumente.FirstOrDefault();
            model.Bearbeiten = bearbeiten && sl && model.Dokument != null;

            if (sl)
            {
                model.Kategorien = kategorien
                    .Select(k => (k, dokumente.Count(d => d.Kategorien.Any(x => x.ArchivKategorieId == k.Id))))
                    .ToList();
                model.KategorienOffen = kisten;
            }
            return View(model);
        }

        // =====================================================================
        // Die Tür
        // =====================================================================

        /// <summary>
        /// Schlüssel drehen (JS: data-arkey). Aus dem Browser (fetch) kommt nur "passt / passt nicht" zurück,
        /// die Tür dreht sich dort; ohne JavaScript geht es per Weiterleitung.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Oeffnen(string? schluessel)
        {
            var akademie = await AkademieLadenAsync();
            bool passt = ArchivRegeln.Normalisieren(schluessel) == ArchivRegeln.Normalisieren(Schluessel(akademie));
            if (passt)
            {
                TuerAufschliessen(akademie);
            }
            if (Request.Headers["X-Requested-With"] == "fetch")
            {
                return Json(new { passt });
            }
            if (!passt)
            {
                TempData["Error"] = "Der Schlüssel passt nicht.";
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>Spielleitung: ohne Schlüssel hinein (JS: data-arslin).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OhneSchluessel()
        {
            if (!IsGott()) return Forbid();
            TuerAufschliessen(await AkademieLadenAsync());
            return RedirectToAction(nameof(Index));
        }

        /// <summary>Tür hinter euch abschließen (JS: data-arlock).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Abschliessen()
        {
            Response.Cookies.Delete(TuerCookie, new CookieOptions { Path = TuerPfad });
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Spielleitung: Schlüsselwort oder Zettel am Türrahmen ändern (JS: data-arkeyset, data-arhint).
        /// Leeres Schlüsselwort wird ignoriert. Speichert beim Tippen im Hintergrund.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tuer(string feld, string? wert)
        {
            if (!IsGott()) return Forbid();
            var akademie = await AkademieLadenAsync();
            bool warOffen = TuerOffen(akademie);
            if (feld == "schluessel")
            {
                wert = Kuerzen(wert?.Trim(), 60);
                if (wert.Length > 0) akademie.ArchivSchluessel = wert;
            }
            else if (feld == "hinweis")
            {
                akademie.ArchivHinweis = LeerZuNull(Kuerzen(wert, 300));
            }
            else
            {
                return BadRequest();
            }
            await _context.SaveChangesAsync();
            // Die Spielleitung bleibt drin, auch wenn sie gerade das Schlüsselwort ändert
            if (warOffen) TuerAufschliessen(akademie);
            return NoContent();
        }

        // =====================================================================
        // Dokumente (Gott)
        // =====================================================================

        /// <summary>
        /// Neues Dokument in der offenen Kiste, zunächst für die Spieler versteckt (JS: data-arnew).
        /// Signatur wie "A-12 / 8": Kiste und laufende Nummer.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DokumentNeu(string? kiste)
        {
            if (!IsGott()) return Forbid();
            var akademie = await AkademieLadenAsync();
            var kat = await _context.ArchivKategorien.FirstOrDefaultAsync(k => k.KampagneId == akademie.Id && k.Code == kiste);

            var dokument = new ArchivDokument
            {
                KampagneId = akademie.Id,
                Art = "Bericht",
                Verborgen = true,
            };
            if (kat != null)
            {
                int anzahl = await _context.ArchivDokumentKategorien.CountAsync(x => x.ArchivKategorieId == kat.Id);
                dokument.Signatur = Kuerzen((kat.Signatur ?? "") + " / " + (anzahl + 1), 200);
                dokument.Kategorien.Add(new ArchivDokumentKategorie { ArchivKategorieId = kat.Id });
            }
            _context.ArchivDokumente.Add(dokument);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { kiste, dok = dokument.Id, bearbeiten = true });
        }

        /// <summary>
        /// Dokument speichern (JS: data-arform). Liegt es danach nicht mehr in der offenen Kiste, springt das Regal
        /// zu seiner ersten Kiste bzw. zu "Ohne Kategorie".
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DokumentSpeichern(int id, string? kiste, string? titel, string? art, string? signatur,
            string? datum, string? text, string? unterschrift, bool versiegelt, bool verborgen, string[]? kisten)
        {
            if (!IsGott()) return Forbid();
            var akademie = await AkademieLadenAsync();
            var dokument = await DokumentLadenAsync(akademie.Id, id);
            if (dokument == null) return NotFound();

            dokument.Titel = Kuerzen(titel?.Trim(), 80);
            dokument.Art = ArchivRegeln.Arten.Contains(art) ? art : "Bericht";
            dokument.Signatur = LeerZuNull(Kuerzen(signatur?.Trim(), 20));
            dokument.DatumText = LeerZuNull(Kuerzen(datum?.Trim(), 80));
            dokument.Text = LeerZuNull(Kuerzen(text?.TrimEnd(), 4000));
            dokument.Unterschrift = LeerZuNull(Kuerzen(unterschrift?.Trim(), 60));
            dokument.Versiegelt = versiegelt;
            dokument.Verborgen = verborgen;

            var gewaehlt = await _context.ArchivKategorien
                .Where(k => k.KampagneId == akademie.Id && (kisten ?? Array.Empty<string>()).Contains(k.Code))
                .ToListAsync();
            foreach (var x in dokument.Kategorien.Where(x => gewaehlt.All(k => k.Id != x.ArchivKategorieId)).ToList())
            {
                dokument.Kategorien.Remove(x);
            }
            foreach (var k in gewaehlt.Where(k => dokument.Kategorien.All(x => x.ArchivKategorieId != k.Id)))
            {
                dokument.Kategorien.Add(new ArchivDokumentKategorie { ArchivKategorieId = k.Id });
            }
            await _context.SaveChangesAsync();

            if (gewaehlt.Count == 0)
            {
                kiste = ArchivRegeln.OhneKategorie;
            }
            else if (gewaehlt.All(k => k.Code != kiste))
            {
                kiste = gewaehlt.OrderBy(k => k.Position).First().Code;
            }
            return RedirectToAction(nameof(Index), new { kiste, dok = id });
        }

        /// <summary>Siegel brechen / versiegeln (JS: data-arseal).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Siegel(int id, string? kiste)
        {
            if (!IsGott()) return Forbid();
            var akademie = await AkademieLadenAsync();
            var dokument = await DokumentLadenAsync(akademie.Id, id);
            if (dokument == null) return NotFound();
            dokument.Versiegelt = !dokument.Versiegelt;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { kiste, dok = id });
        }

        /// <summary>Für die Spieler verstecken / zeigen (JS: data-arhide).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verstecken(int id, string? kiste)
        {
            if (!IsGott()) return Forbid();
            var akademie = await AkademieLadenAsync();
            var dokument = await DokumentLadenAsync(akademie.Id, id);
            if (dokument == null) return NotFound();
            dokument.Verborgen = !dokument.Verborgen;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { kiste, dok = id });
        }

        /// <summary>Dokument löschen (JS: data-ardel).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DokumentLoeschen(int id, string? kiste)
        {
            if (!IsGott()) return Forbid();
            var akademie = await AkademieLadenAsync();
            var dokument = await DokumentLadenAsync(akademie.Id, id);
            if (dokument != null)
            {
                _context.ArchivDokumente.Remove(dokument);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { kiste });
        }

        // =====================================================================
        // Kisten (Gott)
        // =====================================================================

        /// <summary>Neue Kiste; ohne Signatur gibt es eine wie "H-08" (JS: data-arkadd).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KisteNeu(string? signatur, string? name)
        {
            if (!IsGott()) return Forbid();
            var akademie = await AkademieLadenAsync();
            name = Kuerzen(name?.Trim(), 40);
            if (name.Length == 0) return RedirectToAction(nameof(Index), new { kisten = true });

            var alle = await _context.ArchivKategorien.Where(k => k.KampagneId == akademie.Id).ToListAsync();
            signatur = Kuerzen(signatur?.Trim(), 8);
            if (signatur.Length == 0)
            {
                signatur = (char)('A' + alle.Count % 26) + "-" + (alle.Count + 1).ToString("00");
            }
            string code;
            do
            {
                code = "k" + Convert.ToHexString(RandomNumberGenerator.GetBytes(3)).ToLowerInvariant();
            } while (alle.Any(k => k.Code == code));

            _context.ArchivKategorien.Add(new ArchivKategorie
            {
                KampagneId = akademie.Id,
                Code = code,
                Name = name,
                Signatur = signatur,
                Position = alle.Select(k => k.Position).DefaultIfEmpty(0).Max() + 1,
            });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { kiste = code, kisten = true });
        }

        /// <summary>Name oder Signatur einer Kiste ändern, beim Tippen im Hintergrund (JS: data-arkname, data-arksig).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KisteFeld(int id, string feld, string? wert)
        {
            if (!IsGott()) return Forbid();
            var akademie = await AkademieLadenAsync();
            var kat = await _context.ArchivKategorien.FirstOrDefaultAsync(k => k.Id == id && k.KampagneId == akademie.Id);
            if (kat == null) return NotFound();
            wert = wert?.Trim();
            // Leer lassen gilt nicht, dann bleibt der alte Wert (JS: t.value.trim() || c.name)
            if (feld == "name" && !string.IsNullOrEmpty(wert)) kat.Name = Kuerzen(wert, 40);
            else if (feld == "signatur" && !string.IsNullOrEmpty(wert)) kat.Signatur = Kuerzen(wert, 8);
            else if (feld != "name" && feld != "signatur") return BadRequest();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Kiste entfernen; ihre Dokumente bleiben erhalten (JS: data-arkdel).</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KisteLoeschen(int id)
        {
            if (!IsGott()) return Forbid();
            var akademie = await AkademieLadenAsync();
            var kat = await _context.ArchivKategorien.FirstOrDefaultAsync(k => k.Id == id && k.KampagneId == akademie.Id);
            if (kat != null)
            {
                var zuordnungen = await _context.ArchivDokumentKategorien.Where(x => x.ArchivKategorieId == kat.Id).ToListAsync();
                _context.ArchivDokumentKategorien.RemoveRange(zuordnungen);
                _context.ArchivKategorien.Remove(kat);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { kisten = true });
        }

        // =====================================================================
        // Hilfen
        // =====================================================================

        private static string Schluessel(Kampagne akademie) =>
            string.IsNullOrWhiteSpace(akademie.ArchivSchluessel) ? ArchivRegeln.StandardSchluessel : akademie.ArchivSchluessel;

        /// <summary>
        /// Im Cookie steht nur ein Abdruck des Schlüsselworts. Ändert die Spielleitung das Wort,
        /// ist die Tür für alle anderen wieder zu.
        /// </summary>
        private static string Abdruck(Kampagne akademie) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(akademie.Id + "|" + ArchivRegeln.Normalisieren(Schluessel(akademie)))))[..24];

        private bool TuerOffen(Kampagne akademie) => Request.Cookies[TuerCookie] == Abdruck(akademie);

        private void TuerAufschliessen(Kampagne akademie)
        {
            // Sitzungs-Cookie: zu, sobald der Browser geschlossen wird
            Response.Cookies.Append(TuerCookie, Abdruck(akademie), new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Secure = Request.IsHttps,
                Path = TuerPfad,
            });
        }

        private Task<ArchivDokument?> DokumentLadenAsync(int kampagneId, int id) => _context.ArchivDokumente
            .Include(d => d.Kategorien)
            .FirstOrDefaultAsync(d => d.Id == id && d.KampagneId == kampagneId);

        /// <summary>
        /// Beim ersten Öffnen: Kisten und Platzhalter-Dokumente aus der HTML, dazu die Kiste "Vor dem Umbruch",
        /// in der alle Startdokumente auch liegen (JS: arData). Wie beim Kalender nur, solange das Archiv ganz leer ist.
        /// </summary>
        private async Task StartinhalteAnlegenAsync(Kampagne akademie)
        {
            bool leer = !await _context.ArchivKategorien.AnyAsync(k => k.KampagneId == akademie.Id)
                        && !await _context.ArchivDokumente.AnyAsync(d => d.KampagneId == akademie.Id);
            if (!leer) return;

            var kisten = new Dictionary<string, ArchivKategorie>
            {
                [ArchivRegeln.Umbruch] = new() { KampagneId = akademie.Id, Code = ArchivRegeln.Umbruch, Name = "Vor dem Umbruch", Signatur = "U-00", Position = 0 },
            };
            int pos = 1;
            foreach (var (code, name, sig) in ArchivRegeln.KistenStart)
            {
                kisten[code] = new ArchivKategorie { KampagneId = akademie.Id, Code = code, Name = name, Signatur = sig, Position = pos++ };
            }
            _context.ArchivKategorien.AddRange(kisten.Values);

            foreach (var s in ArchivRegeln.DokumenteStart)
            {
                var dokument = new ArchivDokument
                {
                    KampagneId = akademie.Id,
                    Art = s.Art,
                    Titel = s.Titel,
                    DatumText = LeerZuNull(s.Datum),
                    Signatur = s.Signatur,
                    Text = s.Text,
                    Unterschrift = LeerZuNull(s.Unterschrift),
                    Versiegelt = s.Versiegelt,
                };
                foreach (var code in new[] { ArchivRegeln.Umbruch }.Concat(s.Kisten))
                {
                    dokument.Kategorien.Add(new ArchivDokumentKategorie { ArchivKategorie = kisten[code] });
                }
                _context.ArchivDokumente.Add(dokument);
            }
            await _context.SaveChangesAsync();
        }

        private static string Kuerzen(string? text, int max)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Length > max ? text[..max] : text;
        }

        private static string? LeerZuNull(string? text) => string.IsNullOrWhiteSpace(text) ? null : text;
    }
}
