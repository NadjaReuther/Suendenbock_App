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

// ------------------------------------------------------------------ Sonderzauber

/// <summary>Kartentisch "Vier Karten, eine Wahl": je Figur vier verdeckte Zauber, zwei aufdecken, einen wählen.</summary>
public class SonderzauberViewModel
{
    public List<SonderzauberFigur> Figuren { get; set; } = new();
    public SonderzauberFigur Auswahl { get; set; } = null!;
    public List<SonderzauberKarte> Karten { get; set; } = new();

    public int Aufgedeckt { get; set; }
    public bool Gewaehlt { get; set; }

    public bool DarfBearbeiten { get; set; }
    public bool IstSpielleitung { get; set; }

    /// <summary>Zustand der drei Schritte oben rechts: "done", "active" oder "".</summary>
    public string Schritt1 { get; set; } = "";
    public string Schritt2 { get; set; } = "";
    public string Schritt3 { get; set; } = "";
}

/// <summary>Knopf in der Figurenleiste (JS: sz-char).</summary>
public class SonderzauberFigur
{
    public string Code { get; set; } = "";
    public string Person { get; set; } = "";
    public string Titel { get; set; } = "";
    public string Element { get; set; } = "";
    public HesparisSigille.Aussehen Aussehen { get; set; } = HesparisSigille.Standard;

    /// <summary>Spielfigur dieser Person (für die Navigation), falls es sie gibt.</summary>
    public int? FigurId { get; set; }

    /// <summary>"open", "progress" oder "done" und der Text darunter (JS: sonderStatus).</summary>
    public string StatusKlasse { get; set; } = "open";
    public string StatusText { get; set; } = "";
}

/// <summary>Eine der vier Karten (JS: sonderCardHtml).</summary>
public class SonderzauberKarte
{
    public int Position { get; set; }
    public string Roemisch { get; set; } = "";
    public SonderzauberOption Option { get; set; } = null!;

    /// <summary>CSS-Klassen der Karte, z. B. "sz-card is-revealed is-chosen".</summary>
    public string Klasse { get; set; } = "";
    public bool IstAufgedeckt { get; set; }
    public bool IstGewaehlt { get; set; }
    public bool KannAufdecken { get; set; }

    /// <summary>Wahl getroffen: verdeckte Karten bekommen ein Schloss.</summary>
    public bool Versiegelt { get; set; }
    public string Hinweis { get; set; } = "";
}

// ------------------------------------------------------------------ Trankküche

/// <summary>Trankküche: Rezeptbuch, Zielwert auf Metallurgie, Brauergebnis am Tisch (JS: MODS.brauen).</summary>
public class TrankkuecheViewModel
{
    public FigurKopfViewModel Kopf { get; set; } = null!;
    public List<FigurChip> Figuren { get; set; } = new();

    /// <summary>Die Spielleitung hat die Trankküche für Spieler gesperrt (JS: brewLock).</summary>
    public bool Gesperrt { get; set; }

    public List<RezeptZeile> Rezepte { get; set; } = new();
    public RezeptZeile? Auswahl { get; set; }

    // Zielwert (JS: brewTarget)
    public int Metallurgie { get; set; }
    public bool SaubererSud { get; set; }
    public int Zielwert { get; set; }

    // Vorteile aus Meisterschaften und Magiestufen
    public bool DoppelterKessel { get; set; }
    public bool Meisterbrau { get; set; }
    public int Schluck { get; set; } = 1;

    public List<BrauZeile> Braubuch { get; set; } = new();
}

/// <summary>Knopf in der Figurenleiste mit der Farbe der Figur (JS: mdPlayers).</summary>
public class FigurChip
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public HesparisSigille.Aussehen Aussehen { get; set; } = HesparisSigille.Standard;
    public bool Aktiv { get; set; }
}

public class RezeptZeile
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Wirkung { get; set; }
    public int Modifikator { get; set; }
    public List<ZutatZeile> Zutaten { get; set; } = new();
    public bool ZutatenDa => Zutaten.All(z => z.Vorhanden >= z.Menge);

    /// <summary>"leicht" bei 0, sonst z. B. "-10".</summary>
    public string Schwierigkeit
    {
        get
        {
            if (Modifikator == 0)
            {
                return "leicht";
            }
            if (Modifikator > 0)
            {
                return "+" + Modifikator;
            }
            return Modifikator.ToString();
        }
    }
}

