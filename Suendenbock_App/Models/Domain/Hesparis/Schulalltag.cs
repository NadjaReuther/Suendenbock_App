namespace Suendenbock_App.Models.Domain.Hesparis;

/// <summary>Kalendereintrag (JS: kal.events).</summary>
public class KalenderTermin : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public DateOnly Datum { get; set; }
    public string Titel { get; set; } = "";
    public Termintyp Typ { get; set; }

    /// <summary>Vom System angelegt (JS: auto); das ursprüngliche Datum, falls verschoben.</summary>
    public DateOnly? AutomatischAm { get; set; }

    /// <summary>Monat der Schülerrat-Sitzung "yyyy-MM" (JS: rat), nur bei Typ = Rat.</summary>
    public string? RatMonat { get; set; }
}

/// <summary>AG in dieser Kampagne mit Notiz der Gruppe (JS: ags[code].notiz).</summary>
public class AgEintrag : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string AgCode { get; set; } = "";
    public Ag Ag { get; set; } = null!;
    public string? Notiz { get; set; }

    public ICollection<AgMitglied> Mitglieder { get; set; } = new List<AgMitglied>();
}

/// <summary>Spielfigur in einer AG (JS: ags[code].mitglieder).</summary>
public class AgMitglied
{
    public int AgEintragId { get; set; }
    public AgEintrag AgEintrag { get; set; } = null!;
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;

    /// <summary>Nur Lernzirkel: gewählter Wochentag (JS: ags.lernzirkel.tage[spieler]).</summary>
    public int? Wochentag { get; set; }
}

/// <summary>Klassenarbeit (JS: arbeiten).</summary>
public class Klassenarbeit : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public int FachId { get; set; }
    public Fach Fach { get; set; } = null!;
    public DateOnly Datum { get; set; }

    /// <summary>Schuljahr, in dem geschrieben wurde (JS: sj).</summary>
    public int? Schuljahr { get; set; }

    /// <summary>Stimmung/Schwierigkeit −2 … +2 (JS: m).</summary>
    public int Stimmung { get; set; }

    public ICollection<KlassenarbeitSpielfigur> Spielfiguren { get; set; } = new List<KlassenarbeitSpielfigur>();
    public ICollection<KlassenarbeitMitschueler> Mitschueler { get; set; } = new List<KlassenarbeitMitschueler>();
}

/// <summary>Ergebnis einer Spielfigur (JS: pl) oder Nachschreiber (JS: nach).</summary>
public class KlassenarbeitSpielfigur
{
    public int KlassenarbeitId { get; set; }
    public Klassenarbeit Klassenarbeit { get; set; } = null!;
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;

    /// <summary>null, wenn die Figur noch nachschreiben muss.</summary>
    public Note? Note { get; set; }
    public bool MussNachschreiben { get; set; }
}

public class KlassenarbeitMitschueler
{
    public int KlassenarbeitId { get; set; }
    public Klassenarbeit Klassenarbeit { get; set; } = null!;
    public int MitschuelerId { get; set; }
    public Mitschueler Mitschueler { get; set; } = null!;
    public Note Note { get; set; }
}

/// <summary>Nachsitzen (JS: nachsitzen).</summary>
public class Nachsitzen : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;

    /// <summary>Verhängende Lehrkraft, falls in der Lehrerliste vorhanden.</summary>
    public int? LehrerId { get; set; }
    public Lehrer? Lehrer { get; set; }

    /// <summary>Name wie eingetragen (auch für Lehrkräfte, die nicht in der Liste stehen).</summary>
    public string LehrerName { get; set; } = "";
    public string Grund { get; set; } = "";
    public string Strafe { get; set; } = "";
    public string? DatumText { get; set; }
    public DateTime ErstelltAm { get; set; }

    /// <summary>Mission, in der das Nachsitzen abgeleistet wird.</summary>
    public int? MissionId { get; set; }
    public Mission? Mission { get; set; }
    public bool Verfallen { get; set; }
}

/// <summary>Ergebnis eines Prüfungsabschnitts im Schuljahr (JS: sj.abs[spieler][abschnitt]).</summary>
public class SchuljahrAbschnitt
{
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;

    /// <summary>hafen, anger, garten, dom.</summary>
    public string Abschnitt { get; set; } = "";

    /// <summary>Ergebnis, z. B. Note oder "alt" (vor der Umstellung bestanden).</summary>
    public string Wert { get; set; } = "";
}

