using Suendenbock_App.Models.Domain.Hesparis;

namespace Suendenbock_App.Models.ViewModels.Hesparis;

// =====================================================================
// Akademie · Archiv (JS: MODS.archiv)
// =====================================================================

/// <summary>Eine Kiste im Regal (JS: arBoxes).</summary>
public class ArchivKiste
{
    public int? Id { get; set; }

    /// <summary>Code für die Adresse (?kiste=fab), "_ohne" für Dokumente ohne Kategorie.</summary>
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Signatur { get; set; } = "";
    public string Farbe { get; set; } = "";
    public int Anzahl { get; set; }
}

public class ArchivViewModel
{
    /// <summary>Tür zu: Schlüsselwort-Seite (JS: arDoorHtml). Bei Gott mit den Einstellungen.</summary>
    public bool TuerSeite { get; set; }

    /// <summary>Zettel am Türrahmen.</summary>
    public string? Hinweis { get; set; }

    /// <summary>Nur für Gott: aktuelles Schlüsselwort.</summary>
    public string? Schluessel { get; set; }

    public List<ArchivKiste> Kisten { get; set; } = new();
    public ArchivKiste? Kiste { get; set; }

    /// <summary>Dokumente in der gewählten Kiste, für Spieler ohne die versteckten.</summary>
    public List<ArchivDokument> Dokumente { get; set; } = new();
    public ArchivDokument? Dokument { get; set; }

    /// <summary>Gott bearbeitet das Dokument auf dem Lesepult.</summary>
    public bool Bearbeiten { get; set; }

    /// <summary>Alle Kategorien mit Anzahl Dokumente, für die Verwaltung (Gott).</summary>
    public List<(ArchivKategorie Kategorie, int Anzahl)> Kategorien { get; set; } = new();

    /// <summary>Verwaltung der Kategorien aufgeklappt lassen (JS: AR.kats).</summary>
    public bool KategorienOffen { get; set; }

    public bool IstSpielleitung { get; set; }
}
