using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Models.Domain.Hesparis;

namespace Suendenbock_App.Data.Hesparis;

/// <summary>
/// Hesparis hat genau eine Akademie (technisch: eine Kampagne). Sie wird beim ersten Aufruf
/// automatisch angelegt und hängt an der Spielwelt "Hesparis" aus dem Tenant-System.
/// Jeder Character mit Spieler, der dieser Welt zugeordnet ist, wird automatisch als Spielfigur eingeschrieben.
/// </summary>
public static class HesparisAkademie
{
    public const string WeltName = "Hesparis";

    /// <summary>
    /// Die Spielfiguren der Akademie, wie in der hesparis.html (Klassenliste, Reihenfolge = Spielerposition).
    /// Bestimmt nur die Reihenfolge (diese fünf zuerst); eingeschrieben wird jeder Character der Welt, der einen Spieler hat.
    /// </summary>
    public static readonly string[] Spielfiguren =
    {
        "Elma Röhrig-Kessler",
        "Levi Wehrmann",
        "Darius von Sonnenfeld",
        "Svea Riemstedt",
        "Rigel Stern",
    };

    /// <summary>
    /// Position der Figur in der Liste oben, null wenn der Character keine Hesparis-Spielfigur ist.
    /// Verglichen wird der Vorname und, falls eingetragen, der Nachname (Groß-/Kleinschreibung egal).
    /// </summary>
    public static int? PositionVon(Character character)
    {
        for (int i = 0; i < Spielfiguren.Length; i++)
        {
            string name = Spielfiguren[i];
            string vorname = name.Split(' ')[0];
            string nachname = name.Substring(vorname.Length).Trim();

            bool vornamePasst = string.Equals(character.Vorname?.Trim(), vorname, StringComparison.OrdinalIgnoreCase);
            bool nachnamePasst = string.IsNullOrWhiteSpace(character.Nachname)
                                 || nachname.Contains(character.Nachname.Trim(), StringComparison.OrdinalIgnoreCase);
            if (vornamePasst && nachnamePasst)
            {
                return i;
            }
        }
        return null;
    }

    /// <summary>
    /// Liefert die Akademie und legt sie (und notfalls die Spielwelt) beim ersten Aufruf an.
    /// Die Akademie hängt immer an der Spielwelt "Hesparis". Zeigt eine früher angelegte Kampagne
    /// auf eine andere Welt (z. B. "Sündenbock 1618"), wird sie auf Hesparis umgestellt.
    /// </summary>
    public static async Task<Kampagne> HolenOderAnlegenAsync(ApplicationDbContext context)
    {
        // 1. Die Spielwelt Hesparis: genauer Name zuerst, sonst eine Welt, die "Hesparis" im Namen trägt
        var welt = await context.GameWorlds.FirstOrDefaultAsync(w => w.Name == WeltName)
                   ?? await context.GameWorlds.FirstOrDefaultAsync(w => w.Name.Contains(WeltName));
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
            await context.SaveChangesAsync();
        }

        // 2. Die Akademie dieser Welt
        var akademie = await context.Kampagnen
            .Include(k => k.GameWorld)
            .Where(k => k.GameWorldId == welt.Id)
            .OrderBy(k => k.Id)
            .FirstOrDefaultAsync();
        if (akademie != null) return akademie;

        // 3. Ältere Kampagne an einer anderen Welt? Dann auf Hesparis umstellen statt eine zweite anzulegen
        akademie = await context.Kampagnen
            .Include(k => k.GameWorld)
            .OrderBy(k => k.Id)
            .FirstOrDefaultAsync();
        if (akademie != null)
        {
            akademie.GameWorldId = welt.Id;
            akademie.GameWorld = welt;
            await context.SaveChangesAsync();
            return akademie;
        }

        // 4. Noch gar keine: neu anlegen
        var heute = DateOnly.FromDateTime(DateTime.Today);
        akademie = new Kampagne
        {
            Name = "Akademie Hesparis",
            GameWorldId = welt.Id,
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
