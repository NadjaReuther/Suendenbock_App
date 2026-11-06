using Suendenbock_App.Models.Domain;
using Suendenbock_App.Models.Domain.Hesparis;

namespace Suendenbock_App.Models.ViewModels.Hesparis;

/// <summary>Kopfbereich jeder Figur-Seite: Kampagne, alle Figuren zum Umschalten, gewählte Figur.</summary>
public class FigurKopfViewModel
{
    public Kampagne Kampagne { get; set; } = null!;
    public List<FigurKurz> Figuren { get; set; } = new();
    public int AktiveFigurId { get; set; }

    /// <summary>Darf der Benutzer die gewählte Figur bearbeiten (eigene Figur oder Gott)?</summary>
    public bool DarfBearbeiten { get; set; }
    public bool IstSpielleitung { get; set; }

    /// <summary>Welche Seite gerade offen ist, für die Umschalt-Links (Talente, Zeugnis, Tasche).</summary>
    public string Aktion { get; set; } = "Index";

    public FigurKurz Aktiv => Figuren.First(f => f.Id == AktiveFigurId);
}

public class FigurKurz
{
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public string Name { get; set; } = "";
    public string? Bild { get; set; }
    public string? Farbe { get; set; }
    public bool Eigene { get; set; }

    /// <summary>Anzeigename aus dem Sündenbock-Character: Rufname, sonst Vorname.</summary>
    public static string NameVon(Character c) =>
        !string.IsNullOrWhiteSpace(c.Rufname) ? c.Rufname : c.Vorname;
}

// ------------------------------------------------------------------ Startseite

public class HesparisIndexViewModel
{
    public FigurKopfViewModel Kopf { get; set; } = null!;
    public List<FigurUebersicht> Karten { get; set; } = new();
}

public class FigurUebersicht
{
    public FigurKurz Figur { get; set; } = null!;
    public int LebenspunkteAktuell { get; set; }
    public int LebenspunkteMax { get; set; }
    public int Geld { get; set; }
    public double? Notenschnitt { get; set; }
    public Dictionary<string, int> Kopfwerte { get; set; } = new();
    public int Gegenstaende { get; set; }
    public int AktiveWirkungen { get; set; }
}

// ------------------------------------------------------------------ Talente

public class TalenteViewModel
{
    public FigurKopfViewModel Kopf { get; set; } = null!;
    public List<BegabungZeile> Begabungen { get; set; } = new();
    public int Verteilt { get; set; }
    public int Budget => HesparisRegeln.TalentBudget;
    public List<TalentBonus> Boni { get; set; } = new();
    public List<TalentBonus> Vorteile { get; set; } = new();

    /// <summary>Talent-Code → Name, für Auswahllisten der Boni.</summary>
    public Dictionary<string, string> TalentNamen { get; set; } = new();
    public bool MeisterschaftenFreigeschaltet { get; set; }
}

public class BegabungZeile
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Farbe { get; set; } = "";
    public int Summe { get; set; }
    public int Kopfwert { get; set; }
    public int Geistesblitze { get; set; }
    public List<TalentZeile> Talente { get; set; } = new();
}

public class TalentZeile
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public int Punkte { get; set; }
    public int Bonus { get; set; }
    public List<TalentBonus> Boni { get; set; } = new();

    /// <summary>Punkte + Kopfwert + Boni (JS: talCalc → val).</summary>
    public int Gesamt { get; set; }
    public int AufDemBogen => HesparisRegeln.WertAufDemBogen(Gesamt);
    public int MeisterschaftStufe => HesparisRegeln.MeisterschaftStufe(Gesamt);
    public List<MeisterschaftZeile> Meisterschaften { get; set; } = new();
    public int GewaehlteMeisterschaften => Meisterschaften.Count(m => m.Gewaehlt);

    /// <summary>Noch frei wählbare Meisterschaften (höchstens 3 je Talent).</summary>
    public int FreieMeisterschaften => Math.Max(0, Math.Min(MeisterschaftStufe, 3) - GewaehlteMeisterschaften);
}

public class MeisterschaftZeile
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Beschreibung { get; set; } = "";
    public bool Gewaehlt { get; set; }
}

// ------------------------------------------------------------------ Zeugnis

