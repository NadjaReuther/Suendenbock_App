using Suendenbock_App.Models.Domain.Hesparis;

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

// ------------------------------------------------------------------ Essensplan

/// <summary>Speisesaal mit drei Kesseln und Speiseplan der Spielleitung (JS: MODS.essen).</summary>
public class EssensplanViewModel
{
    /// <summary>Wochen ab heute: -1 = letzte Woche, 0 = diese, 1 = nächste …</summary>
    public int Woche { get; set; }
    public DateOnly Montag { get; set; }
    public DateOnly Heute { get; set; }

    /// <summary>Gewählter Tag, 0 = Montag.</summary>
    public int Tag { get; set; }
    public DateOnly TagDatum => Montag.AddDays(Tag);
    public bool TagIstHeute => TagDatum == Heute;

    public List<EssenKessel> Kessel { get; set; } = new();
    public List<EssenWirkungZeile> Wirkungen { get; set; } = new();

    // Spielleitung
    public bool IstSpielleitung { get; set; }
    public bool Verwalten { get; set; }

    /// <summary>[Wochentag][Mahlzeit]</summary>
    public Gericht?[][] Plan { get; set; } = Array.Empty<Gericht?[]>();
    public bool HalberPreis { get; set; }

    /// <summary>Angetipptes Feld im Speiseplan (Wochentag, Mahlzeit) oder null.</summary>
    public (int Tag, int Mahlzeit)? Feld { get; set; }
    public string Art { get; set; } = "alle";
    public List<Gericht> Auswahl { get; set; } = new();
}

public class EssenKessel
{
    public int Mahlzeit { get; set; }
    public string Name { get; set; } = "";
    public Gericht Gericht { get; set; } = null!;
    public string Farbe { get; set; } = "";
    public int Preis { get; set; }
    public List<EssenGast> Gaeste { get; set; } = new();
}

/// <summary>Figurenknopf unter dem Kessel (JS: es-who).</summary>
public class EssenGast
{
    public int FigurId { get; set; }
    public string Name { get; set; } = "";
    public string Farbe { get; set; } = "";
    public int Geld { get; set; }
    public bool Gegessen { get; set; }
    public bool ZuWenigGeld { get; set; }

    /// <summary>Spielleitung darf für alle kaufen, Spielerinnen nur für die eigene Figur.</summary>
    public bool Darf { get; set; }
}

public class EssenWirkungZeile
{
    public string Name { get; set; } = "";
    public string Farbe { get; set; } = "";
    public string Text { get; set; } = "";
}
