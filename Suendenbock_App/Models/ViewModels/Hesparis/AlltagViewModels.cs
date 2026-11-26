namespace Suendenbock_App.Models.ViewModels.Hesparis;

// ------------------------------------------------------------------ Stundenplan

/// <summary>Stundenplan: Fächer-Übersicht und Wochenraster (JS: renderStundenplan).</summary>
public class StundenplanViewModel
{
    public List<StundenplanFach> Faecher { get; set; } = new();

    /// <summary>Heute (0 = Montag) und laufende Zeile im Raster; −1 = keine.</summary>
    public int HeuteTag { get; set; } = -1;
    public int JetztZeile { get; set; } = -1;

    public StundenplanFach Fach(int id) => Faecher.First(f => f.Id == id);
}

public class StundenplanFach
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Kurzname { get; set; } = "";
    public string? Lehrkraft { get; set; }
    public string Farbe { get; set; } = "";
    public string Rgb { get; set; } = "";

    /// <summary>Räume, in denen das Fach stattfindet, z. B. "Raum 211 · Trainingsplatz".</summary>
    public string Orte { get; set; } = "";
}

// ------------------------------------------------------------------ Kalender

/// <summary>Schulkalender: Jahreszeiten-Panorama mit Laternen (JS: MODS.kalender).</summary>
public class KalenderViewModel
{
    public int Jahr { get; set; }
    public DateOnly Heute { get; set; }
    public List<KalenderZeile> Termine { get; set; } = new();
    public bool IstSpielleitung { get; set; }

    /// <summary>"Termine verwalten" aufgeklappt lassen (nach dem Anlegen oder Löschen).</summary>
    public bool Verwalten { get; set; }

    /// <summary>Daten für das gezeichnete Panorama (hesparis-kalender.js), als JSON.</summary>
    public string PanoramaJson { get; set; } = "{}";
}

public class KalenderZeile
{
    public int Id { get; set; }
    public DateOnly Datum { get; set; }
    public string Titel { get; set; } = "";
    public AlltagRegeln.TerminArt Art { get; set; } = null!;
}
