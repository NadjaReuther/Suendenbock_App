namespace Suendenbock_App.Models.Domain.Hesparis;

// ---------------------------------------------------------------------------
// Stammdaten: feste Spielinhalte, die bisher als Konstanten im JavaScript lagen.
// Sie gelten für alle Kampagnen und werden per HasData eingespielt
// (Data/Stammdaten/StammdatenSeed.cs). Die Schlüssel sind die Codes aus dem
// JavaScript, damit Frontend und Datenbank dieselben IDs benutzen.
// ---------------------------------------------------------------------------

/// <summary>Unterrichtsfach (JS: FAECHER). Id = Position in der Fächerliste (0–9).</summary>
public class Fach
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    /// <summary>Kurzname für Karte/Stundenplan (JS: KM_FACH_SHORT).</summary>
    public string? Kurzname { get; set; }

    /// <summary>Erklärung des Fachs (JS: FAECHER[i].desc).</summary>
    public string? Beschreibung { get; set; }

    /// <summary>Fähigkeit bei Hafenaufsicht (Stufe 3), Angerwacht (7), Gartenhüter (9), Domwächter (10) (JS: perks).</summary>
    public string? PerkHafen { get; set; }
    public string? PerkAnger { get; set; }
    public string? PerkGarten { get; set; }
    public string? PerkDom { get; set; }

    /// <summary>Zusatztext zur Hafen-Fähigkeit (JS: hafenText).</summary>
    public string? HafenText { get; set; }

    public ICollection<FachStufe> Stufen { get; set; } = new List<FachStufe>();
}

/// <summary>Freischaltbare Stufe eines Fachs (JS: FAECHER[i].tiers).</summary>
public class FachStufe
{
    public int FachId { get; set; }
    public Fach Fach { get; set; } = null!;

    /// <summary>Stufe 1–10.</summary>
    public int Stufe { get; set; }
    public string Beschreibung { get; set; } = "";
}

/// <summary>Begabung: Handeln (h), Wissen (w), Soziales (s).</summary>
public class Begabung
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public ICollection<Talent> Talente { get; set; } = new List<Talent>();
}

/// <summary>Talent, Code wie "h0" = erstes Talent der Begabung Handeln.</summary>
public class Talent
{
    public string Code { get; set; } = "";
    public string BegabungCode { get; set; } = "";
    public Begabung Begabung { get; set; } = null!;
    public int Position { get; set; }
    public string Name { get; set; } = "";

    public ICollection<MeisterschaftDefinition> Meisterschaften { get; set; } = new List<MeisterschaftDefinition>();
}

/// <summary>Wählbare Meisterschaft eines Talents (JS: MEISTERSCHAFTEN).</summary>
public class MeisterschaftDefinition
{
    public int Id { get; set; }
    public string TalentCode { get; set; } = "";
    public Talent Talent { get; set; } = null!;

    /// <summary>Position in der Liste des Talents (0–2); so speichert das JS die Wahl.</summary>
    public int Position { get; set; }
    public string Name { get; set; } = "";
    public string Beschreibung { get; set; } = "";
}

/// <summary>Arbeitsgemeinschaft (JS: AGS).</summary>
public class Ag
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Untertitel { get; set; }
    public string? Kategorie { get; set; }
    public string? Leitung { get; set; }

    /// <summary>Talent, auf das Mitglieder einen Bonus bekommen (JS: AGS[i].bonus.talent).</summary>
    public string? BonusTalentCode { get; set; }
    public Talent? BonusTalent { get; set; }

    /// <summary>Höhe des Bonus in Talentpunkten (JS: AGS[i].bonus.amount).</summary>
    public int BonusWert { get; set; }
}

/// <summary>Akademie-Mannschaft der Liga (JS: GLIM_TEAMS), auch für die Gesamtwertung.</summary>
public class AkademieTeam
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Kurz { get; set; } = "";
    public string? Ort { get; set; }

    /// <summary>Basis-Stärke; die aktuelle Stärke steht in <see cref="GlimTeamStaerke"/>.</summary>
    public decimal? Grundwert { get; set; }

    /// <summary>true = eure eigene Akademie (Hesparis).</summary>
    public bool Eigen { get; set; }
}

/// <summary>Sonderzauber-Auswahl einer Spielfigur (JS: SONDERZAUBER).</summary>
public class Sonderzauber
{
    public string Code { get; set; } = "";

    /// <summary>Name der Spielfigur, für die der Zauber gedacht ist.</summary>
    public string Person { get; set; } = "";
    public string Titel { get; set; } = "";
    public ICollection<SonderzauberOption> Optionen { get; set; } = new List<SonderzauberOption>();
}

public class SonderzauberOption
{
    public string SonderzauberCode { get; set; } = "";
    public Sonderzauber Sonderzauber { get; set; } = null!;
    public int Position { get; set; }
    public string Name { get; set; } = "";
    public string? Wirkung { get; set; }
    public string? Anwendung { get; set; }
    public int? Slots { get; set; }
    public string? Schaden { get; set; }
}

/// <summary>Gericht der Mensa (JS: ESS_DISHES).</summary>
public class Gericht
{
    public string Code { get; set; } = "";
    public Mahlzeit Mahlzeit { get; set; }
    public string Name { get; set; } = "";
    public string? Wirkung { get; set; }
    public int Preis { get; set; }

    /// <summary>heal, tal oder kurios.</summary>
    public string? Art { get; set; }

    /// <summary>Lebenspunkte, die das Gericht sofort heilt.</summary>
    public int? Lebenspunkte { get; set; }
}
