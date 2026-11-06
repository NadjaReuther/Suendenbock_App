namespace Suendenbock_App.Models.Domain.Hesparis;

/// <summary>Ware im Schulladen (JS: shop).</summary>
public class LadenArtikel : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public int Position { get; set; }
    public string Name { get; set; } = "";
    public string Kategorie { get; set; } = "Sonstiges";

    /// <summary>Preis in Kerzen.</summary>
    public int Preis { get; set; }
    public string? Beschreibung { get; set; }

    /// <summary>Vorrat; null = unbegrenzt.</summary>
    public int? Bestand { get; set; }
    public Effekt Effekt { get; set; } = new();
}

/// <summary>Ware am Schwarzmarkt (JS: schwarz.waren). Preis in Süßigkeiten.</summary>
public class SchwarzmarktWare : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public int Position { get; set; }
    public string Name { get; set; } = "";
    public string Kategorie { get; set; } = "Sonstiges";
    public int Preis { get; set; }
    public int? Bestand { get; set; }
    public string? Notiz { get; set; }
    public Effekt Effekt { get; set; } = new();
}

/// <summary>Verkaufslog des Schwarzmarkts (JS: schwarz.log, die letzten 12).</summary>
public class SchwarzmarktVerkauf : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;

    /// <summary>0 = neuester Eintrag.</summary>
    public int Position { get; set; }
    public string Text { get; set; } = "";
}

/// <summary>Trankrezept (JS: recipes).</summary>
public class Rezept : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public int Position { get; set; }
    public string Name { get; set; } = "";
    public string? Wirkung { get; set; }

    /// <summary>Modifikator auf die Brauprobe (JS: mod).</summary>
    public int Modifikator { get; set; }

    public ICollection<RezeptZutat> Zutaten { get; set; } = new List<RezeptZutat>();
}

public class RezeptZutat
{
    public int Id { get; set; }
    public int RezeptId { get; set; }
    public Rezept Rezept { get; set; } = null!;
    public int Position { get; set; }
    public string Name { get; set; } = "";
    public int Menge { get; set; } = 1;
}

/// <summary>Braubuch (JS: brewLog, die letzten 30).</summary>
public class Braueintrag : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public int Position { get; set; }
    public int? SpielfigurId { get; set; }
    public Spielfigur? Spielfigur { get; set; }

    /// <summary>ok, fail …</summary>
    public string? Ergebnis { get; set; }
    public string Text { get; set; } = "";
}

/// <summary>Zuletzt gewürfelte Ätherwaffen (JS: weaponHistory).</summary>
public class Waffenwurf : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public int Position { get; set; }
    public Waffenart Waffenart { get; set; }
    public string Name { get; set; } = "";
    public string Schaden { get; set; } = "";
}

/// <summary>Patient der Krankenstation (JS: kranken.faelle).</summary>
public class Krankheitsfall : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;

    /// <summary>Gesetzt, wenn eine Spielfigur behandelt wird.</summary>
    public int? SpielfigurId { get; set; }
    public Spielfigur? Spielfigur { get; set; }

    /// <summary>Name des Patienten (auch NPCs wie Glimrack-Spieler).</summary>
    public string Name { get; set; } = "";

    /// <summary>Verletzung, Krankheit …</summary>
    public string Art { get; set; } = "";
    public string? Diagnose { get; set; }
    public string? Behandlung { get; set; }
    public string? Malus { get; set; }
    public string? Notiz { get; set; }
    public int? Tage { get; set; }
    public DateOnly Aufnahme { get; set; }
    public DateOnly? Entlassung { get; set; }
    public int? Bett { get; set; }

    /// <summary>Simuliert / geschwänzt (JS: fake).</summary>
    public bool Simuliert { get; set; }

    /// <summary>Woher der Fall kam, z. B. "glimrack".</summary>
    public string? Quelle { get; set; }
}

/// <summary>Glimrack-Verletzte, die schon automatisch eingeliefert wurden (JS: kranken.glimSeen).</summary>
public class KrankenGlimGemeldet : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string Name { get; set; } = "";
}
