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
    /// Gruppe "Alltag" (/Hesparis/Alltag/…): Stundenplan und Kalender, später Essensplan, AGs und Nachsitzen.
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
    }
}
