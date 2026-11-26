using Suendenbock_App.Models.Domain.Hesparis;

namespace Suendenbock_App.Models.ViewModels.Hesparis;

/// <summary>
/// Gruppe "Alltag" aus der hesparis.html: Stundenplan, Kalender, Essensplan, AGs, Nachsitzen.
/// Feste Spielinhalte (Stundenplan, Zeiten, Farben) stehen hier im Code, wie im JavaScript als Konstanten.
/// </summary>
public static class AlltagRegeln
{
    // ------------------------------------------------------------------ Stundenplan

    public static readonly string[] Tage = { "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag" };

    /// <summary>Eine Zeile im Wochenraster: Stunde (Nr) oder Pause (Pause = Name).</summary>
    public record Zeile(int? Nr, string? Pause, string Von, string Bis);

    /// <summary>Stunden und Pausen (JS: SP_ZEILEN). Die 4. und 7. Stunde sind die Pausen.</summary>
    public static readonly Zeile[] Zeilen =
    {
        new(1, null, "8:00", "8:45"),
        new(2, null, "8:50", "9:35"),
        new(3, null, "9:40", "10:25"),
        new(null, "Frühstückspause", "10:25", "10:45"),
        new(5, null, "10:45", "11:30"),
        new(6, null, "11:35", "12:20"),
        new(null, "Mittagspause", "12:20", "13:10"),
        new(8, null, "13:10", "13:55"),
    };

    /// <summary>Unterrichtsstunden in der Reihenfolge der Spalten im Plan (JS: SP_STUNDEN).</summary>
    public static readonly int[] Stunden = { 1, 2, 3, 5, 6, 8 };

    /// <summary>
    /// Die Schulwoche (JS: STUNDENPLAN): je Tag sechs Stunden als (Fach-Id, Raum). Raum "T" = Trainingsplatz.
    /// </summary>
    public static readonly (int Fach, string Raum)[][] Plan =
    {
        new[] { (0, "029"), (1, "327"), (2, "341"), (6, "101"), (4, "211"), (5, "T") },
        new[] { (3, "211"), (7, "235"), (8, "250"), (9, "001"), (6, "T"), (1, "327") },
        new[] { (4, "211"), (3, "211"), (7, "235"), (2, "341"), (0, "029"), (5, "T") },
        new[] { (8, "250"), (4, "211"), (1, "327"), (0, "029"), (9, "001"), (5, "T") },
        new[] { (8, "250"), (3, "211"), (9, "001"), (2, "341"), (7, "235"), (6, "T") },
    };

    /// <summary>Farbe je Fach-Id 0–9 (JS: SP_FARBEN), als Hex und als RGB-Zahlen.</summary>
    public static readonly (string Farbe, string Rgb)[] FachFarben =
    {
        ("#d27aa8", "210,122,168"), ("#8b93ff", "139,147,255"), ("#5fc8a0", "95,200,160"), ("#4fa3e8", "79,163,232"), ("#e7c66a", "231,198,106"),
        ("#f08f5a", "240,143,90"), ("#e5606a", "229,96,106"), ("#a9d36a", "169,211,106"), ("#49d9c9", "73,217,201"), ("#c9a0ff", "201,160,255"),
    };

    /// <summary>
    /// Lehrkräfte je Fach aus der HTML (JS: LEHRER). Gilt nur, solange in der Datenbank
    /// noch keine Lehrkraft mit Fächern eingetragen ist.
    /// </summary>
    public static readonly Dictionary<string, string> LehrerStandard = new()
    {
        ["Pokusweben"] = "Hr. Zielinski",
        ["Resonanzkunde"] = "Hr. Haas",
        ["Elementarharmonie"] = "Fr. Boeckensteedt",
        ["Astronomische Orientierung"] = "Fr. Hirschfelt",
        ["Kartopädie"] = "Fr. Hirschfelt",
        ["Chronotaktik"] = "Hr. von Graffenried",
        ["Ätherfechten"] = "Hr. Rasmussen-Kukk",
        ["Liminaltheorie"] = "Hr. Zielinski",
        ["Grenzchemie"] = "Fr. Krautler",
        ["Magiatur"] = "Hr. Sönneberg",
    };

    /// <summary>"Raum 211" bzw. "Trainingsplatz" (JS: spRaumText).</summary>
    public static string RaumText(string raum)
    {
        if (raum == "T")
        {
            return "Trainingsplatz";
        }
        return "Raum " + raum;
    }

