namespace Suendenbock_App.Models.Domain.Hesparis;

// Im Glimrack werden Personen über eine „Mitglieds-Referenz“ angesprochen, weil im
// Team drei Sorten Leute stehen können: Kader-NPCs ("k:Maren Holt"), Nachwuchsplätze
// ("nw:stein3") und Spielfiguren. Statistik, Karriere und Verletzungen hängen am
// Namen. Beides übernehmen wir unverändert, damit das Frontend weiter funktioniert.

/// <summary>Spieler im Kader oder in der Reserve (JS: glim.kader).</summary>
public class GlimSpieler : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;

    /// <summary>Mitglieds-Referenz, z. B. "k:Maren Holt".</summary>
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public int Jahrgang { get; set; }

    /// <summary>Fester Platz in der Startaufstellung (werfer, faenger, stein1 …), null = Reserve.</summary>
    public string? Slot { get; set; }

    /// <summary>Rolle in der Reserve, z. B. "Außenfeld".</summary>
    public string? Rolle { get; set; }
    public Waffenart Waffe { get; set; }
    public GlimWerte Werte { get; set; } = new();
    public string? Stil { get; set; }

    /// <summary>Erfahrener Spieler (JS: vet).</summary>
    public bool Veteran { get; set; }

    /// <summary>Statistik aus früheren Saisons (JS: saison).</summary>
    public int? VorsaisonTreffer { get; set; }
    public int? VorsaisonPunkte { get; set; }
    public int? VorsaisonAbwuerfe { get; set; }

    public ICollection<GlimTraining> Training { get; set; } = new List<GlimTraining>();
}

/// <summary>Nachwuchsplatz (JS: glim.nw[slot]), besetzt mit einem Mitschüler.</summary>
public class GlimNachwuchs : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;

    /// <summary>stein3 oder feld3.</summary>
    public string Slot { get; set; } = "";
    public int? MitschuelerId { get; set; }
    public Mitschueler? Mitschueler { get; set; }
    public int? Jahrgang { get; set; }
    public GlimWerte Werte { get; set; } = new();

    public ICollection<GlimNachwuchsTraining> Training { get; set; } = new List<GlimNachwuchsTraining>();
}

/// <summary>Trainingsfortschritt eines Kaderspielers je Wert (JS: tr = Steigerungen, prog = Fortschritt zur nächsten).</summary>
public class GlimTraining
{
    public int GlimSpielerId { get; set; }
    public GlimSpieler GlimSpieler { get; set; } = null!;

    /// <summary>wurf, schlag, tempo, fang, nerven.</summary>
    public string Wert { get; set; } = "";
    public int Steigerungen { get; set; }
    public int Fortschritt { get; set; }
}

/// <summary>Trainingsfortschritt eines Nachwuchsplatzes je Wert.</summary>
public class GlimNachwuchsTraining
{
    public int GlimNachwuchsId { get; set; }
    public GlimNachwuchs GlimNachwuchs { get; set; } = null!;
    public string Wert { get; set; } = "";
    public int Steigerungen { get; set; }
    public int Fortschritt { get; set; }
}

/// <summary>Gespieltes Spiel (JS: glim.spiele).</summary>
public class GlimSpiel : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string GegnerTeamCode { get; set; } = "";
    public AkademieTeam GegnerTeam { get; set; } = null!;

    /// <summary>Punkte Hesparis (JS: heim) und Gegner (JS: gast).</summary>
    public int PunkteHesparis { get; set; }
    public int PunkteGegner { get; set; }
    public DateOnly? Datum { get; set; }
    public bool Heimspiel { get; set; }

    /// <summary>Spielplan-Kennung, z. B. "hf1"; null = Freundschaftsspiel.</summary>
    public string? Fixture { get; set; }
    public int Saison { get; set; }

    /// <summary>"2. Spieltag", "Finale", "Freundschaftsspiel" …</summary>
    public string? Art { get; set; }
    public string? Notiz { get; set; }

    public ICollection<GlimSpielLogEintrag> Log { get; set; } = new List<GlimSpielLogEintrag>();
    public ICollection<GlimSpielStatistik> Statistik { get; set; } = new List<GlimSpielStatistik>();
}

/// <summary>Spielbericht-Zeile (JS: spiele.log).</summary>
public class GlimSpielLogEintrag
{
    public int Id { get; set; }
    public int GlimSpielId { get; set; }
    public GlimSpiel GlimSpiel { get; set; } = null!;

