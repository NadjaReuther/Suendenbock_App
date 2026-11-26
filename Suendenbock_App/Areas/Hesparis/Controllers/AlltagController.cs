using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.ViewModels.Hesparis;

namespace Suendenbock_App.Areas.Hesparis.Controllers
{
    /// <summary>
    /// Gruppe "Alltag" (/Hesparis/Alltag/…): Stundenplan, später Kalender, Essensplan, AGs und Nachsitzen.
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
    }
}
