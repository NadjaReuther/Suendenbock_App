using System.Globalization;
using System.Text.RegularExpressions;
using Suendenbock_App.Models.Domain.Hesparis;

namespace Suendenbock_App.Models.ViewModels.Hesparis;

/// <summary>
/// Ätherwaffen aus der hesparis.html (JS: WAFFEN, WAFFEN_LABEL …). Noch keine feste Waffe:
/// Bei jedem Zauber formt der Zufall den Stab neu. Die Formen sind Spielregeln und stehen deshalb im Code.
/// </summary>
public static class WaffenRegeln
{
    public record Waffe(string Name, string Schaden, string? Notiz);

    public record Kategorie(Waffenart Art, string Code, string Name, string Beschreibung, string Symbol);

    public static readonly Kategorie[] Kategorien =
    {
        new(Waffenart.Nah, "nah", "Nahkampf", "Klingen, Hämmer, Sensen", "<svg class=\"ico\" viewBox=\"0 0 24 24\" aria-hidden=\"true\"><path d=\"M14.5 4.5L20 4l-.5 5.5-8.5 8.5-3-3z\"/><path d=\"M6 13l5 5\"/><path d=\"M8.5 15.5L4 20\"/></svg>"),
        new(Waffenart.Fern, "fern", "Fernkampf", "Pfeile, Kugeln, Blitze", "<svg class=\"ico\" viewBox=\"0 0 24 24\" aria-hidden=\"true\"><path d=\"M7 3c5 2.5 7 5.5 7 9s-2 6.5-7 9\"/><path d=\"M7 3v18\"/><path d=\"M3 12h17\"/><path d=\"M17 9l3 3-3 3\"/></svg>"),
        new(Waffenart.Vert, "vert", "Verteidigung", "Schilde, Schleier, Runen", "<svg class=\"ico\" viewBox=\"0 0 24 24\" aria-hidden=\"true\"><path d=\"M12 3l8 3v6c0 4.6-3.4 8-8 9-4.6-1-8-4.4-8-9V6z\"/><path d=\"M12 7.5v9M8.5 11h7\"/></svg>"),
    };

    public static Kategorie KategorieVon(Waffenart art) => Kategorien.First(k => k.Art == art);

    /// <summary>"nah" → Nahkampf; Unbekanntes wird Nahkampf.</summary>
    public static Kategorie KategorieAus(string? code) => Kategorien.FirstOrDefault(k => k.Code == code) ?? Kategorien[0];

    /// <summary>Alle möglichen Formen je Kategorie (JS: WAFFEN).</summary>
    public static readonly Dictionary<Waffenart, Waffe[]> Waffen = new()
    {
        [Waffenart.Nah] = new Waffe[]
        {
            new("Ätherdolch", "1W10", null),
            new("Klingenpeitsche", "1W10 +2", null),
            new("Kristall-Streitkolben", "2W10", null),
            new("Zwillingsklingen", "1W10 +1", "trifft in einem Zug zweimal, halber Schaden je Treffer"),
            new("Kriegssense", "2W10 +1", null),
            new("Ätherfaustklinge (Stich)", "1W10 +1", null),
            new("Ätherhammer", "2W10 +2", null),
            new("Wirbelklinge", "1W10 +3", null),
            new("Lanzenspross", "2W10", null),
            new("Rabenschnabel-Axt", "1W10 +4", null),
        },
        [Waffenart.Fern] = new Waffe[]
        {
            new("Ätherpfeil", "1W10", null),
            new("Funkenschleuder", "1W10 +1", null),
            new("Kristallgeschoss", "2W10", null),
            new("Wispernder Bogen", "1W10 +2", null),
            new("Splitterhagel", "1W10", "trifft bis zu zwei nahe beieinanderstehende Ziele"),
            new("Donnerknall-Rohr", "2W10 +1", null),
            new("Nadelschwarm", "1W10 +3", null),
            new("Kettenblitz", "2W10", "springt bei Erfolg auf ein zweites Ziel über (halber Schaden)"),
            new("Glutkugel", "2W10 +2", null),
            new("Sternenschleuder", "1W10 +4", null),
        },
        [Waffenart.Vert] = new Waffe[]
        {
            new("Ätherschild (rund)", "1W10", "+10 RÜW bis zum nächsten eigenen Zug"),
            new("Bannspiegel", "1W10", "nächste REA-Probe = Vorteil"),
            new("Kristallpanzer-Handschuh", "1W10 +1", "+5 RÜW bis zum nächsten eigenen Zug"),
            new("Wirbelwand", "1W10", "blockt den nächsten Fernangriff komplett"),
            new("Dornenranke (Konter)", "1W10 +2", null),
            new("Nebelschleier", "1W10", "nächster gegnerischer Angriff auf dich = Nachteil"),
            new("Ätherlaterne", "1W10", "+10 MAGW bis zum nächsten eigenen Zug"),
            new("Schutzrune-Amulett", "1W10", "ein eigener Patzer wird einmalig zu Abgeschwächt aufgewertet"),
            new("Reflexplatte", "1W10 +1", "bei gegnerischem Patzer: 1W10 Schaden zurück"),
            new("Standfester Kern", "1W10", "+10 LP bis zum Ende des Kampfes"),
        },
    };

    /// <summary>So viele Würfe merkt sich die Chronik (JS: weaponHistory.slice(0, 8)).</summary>
    public const int ChronikMax = 8;

    /// <summary>"2W10 +1" → 2 Würfel, Bonus 1 (JS: parseDmg).</summary>
    public static (int Wuerfel, int Bonus) SchadenLesen(string schaden)
    {
        var m = Regex.Match(schaden ?? "", @"(\d+)\s*W10\s*(?:\+\s*(\d+))?", RegexOptions.IgnoreCase);
        if (!m.Success) return (1, 0);
        int bonus = 0;
        if (m.Groups[2].Success)
        {
            bonus = int.Parse(m.Groups[2].Value);
        }
        return (int.Parse(m.Groups[1].Value), bonus);
    }

    /// <summary>Durchschnittlicher Schaden: je W10 5,5 plus Bonus (JS: dmgAvg).</summary>
    public static double Durchschnitt(string schaden)
    {
        var (w, b) = SchadenLesen(schaden);
        return w * 5.5 + b;
    }

    /// <summary>Stärkste Waffe aller Kategorien, für die Länge des Wucht-Balkens (JS: DMG_MAX).</summary>
    public static readonly double DurchschnittMax = Waffen.Values.SelectMany(x => x).Max(w => Durchschnitt(w.Schaden));

    /// <summary>Wucht in Prozent des Stärksten.</summary>
    public static int Wucht(string schaden) => (int)Math.Round(Durchschnitt(schaden) / DurchschnittMax * 100);

    /// <summary>"Ø 11,5" wie im Zeugnis mit Komma.</summary>
    public static string DurchschnittText(string schaden) =>
        Durchschnitt(schaden).ToString("0.#", CultureInfo.GetCultureInfo("de-DE"));
}
