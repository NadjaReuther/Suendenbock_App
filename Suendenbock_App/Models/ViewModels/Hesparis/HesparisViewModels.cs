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
    public string Name { get; set; } = "";
    public string? Bild { get; set; }
    public bool Eigene { get; set; }
}

// ------------------------------------------------------------------ Übersicht (Türme)

/// <summary>Gruppenbildschirm nach dem Startbild: ein Turm je Spielfigur, darunter die Detailleiste.</summary>
public class UebersichtViewModel
{
    public FigurKopfViewModel Kopf { get; set; } = null!;
    public List<Turm> Tuerme { get; set; } = new();
    public Turm Auswahl => Tuerme.First(t => t.Id == Kopf.AktiveFigurId);

    /// <summary>true, wenn eine Figur angeklickt wurde: dann steigen die Türme nicht erneut auf.</summary>
    public bool Ruhig { get; set; }
}

public class Turm
{
    public int Id { get; set; }
    public string Vorname { get; set; } = "";
    public string VollerName { get; set; } = "";
    public bool Eigene { get; set; }

    /// <summary>Element aus dem Sonderzauber, z. B. "Fauna/Ichtyo".</summary>
    public string Element { get; set; } = "";
    public string? Rolle { get; set; }
    public HesparisSigille.Aussehen Aussehen { get; set; } = HesparisSigille.Standard;

    public int LpAktuell { get; set; }
    public int LpMax { get; set; }

    public double? Notenschnitt { get; set; }
    public int BenoteteFaecher { get; set; }
    public string? Lieblingsfach { get; set; }
    public string? AgName { get; set; }
    public string? AgBonus { get; set; }
    public string? Sonderzauber { get; set; }

    /// <summary>Fach mit der höchsten Magiestufe (für den Link "Magiestufen").</summary>
    public int? BestesFachId { get; set; }
    public string? BestesFach { get; set; }
    public int BesteStufe { get; set; }

    /// <summary>Segmente der Lebensbalken (JS: ptLpBar): gefüllte Anzahl und Zustand "", "mid", "low".</summary>
    public int LpGefuellt(int segmente)
    {
        if (LpMax <= 0)
        {
            return 0;
        }
        return (int)Math.Ceiling((double)LpAktuell / LpMax * segmente - 0.0001);
    }

    public string LpZustand
    {
        get
        {
            double anteil = 0;
            if (LpMax > 0)
            {
                anteil = (double)LpAktuell / LpMax;
            }
            if (anteil <= 0.25)
            {
                return "low";
            }
            if (anteil <= 0.5)
            {
                return "mid";
            }
            return "";
        }
    }
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

// ------------------------------------------------------------------ Charaktere (Gott-Modus)

public class CharakterIndexViewModel
{
    public List<Charakterart> Arten { get; set; } = new();
    public List<HesparisCharakter> Charaktere { get; set; } = new();
}

/// <summary>Formularfelder eines Hesparis-Charakters.</summary>
public class CharakterFormular
{
    public int? Id { get; set; }
    public string Vorname { get; set; } = "";
    public string? Nachname { get; set; }
    public string? Rufname { get; set; }
    public string? BildPfad { get; set; }
    public int CharakterartId { get; set; }

    /// <summary>Spieler (nur bei Art Spieler).</summary>
    public string? UserId { get; set; }

    /// <summary>Verknüpfter Sündenbock-Character (dieselbe Person in beiden Welten).</summary>
    public int? CharacterId { get; set; }
}

public class CharakterFormularViewModel
{
    public CharakterFormular Formular { get; set; } = new();
    public List<Charakterart> Arten { get; set; } = new();
    public List<ApplicationUser> Spieler { get; set; } = new();
    public List<Character> SuendenbockCharacters { get; set; } = new();
}

// ------------------------------------------------------------------ Magiestufen

public class StufenViewModel
{
    public FigurKopfViewModel Kopf { get; set; } = null!;

    /// <summary>Abschnitt 1–4 (Unterstufe … Domstufe) und höchste wählbare Stufe darin.</summary>
    public int Abschnitt { get; set; }
    public string AbschnittName { get; set; } = "";
    public int MaxStufe { get; set; }

    public int PunkteGesamt { get; set; }
    public int PunkteVerbraucht { get; set; }
    public int PunkteFrei => PunkteGesamt - PunkteVerbraucht;
    public int Bonuspunkte { get; set; }
    public int AbschlussPunkte { get; set; }

    public List<StufenFach> Faecher { get; set; } = new();
    public StufenFach Auswahl { get; set; } = null!;
    public string? Beschreibung { get; set; }
    public List<StufenSchritt> Schritte { get; set; } = new();
    public List<StufenPerk> Perks { get; set; } = new();

    /// <summary>Wahlmöglichkeiten für die nächste Stufe (mehrere bei "… ODER …").</summary>
    public List<string> Optionen { get; set; } = new();
    public bool KannWaehlen { get; set; }
    public string? Grund { get; set; }

    /// <summary>Abschlüsse (hafen, anger, garten, dom) → Note, für Gott.</summary>
    public Dictionary<string, string?> Abschluesse { get; set; } = new();

    // Ring
    public string Polygon { get; set; } = "";
    public List<RingPunkt> Punkte { get; set; } = new();
    public List<RingBeschriftung> Beschriftungen { get; set; } = new();
    public List<RingLinie> Speichen { get; set; } = new();
    public List<RingLinie> Pfade { get; set; } = new();
    public List<RingTor> Tore { get; set; } = new();
}

/// <summary>Linie im SVG des Rings (Koordinaten im 720er Raster).</summary>
public class RingLinie
{
    public string X1 { get; set; } = "";
    public string Y1 { get; set; } = "";
    public string X2 { get; set; } = "";
    public string Y2 { get; set; } = "";
}

/// <summary>Beschriftung eines Tors zwischen zwei Abschnitten (Hafenaufsicht, Angerwacht, Gartenhüter).</summary>
public class RingTor
{
    public string Name { get; set; } = "";
    public string Links { get; set; } = "";
    public string Oben { get; set; } = "";
    public bool Offen { get; set; }
}

public class StufenFach
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Stufe { get; set; }
    public int Fachpunkte { get; set; }
    public int FachpunkteFrei => Math.Max(0, Fachpunkte - Stufe);
}

public class StufenSchritt
{
    public int Stufe { get; set; }
    public string Text { get; set; } = "";

    /// <summary>"got" = erreicht, "next" = jetzt wählbar, "" = noch verdeckt.</summary>
    public string Status { get; set; } = "";
}

public class StufenPerk
{
    public string Tor { get; set; } = "";
    public string Name { get; set; } = "";
    public bool Erhalten { get; set; }
    public string Hinweis { get; set; } = "";
    public string? Text { get; set; }
}

public class RingPunkt
{
    public int FachId { get; set; }
    public int Stufe { get; set; }
    public string Links { get; set; } = "";
    public string Oben { get; set; } = "";
    public string Klasse { get; set; } = "";
    public string Beschriftung { get; set; } = "";
}

public class RingBeschriftung
{
    public int FachId { get; set; }
    public string Name { get; set; } = "";
    public int Stufe { get; set; }
    public string Links { get; set; } = "";
    public string Oben { get; set; } = "";
    public bool Aktiv { get; set; }
}