public class ZutatZeile
{
    public string Name { get; set; } = "";
    public int Menge { get; set; }

    /// <summary>So viele liegen in der Tasche der gewählten Figur.</summary>
    public int Vorhanden { get; set; }
}

public class BrauZeile
{
    public string Wer { get; set; } = "";
    /// <summary>ok, krit oder fail.</summary>
    public string Ergebnis { get; set; } = "";
    public string Text { get; set; } = "";
}

// ------------------------------------------------------------------ Zirkel der Priore

/// <summary>Zirkel der Priore: Brett, Rückseite zum Raten, Notizen, Mordfall (JS: MODS.zirkel).</summary>
public class ZirkelViewModel
{
    /// <summary>Gewählte Magie 1–12.</summary>
    public int Nr { get; set; }
    public ZirkelRegeln.Magie Magie => ZirkelRegeln.MagieVon(Nr);

    public bool IstSpielleitung { get; set; }
    public bool DarfRaten { get; set; }

    /// <summary>Brett beim Laden schon gewendet (nach einem Rateversuch).</summary>
    public bool Wenden { get; set; }

    public ZirkelSpalte Prior { get; set; } = null!;
    public ZirkelSpalte Sekundant { get; set; } = null!;

    /// <summary>Daten für das gezeichnete Brett (zirkel.js), als JSON.</summary>
    public string BrettJson { get; set; } = "{}";

    // Spielleitung: Geheimes zu Prior und Sekundant der gewählten Magie
    public ZirkelPerson PriorPerson { get; set; } = null!;
    public ZirkelPerson SekundantPerson { get; set; } = null!;

    /// <summary>Alle 24 Personen als Auswahl ("7|p" → "Prior des Lichts · Aldric Morn").</summary>
    public List<(string Wert, string Text)> Personen { get; set; } = new();

    public List<ZirkelNotizZeile> Notizen { get; set; } = new();
    public List<FigurChip> Figuren { get; set; } = new();

    /// <summary>null = Spieler sehen den Fall noch nicht (kein Toter hinterlegt).</summary>
    public ZirkelFallViewModel? Fall { get; set; }

    // Spielleitung: Fehlversuche
    public List<ZirkelVersuchZeile> Versuche { get; set; } = new();
    public int AktiveSperren { get; set; }
}

public class ZirkelSpalte
{
    public ZirkelSeite Seite { get; set; }
    public string Titel { get; set; } = "";
    public string Genitiv { get; set; } = "";

    /// <summary>Bekannter Name (für Spieler nur gefundene Teile).</summary>
    public string Name { get; set; } = "";
    public List<ZirkelZeile> Zeilen { get; set; } = new();
}

/// <summary>Ein ratbares Feld auf der Rückseite (JS: zkPlRow / zkSlRow).</summary>
public class ZirkelZeile
{
    public string Feld { get; set; } = "";
    public string Name { get; set; } = "";
    public string Typ { get; set; } = "text";

    /// <summary>Hinterlegte Wahrheit (roh, nur für die Spielleitung).</summary>
    public string? Wahrheit { get; set; }
    public bool HatWahrheit => !string.IsNullOrEmpty(Wahrheit);

    /// <summary>Offiziell gefunden: Spieler sehen den Wert.</summary>
    public bool Gefunden { get; set; }
    public string Anzeige { get; set; } = "";

    /// <summary>Noch gesperrt nach einem Fehlversuch (Sekunden), mit dem verworfenen Versuch.</summary>
    public int SperrSekunden { get; set; }
    public string? LetzterVersuch { get; set; }

    /// <summary>"ok" oder "bad" direkt nach einem Versuch.</summary>
    public string? Rueckmeldung { get; set; }

    /// <summary>Schlüssel wie im JavaScript, z. B. "7|p|vn".</summary>
    public string Schluessel { get; set; } = "";
}

