using Suendenbock_App.Models.Domain.Hesparis;

namespace Suendenbock_App.Models.ViewModels.Hesparis;

// =====================================================================
// Schuljahr · Protokollhefte (JS: MODS.hefte)
// =====================================================================

/// <summary>Teammitglied eines Hefts: Spielfigur ("f12") oder der Mitschüler als +1 ("m7") (JS: phMembers).</summary>
public class HeftMitglied
{
    public string Schluessel { get; set; } = "";
    public int? SpielfigurId { get; set; }
    public int? MitschuelerId { get; set; }

    /// <summary>Voller Name (JS: phFull bzw. st.name).</summary>
    public string Name { get; set; } = "";

    /// <summary>Rufname (JS: short).</summary>
    public string Kurz { get; set; } = "";
    public string Farbe { get; set; } = "#22306a";
    public bool AusDerKlasse => MitschuelerId.HasValue;
}

/// <summary>
/// Ein Heft mit allem, was die Seiten brauchen, aus der Datenbank ausgerechnet
/// (JS: phMembers, phSecDone, phDone, phNext, phStatus, phDue …).
/// </summary>
public class HeftAnsicht
{
    public Protokollheft Heft { get; }
    public DateOnly Heute { get; }

    /// <summary>Alle Spielfiguren der Akademie in Reihenfolge (auch die, die fehlen).</summary>
    public List<HeftMitglied> Figuren { get; }

    /// <summary>Wer mitmacht: anwesende Spielfiguren plus der Mitschüler.</summary>
    public List<HeftMitglied> Team { get; }
    public HeftMitglied? Begleiter { get; }
    public HeftMitglied? Schreiber { get; }

    public HeftAnsicht(Protokollheft heft, List<HeftMitglied> figuren, HeftMitglied? begleiter, DateOnly heute)
    {
        Heft = heft;
        Heute = heute;
        Figuren = figuren;
        var fehlend = heft.Fehlend.Select(f => f.SpielfigurId).ToHashSet();
        Team = figuren.Where(f => !fehlend.Contains(f.SpielfigurId!.Value)).ToList();
        Begleiter = begleiter;
        if (begleiter != null) Team.Add(begleiter);
        Schreiber = Team.FirstOrDefault(m =>
            (heft.SchreiberSpielfigurId.HasValue && m.SpielfigurId == heft.SchreiberSpielfigurId) ||
            (heft.SchreiberMitschuelerId.HasValue && m.MitschuelerId == heft.SchreiberMitschuelerId));
    }

    public bool Fehlt(int spielfigurId) => Heft.Fehlend.Any(f => f.SpielfigurId == spielfigurId);

    public string Titel => string.IsNullOrWhiteSpace(Heft.Titel) ? "ohne Titel" : Heft.Titel;

    public string? Text(string abschnitt) => Heft.Abschnitte.FirstOrDefault(a => a.Abschnitt == abschnitt)?.Text;

    public string? Jeder(string schluessel) => Heft.JederEintraege.FirstOrDefault(j => j.Schluessel == schluessel)?.Text;

    public bool Frage(string abschnitt, int position) =>
        Heft.Fragen.Any(f => f.Abschnitt == abschnitt && f.Position == position && f.Beantwortet);

    public bool Check(string schluessel) => Heft.Checks.Any(c => c.Schluessel == schluessel && c.Erledigt);

    public List<HeftAblauf> AblaufSortiert => Heft.Ablauf
        .OrderBy(r => (r.Datum?.ToString("yyyy-MM-dd") ?? "") + (r.Uhrzeit ?? ""), StringComparer.Ordinal)
        .ThenBy(r => r.Position).ThenBy(r => r.Id).ToList();

    public List<HeftNotiz> NotizenSortiert => Heft.Notizen.OrderBy(n => n.Position).ThenBy(n => n.Id).ToList();

    public List<HeftPerson> PersonenSortiert => Heft.Personen.OrderBy(p => p.Position).ThenBy(p => p.Id).ToList();

    /// <summary>Abschnitt erfüllt? (JS: phSecDone)</summary>
    public bool Erfuellt(ProtokollheftRegeln.Abschnitt s) => s.Art switch
    {
        "kopf" => !string.IsNullOrWhiteSpace(Heft.Titel) && !string.IsNullOrWhiteSpace(Heft.Lehrer)
                  && !string.IsNullOrWhiteSpace(Heft.Ort) && Heft.Von.HasValue,
        "ablauf" => AblaufEintraege >= s.Minimum,
        "personen" => Steckbriefe >= s.Minimum,
        "jeder" => Team.Count > 0 && Team.All(m => ProtokollheftRegeln.Woerter(Jeder(m.Schluessel)) >= s.Minimum),
        _ => ProtokollheftRegeln.Woerter(Text(s.Schluessel)) >= s.Minimum,
    };

    public int AblaufEintraege => Heft.Ablauf.Count(r => !string.IsNullOrWhiteSpace(r.Text));

    public int Steckbriefe => Heft.Personen.Count(p => !string.IsNullOrWhiteSpace(p.Name)
                                                       && (!string.IsNullOrWhiteSpace(p.Sagt) || !string.IsNullOrWhiteSpace(p.Eindruck)));

