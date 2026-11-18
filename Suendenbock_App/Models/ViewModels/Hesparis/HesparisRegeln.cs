using System.Text.RegularExpressions;
using Suendenbock_App.Models.Domain.Hesparis;

namespace Suendenbock_App.Models.ViewModels.Hesparis;

/// <summary>
/// Spielregeln aus der hesparis.html (JavaScript) nach C# übertragen.
/// Die Namen der JS-Funktionen stehen jeweils dabei, damit man sie im Original wiederfindet.
/// </summary>
public static class HesparisRegeln
{
    // ---------------------------------------------------------------- Talente

    /// <summary>Punkte, die auf alle Talente verteilt werden dürfen (JS: TALENT_BUDGET).</summary>
    public const int TalentBudget = 650;

    /// <summary>Farben der Begabungen (JS: BEGABUNGEN[i].color). Namen und Talente kommen aus der Datenbank.</summary>
    public static readonly Dictionary<string, string> BegabungFarbe = new()
    {
        ["h"] = "#d27aa8",
        ["w"] = "#4fa3e8",
        ["s"] = "#cf9a68",
    };

    /// <summary>Kürzel aus den Magiestufen, die direkt auf ein Talent gehen (JS: CODE_TALENT).</summary>
    private static readonly Dictionary<string, string> CodeTalent = new()
    {
        ["AUR"] = "h8",
        ["SIL"] = "h7",
        ["MAGW"] = "h1",
        ["HAPW"] = "h0",
    };

    /// <summary>"+TL" eines Fachs geht auf dieses Talent, sonst frei wählbar (JS: FACH_TL).</summary>
    private static readonly Dictionary<string, string> FachTalent = new()
    {
        ["Ätherfechten"] = "h9",
    };

    /// <summary>Tascheninhalt dieser Arten wirkt, solange man ihn dabeihat (JS: FX_PASSIV).</summary>
    public static readonly string[] PassiveKategorien = { "Buch", "Ausrüstung", "Sonstiges" };

    /// <summary>Arten in der Tasche (JS: ITEM_CATS).</summary>
    public static readonly string[] Kategorien = { "Ausrüstung", "Trank", "Zutat", "Buch", "Süßes", "Sonstiges" };

    /// <summary>Fach-Id von Chronotaktik: ab Stufe 3 sind Meisterschaften freigeschaltet (JS: mstUnlocked).</summary>
    public const int FachChronotaktik = 5;

    /// <summary>Rundet wie JavaScript Math.round (bei ,5 immer nach oben, auch bei negativen Zahlen).</summary>
    public static int JsRunden(double wert) => (int)Math.Floor(wert + 0.5);

    /// <summary>Kopfwert einer Begabung: je 10 Punkte ein Punkt (JS: talCalc → kw).</summary>
    public static int Kopfwert(int summe) => JsRunden(summe / 10.0);

    /// <summary>Geistesblitze: je 10 Kopfwert ein Geistesblitz (JS: talCalc → gbp).</summary>
    public static int Geistesblitze(int kopfwert) => JsRunden(kopfwert / 10.0);

    /// <summary>Wie viele Meisterschafts-Schwellen erreicht sind: 100, 150, 200, 250 (JS: mstReached).</summary>
    public static int MeisterschaftStufe(int gesamt) =>
        gesamt >= 250 ? 4 : gesamt >= 200 ? 3 : gesamt >= 150 ? 2 : gesamt >= 100 ? 1 : 0;

    /// <summary>Wert auf dem Bogen: je erreichter Schwelle 50 weniger (JS: talShown).</summary>
    public static int WertAufDemBogen(int gesamt) => gesamt - 50 * MeisterschaftStufe(gesamt);

    /// <summary>Punkte werden auf 5er gerundet und begrenzt (JS: Eingabe im Talentbogen).</summary>
    public static int PunkteNormalisieren(int punkte)
    {
        int gerundet = JsRunden(punkte / 5.0) * 5;
        return Math.Clamp(gerundet, -995, 995);
    }

    private static readonly Regex StufenBonusMuster = new(@"\+(\d+)\s*([A-ZÄÖÜ]+(?:/[A-ZÄÖÜ]+)*)", RegexOptions.Compiled);

