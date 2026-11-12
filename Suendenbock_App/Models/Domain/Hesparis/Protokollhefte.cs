namespace Suendenbock_App.Models.Domain.Hesparis;

/// <summary>
/// Protokollheft: Außeneinsatz der Gruppe, je Auftrag ein Heft (JS: hefte).
/// </summary>
public class Protokollheft : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public int Nummer { get; set; }

    /// <summary>Noch nicht ausgegeben (JS: draft).</summary>
    public bool Entwurf { get; set; }
    public string Titel { get; set; } = "";
    public string? Lehrer { get; set; }
    public string? Ort { get; set; }
    public DateOnly? Von { get; set; }
    public DateOnly? Bis { get; set; }
    public DateOnly? Abgabe { get; set; }
    public string? Auftrag { get; set; }

    /// <summary>Mitschüler, der mitkommt (JS: plus).</summary>
    public int? BegleiterId { get; set; }
    public Mitschueler? Begleiter { get; set; }

    /// <summary>Wer schreibt: "p0" für Spielfigur 0 oder Mitschüler-Code (JS: schreiber).</summary>
    public int? SchreiberSpielfigurId { get; set; }
    public Spielfigur? SchreiberSpielfigur { get; set; }
    public int? SchreiberMitschuelerId { get; set; }
    public Mitschueler? SchreiberMitschueler { get; set; }

    public DateTime? AbgegebenAm { get; set; }
    public string? Note { get; set; }
    public string? Rueckmeldung { get; set; }
    public int? KalenderTerminId { get; set; }
    public KalenderTermin? KalenderTermin { get; set; }

    public ICollection<HeftSchwerpunkt> Schwerpunkte { get; set; } = new List<HeftSchwerpunkt>();
    public ICollection<HeftFehlend> Fehlend { get; set; } = new List<HeftFehlend>();
    public ICollection<HeftNotiz> Notizen { get; set; } = new List<HeftNotiz>();
    public ICollection<HeftAbschnitt> Abschnitte { get; set; } = new List<HeftAbschnitt>();
    public ICollection<HeftFrage> Fragen { get; set; } = new List<HeftFrage>();
    public ICollection<HeftAblauf> Ablauf { get; set; } = new List<HeftAblauf>();
    public ICollection<HeftPerson> Personen { get; set; } = new List<HeftPerson>();
    public ICollection<HeftJederEintrag> JederEintraege { get; set; } = new List<HeftJederEintrag>();
    public ICollection<HeftCheck> Checks { get; set; } = new List<HeftCheck>();
}

/// <summary>Worauf die Lehrkraft achtet (JS: achtet).</summary>
public class HeftSchwerpunkt
{
    public int ProtokollheftId { get; set; }
    public Protokollheft Protokollheft { get; set; } = null!;
    public string Text { get; set; } = "";
}

/// <summary>Spielfigur war beim Einsatz nicht dabei (JS: fehlt).</summary>
public class HeftFehlend
{
    public int ProtokollheftId { get; set; }
    public Protokollheft Protokollheft { get; set; } = null!;
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;
}

/// <summary>Notiz von unterwegs (JS: notizen).</summary>
public class HeftNotiz
{
    public int Id { get; set; }
    public int ProtokollheftId { get; set; }
    public Protokollheft Protokollheft { get; set; } = null!;
    public int Position { get; set; }
    public DateOnly? Datum { get; set; }
    public string? Uhrzeit { get; set; }
    public string Text { get; set; } = "";

    public ICollection<HeftNotizUebernahme> Uebernommen { get; set; } = new List<HeftNotizUebernahme>();
}

/// <summary>In welchen Abschnitt eine Notiz schon übernommen wurde (JS: imp).</summary>
public class HeftNotizUebernahme
{
    public int HeftNotizId { get; set; }
    public HeftNotiz HeftNotiz { get; set; } = null!;
    public string Abschnitt { get; set; } = "";
}

/// <summary>Text eines Heftabschnitts (JS: txt[abschnitt]).</summary>
public class HeftAbschnitt
{
    public int ProtokollheftId { get; set; }
    public Protokollheft Protokollheft { get; set; } = null!;
    public string Abschnitt { get; set; } = "";
    public string Text { get; set; } = "";
}

/// <summary>Abgehakte Leitfrage eines Abschnitts (JS: fq[abschnitt][i]).</summary>
public class HeftFrage
{
    public int ProtokollheftId { get; set; }
    public Protokollheft Protokollheft { get; set; } = null!;
    public string Abschnitt { get; set; } = "";
    public int Position { get; set; }
    public bool Beantwortet { get; set; }
}

/// <summary>Zeile im Ablauf (JS: ablauf).</summary>
public class HeftAblauf
{
    public int Id { get; set; }
    public int ProtokollheftId { get; set; }
    public Protokollheft Protokollheft { get; set; } = null!;
    public int Position { get; set; }
    public DateOnly? Datum { get; set; }
    public string? Uhrzeit { get; set; }
    public string Text { get; set; } = "";
    public string? Wer { get; set; }

    /// <summary>Als wichtig markiert (JS: star).</summary>
    public bool Wichtig { get; set; }
}

/// <summary>Angetroffene Person (JS: personen).</summary>
public class HeftPerson
{
    public int Id { get; set; }
    public int ProtokollheftId { get; set; }
    public Protokollheft Protokollheft { get; set; } = null!;
    public int Position { get; set; }
    public string Name { get; set; } = "";
    public string? Rolle { get; set; }
    public string? Sagt { get; set; }
    public string? Eindruck { get; set; }
    public bool Wichtig { get; set; }
}

/// <summary>Beitrag je Person/Schlüssel (JS: jeder[key]).</summary>
public class HeftJederEintrag
{
    public int ProtokollheftId { get; set; }
    public Protokollheft Protokollheft { get; set; } = null!;
    public string Schluessel { get; set; } = "";
    public string Text { get; set; } = "";
}

/// <summary>Abgabe-Checkliste (JS: ck[key]).</summary>
public class HeftCheck
{
    public int ProtokollheftId { get; set; }
    public Protokollheft Protokollheft { get; set; } = null!;
    public string Schluessel { get; set; } = "";
    public bool Erledigt { get; set; }
}
