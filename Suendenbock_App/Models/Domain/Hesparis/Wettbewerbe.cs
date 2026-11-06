namespace Suendenbock_App.Models.Domain.Hesparis;

/// <summary>Wettbewerb der Akademie-Gesamtwertung (JS: wertung.comps).</summary>
public class Wettbewerb : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public int Position { get; set; }
    public string Name { get; set; } = "";

    /// <summary>Disziplin als Text, z. B. "Glimrack" oder "Überzeugen und Reizen" (JS: fach).</summary>
    public string? Disziplin { get; set; }

    /// <summary>Wie das Ergebnis zustande kam, z. B. "zufall" (JS: how).</summary>
    public string? Ermittlung { get; set; }

    public ICollection<WettbewerbPlatzierung> Platzierungen { get; set; } = new List<WettbewerbPlatzierung>();
}

/// <summary>Platz einer Akademie in einem Wettbewerb (JS: comps.res[team]).</summary>
public class WettbewerbPlatzierung
{
    public int WettbewerbId { get; set; }
    public Wettbewerb Wettbewerb { get; set; } = null!;
    public string TeamCode { get; set; } = "";
    public AkademieTeam Team { get; set; } = null!;
    public int Platz { get; set; }
}

/// <summary>Notiz zur Rivalität mit einer Akademie (JS: wertung.notes[team]).</summary>
public class TeamNotiz : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string TeamCode { get; set; } = "";
    public AkademieTeam Team { get; set; } = null!;
    public string Text { get; set; } = "";
}

/// <summary>Zustand eines Wettkampf-Moduls (JS: wk.duell, wk.rede, wk.braumeister, wk.rallye).</summary>
public class WettkampfStand : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public Wettkampfart Art { get; set; }

    /// <summary>
    /// Gerade laufendes Turnier (JS: wk[art].t): Spielplan, Runden, aktuelles Duell.
    /// Engine-Zustand des Frontends, deshalb JSON. Abgeschlossene Turniere landen in
    /// <see cref="WettkampfErgebnis"/> und <see cref="WettbewerbPlatzierung"/>.
    /// </summary>
    public string? LaufendesTurnierJson { get; set; }

    /// <summary>Nur Duell: gerade laufendes Übungsduell im Club (JS: wk.duell.club.cur). JSON.</summary>
    public string? LaufendesClubduellJson { get; set; }
}

/// <summary>Teilnehmer eines Wettkampfs (Rallye-Team, Duellclub-Mitglieder).</summary>
public class WettkampfTeilnehmer : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public Wettkampfart Art { get; set; }

    /// <summary>"team" (JS: wk.rallye.team) oder "club" (JS: wk.duell.club.pcs).</summary>
    public string Gruppe { get; set; } = "";
    public int Position { get; set; }

    /// <summary>Referenz-Typ aus dem Frontend: "pc" = Spielfigur, sonst NPC/Mitschüler.</summary>
    public string Typ { get; set; } = "pc";
    public int? SpielfigurId { get; set; }
    public Spielfigur? Spielfigur { get; set; }

    /// <summary>Kennung des NPCs/Mitschülers, wenn keine Spielfigur.</summary>
    public string? Code { get; set; }
}

/// <summary>Siege und Niederlagen im Duellclub (JS: wk.duell.club.bilanz).</summary>
public class DuellClubBilanz : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string Typ { get; set; } = "pc";
    public int? SpielfigurId { get; set; }
    public Spielfigur? Spielfigur { get; set; }
    public string? Code { get; set; }
    public int Siege { get; set; }
    public int Niederlagen { get; set; }
}

/// <summary>Abgeschlossenes Turnier (JS: wk[art].archiv).</summary>
public class WettkampfErgebnis : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public Wettkampfart Art { get; set; }

    /// <summary>0 = neuestes Ergebnis.</summary>
    public int Position { get; set; }
    public string? DatumText { get; set; }

    /// <summary>Wer für Hesparis angetreten ist.</summary>
    public string? Starter { get; set; }

    /// <summary>Platz von Hesparis.</summary>
    public int? Platz { get; set; }
    public string? SiegerTeamCode { get; set; }
    public AkademieTeam? SiegerTeam { get; set; }

    /// <summary>Ausgewürfelt, ohne Spielfiguren (JS: auto).</summary>
    public bool Ausgewuerfelt { get; set; }
}

/// <summary>Halbjahres-Abrechnung der Wettkämpfe (JS: wk.zeugnis[stichtag]).</summary>
public class WettkampfZeugnis : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public DateOnly Stichtag { get; set; }

    /// <summary>Spieltag, an dem abgerechnet wurde (JS: at).</summary>
    public DateOnly? ErstelltAm { get; set; }

    public ICollection<WettkampfZeugnisEintrag> Eintraege { get; set; } = new List<WettkampfZeugnisEintrag>();
}

public class WettkampfZeugnisEintrag
{
    public int Id { get; set; }
    public int WettkampfZeugnisId { get; set; }
    public WettkampfZeugnis WettkampfZeugnis { get; set; } = null!;
    public Wettkampfart Art { get; set; }

    /// <summary>fertig, offen oder auto.</summary>
    public string Status { get; set; } = "";
    public int? Platz { get; set; }
}

/// <summary>NPC-Konkurrent in einem Wettkampf (JS: wkNpc bzw. WK_HES_NPC_START).</summary>
public class WettkampfNpc : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public Wettkampfart Art { get; set; }
    public int Position { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public int Jahrgang { get; set; }

    /// <summary>Grundwert (JS: base), nicht bei allen Wettkämpfen.</summary>
    public int? Grundwert { get; set; }
    public string? Text { get; set; }

    public ICollection<WettkampfNpcWert> Werte { get; set; } = new List<WettkampfNpcWert>();
}

/// <summary>Einzelwert eines NPCs, z. B. angriff = 58 oder Wahrnehmung = 60 (JS: v).</summary>
public class WettkampfNpcWert
{
    public int WettkampfNpcId { get; set; }
    public WettkampfNpc WettkampfNpc { get; set; } = null!;
    public string Schluessel { get; set; } = "";
    public int Wert { get; set; }
}
