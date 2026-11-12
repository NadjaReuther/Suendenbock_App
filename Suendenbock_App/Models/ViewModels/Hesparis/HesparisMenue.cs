namespace Suendenbock_App.Models.ViewModels.Hesparis;

/// <summary>
/// Hauptmenü von Hesparis wie in der hesparis.html: fünf Gruppen mit Ausklappmenü.
/// Bereiche ohne Controller sind noch nicht gebaut und erscheinen ausgegraut mit "bald".
/// </summary>
public static class HesparisMenue
{
    /// <param name="Bereich">Wert von ViewData["Bereich"] der jeweiligen Seite (markiert den Eintrag als aktiv).</param>
    /// <param name="Controller">null = Seite gibt es noch nicht.</param>
    /// <param name="MitFigur">Link bekommt die gerade gewählte Figur (asp-route-id) mit.</param>
    /// <param name="NurGott">Nur für die Spielleitung sichtbar (in der HTML: sl-only).</param>
    public record Eintrag(string Bereich, string Text, string Glyph, string? Controller, string? Action,
                          bool MitFigur = true, bool NurGott = false);

    public record Gruppe(string Text, string Glyph, Eintrag[] Eintraege);

    public static readonly Gruppe[] Gruppen =
    {
        new("Figur", "✧", new Eintrag[]
        {
            new("Talente", "Talente", "✧", "Figur", "Talente"),
            new("Zeugnis", "Zeugnis", "✎", "Figur", "Zeugnis"),
            new("Tasche", "Tasche", "◍", "Figur", "Tasche"),
        }),
        new("Magie", "✦", new Eintrag[]
        {
            new("Stufen", "Magiestufen", "✦", "Magie", "Stufen"),
            new("Waffen", "Ätherwaffen", "⚔", null, null),
            new("Sonder", "Sonderzauber", "☾", "Magie", "Sonderzauber"),
            new("Brauen", "Trankküche", "⚗", null, null),
            new("Zirkel", "Zirkel der Priore", "✺", null, null),
        }),
        new("Alltag", "▦", new Eintrag[]
        {
            new("Stundenplan", "Stundenplan", "▦", null, null),
            new("Kalender", "Kalender", "☼", null, null),
            new("Essen", "Essensplan", "♨", null, null),
            new("Ags", "AGs", "⚘", null, null),
            new("Nachsitzen", "Nachsitzen", "⧗", null, null),
        }),
        new("Akademie", "✥", new Eintrag[]
        {
            new("Karte", "Karte", "✥", null, null),
            new("Klasse", "Klassenliste", "♡", null, null),
            new("Rat", "Schülerrat", "⚜", null, null),
            new("Lehrer", "Lehrkräfte", "✒", null, null),
            new("Laden", "Schulladen", "⚖", null, null),
            new("Kranken", "Krankenflügel", "✚", null, null),
            new("Archiv", "Archiv", "▤", null, null),
            new("Charakter", "Charaktere", "⚉", "Charakter", "Index", MitFigur: false, NurGott: true),
        }),
        new("Schuljahr", "❧", new Eintrag[]
        {
            new("Missionen", "Questbrett", "✠", null, null),
            new("Hefte", "Protokollhefte", "✎", null, null),
            new("Protokoll", "Tagebuch", "❧", null, null),
            new("Wertung", "Akademie-Wertung", "♛", null, null),
            new("Jahr", "Jahresabschluss", "⌛", null, null, NurGott: true),
        }),
    };

    private static readonly string[] Wochentage = { "So", "Mo", "Di", "Mi", "Do", "Fr", "Sa" };
    private static readonly string[] Monate =
    {
        "Januar", "Februar", "März", "April", "Mai", "Juni",
        "Juli", "August", "September", "Oktober", "November", "Dezember",
    };

    /// <summary>Spieltag für die Kopfleiste, z. B. "Do, 3. September" (wie in der HTML).</summary>
    public static string Spieltag(DateOnly tag) =>
        Wochentage[(int)tag.DayOfWeek] + ", " + tag.Day + ". " + Monate[tag.Month - 1];
}
