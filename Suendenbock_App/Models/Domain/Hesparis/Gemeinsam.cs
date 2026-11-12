namespace Suendenbock_App.Models.Domain.Hesparis;

// ---------------------------------------------------------------------------
// Gemeinsame Bausteine: Enums und Wertobjekte (Owned Types), die von mehreren
// Tabellen genutzt werden. Enums werden in der Datenbank als Text gespeichert
// (siehe HesparisDbContext.ConfigureConventions), damit die Tabellen lesbar bleiben.
// ---------------------------------------------------------------------------

/// <summary>Schulnote. S ist die Sondernote (zählt 0,9).</summary>
public enum Note { S, A, B, C, D, E }

public enum Termintyp { Schule, Feiertag, Ferien, Glimrack, Wettbewerb, Pruefung, Rat, Sonstiges }

public enum Waffenart { Nah, Fern, Vert }

public enum Mahlzeit { Fruehstueck, Mittag, Abendbrot }

public enum Prioritaet { Hoch, Normal, Spaeter }

/// <summary>Die vier Akademie-Wettkämpfe mit eigenem Modul (Glimrack läuft separat).</summary>
public enum Wettkampfart { Duell, Rede, Braumeister, Rallye }

public enum EffektArt
{
    /// <summary>Talentbonus/-malus (Talent + Wert).</summary>
    Talent,
    /// <summary>Vorteil auf eine Probe (VorteilAuf).</summary>
    Vorteil,
}

public enum EffektDauerArt
{
    /// <summary>Wirkt eine Anzahl Spieltage.</summary>
    Tage,
    /// <summary>Wirkt eine Anzahl Anwendungen.</summary>
    Anwendungen,
}

/// <summary>Seite einer Person im Zirkel-Rätsel.</summary>
public enum ZirkelSeite { Paar, Spiegel }

/// <summary>
/// Wirkung eines Gegenstands, einer Ware oder eines Trankes (JS: <c>fx</c>).
/// Wird als Owned Type in die Spalten der jeweiligen Tabelle eingebettet.
/// </summary>
public class Effekt
{
    public EffektArt? Art { get; set; }

    /// <summary>Talent-Code wie "s7" (FK auf <see cref="Talent"/>), nur bei Art = Talent.</summary>
    public string? TalentCode { get; set; }

    /// <summary>Bonus/Malus in Talentpunkten (JS: <c>n</c>).</summary>
    public int? Wert { get; set; }

    /// <summary>Worauf der Vorteil gilt, z. B. "Schulwissen" (JS: <c>auf</c>).</summary>
    public string? VorteilAuf { get; set; }

    public EffektDauerArt? DauerArt { get; set; }

    /// <summary>Anzahl Tage bzw. Anwendungen (JS: <c>dn</c>).</summary>
    public int? DauerAnzahl { get; set; }

    /// <summary>Freitext-Dauer aus älteren Einträgen, z. B. "1 Tag" (JS: <c>dauer</c>).</summary>
    public string? DauerText { get; set; }
}

/// <summary>Die fünf Glimrack-Werte eines Spielers (1–10).</summary>
public class GlimWerte
{
    public int Wurf { get; set; }
    public int Schlag { get; set; }
    public int Tempo { get; set; }
    public int Fang { get; set; }
    public int Nerven { get; set; }
}

/// <summary>Beziehung in eine Richtung, fünf Kategorien mit Stufen 0–5.</summary>
public class Beziehungswerte
{
    public int Freundschaft { get; set; }
    public int Romantik { get; set; }
    public int Rivalitaet { get; set; }
    public int Abneigung { get; set; }
    public int Respekt { get; set; }
}

/// <summary>Alles, was zu genau einer Kampagne (Spielrunde) gehört.</summary>
public interface IKampagnenDaten
{
    int KampagneId { get; set; }
}