/// <summary>Abgeschlossenes Schuljahr (JS: sj.hist).</summary>
public class SchuljahrBilanz : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public int Jahr { get; set; }
    public DateOnly Datum { get; set; }

    public ICollection<SchuljahrBilanzSpielfigur> Spielfiguren { get; set; } = new List<SchuljahrBilanzSpielfigur>();
    public ICollection<SchuljahrZeugnisnote> Zeugnis { get; set; } = new List<SchuljahrZeugnisnote>();
}

public class SchuljahrBilanzSpielfigur
{
    public int SchuljahrBilanzId { get; set; }
    public SchuljahrBilanz SchuljahrBilanz { get; set; } = null!;
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;

    /// <summary>Fach-, Haltungs- und AG-Zusatzpunkte (JS: fz, hz, az).</summary>
    public decimal? Fachpunkte { get; set; }
    public decimal? Haltungspunkte { get; set; }
    public decimal? AgPunkte { get; set; }
    public string? Abschnitt { get; set; }
    public string? Note { get; set; }
    public decimal? Schnitt { get; set; }
}

public class SchuljahrZeugnisnote
{
    public int SchuljahrBilanzId { get; set; }
    public SchuljahrBilanz SchuljahrBilanz { get; set; } = null!;
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;
    public int FachId { get; set; }
    public Fach Fach { get; set; } = null!;
    public decimal Schnitt { get; set; }
}

/// <summary>Notiz der Gruppe zu einem Ort auf der Schulkarte (JS: karte[ort]).</summary>
public class KartenNotiz : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;

    /// <summary>Ort-Kennung der Karte, z. B. "r029".</summary>
    public string Ort { get; set; } = "";
    public string Text { get; set; } = "";
}

/// <summary>Uhrzeit-Eintrag auf der Karte (JS: karteZeiten[key]).</summary>
public class KartenZeit : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string Schluessel { get; set; } = "";
    public string Wert { get; set; } = "";
}

/// <summary>Eintrag im Speiseplan einer Woche (JS: ess.weeks[montag][tag][slot]).</summary>
public class SpeiseplanEintrag : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;

    /// <summary>Montag der Woche.</summary>
    public DateOnly Woche { get; set; }

    /// <summary>0 = Montag … 6 = Sonntag.</summary>
    public int Wochentag { get; set; }
    public Mahlzeit Mahlzeit { get; set; }
    public string GerichtCode { get; set; } = "";
    public Gericht Gericht { get; set; } = null!;
}

/// <summary>Gekauftes Essen (JS: ess.bought[datum][spieler][slot]).</summary>
public class Essenskauf
{
    public int Id { get; set; }
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;
    public DateOnly Datum { get; set; }
    public Mahlzeit Mahlzeit { get; set; }
    public string GerichtCode { get; set; } = "";
    public Gericht Gericht { get; set; } = null!;
}

/// <summary>Wirkung eines Gerichts (JS: ess.buffs).</summary>
public class Essenswirkung
{
    public int Id { get; set; }
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;
    public DateOnly Datum { get; set; }
    public string Gericht { get; set; } = "";
    public string? Wirkung { get; set; }
}

/// <summary>Eintrag im Spielprotokoll der Spielleitung (JS: protokoll).</summary>
public class ProtokollEintrag : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string Titel { get; set; } = "";
    public string? Text { get; set; }
    public DateTime ErstelltAm { get; set; }

    /// <summary>Spieltag, an dem es passiert ist (JS: spiel).</summary>
    public DateOnly? Spieltag { get; set; }

    public ICollection<ProtokollBeteiligter> Beteiligte { get; set; } = new List<ProtokollBeteiligter>();
}

public class ProtokollBeteiligter
{
    public int ProtokollEintragId { get; set; }
    public ProtokollEintrag ProtokollEintrag { get; set; } = null!;
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;
}

/// <summary>To-do der Spielleitung (JS: todos).</summary>
public class Aufgabe : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string Text { get; set; } = "";
    public Prioritaet Prioritaet { get; set; } = Prioritaet.Normal;
    public bool Erledigt { get; set; }
    public DateTime ErstelltAm { get; set; }
}

/// <summary>Mission/Auftrag der Gruppe (JS: missionen).</summary>
public class Mission : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public int Nummer { get; set; }
    public string Titel { get; set; } = "";
    public string? Ziel { get; set; }

    /// <summary>z. B. aktiv, erledigt.</summary>
    public string Status { get; set; } = "aktiv";
    public string? Notiz { get; set; }
    public string? StartText { get; set; }

    public ICollection<MissionHinweis> Hinweise { get; set; } = new List<MissionHinweis>();
}

public class MissionHinweis
{
    public int Id { get; set; }
    public int MissionId { get; set; }
    public Mission Mission { get; set; } = null!;
    public int Position { get; set; }
    public string Text { get; set; } = "";
    public bool Erledigt { get; set; }
}