    /// <summary>
    /// Talentboni aus den Magiestufen (JS: fachTalentBoni). Liest den Text der Stufe bzw. die gewählte
    /// Belohnung und sucht Muster wie "+10 TL" oder "+5 AUR/SIL".
    /// </summary>
    public static List<TalentBonus> FachBoni(Spielfigur figur, IEnumerable<Fach> faecher)
    {
        var boni = new List<TalentBonus>();

        foreach (var fach in faecher.OrderBy(f => f.Id))
        {
            int stufe = figur.FachStufen.FirstOrDefault(s => s.FachId == fach.Id)?.Stufe ?? 0;
            for (int t = 1; t <= stufe; t++)
            {
                string k = fach.Id + "-" + t;
                string text = figur.StufenWahlen.FirstOrDefault(w => w.FachId == fach.Id && w.Stufe == t)?.Wahl
                              ?? fach.Stufen.FirstOrDefault(s => s.Stufe == t)?.Beschreibung
                              ?? "";
                if (text.Contains(" ODER ")) continue;

                int n = 0;
                foreach (Match m in StufenBonusMuster.Matches(text))
                {
                    int betrag = int.Parse(m.Groups[1].Value);
                    var kandidaten = new List<string>();
                    foreach (var kuerzel in m.Groups[2].Value.Split('/'))
                    {
                        if (kuerzel == "TL")
                        {
                            if (FachTalent.TryGetValue(fach.Name, out var festesTalent))
                                kandidaten.Add(festesTalent);
                            else
                                kandidaten.AddRange(AlleTalentCodes);
                        }
                        else if (CodeTalent.TryGetValue(kuerzel, out var code))
                        {
                            kandidaten.Add(code);
                        }
                    }
                    kandidaten = kandidaten.Distinct().ToList();
                    if (kandidaten.Count == 0) continue;

                    string schluessel = k + "#" + n++;
                    string? gewaehlt = figur.TalentZiele.FirstOrDefault(z => z.Schluessel == schluessel)?.TalentCode;
                    string? ziel;
                    if (kandidaten.Count == 1)
                        ziel = kandidaten[0];
                    else if (gewaehlt != null && kandidaten.Contains(gewaehlt))
                        ziel = gewaehlt;
                    else
                        ziel = null;

                    boni.Add(new TalentBonus
                    {
                        Schluessel = schluessel,
                        Quelle = fach.Name,
                        Herkunft = "Stufe " + t,
                        Betrag = betrag,
                        Beschriftung = m.Value,
                        Kandidaten = kandidaten,
                        Ziel = ziel,
                    });
                }
            }
        }
        return boni;
    }

    /// <summary>Alle 30 Talent-Codes h0…s9 (für "+TL" ohne festes Talent).</summary>
    public static readonly string[] AlleTalentCodes =
        new[] { "h", "w", "s" }.SelectMany(b => Enumerable.Range(0, 10).Select(i => b + i)).ToArray();

    /// <summary>Hat der Effekt überhaupt eine Wirkung? (JS: fxOk)</summary>
    public static bool EffektGueltig(Effekt? fx) =>
        fx != null && (
            (fx.Art == EffektArt.Talent && !string.IsNullOrEmpty(fx.TalentCode) && fx.Wert.HasValue && fx.Wert != 0) ||
            (fx.Art == EffektArt.Vorteil && !string.IsNullOrWhiteSpace(fx.VorteilAuf)));

    /// <summary>Ist eine Wirkung am aktuellen Spieltag noch aktiv? (JS: wirkAus, umgedreht)</summary>
    public static bool WirkungAktiv(Wirkung w, DateOnly spieltag)
    {
        if (!EffektGueltig(w.Effekt)) return false;
        if (w.Effekt.DauerArt == EffektDauerArt.Tage)
        {
            int tage = spieltag.DayNumber - (w.Ab ?? spieltag).DayNumber;
            return tage >= 0 && tage < (w.Effekt.DauerAnzahl ?? 1);
        }
        if (w.Effekt.DauerArt == EffektDauerArt.Anwendungen)
            return (w.Restanwendungen ?? 1) > 0;
        return true;
    }

    /// <summary>Restdauer als Text (JS: wirkRest).</summary>
    public static string WirkungRest(Wirkung w, DateOnly spieltag)
    {
        if (w.Effekt.DauerArt == EffektDauerArt.Tage)
        {
            int rest = (w.Effekt.DauerAnzahl ?? 1) - (spieltag.DayNumber - (w.Ab ?? spieltag).DayNumber);
            return rest <= 1 ? "nur noch heute" : "noch " + rest + " Tage";
        }
        if (w.Effekt.DauerArt == EffektDauerArt.Anwendungen)
        {
            int rest = w.Restanwendungen ?? 1;
            return rest == 1 ? "noch 1 Anwendung" : "noch " + rest + " Anwendungen";
        }
        return w.Effekt.DauerText ?? "";
    }

    // ---------------------------------------------------------------- Magiestufen

    /// <summary>Abschnitte der Magiestufen (JS: RG_PHASES / SJ_ABS). Max = höchste Stufe in diesem Abschnitt.</summary>
    public static readonly (string Name, string Tor, string Schluessel, int Max)[] Abschnitte =
    {
        ("Unterstufe", "Hafenaufsicht", "hafen", 3),
        ("Mittelstufe", "Angerwacht", "anger", 7),
        ("Oberstufe", "Gartenhüter", "garten", 9),
        ("Domstufe", "Domwächter", "dom", 10),
    };