    /// <summary>"8:50" → 530 Minuten seit Mitternacht.</summary>
    public static int Minuten(string zeit)
    {
        var teile = zeit.Split(':');
        return int.Parse(teile[0]) * 60 + int.Parse(teile[1]);
    }

    /// <summary>
    /// Heutiger Wochentag (0 = Montag … 4 = Freitag, −1 am Wochenende) und laufende Zeile im Raster (−1 = keine),
    /// nach deutscher Zeit (JS: spNow, dort nach der Uhr des Geräts).
    /// </summary>
    public static (int Tag, int Zeile) Jetzt()
    {
        var zone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
        var jetzt = TimeZoneInfo.ConvertTime(DateTime.UtcNow, zone);

        int tag = -1;
        if (jetzt.DayOfWeek >= DayOfWeek.Monday && jetzt.DayOfWeek <= DayOfWeek.Friday)
        {
            tag = (int)jetzt.DayOfWeek - 1;
        }
        if (tag < 0)
        {
            return (-1, -1);
        }

        int minuten = jetzt.Hour * 60 + jetzt.Minute;
        int zeile = Array.FindIndex(Zeilen, z => minuten >= Minuten(z.Von) && minuten < Minuten(z.Bis));
        return (tag, zeile);
    }

    // ------------------------------------------------------------------ Kalender

    public record TerminArt(Termintyp Typ, string Code, string Name, string Farbe);

    /// <summary>Arten von Terminen mit Farbe der Laterne (JS: KAL_TYPES).</summary>
    public static readonly TerminArt[] TerminArten =
    {
        new(Termintyp.Rat, "rat", "Schülerrat", "#f4e8c0"),
        new(Termintyp.Pruefung, "pruefung", "Prüfung", "#e5606a"),
        new(Termintyp.Glimrack, "glimrack", "Glimrack", "#49d9c9"),
        new(Termintyp.Wettbewerb, "wettbewerb", "Wettbewerb", "#d9a24a"),
        new(Termintyp.Feiertag, "feiertag", "Feiertag", "#f08fbf"),
        new(Termintyp.Ferien, "ferien", "Ferien", "#8fd67a"),
        new(Termintyp.Schule, "schule", "Schule", "#ad8ff7"),
        new(Termintyp.Sonstiges, "sonstiges", "Sonstiges", "#a89568"),
    };

    public static TerminArt Art(Termintyp typ) => TerminArten.FirstOrDefault(a => a.Typ == typ) ?? TerminArten[^1];

    /// <summary>
    /// Termine, mit denen der Kalender beim ersten Öffnen gefüllt wird (JS: kalData).
    /// Monat ab September gehört zum Startjahr, Januar bis August zum Folgejahr.
    /// </summary>
    public static readonly (int Monat, int Tag, string Titel, Termintyp Typ)[] TermineStandard =
    {
        (9, 1, "Erster Schultag", Termintyp.Schule),
        (10, 5, "Herbst-Cup: Auftakt", Termintyp.Glimrack),
        (11, 12, "Hiro-Tag: Fest des heiligen Hiro", Termintyp.Feiertag),
        (11, 21, "Braumeisterschaft", Termintyp.Wettbewerb),
        (12, 19, "Beginn der Winterferien", Termintyp.Ferien),
        (1, 7, "Unterricht beginnt wieder", Termintyp.Schule),
        (1, 23, "Duellturnier", Termintyp.Wettbewerb),
        (3, 20, "Sternenrallye", Termintyp.Wettbewerb),
        (4, 10, "Frühjahrs-Cup: Auftakt", Termintyp.Glimrack),
        (5, 8, "Rededuell", Termintyp.Wettbewerb),
        (6, 22, "Abschlussprüfungen", Termintyp.Pruefung),
        (7, 10, "Letzter Schultag", Termintyp.Schule),
    };

    public static readonly string[] Monate =
    {
        "Januar", "Februar", "März", "April", "Mai", "Juni",
        "Juli", "August", "September", "Oktober", "November", "Dezember",
    };

    /// <summary>"3. September" (JS: kalFmt).</summary>
    public static string Datum(DateOnly tag) => tag.Day + ". " + Monate[tag.Month - 1];

    /// <summary>Startjahr des Schuljahres: ab August das laufende Jahr, sonst das Vorjahr.</summary>
    public static int Schuljahresbeginn(DateOnly tag)
    {
        if (tag.Month >= 8)
        {
            return tag.Year;
        }
        return tag.Year - 1;
    }
}
