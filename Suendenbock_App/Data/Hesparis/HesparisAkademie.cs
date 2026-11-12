using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Models.Domain.Hesparis;

namespace Suendenbock_App.Data.Hesparis;

/// <summary>
/// Hesparis hat genau eine Akademie (technisch: eine Kampagne). Sie wird beim ersten Aufruf automatisch angelegt.
/// </summary>
public static class HesparisAkademie
{
    /// <summary>
    /// Die Spielfiguren der Akademie, wie in der hesparis.html (Klassenliste, Reihenfolge = Spielerposition).
    /// Bestimmt nur die Reihenfolge der Spielercharaktere (diese fünf zuerst).
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
    /// Position in der Liste oben, null wenn der Name nicht dazugehört.
    /// Verglichen wird der Vorname und, falls eingetragen, der Nachname (Groß-/Kleinschreibung egal).
    /// </summary>
    public static int? PositionVon(string vorname, string? nachname)
    {
        for (int i = 0; i < Spielfiguren.Length; i++)
        {
            string name = Spielfiguren[i];
            string listenVorname = name.Split(' ')[0];
            string listenNachname = name.Substring(listenVorname.Length).Trim();

            // Nur der erste Vorname zählt ("Svea Marie" passt zu "Svea")
            bool vornamePasst = string.Equals(HesparisCharakter.ErsterName(vorname), listenVorname, StringComparison.OrdinalIgnoreCase);
            bool nachnamePasst = string.IsNullOrWhiteSpace(nachname)
                                 || listenNachname.Contains(nachname.Trim(), StringComparison.OrdinalIgnoreCase);
            if (vornamePasst && nachnamePasst)
            {
                return i;
            }
        }
        return null;
    }

    /// <summary>Liefert die Akademie und legt sie beim ersten Aufruf an.</summary>
    public static async Task<Kampagne> HolenOderAnlegenAsync(ApplicationDbContext context)
    {
        var akademie = await context.Kampagnen.OrderBy(k => k.Id).FirstOrDefaultAsync();
        if (akademie != null) return akademie;

        var heute = DateOnly.FromDateTime(DateTime.Today);
        akademie = new Kampagne
        {
            Name = "Akademie Hesparis",
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