public class ZirkelNotizZeile
{
    public int Id { get; set; }
    public string Typ { get; set; } = "";
    public string Zu { get; set; } = "";
    public string Von { get; set; } = "";
    public string Text { get; set; } = "";
    public bool Bestaetigt { get; set; }
}

public class ZirkelFallViewModel
{
    public ZirkelFallStufe Tot { get; set; } = null!;
    public ZirkelFallStufe Moerder { get; set; } = null!;
    public bool MotivOffen { get; set; }
    public bool MoerderGeloest { get; set; }
    public string? Motiv { get; set; }

    // Spielleitung: Lösung
    public bool HatToten { get; set; }
    public int? TotNr { get; set; }
    public string? MoerderWert { get; set; }
    public int MaxVersuche { get; set; }
}

/// <summary>Eine Frage im Mordfall (JS: zkStep).</summary>
public class ZirkelFallStufe
{
    public string Frage { get; set; } = "";
    public string Titel { get; set; } = "";
    public bool Geloest { get; set; }
    public bool Offen { get; set; }
    public bool HatLoesung { get; set; }
    public string Antwort { get; set; } = "";
    public int Rest { get; set; }
    public int Max { get; set; }

    /// <summary>"1 Versuch" bzw. "3 Versuche".</summary>
    public string RestText
    {
        get
        {
            if (Rest == 1)
            {
                return "1 Versuch";
            }
            return Rest + " Versuche";
        }
    }
    public int SperrSekunden { get; set; }
    public string? Rueckmeldung { get; set; }
    public List<(string Wert, string Text)> Optionen { get; set; } = new();
}

public class ZirkelVersuchZeile
{
    public string Tag { get; set; } = "";
    public string Was { get; set; } = "";
    public string Wert { get; set; } = "";
}

// ------------------------------------------------------------------ Ätherwaffen

/// <summary>Ätherwaffen: Kategorie, Altar, Arsenal und Chronik (JS: renderArmory).</summary>
public class WaffenViewModel
{
    public WaffenRegeln.Kategorie Kategorie { get; set; } = null!;
    public WaffenRegeln.Waffe[] Arsenal => WaffenRegeln.Waffen[Kategorie.Art];

    /// <summary>Zuletzt beschworene Waffe dieser Kategorie (null = noch keine).</summary>
    public WaffenRegeln.Waffe? Letzte { get; set; }

    /// <summary>Gerade beschworen: Platz im Arsenal, für die Glücksrad-Animation.</summary>
    public int? Neu { get; set; }

    public List<Waffenwurf> Chronik { get; set; } = new();
    public bool DarfBeschwoeren { get; set; }
}

/// <summary>Karte einer Waffe auf dem Altar (JS: weaponCardHtml). Vergangen = "zuletzt beschworen".</summary>
public record WaffenKarte(WaffenRegeln.Kategorie Kategorie, WaffenRegeln.Waffe Waffe, bool Vergangen);

// =====================================================================
// Schuljahr · Questbrett (JS: MODS.missionen)
// =====================================================================

/// <summary>Das Brett im Ostflügel und das Ermittlungsbuch.</summary>
public class QuestbrettViewModel
{
    /// <summary>Die Hauptquest am Brett (erste aktive), null = kein Aushang.</summary>
    public QuestZeile? Haupt { get; set; }

    /// <summary>Alle Quests fürs Buch: erst die aktiven in Brett-Reihenfolge, dann die erledigten.</summary>
    public List<QuestZeile> Buch { get; set; } = new();

    /// <summary>Anzahl der laufenden Nebenquests (aktiv, aber nicht am Brett).</summary>
    public int Nebenquests { get; set; }

    /// <summary>Im Buch aufgeschlagener Fall (JS: MS.sel).</summary>
    public int? AuswahlId { get; set; }

    /// <summary>Buch nach dem Neuladen gleich offen lassen (z. B. nach Abhaken im Buch).</summary>
    public bool BuchOffen { get; set; }

    /// <summary>Gerade geklärter Hinweis, der rechts frisch angepinnt wird (JS: MS.pinned).</summary>
    public int? FrischId { get; set; }

    public bool DarfBearbeiten { get; set; }
    public bool IstSpielleitung { get; set; }
}

