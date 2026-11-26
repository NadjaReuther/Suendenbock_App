using Suendenbock_App.Models.Domain.Hesparis;

namespace Suendenbock_App.Models.ViewModels.Hesparis;

/// <summary>
/// Akademie · Archiv: feste Inhalte aus der hesparis.html (AR_KINDS, AR_ICON, AR_BOXCOL, AR_START_KATS, AR_START_DOCS).
/// </summary>
public static class ArchivRegeln
{
    /// <summary>Schlüsselwort, solange die Spielleitung keins gesetzt hat (JS: arKey).</summary>
    public const string StandardSchluessel = "Sekretariat";

    /// <summary>Kiste für alles aus der Zeit vor dem Umbruch (JS: umb). Alle Startdokumente liegen auch darin.</summary>
    public const string Umbruch = "umb";

    /// <summary>Unsichtbare Kiste für Dokumente ohne Kategorie (JS: _ohne).</summary>
    public const string OhneKategorie = "_ohne";

    public static readonly string[] Arten = { "Bericht", "Protokoll", "Brief", "Telegramm", "Zeitung", "Akte", "Notiz" };

    private static readonly Dictionary<string, string> Symbole = new() { ["Bericht"] = "▤", ["Protokoll"] = "☰", ["Brief"] = "✉", ["Telegramm"] = "⌁", ["Zeitung"] = "❧", ["Akte"] = "⊘", ["Notiz"] = "✎" };

    public static string Symbol(string? art) => Symbole.GetValueOrDefault(art ?? "", "▤");

    private static readonly string[] KistenFarben = { "#6b4a2a", "#5a3e2a", "#71503a", "#4e3a2c", "#634634", "#3e2a3a", "#5c4430", "#4a3426" };

    /// <summary>Farbe der Kiste nach ihrer Reihenfolge im Regal; "Vor dem Umbruch" ist rot (JS: arBoxes).</summary>
    public static string KistenFarbe(string code, int index) => code switch
    {
        Umbruch => "#6a2a2a",
        OhneKategorie => "#3a3030",
        _ => KistenFarben[index % KistenFarben.Length],
    };

    /// <summary>Kisten beim ersten Öffnen: Code, Name, Signatur (JS: AR_START_KATS).</summary>
    public static readonly (string Code, string Name, string Signatur)[] KistenStart =
    {
        ("fab", "Fabrikaufsicht", "A-12"),
        ("kon", "Lehrerkonferenz", "B-03"),
        ("bri", "Briefe", "C-07"),
        ("tel", "Telegramme", "D-01"),
        ("zei", "Hesparis-Bote", "E-22"),
        ("sch", "Schulleitung", "S-00"),
    };

    public record Startdokument(string Art, string Titel, string Datum, string Signatur, string[] Kisten, string Text, string Unterschrift, bool Versiegelt);

    /// <summary>Dokumente beim ersten Öffnen (JS: AR_START_DOCS). Platzhalter, die Spielleitung ersetzt sie.</summary>
    public static readonly Startdokument[] DokumenteStart =
    {
        new("Bericht", "Zwischenfall in Halle 4", "Hesparis, 14. März · Jahrgang 61", "A-12 / 7", new[] { "fab" },
            "Um 22 Uhr 40 fiel in Halle 4 die Ätherleitung aus. Die Maschinen liefen ohne Antrieb weiter, etwa elf Minuten lang. Die Nachtschicht berichtet von einem Summen, das aus dem Boden kam. Drei Arbeiter wurden ins Krankenzimmer der Akademie gebracht. Ursache: ██████████. Weitere Untersuchung angeordnet.",
            "gez. Aufsicht Nord", false),
        new("Bericht", "Messprotokoll Leitung Nord", "Hesparis, 20. März · Jahrgang 61", "A-12 / 9", new[] { "fab" },
            "Die Messwerte an der Leitung Nord schwanken seit drei Wochen. Jeden Abend kurz vor elf ein Anstieg, dann ein Abfall auf null. Wir empfehlen, den Unterricht in den Laboren bis auf Weiteres vor 22 Uhr zu beenden.",
            "gez. Ing. ███████", false),
        new("Protokoll", "Konferenz vom 2. April", "Lehrerzimmer · Jahrgang 61", "B-03 / 41", new[] { "kon" },
            "Anwesend: alle Fachlehrkräfte außer Grenzchemie.\nPunkt 1: Die Sternenwache bleibt bis auf Weiteres ausgesetzt.\nPunkt 2: Schülerinnen und Schüler dürfen das Gelände nach Einbruch der Dunkelheit nicht verlassen.\nPunkt 3: Über die Vorfälle in der Stadt wird im Unterricht nicht gesprochen.",
            "Protokoll: Sekretariat", false),
        new("Brief", "Liebe Mama", "Schlafsaal Ost · im Frühjahr", "C-07 / 3", new[] { "bri" },
            "Liebe Mama, mach dir keine Sorgen. Hier ist alles ruhig, nur nachts flackern die Lampen. Die Großen sagen, das liegt an der Leitung. Ich habe Frau Hirschfelt gefragt, warum die Sterne über der Stadt so komisch aussehen. Sie hat nicht geantwortet. Ich komme zu Ostern heim, versprochen.",
            "deine Ida", false),
        new("Telegramm", "Eiltelegramm an die Schulleitung", "Telegrafenstube · 30. April · Jahrgang 61", "D-01 / 1", new[] { "tel", "sch" },
            "LEITUNG NORD UND SUED AUSGEFALLEN STOP STADT OHNE LICHT STOP SCHUELER AUF DEM GELAENDE HALTEN STOP KEINE KUTSCHEN SCHICKEN STOP WEITERE NACHRICHT FOLGT",
            "Absender unleserlich", false),
        new("Zeitung", "Der Hesparis-Bote, Ausgabe 112", "Erschienen im April · Jahrgang 61", "E-22 / 112", new[] { "zei" },
            "GROSSES FRÜHLINGSFEST ABGESAGT. Die Schulleitung bittet um Verständnis. Stattdessen findet am Samstag ein Leseabend in der Bibliothek statt. Außerdem: Die Glimrack-Mannschaft gewinnt gegen Kaltenbrück, und wer hat eigentlich den Kater des Hausmeisters gesehen?",
            "die Redaktion", false),
        new("Akte", "Akte S-00", "", "S-00 / 1", new[] { "sch" },
            "Hier steht, was die Schulleitung wirklich weiß. Die Spielleitung trägt es ein.",
            "", true),
    };

    /// <summary>Schlüsselwort vergleichen: Groß-/Kleinschreibung und doppelte Leerzeichen egal (JS: arKeyNorm).</summary>
    public static string Normalisieren(string? wort) =>
        System.Text.RegularExpressions.Regex.Replace((wort ?? "").Trim().ToLowerInvariant(), @"\s+", " ");

    /// <summary>Regalnummer in römisch (JS: ['I','II',…][s]).</summary>
    public static string Regal(int nr) => nr switch { 0 => "I", 1 => "II", 2 => "III", 3 => "IV", 4 => "V", 5 => "VI", _ => (nr + 1).ToString() };
}
