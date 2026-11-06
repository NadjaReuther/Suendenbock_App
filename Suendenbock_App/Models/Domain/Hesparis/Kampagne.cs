using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Models.Domain.Hesparis;

/// <summary>
/// Eine Spielrunde. Alle Spieldaten hängen an einer Kampagne, damit ihr später
/// auch mehrere Gruppen oder einen Testlauf parallel führen könnt.
/// Ersetzt den bisherigen „Spielstand“ als Ganzes.
/// </summary>
public class Kampagne
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    /// <summary>
    /// Spielwelt aus dem Sündenbock-Tenant-System, zu der diese Kampagne gehört.
    /// Darüber sieht ein Spieler nur die Hesparis-Kampagne seiner aktiven Welt.
    /// </summary>
    public int GameWorldId { get; set; }
    public GameWorld GameWorld { get; set; } = null!;

    /// <summary>Der aktuelle Spieltag im Kalender (JS: kal.heute).</summary>
    public DateOnly Spieltag { get; set; }

    /// <summary>Startjahr des laufenden Schuljahres im Kalender (JS: kal.year).</summary>
    public int Kalenderjahr { get; set; }

    /// <summary>Schuljahr der Gruppe, 1 = erstes Jahr (JS: sj.jahr).</summary>
    public int Schuljahr { get; set; } = 1;

    /// <summary>Freigeschaltete Phase der Magiestufen 1–3 (JS: stufenPhase).</summary>
    public int? StufenPhase { get; set; }

    /// <summary>Hafen-Abschnitt für die Spieler geöffnet (JS: hafenOpen).</summary>
    public bool HafenOffen { get; set; }

    /// <summary>Brauen nur mit Freigabe der Spielleitung (JS: brewLock).</summary>
    public bool BrauenGesperrt { get; set; }

    /// <summary>Notiz am Schulladen (JS: shopNote).</summary>
    public string? LadenNotiz { get; set; }

    /// <summary>Spaltenanzahl des Sitzplans (JS: klSitzCols).</summary>
    public int SitzplanSpalten { get; set; } = 5;

    // ---- Schwarzmarkt ----
    /// <summary>Klopfcode als Folge der Münzkürzel, z. B. "kklk" (JS: schwarz.code).</summary>
    public string SchwarzmarktCode { get; set; } = "kklk";
    public string? SchwarzmarktRaetsel { get; set; }

    // ---- Glimrack ----
    /// <summary>Nummer des laufenden Cups (JS: glim.saison.nr). Ungerade = Herbst, gerade = Frühjahr.</summary>
    public int GlimSaisonNr { get; set; } = 1;

    /// <summary>Name des Kapitäns (JS: glim.kapitaen). Name, weil auch Nachwuchs/Spielfiguren Kapitän werden können.</summary>
    public string? GlimKapitaen { get; set; }

    /// <summary>
    /// Momentaufnahme eines gerade laufenden Glimrack-Spiels (JS: glim.game).
    /// Das ist der interne Zustand der Spiel-Engine (Würfe, Schritte, Undo-Historie)
    /// und wird nur vom Frontend gelesen/geschrieben, deshalb JSON.
    /// Nach Spielende landet das Ergebnis normalisiert in <see cref="GlimSpiel"/>.
    /// </summary>
    public string? GlimLaufendesSpielJson { get; set; }

    /// <summary>Laufender Ablauf zwischen den Spielen (Sichtung, Kapitänswahl …; JS: glim.flow). Engine-Zustand, JSON.</summary>
    public string? GlimAblaufJson { get; set; }

    /// <summary>Optimistische Sperre: verhindert, dass zwei Leute sich gegenseitig überschreiben.</summary>
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<Spielfigur> Spielfiguren { get; set; } = new List<Spielfigur>();
    public ICollection<Mitschueler> Mitschueler { get; set; } = new List<Mitschueler>();
}
