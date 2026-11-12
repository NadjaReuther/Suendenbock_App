using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Models.Domain.Hesparis;

/// <summary>
/// Spielwerte eines Spielercharakters (JS: state.players[i]). Gehört zu einem HesparisCharakter der Art Spieler.
/// Bearbeiten dürfen alle Spieler und die Spielleitung (Gott).
/// </summary>
public class Spielfigur : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;

    /// <summary>Reihenfolge in der Gruppe (bisheriger Array-Index, 0–4).</summary>
    public int Position { get; set; }

    /// <summary>Der Hesparis-Charakter (Art Spieler) zu diesen Spielwerten.</summary>
    public int? HesparisCharakterId { get; set; }
    public HesparisCharakter? HesparisCharakter { get; set; }

    /// <summary>
    /// ALT: Verweis auf den Sündenbock-Character aus der Zeit vor der eigenen Hesparis-Tabelle.
    /// Wird nur noch von HesparisUmzug gelesen und im nächsten Schritt entfernt.
    /// </summary>
    public int? CharacterId { get; set; }
    public Character? Character { get; set; }

    /// <summary>Meilenstein 2/3 der Magiestufen erreicht (JS: m2, m3).</summary>
    public bool Meilenstein2 { get; set; }
    public bool Meilenstein3 { get; set; }

    /// <summary>Zusätzliche Stufenpunkte (JS: bonus).</summary>
    public int Bonuspunkte { get; set; }

    /// <summary>Geld in Kerzen, der kleinsten Münze (1 Hiro = 1000, 1 Nimbus = 100, 1 Lumen = 10).</summary>
    public int Geld { get; set; }

    public int LebenspunkteAktuell { get; set; } = 100;
    public int LebenspunkteMax { get; set; } = 100;

    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<SpielfigurFachStufe> FachStufen { get; set; } = new List<SpielfigurFachStufe>();
    public ICollection<StufenWahl> StufenWahlen { get; set; } = new List<StufenWahl>();
    public ICollection<Schulnote> Noten { get; set; } = new List<Schulnote>();
    public ICollection<Pruefungseintrag> Pruefungen { get; set; } = new List<Pruefungseintrag>();
    public ICollection<TalentWert> Talente { get; set; } = new List<TalentWert>();
    public ICollection<TalentZiel> TalentZiele { get; set; } = new List<TalentZiel>();
    public ICollection<SpielfigurMeisterschaft> Meisterschaften { get; set; } = new List<SpielfigurMeisterschaft>();
    public ICollection<Gegenstand> Gegenstaende { get; set; } = new List<Gegenstand>();
    public ICollection<Geldbuchung> Geldbuchungen { get; set; } = new List<Geldbuchung>();
    public ICollection<Wirkung> Wirkungen { get; set; } = new List<Wirkung>();
}

/// <summary>Erreichte Stufe in einem Fach (JS: levels[fach]).</summary>
public class SpielfigurFachStufe
{
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;
    public int FachId { get; set; }
    public Fach Fach { get; set; } = null!;
    public int Stufe { get; set; }
}

/// <summary>Gewählte Belohnung für Fach/Stufe (JS: choices["fach-stufe"]).</summary>
public class StufenWahl
{
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;
    public int FachId { get; set; }
    public Fach Fach { get; set; } = null!;
    public int Stufe { get; set; }
    public string Wahl { get; set; } = "";
}

/// <summary>Einzelne Zeugnisnote in einem Fach (JS: noten[fach][i]).</summary>
public class Schulnote
{
    public int Id { get; set; }
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;
    public int FachId { get; set; }
    public Fach Fach { get; set; } = null!;

    /// <summary>Reihenfolge innerhalb des Fachs.</summary>
    public int Position { get; set; }
    public Note Note { get; set; }
}

/// <summary>Prüfungsnoten der Abschlussprüfungen (JS: pruef[abschnitt][fach] = note).</summary>
public class Pruefungseintrag
{
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;

