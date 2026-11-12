namespace Suendenbock_App.Models.Domain.Hesparis;

/// <summary>Tagesordnungspunkt des Schülerrats (JS: rat.tops).</summary>
public class RatTagesordnungspunkt : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public int Position { get; set; }
    public string Text { get; set; } = "";
    public string? Von { get; set; }
    public bool Erledigt { get; set; }
}

/// <summary>Antrag in einer Ratssitzung (JS: rat.ant[monat]).</summary>
public class RatAntrag : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;

    /// <summary>Sitzungsmonat "yyyy-MM".</summary>
    public string Monat { get; set; } = "";
    public int Position { get; set; }

    /// <summary>Kennung des Antragstyps aus dem Antragskatalog, z. B. "umhang".</summary>
    public string Schluessel { get; set; } = "";

    /// <summary>geld, ordnung, essen …</summary>
    public string? Thema { get; set; }
    public string Titel { get; set; } = "";
    public string? Wirkung { get; set; }

    /// <summary>Hat eine Wirkung in der App (JS: app).</summary>
    public bool WirktInApp { get; set; }
    public string? Von { get; set; }

    /// <summary>Haltung der 1c: ja / nein (JS: pos).</summary>
    public string? Haltung { get; set; }

    /// <summary>an = angenommen, ab = abgelehnt, null = noch nicht abgestimmt.</summary>
    public string? Ergebnis { get; set; }
    public DateOnly? AbgestimmtAm { get; set; }

    /// <summary>Ergebnisgewicht aus der Auszählung (JS: w).</summary>
    public decimal? Gewicht { get; set; }

    public ICollection<RatUnterschrift> Unterschriften { get; set; } = new List<RatUnterschrift>();
    public ICollection<RatGefallen> Gefallen { get; set; } = new List<RatGefallen>();
    public ICollection<RatStimme> Stimmen { get; set; } = new List<RatStimme>();
}

/// <summary>Unterschrift eines Mitschülers (JS: sig) oder ausdrückliche Weigerung (JS: nosig).</summary>
public class RatUnterschrift
{
    public int RatAntragId { get; set; }
    public RatAntrag RatAntrag { get; set; } = null!;
    public int MitschuelerId { get; set; }
    public Mitschueler Mitschueler { get; set; } = null!;
    public bool Verweigert { get; set; }
}

/// <summary>Gefallen, den ein Mitschüler für seine Unterschrift will (JS: q[mitschueler]).</summary>
public class RatGefallen
{
    public int RatAntragId { get; set; }
    public RatAntrag RatAntrag { get; set; } = null!;
    public int MitschuelerId { get; set; }
    public Mitschueler Mitschueler { get; set; } = null!;

    /// <summary>Spielfigur, die sich kümmert (JS: by).</summary>
    public int? SpielfigurId { get; set; }
    public Spielfigur? Spielfigur { get; set; }
    public bool Erledigt { get; set; }
}

/// <summary>Stimme einer Klasse bei der Abstimmung (JS: votes[klasse]).</summary>
public class RatStimme
{
    public int RatAntragId { get; set; }
    public RatAntrag RatAntrag { get; set; } = null!;

    /// <summary>Klasse, z. B. "2a".</summary>
    public string Klasse { get; set; } = "";
    public bool Ja { get; set; }
}

/// <summary>Beschlossene Schulregel (JS: rat.regeln).</summary>
public class RatRegel : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string Schluessel { get; set; } = "";
    public string? Thema { get; set; }
    public string Titel { get; set; } = "";
    public string? Wirkung { get; set; }
    public bool WirktInApp { get; set; }
    public DateOnly? Seit { get; set; }

    /// <summary>Wieder aufgehoben am (JS: aus).</summary>
    public DateOnly? AufgehobenAm { get; set; }
    public int? RatAntragId { get; set; }
    public RatAntrag? RatAntrag { get; set; }
}

/// <summary>Antragstyp, der schon einmal dran war (JS: rat.used).</summary>
public class RatVerbrauchterAntrag : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string Schluessel { get; set; } = "";
}

/// <summary>Vorgeplanter Antrag für eine künftige Sitzung (JS: rat.plan[monat][slot]).</summary>
public class RatPlanung : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string Monat { get; set; } = "";

    /// <summary>Platz 0–2 in der Sitzung.</summary>
    public int Slot { get; set; }
    public string Schluessel { get; set; } = "";
    public string? Thema { get; set; }
    public string? Titel { get; set; }
    public string? Wirkung { get; set; }
    public bool WirktInApp { get; set; }

    /// <summary>Grundzustimmung (JS: base).</summary>
    public decimal? Basis { get; set; }
}
