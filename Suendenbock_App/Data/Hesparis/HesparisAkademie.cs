using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Models.Domain.Hesparis;

namespace Suendenbock_App.Data.Hesparis;

/// <summary>
/// Hesparis hat genau eine Akademie (technisch: eine Kampagne). Sie wird beim ersten Aufruf
/// automatisch angelegt und hängt an der Spielwelt "Hesparis" aus dem Tenant-System.
/// Spieler, deren Characters dieser Welt zugeordnet sind, spielen dort automatisch mit.
/// </summary>
public static class HesparisAkademie
{
    public const string WeltName = "Hesparis";

    /// <summary>Liefert die Akademie und legt sie (und notfalls die Spielwelt) beim ersten Aufruf an.</summary>
    public static async Task<Kampagne> HolenOderAnlegenAsync(ApplicationDbContext context)
    {
        var akademie = await context.Kampagnen
            .Include(k => k.GameWorld)
            .OrderBy(k => k.Id)
            .FirstOrDefaultAsync();
        if (akademie != null) return akademie;

        var welt = await context.GameWorlds.FirstOrDefaultAsync(w => w.Name.Contains(WeltName));
        if (welt == null)
        {
            welt = new GameWorld
            {
                Name = WeltName,
                Description = "Akademie Hesparis",
                IsActive = true,
                SortOrder = 3,
            };
            context.GameWorlds.Add(welt);
        }

        var heute = DateOnly.FromDateTime(DateTime.Today);
        akademie = new Kampagne
        {
            Name = "Akademie Hesparis",
            GameWorld = welt,
            Spieltag = heute,
            // Schuljahr beginnt im August
            Kalenderjahr = heute.Month >= 8 ? heute.Year : heute.Year - 1,
            Schuljahr = 1,
        };
        context.Kampagnen.Add(akademie);
        await context.SaveChangesAsync();
        return akademie;
    }
}