public class QuestZeile
{
    public int Id { get; set; }
    public int Nummer { get; set; }

    /// <summary>Römische Nummer für das Buch (JS: mRoman).</summary>
    public string Roemisch { get; set; } = "";
    public string Titel { get; set; } = "";
    public string? Ziel { get; set; }
    public string? Notiz { get; set; }
    public string? StartText { get; set; }
    public string? EndeText { get; set; }
    public string? VerfallenText { get; set; }
    public bool Erledigt { get; set; }
    public bool IstHaupt { get; set; }
    public List<QuestHinweisZeile> Hinweise { get; set; } = new();

    public int Offen => Hinweise.Count(h => !h.Erledigt);
    public int Geklaert => Hinweise.Count(h => h.Erledigt);
}

public class QuestHinweisZeile
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public bool Erledigt { get; set; }
}

// =====================================================================
// Schuljahr · Tagebuch „Der Hesparis-Bote“ (JS: renderProtokoll)
// =====================================================================

public class TagebuchViewModel
{
    /// <summary>Alle Ausgaben, neueste zuerst. Nr. zählt von der ältesten an.</summary>
    public List<AusgabeZeile> Ausgaben { get; set; } = new();

    /// <summary>Aufgeschlagene Ausgabe; null = neue Ausgabe setzen (auch, wenn es noch keine gibt).</summary>
    public AusgabeZeile? Auswahl { get; set; }

    /// <summary>Die Auswahl wird gerade überarbeitet.</summary>
    public bool Bearbeiten { get; set; }

    /// <summary>Gerade gedruckt, kommt mit Schwung herein (JS: protoFresh).</summary>
    public int? FrischAusgabeId { get; set; }

    /// <summary>Datumszeile über dem Blatt, wenn keine Ausgabe offen ist (heute, im Geschichtsjahr).</summary>
    public string HeuteDatum { get; set; } = "";

    /// <summary>Spieltag im Kalender wie "14. Oktober".</summary>
    public string Spieltag { get; set; } = "";

    /// <summary>Spielfiguren für „Dabei“ in ihren Farben.</summary>
    public List<FigurChip> Figuren { get; set; } = new();

    public List<AnzeigeZeile> Offen { get; set; } = new();
    public List<AnzeigeZeile> Erledigt { get; set; } = new();

    /// <summary>Kleinanzeigen gibt es überhaupt (für "Alles erledigt" statt "Keine Anzeigen").</summary>
    public bool HatAnzeigen => Offen.Count > 0 || Erledigt.Count > 0;
    public int? FrischAnzeigeId { get; set; }
    public bool ErledigtOffen { get; set; }

    public bool DarfBearbeiten { get; set; }
}

public class AusgabeZeile
{
    public int Id { get; set; }
    public int Nummer { get; set; }
    public string? Titel { get; set; }
    public string? Text { get; set; }

    /// <summary>"Donnerstag, 26. November 1818" (JS: boDay).</summary>
    public string Datum { get; set; } = "";

    /// <summary>"19:42" (JS: boTime).</summary>
    public string Uhrzeit { get; set; } = "";

    /// <summary>"26. Nov." fürs Archiv.</summary>
    public string DatumKurz { get; set; } = "";

    /// <summary>Spieltag wie "14. Oktober" (JS: boSpiel).</summary>
    public string? Spieltag { get; set; }
    public bool Ueberarbeitet { get; set; }
    public HashSet<int> Dabei { get; set; } = new();
}

public class AnzeigeZeile
{
    public int Id { get; set; }
    public string Text { get; set; } = "";

    /// <summary>"hoch", "normal" oder "spaeter" (CSS-Klasse p-…).</summary>
    public string Prio { get; set; } = "normal";
    public string PrioText { get; set; } = "";
    public bool Erledigt { get; set; }
}

/// <summary>Daten für das Teil _Kleinanzeige: die Anzeige plus Zustand der Seite für die Rücksprünge.</summary>
public record KleinanzeigeTeil(AnzeigeZeile Zeile, bool Frisch, bool Darf, int? Ausgabe, bool ErledigtOffen);