    /// <summary>0 = letzte Zeile des Spiels (das Frontend speichert neueste zuerst).</summary>
    public int Position { get; set; }
    public string Text { get; set; } = "";

    /// <summary>Zwischenüberschrift (JS: sep).</summary>
    public bool Trenner { get; set; }
    public int? Durchgang { get; set; }
    public string? Haelfte { get; set; }

    /// <summary>us / them.</summary>
    public string? Seite { get; set; }
    public bool? Aus { get; set; }

    /// <summary>Spielstand nach dieser Aktion, z. B. "3:2".</summary>
    public string? Spielstand { get; set; }
}

/// <summary>Statistik eines Spielers in einem Spiel (JS: spiele.stats).</summary>
public class GlimSpielStatistik
{
    public int Id { get; set; }
    public int GlimSpielId { get; set; }
    public GlimSpiel GlimSpiel { get; set; } = null!;
    public string Name { get; set; } = "";
    public int Treffer { get; set; }
    public int Punkte { get; set; }
    public int Abwuerfe { get; set; }
    public int Aus { get; set; }
}

/// <summary>Abgeschlossener Cup (JS: glim.archiv).</summary>
public class GlimCupArchiv : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public int CupNr { get; set; }
    public string Name { get; set; } = "";
    public string? MeisterTeamCode { get; set; }
    public AkademieTeam? MeisterTeam { get; set; }
    public int? Platz { get; set; }
    public int Siege { get; set; }
    public int Niederlagen { get; set; }
    public string? SpielerDesCups { get; set; }
}

/// <summary>Aktuelle Stärke einer gegnerischen Akademie (JS: glim.wert[team]).</summary>
public class GlimTeamStaerke : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string TeamCode { get; set; } = "";
    public AkademieTeam Team { get; set; } = null!;
    public decimal Wert { get; set; }
}

/// <summary>Ergebnis eines Ligaspiels im laufenden Cup (JS: glim.saison.res[fixture]).</summary>
public class GlimSaisonErgebnis : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string Fixture { get; set; } = "";
    public int PunkteHeim { get; set; }
    public int PunkteGast { get; set; }

    /// <summary>Ohne Spielfiguren ausgewürfelt (JS: sim).</summary>
    public bool Simuliert { get; set; }
}

/// <summary>K.-o.-Paarung im laufenden Cup (JS: glim.saison.po: hf1, hf2, fin).</summary>
public class GlimPlayoff : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string Runde { get; set; } = "";
    public string HeimTeamCode { get; set; } = "";
    public AkademieTeam HeimTeam { get; set; } = null!;
    public string GastTeamCode { get; set; } = "";
    public AkademieTeam GastTeam { get; set; } = null!;
}

/// <summary>Saisonstatistik je Person (JS: glim.plus[name]).</summary>
public class GlimSaisonStatistik : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string Name { get; set; } = "";
    public int Treffer { get; set; }
    public int Punkte { get; set; }
    public int Abwuerfe { get; set; }
}

/// <summary>Laufbahn je Person, für das Kapitänsamt (JS: glim.karriere[name]).</summary>
public class GlimKarriere : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string Name { get; set; } = "";
    public int Spiele { get; set; }
    public int Siege { get; set; }

    public ICollection<GlimKarriereCup> Cups { get; set; } = new List<GlimKarriereCup>();
}

/// <summary>Einsätze in einem Cup (JS: karriere.cups[nr]).</summary>
public class GlimKarriereCup
{
    public int GlimKarriereId { get; set; }
    public GlimKarriere GlimKarriere { get; set; } = null!;
    public int CupNr { get; set; }
    public int Einsaetze { get; set; }
}

/// <summary>Gewünschte Startaufstellung (JS: glimAuf[slot] = mitglied).</summary>
public class GlimAufstellung : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string Slot { get; set; } = "";
    public string Mitglied { get; set; } = "";
}

/// <summary>Ersatzbank (JS: glimBank).</summary>
public class GlimBankplatz : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public int Position { get; set; }
    public string Mitglied { get; set; } = "";
}

/// <summary>Trikotnummer (JS: glimNr[mitglied]).</summary>
public class GlimTrikotnummer : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string Mitglied { get; set; } = "";
    public int Nummer { get; set; }
}

/// <summary>Verletzung: so viele Spiele fällt die Person noch aus (JS: glimVerl[name]).</summary>
public class GlimVerletzung : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string Name { get; set; } = "";
    public int Spiele { get; set; }
}