    /// <summary>Startpunkte für Magiestufen (JS: cap → 6).</summary>
    public const int StartPunkte = 6;

    /// <summary>Zusatzpunkte je Abschlussnote, in Zehnteln (JS: SJ_STAFFEL).</summary>
    private static readonly Dictionary<string, double> Staffel = new()
    {
        ["S"] = 2, ["A"] = 2, ["B"] = 1.5, ["C"] = 1, ["D"] = 0.5, ["E"] = 0,
    };

    /// <summary>Abschluss bestanden? Leer und "E" nicht (JS: sjPassed).</summary>
    public static bool Bestanden(string? wert) => !string.IsNullOrEmpty(wert) && wert != "E";

    /// <summary>
    /// Abschnitt (1–4), bis zu dem eine Figur Stufen wählen darf: jeder bestandene Abschluss
    /// (Hafen, Anger, Garten, in dieser Reihenfolge) öffnet den nächsten (JS: sjPhase).
    /// </summary>
    public static int Abschnitt(IEnumerable<SchuljahrAbschnitt> abschluesse)
    {
        var liste = abschluesse.ToList();
        int n = 0;
        foreach (var x in Abschnitte.Take(3))
        {
            if (Bestanden(liste.FirstOrDefault(a => a.Abschnitt == x.Schluessel)?.Wert)) n++;
            else break;
        }
        return n + 1;
    }

    /// <summary>Höchste Stufe, die im aktuellen Abschnitt erreichbar ist (JS: rgMax).</summary>
    public static int MaxStufe(int abschnitt) => Abschnitte[abschnitt - 1].Max;

    /// <summary>Freipunkte aus Abschlüssen in Zehnteln: je bestandener Abschluss 30 + Notenbonus (JS: sjAbsZ).</summary>
    public static int AbschlussZehntel(IEnumerable<SchuljahrAbschnitt> abschluesse) =>
        abschluesse
            .Where(a => Bestanden(a.Wert) && a.Wert != "alt")
            .Sum(a => 30 + JsRunden(Staffel.GetValueOrDefault(a.Wert, 0) * 10));

    /// <summary>
    /// Punkte für Magiestufen: 6 + Punkte der Spielleitung + je 10 Zehntel aus Abschlüssen einer (JS: cap).
    /// Der Anteil aus Protokollheften (JS: sjHeftZ) kommt mit dem Bereich "Spieltisch" dazu.
    /// </summary>
    public static int PunkteGesamt(Spielfigur figur, IEnumerable<SchuljahrAbschnitt> abschluesse) =>
        StartPunkte + figur.Bonuspunkte + AbschlussZehntel(abschluesse) / 10;

    /// <summary>
    /// Fachpunkte eines Fachs: je A oder S in den Abschlussprüfungen ein Punkt (JS: fachPunkte).
    /// Der Anteil aus Klassenarbeiten (JS: sjArbZ) kommt mit dem Bereich "Alltag" dazu.
    /// </summary>
    public static int Fachpunkte(Spielfigur figur, int fachId) =>
        figur.Pruefungen.Count(p =>
            FachpunktAbschnitte.Contains(p.Abschnitt) &&
            p.Teil == fachId.ToString() &&
            (p.Wert == "A" || p.Wert == "S"));

    public static int Stufe(Spielfigur figur, int fachId) =>
        figur.FachStufen.FirstOrDefault(s => s.FachId == fachId)?.Stufe ?? 0;

    /// <summary>Verbrauchte Punkte: Stufen über den Fachpunkten hinaus (JS: used).</summary>
    public static int PunkteVerbraucht(Spielfigur figur, IEnumerable<int> fachIds) =>
        fachIds.Sum(id => Math.Max(0, Stufe(figur, id) - Fachpunkte(figur, id)));

    /// <summary>Darf diese Stufe jetzt gewählt werden? (JS: canBuy)</summary>
    public static bool KannWaehlen(Spielfigur figur, int fachId, int stufe, int maxStufe, int punkteFrei)
    {
        if (stufe != Stufe(figur, fachId) + 1 || stufe > maxStufe) return false;
        bool fachpunktFrei = Fachpunkte(figur, fachId) - Stufe(figur, fachId) > 0;
        return fachpunktFrei || punkteFrei > 0;
    }