    /// <summary>Erfüllte Abschnitte (JS: phDone).</summary>
    public int Fertig => ProtokollheftRegeln.Abschnitte.Count(Erfuellt);

    /// <summary>Erster offener Abschnitt, -1 wenn alles erfüllt ist (JS: phNext).</summary>
    public int Naechster => Array.FindIndex(ProtokollheftRegeln.Abschnitte, s => !Erfuellt(s));

    /// <summary>neu, unterwegs, abgegeben oder benotet (JS: phStatus).</summary>
    public string Status
    {
        get
        {
            if (!string.IsNullOrEmpty(Heft.Note)) return "benotet";
            if (Heft.AbgegebenAm.HasValue) return "abgegeben";
            return Fertig > 1 || Heft.Notizen.Count > 0 ? "unterwegs" : "neu";
        }
    }

    public string Umschlag => ProtokollheftRegeln.Umschlag(Heft.Nummer);

    /// <summary>"30. Oktober – 31. Oktober" bzw. ein Tag (JS: phRange).</summary>
    public string Zeitraum
    {
        get
        {
            if (!Heft.Von.HasValue) return "–";
            var von = HesparisRegeln.TagUndMonat(Heft.Von.Value);
            return Heft.Bis.HasValue && Heft.Bis != Heft.Von ? von + " – " + HesparisRegeln.TagUndMonat(Heft.Bis.Value) : von;
        }
    }

    public static string Datum(DateOnly? d) => d.HasValue ? HesparisRegeln.TagUndMonat(d.Value) : "–";

    public string AbgegebenText => Heft.AbgegebenAm.HasValue ? Datum(DateOnly.FromDateTime(Heft.AbgegebenAm.Value)) : "";

    /// <summary>Tage bis zur Abgabe, negativ = überfällig.</summary>
    public int? TageBisAbgabe => Heft.Abgabe.HasValue ? Heft.Abgabe.Value.DayNumber - Heute.DayNumber : null;

    /// <summary>"Abgabe in 3 Tagen" / "Abgabe heute!" / "Abgabe 2 Tage überfällig", leer wenn erledigt (JS: phDue).</summary>
    public string? AbgabeHinweis
    {
        get
        {
            if (!string.IsNullOrEmpty(Heft.Note) || Heft.AbgegebenAm.HasValue || TageBisAbgabe is not int d) return null;
            if (d < 0) return "Abgabe " + (-d) + " Tag" + (d == -1 ? "" : "e") + " überfällig";
            if (d == 0) return "Abgabe heute!";
            return "Abgabe in " + d + " Tag" + (d == 1 ? "" : "en");
        }
    }

    public bool AbgabeUeberfaellig => TageBisAbgabe < 0;
}

/// <summary>Zeile der Klassenliste rechts am Deckblatt (JS: mates in phDeck).</summary>
public class HeftKlassenZeile
{
    /// <summary>"m7" = vorhandener Mitschüler-Eintrag, "c12" = Mitschüler-Charakter ohne Eintrag (wird beim Wählen angelegt).</summary>
    public string Wert { get; set; } = "";
    public string Name { get; set; } = "";
    public int WarDabei { get; set; }
    public bool Unterwegs { get; set; }
    public int Herzen { get; set; }
    public bool Gewaehlt { get; set; }
}

public class HeftLehrkraft
{
    public string Name { get; set; } = "";
    public string? Faecher { get; set; }
}

/// <summary>Daten für alle Heftseiten. Was gebraucht wird, hängt von der Seite ab.</summary>
public class ProtokollheftViewModel
{
    public HeftAnsicht? Heft { get; set; }

    /// <summary>Aufgeschlagener Abschnitt (0–11) auf der Abschnitt-Seite.</summary>
    public int Abschnitt { get; set; }

    /// <summary>Was gerade frisch abgehakt/gestempelt wurde (JS: HS.fresh), z. B. "q|vorb|2", "abgabe", "note".</summary>
    public string? Frisch { get; set; }

    /// <summary>Beim Hereinkommen animieren (JS: HS.anim): beim Seitenwechsel ja, nach einem Klick auf derselben Seite nein.</summary>
    public bool Animieren { get; set; } = true;

    public bool DarfBearbeiten { get; set; }
    public bool IstSpielleitung { get; set; }

    // ---- Regal
    public List<HeftAnsicht> Regal { get; set; } = new();
    public HeftAnsicht? NaechsteAbgabe { get; set; }
    public double? Gruppenschnitt { get; set; }
    public List<(string Name, int Mal)> SchonDabei { get; set; } = new();
    public int NochNieDabei { get; set; }

    // ---- Deckblatt
    public List<HeftLehrkraft> Lehrkraefte { get; set; } = new();
    public List<HeftKlassenZeile> Klasse { get; set; } = new();

    /// <summary>Vorschlag für den Schriftführer und wie oft er es schon war (JS: sugg).</summary>
    public HeftMitglied? Vorschlag { get; set; }
    public int VorschlagMal { get; set; }

    // ---- Abgabe
    /// <summary>Freipunkte (Zehntel) für alle bei dieser Note.</summary>
    public int NotenZehntel { get; set; }

    /// <summary>Das ganze Heft als Text zum Kopieren (JS: phText).</summary>
    public string? AlsText { get; set; }
}