public class ZeugnisViewModel
{
    public FigurKopfViewModel Kopf { get; set; } = null!;
    public List<ZeugnisFach> Faecher { get; set; } = new();
    public double? Gesamtschnitt { get; set; }
    public int FachpunkteGesamt { get; set; }
    public int FachpunkteFrei { get; set; }
}

public class ZeugnisFach
{
    public int FachId { get; set; }
    public string Name { get; set; } = "";
    public List<Schulnote> Noten { get; set; } = new();
    public double? Schnitt { get; set; }
    public int Stufe { get; set; }
    public int Fachpunkte { get; set; }
    public int FachpunkteFrei => Math.Max(0, Fachpunkte - Stufe);
}

// ------------------------------------------------------------------ Tasche

public class TascheViewModel
{
    public FigurKopfViewModel Kopf { get; set; } = null!;
    public int Geld { get; set; }
    public List<(string Name, int Anzahl)> Muenzen { get; set; } = new();
    public List<Geldbuchung> Buchungen { get; set; } = new();

    /// <summary>Gegenstände gruppiert nach Art in der Reihenfolge von HesparisRegeln.Kategorien.</summary>
    public List<(string Kategorie, List<Gegenstand> Gegenstaende)> Gruppen { get; set; } = new();
    public List<(Wirkung Wirkung, string Restdauer)> Wirkungen { get; set; } = new();
    public Dictionary<string, string> TalentNamen { get; set; } = new();

    /// <summary>Gegenstand, der gerade im Formular bearbeitet wird (null = neuer Gegenstand).</summary>
    public GegenstandFormular Formular { get; set; } = new();
}

/// <summary>Formular für einen Gegenstand inklusive Wirkung.</summary>
public class GegenstandFormular
{
    public int? Id { get; set; }
    public string Name { get; set; } = "";
    public string Kategorie { get; set; } = "Sonstiges";
    public int Menge { get; set; } = 1;
    public string? Notiz { get; set; }
    public string? Quelle { get; set; }
    public int? Preis { get; set; }
    public EffektArt? EffektArt { get; set; }
    public string? EffektTalentCode { get; set; }
    public int? EffektWert { get; set; }
    public string? EffektVorteilAuf { get; set; }
    public EffektDauerArt? EffektDauerArt { get; set; }
    public int? EffektDauerAnzahl { get; set; }
}

/// <summary>Geldbuchung: Betrag in Münzen, Vorzeichen über Richtung.</summary>
public class GeldFormular
{
    public int Hiro { get; set; }
    public int Nimbus { get; set; }
    public int Lumen { get; set; }
    public int Kerze { get; set; }

    /// <summary>true = einnehmen, false = ausgeben.</summary>
    public bool Einnahme { get; set; }
    public string Grund { get; set; } = "";

    public int Kerzen => Hiro * 1000 + Nimbus * 100 + Lumen * 10 + Kerze;
}

// ------------------------------------------------------------------ Spielleitung

public class SpielleitungIndexViewModel
{
    public List<KampagneZeile> Kampagnen { get; set; } = new();
    public List<GameWorld> Welten { get; set; } = new();
    public KampagneFormular Neu { get; set; } = new();
}

public class KampagneZeile
{
    public Kampagne Kampagne { get; set; } = null!;
    public string Welt { get; set; } = "";
    public int Figuren { get; set; }
}

public class KampagneFormular
{
    public int? Id { get; set; }
    public string Name { get; set; } = "";
    public int GameWorldId { get; set; }
    public DateOnly Spieltag { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public int Schuljahr { get; set; } = 1;
}

public class SpielleitungFigurenViewModel
{
    public Kampagne Kampagne { get; set; } = null!;
    public List<GameWorld> Welten { get; set; } = new();
    public List<SpielleitungFigurZeile> Figuren { get; set; } = new();

    /// <summary>Characters, die noch nicht in der Kampagne sind.</summary>
    public List<CharacterAuswahl> Verfuegbar { get; set; } = new();
}

public class SpielleitungFigurZeile
{
    public Spielfigur Figur { get; set; } = null!;
    public string Name { get; set; } = "";
    public string? Spieler { get; set; }

    /// <summary>Gehört der Character zu den Spielfiguren aus der hesparis.html?</summary>
    public bool IstHesparisFigur { get; set; }
}

public class CharacterAuswahl
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public bool HatSpieler { get; set; }

    /// <summary>Character ist schon der Welt der Kampagne zugeordnet.</summary>
    public bool InWelt { get; set; }
}