    /// <summary>Abkürzungen in den Stufentexten (JS: LEGEND).</summary>
    public static readonly (string Kurz, string Lang)[] Abkuerzungen =
    {
        ("TL", "Talentwert"), ("SEP", "Sepsiswert"), ("SIL", "Silizieren/Bannen"), ("HLF", "Helfen"),
        ("MAGW", "Magische Wahrnehmung"), ("FD", "Flatdamage"), ("RES", "Resistenz"), ("REA", "Reaktion"),
        ("LP", "Lebenspunkte"), ("HAPW", "Haptische Wahrnehmung"), ("MSW", "Meisterschaftswürfel"),
        ("AUR", "Auritieren/Bündeln"), ("RW", "Reichweite"), ("RÜW", "Rüstwert"),
    };

    // ---------------------------------------------------------------- Noten

    /// <summary>Zahlenwert einer Note, S ist die Sondernote (JS: NOTE_VAL).</summary>
    public static double NotenWert(Note note) => note switch
    {
        Note.S => 0.9,
        Note.A => 1,
        Note.B => 2,
        Note.C => 3,
        Note.D => 4,
        _ => 5,
    };

    /// <summary>Durchschnitt, null wenn noch keine Note da ist (JS: noteAvg).</summary>
    public static double? Schnitt(IEnumerable<Note> noten)
    {
        var liste = noten.ToList();
        if (liste.Count == 0) return null;
        return liste.Average(NotenWert);
    }

    /// <summary>Durchschnitt als Buchstabe (JS: noteLetter).</summary>
    public static string NotenBuchstabe(double wert)
    {
        if (wert < 0.95) return "S";
        int stufe = Math.Min(5, Math.Max(1, JsRunden(wert)));
        return "ABCDE"[stufe - 1].ToString();
    }

    /// <summary>Durchschnitt mit einer Nachkommastelle und Komma (JS: noteFmt).</summary>
    public static string NotenFormat(double wert) =>
        (Math.Floor(wert * 10 + 0.5 + 1e-9) / 10).ToString("0.0", System.Globalization.CultureInfo.GetCultureInfo("de-DE"));

    /// <summary>Prüfungsabschnitte, deren A/S je Fach einen Fachpunkt bringen (JS: fachPunkte).</summary>
    public static readonly string[] FachpunktAbschnitte = { "hafen", "anger", "garten" };

    // ---------------------------------------------------------------- Geld

    /// <summary>Münzen, Wert in Kerzen (JS: COINS).</summary>
    public static readonly (string Name, int Wert)[] Muenzen =
    {
        ("Hiro", 1000),
        ("Nimbus", 100),
        ("Lumen", 10),
        ("Kerze", 1),
    };

    /// <summary>Betrag in Kerzen auf Münzen aufteilen (JS: moneySplit).</summary>
    public static List<(string Name, int Anzahl)> MuenzenAufteilen(int kerzen)
    {
        var ergebnis = new List<(string, int)>();
        int rest = Math.Max(0, kerzen);
        foreach (var (name, wert) in Muenzen)
        {
            ergebnis.Add((name, rest / wert));
            rest %= wert;
        }
        return ergebnis;
    }

    /// <summary>Kurztext wie "2 Hiro, 3 Lumen" (nur Münzen mit Anzahl &gt; 0).</summary>
    public static string GeldText(int kerzen)
    {
        if (kerzen == 0) return "0 Kerzen";
        var teile = MuenzenAufteilen(Math.Abs(kerzen)).Where(m => m.Anzahl > 0).Select(m => m.Anzahl + " " + m.Name);
        return (kerzen < 0 ? "−" : "") + string.Join(", ", teile);
    }

    /// <summary>Wie viele Geldbuchungen aufgehoben werden (JS: purseLog.slice(0, 5)).</summary>
    public const int GeldbuchungenMax = 5;
}

/// <summary>Ein Bonus auf ein Talent, egal woher (Magiestufe, AG, Tasche, Wirkung).</summary>
public class TalentBonus
{
    /// <summary>Bei Magiestufen der Schlüssel "Fach-Stufe#n" für die Talentwahl, sonst leer.</summary>
    public string Schluessel { get; set; } = "";

    /// <summary>Fach, AG oder Gegenstand, von dem der Bonus kommt.</summary>
    public string Quelle { get; set; } = "";

    /// <summary>"Stufe 3", "AG", "in der Tasche", "wirkt gerade".</summary>
    public string Herkunft { get; set; } = "";
    public int Betrag { get; set; }
    public string Beschriftung { get; set; } = "";

    /// <summary>Mögliche Talente; mehr als eins = Spieler muss wählen.</summary>
    public List<string> Kandidaten { get; set; } = new();
    public string? Ziel { get; set; }

    /// <summary>Restdauer bei Wirkungen.</summary>
    public string? Dauer { get; set; }

    /// <summary>Id der Wirkung, damit man sie beenden kann.</summary>
    public int? WirkungId { get; set; }
    public bool Anwendungen { get; set; }
}