    /// <summary>Abschnitt (JS: SJ_ABS.key), z. B. "hafen".</summary>
    public string Abschnitt { get; set; } = "";

    /// <summary>Teil der Prüfung, wie das Frontend ihn vergibt (meist Fach-Index).</summary>
    public string Teil { get; set; } = "";
    public string Wert { get; set; } = "";
}

/// <summary>Talentpunkte (JS: talente[begabung][i]).</summary>
public class TalentWert
{
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;
    public string TalentCode { get; set; } = "";
    public Talent Talent { get; set; } = null!;
    public int Wert { get; set; }
}

/// <summary>Auf welches Talent ein wählbarer Stufenbonus geht (JS: talentZiel["8-1#0"] = "w3").</summary>
public class TalentZiel
{
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;

    /// <summary>Schlüssel "Fach-Stufe#n" aus dem Frontend.</summary>
    public string Schluessel { get; set; } = "";
    public string TalentCode { get; set; } = "";
    public Talent Talent { get; set; } = null!;
}

/// <summary>Gewählte Meisterschaft (JS: mst[talent] = [index, …]).</summary>
public class SpielfigurMeisterschaft
{
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;
    public int MeisterschaftDefinitionId { get; set; }
    public MeisterschaftDefinition MeisterschaftDefinition { get; set; } = null!;
}

/// <summary>Gegenstand in der Tasche (JS: items).</summary>
public class Gegenstand
{
    public int Id { get; set; }
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;
    public string Name { get; set; } = "";

    /// <summary>Ausrüstung, Trank, Zutat, Buch, Süßes, Sonstiges.</summary>
    public string Kategorie { get; set; } = "Sonstiges";
    public int Menge { get; set; } = 1;
    public string? Notiz { get; set; }

    /// <summary>Herkunft: gefunden, laden, schwarz …</summary>
    public string? Quelle { get; set; }

    /// <summary>Kaufpreis, falls gekauft.</summary>
    public int? Preis { get; set; }

    /// <summary>Neu in der Tasche, noch nicht angesehen (JS: fresh).</summary>
    public bool Neu { get; set; }
    public Effekt Effekt { get; set; } = new();
}

/// <summary>Letzte Bewegungen im Geldbeutel (JS: purseLog, das Frontend behält die letzten 5).</summary>
public class Geldbuchung
{
    public int Id { get; set; }
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;

    /// <summary>0 = neueste Buchung.</summary>
    public int Position { get; set; }

    /// <summary>Betrag in Kerzen, negativ = ausgegeben.</summary>
    public int Betrag { get; set; }
    public string Grund { get; set; } = "";
}

/// <summary>Gerade aktive Wirkung, z. B. ein getrunkener Trank (JS: wirk).</summary>
public class Wirkung
{
    public int Id { get; set; }
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;
    public string Name { get; set; } = "";
    public Effekt Effekt { get; set; } = new();

    /// <summary>Spieltag, ab dem die Wirkung gilt (JS: ab).</summary>
    public DateOnly? Ab { get; set; }

    /// <summary>Verbleibende Anwendungen (JS: left).</summary>
    public int? Restanwendungen { get; set; }
}

/// <summary>Stand der Sonderzauber-Wahl (JS: sonderzauber[code]).</summary>
public class SonderzauberStand : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string SonderzauberCode { get; set; } = "";
    public Sonderzauber Sonderzauber { get; set; } = null!;

    /// <summary>Endgültig gewählte Option (Position), null = noch offen.</summary>
    public int? GewaehltePosition { get; set; }

    public ICollection<SonderzauberAufgedeckt> Aufgedeckt { get; set; } = new List<SonderzauberAufgedeckt>();
}

public class SonderzauberAufgedeckt
{
    public int SonderzauberStandId { get; set; }
    public SonderzauberStand SonderzauberStand { get; set; } = null!;
    public int Position { get; set; }
}
