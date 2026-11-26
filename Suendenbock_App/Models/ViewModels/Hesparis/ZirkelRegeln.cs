using System.Globalization;
using System.Text;
using Suendenbock_App.Models.Domain.Hesparis;

namespace Suendenbock_App.Models.ViewModels.Hesparis;

/// <summary>
/// Regeln des Zirkels der Priore aus der hesparis.html (JS: ZK_EL, ZK_FIG, zkMatchText …).
/// Die zwölf Magien sind allen bekannt. Gesucht sind die Personen dahinter:
/// Die Spielleitung hinterlegt die Wahrheit, die Spieler raten, der Server prüft.
/// </summary>
public static class ZirkelRegeln
{
    public record Magie(int Nr, string Name, string Genitiv, string Farbe, string Phase);

    /// <summary>Die zwölf Magien im Kreislauf (JS: ZK_EL).</summary>
    public static readonly Magie[] Magien =
    {
        new(1, "Dunkel", "des Dunkels", "#8b7ed8", "Ursprung"),
        new(2, "Wasser", "des Wassers", "#4fa3e8", "Ursprung"),
        new(3, "Luft", "der Luft", "#cfe4ff", "Ursprung"),
        new(4, "Seele", "der Seele", "#f08fbf", "Emergenz"),
        new(5, "Flora", "der Flora", "#8fd67a", "Emergenz"),
        new(6, "Fauna", "der Fauna", "#e8894a", "Emergenz"),
        new(7, "Licht", "des Lichts", "#ffe08a", "Höhepunkt"),
        new(8, "Schmerz", "des Schmerzes", "#e5606a", "Höhepunkt"),
        new(9, "Kunst", "der Kunst", "#c9a0ff", "Höhepunkt"),
        new(10, "Geist", "des Geistes", "#49d9c9", "Zerstörung"),
        new(11, "Feuer", "des Feuers", "#f0703a", "Zerstörung"),
        new(12, "Zeit", "der Zeit", "#d9a24a", "Zerstörung"),
    };

    public static readonly string[] Phasen = { "Ursprung", "Emergenz", "Höhepunkt", "Zerstörung" };

    public record Schachfigur(string Name, string Bedeutung);

    /// <summary>Schachfiguren und ihre Bedeutung (JS: ZK_FIG). Der Schlüssel steht in ZirkelPerson.Figur.</summary>
    public static readonly Dictionary<string, Schachfigur> Figuren = new()
    {
        ["koenig"] = new("König", "Herrschaft. Um ihn dreht sich alles. Fällt er, wankt der Zirkel."),
        ["dame"] = new("Dame", "Macht und Einfluss. Frei in jede Richtung, die gefährlichste Figur."),
        ["turm"] = new("Turm", "Treue und Standhaftigkeit. Geht nur gerade, weicht nicht ab."),
        ["laeufer"] = new("Läufer", "Diplomatie und Intrige. Geht schräg und bleibt auf seiner Seite."),
        ["springer"] = new("Springer", "Unberechenbar. Überspringt andere und geht eigene Wege."),
        ["bauer"] = new("Bauer", "Diener und Werkzeug. Kann aufsteigen, wenn er durchkommt."),
    };

    /// <summary>Umriss der Schachfiguren als SVG-Pfad, 40×56 groß (JS: ZK_PATH + ZK_BASE). Für die Legende.</summary>
    public static readonly Dictionary<string, string> FigurPfad = new()
    {
        ["koenig"] = "M18.5 1 h3 v4 h4 v3 h-4 v4 h-3 v-4 h-4 v-3 h4 z M12 14 c4 -2 12 -2 16 0 c3 4 1 10 -2 14 l2 15 h-16 l2 -15 c-3 -4 -5 -10 -2 -14 z" + Sockel,
        ["dame"] = "M5 9 l5 20 h20 l5 -20 l-8 10 l-3 -14 l-4 13 l-4 -13 l-3 14 z M10 31 h20 l-2 12 h-16 z" + Sockel,
        ["turm"] = "M10 6 h5 v4 h3 v-4 h4 v4 h3 v-4 h5 v9 l-4 3 v17 l3 8 h-20 l3 -8 v-17 l-4 -3 z" + Sockel,
        ["laeufer"] = "M20 2 a3 3 0 1 1 0 6 a3 3 0 1 1 0 -6 z M20 9 c6 4 8 9 8 13 c0 4 -3 6 -5 7 l2 14 h-10 l2 -14 c-2 -1 -5 -3 -5 -7 c0 -4 2 -9 8 -13 z" + Sockel,
        ["springer"] = "M13 43 c0 -8 2 -12 6 -16 c-3 1 -6 3 -9 2 c-2 -1 -2 -3 0 -5 c3 -3 5 -8 6 -12 l2 -5 l2 4 c6 1 11 7 11 17 c0 6 -1 11 -2 15 z" + Sockel,
        ["bauer"] = "M20 7 a6 6 0 1 1 0 12 a6 6 0 1 1 0 -12 z M14 21 h12 l-2 3 c2 6 4 12 5 19 h-18 c1 -7 3 -13 5 -19 z" + Sockel,
    };

    private const string Sockel = " M7 44 h26 c1 0 2 1 2 2 v2 h-30 v-2 c0 -1 1 -2 2 -2 z";

    public static readonly string[] Geschlechter = { "männlich", "weiblich", "divers" };
    public static readonly string[] Treue = { "treu", "schwankend", "korrumpiert", "Verräter" };
    public static readonly string[] Status = { "aktiv", "zurückgezogen", "verschollen" };

    public record FeldInfo(string Code, string Name, string Typ);

    /// <summary>
    /// Ratbare Felder (JS: ZK_FIELDS). Typ: text (freie Eingabe, Tippfehler erlaubt),
    /// ge, fig, mag (Auswahl, muss genau stimmen). Beim Prior ist die Magie bekannt.
    /// </summary>
    public static FeldInfo[] Felder(ZirkelSeite seite)
    {
        var liste = new List<FeldInfo>
        {
            new("vn", "Vorname", "text"),
            new("nn", "Nachname", "text"),
            new("ge", "Geschlecht", "ge"),
            new("beruf", "Beruf", "text"),
        };
        if (seite == ZirkelSeite.Sekundant)
        {
            liste.Add(new("mag", "Magie", "mag"));
        }
        liste.Add(new("fig", "Figur", "fig"));
        return liste.ToArray();
    }

    /// <summary>Nach einem Fehlversuch ist das Feld so lange gesperrt (JS: ZK_LOCK).</summary>
    public static readonly TimeSpan Sperre = TimeSpan.FromMinutes(5);

    /// <summary>So viele Fehlversuche behält die Liste der Spielleitung (JS: log.slice(0, 60)).</summary>
    public const int VersucheMax = 60;

    public static Magie MagieVon(int nr) => Magien[nr - 1];

    /// <summary>"p" bzw. "s" wie im JavaScript, für Schlüssel wie "7|p|vn".</summary>
    public static string Kurz(ZirkelSeite seite)
    {
        if (seite == ZirkelSeite.Prior)
        {
            return "p";
        }
        return "s";
    }

    public static ZirkelSeite SeiteAus(string kurz)
    {
        if (kurz == "s")
        {
            return ZirkelSeite.Sekundant;
        }
        return ZirkelSeite.Prior;
    }

    /// <summary>"Prior des Lichts" bzw. "Sekundant des Lichts" (JS: zkRole).</summary>
    public static string Rolle(int nr, ZirkelSeite seite)
    {
        if (seite == ZirkelSeite.Prior)
        {
            return "Prior " + MagieVon(nr).Genitiv;
        }
        return "Sekundant " + MagieVon(nr).Genitiv;
    }

    /// <summary>Wert eines Feldes aus der hinterlegten Wahrheit.</summary>
    public static string? Wert(ZirkelPerson p, string feld) => feld switch
    {
        "vn" => p.Vorname,
        "nn" => p.Nachname,
        "ge" => p.Geschlecht,
        "beruf" => p.Beruf,
        "mag" => p.Magie,
        "fig" => p.Figur,
        _ => null,
    };

    public static void WertSetzen(ZirkelPerson p, string feld, string? wert)
    {
        if (string.IsNullOrWhiteSpace(wert))
        {
            wert = null;
        }
        else
        {
            wert = wert.Trim();
        }
        switch (feld)
        {
            case "vn": p.Vorname = wert; break;
            case "nn": p.Nachname = wert; break;
            case "ge": p.Geschlecht = wert; break;
            case "beruf": p.Beruf = wert; break;
            case "mag": p.Magie = wert; break;
            case "fig": p.Figur = wert; break;
        }
    }

    /// <summary>
    /// Wie ein Wert angezeigt wird (JS: zkShow): Schlüssel der Figur als Name, Magie-Nummer als Name,
    /// bei Text mit mehreren Schreibweisen nur die erste.
    /// </summary>
    public static string Anzeige(string feld, string? wert)
    {
        if (string.IsNullOrEmpty(wert))
        {
            return "";
        }
        if (feld == "fig")
        {
            if (Figuren.TryGetValue(wert, out var f))
            {
                return f.Name;
            }
            return wert;
        }
        if (feld == "mag")
        {
            if (int.TryParse(wert, out var nr) && nr >= 1 && nr <= 12)
            {
                return MagieVon(nr).Name;
            }
            return wert;
        }
        if (feld == "vn" || feld == "nn" || feld == "beruf")
        {
            return wert.Split('|')[0].Trim();
        }
        return wert;
    }

    /// <summary>
    /// Text vergleichbar machen (JS: zkNorm): klein, Umlaute ausgeschrieben, Akzente weg,
    /// nur Buchstaben und Ziffern.
    /// </summary>
    public static string Normalisieren(string? text)
    {
        var s = (text ?? "").ToLowerInvariant()
            .Replace("ä", "ae").Replace("ö", "oe").Replace("ü", "ue").Replace("ß", "ss");
        var zerlegt = s.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in zerlegt)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark) continue;
            if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')) sb.Append(c);
        }
        return sb.ToString();
    }

    /// <summary>Wie viele Zeichen man ändern muss, damit a zu b wird (Levenshtein, JS: zkLev).</summary>
    public static int Abstand(string a, string b)
    {
        if (a.Length == 0) return b.Length;
        if (b.Length == 0) return a.Length;
        var vorher = new int[b.Length + 1];
        for (int j = 0; j <= b.Length; j++) vorher[j] = j;
        for (int i = 1; i <= a.Length; i++)
        {
            var jetzt = new int[b.Length + 1];
            jetzt[0] = i;
            for (int j = 1; j <= b.Length; j++)
            {
                int kosten = 1;
                if (a[i - 1] == b[j - 1])
                {
                    kosten = 0;
                }
                jetzt[j] = Math.Min(Math.Min(vorher[j] + 1, jetzt[j - 1] + 1), vorher[j - 1] + kosten);
            }
            vorher = jetzt;
        }
        return vorher[b.Length];
    }

    /// <summary>
    /// Passt eine Eingabe zur Wahrheit? (JS: zkMatchText) Bis 4 Zeichen muss es genau stimmen,
    /// bis 7 Zeichen ist ein Tippfehler erlaubt, darüber zwei. Varianten in der Wahrheit mit | trennen.
    /// </summary>
    public static bool TextPasst(string wahrheit, string eingabe)
    {
        var v = Normalisieren(eingabe);
        if (v.Length == 0) return false;
        foreach (var teil in wahrheit.Split('|'))
        {
            var t = Normalisieren(teil);
            if (t.Length == 0) continue;
            int erlaubt = 2;
            if (t.Length <= 4)
            {
                erlaubt = 0;
            }
            else if (t.Length <= 7)
            {
                erlaubt = 1;
            }
            if (Abstand(t, v) <= erlaubt) return true;
        }
        return false;
    }

    /// <summary>Farbe als rgba(…) mit Deckkraft (JS: zkRgba), z. B. für Leuchten in der Farbe der Magie.</summary>
    public static string Rgba(string hex, double deckkraft)
    {
        int v = Convert.ToInt32(hex.TrimStart('#'), 16);
        return "rgba(" + (v >> 16) + "," + ((v >> 8) & 255) + "," + (v & 255) + "," +
               deckkraft.ToString(CultureInfo.InvariantCulture) + ")";
    }

    /// <summary>"m:ss" für die Sperr-Anzeige (JS: zkClock).</summary>
    public static string Uhr(int sekunden) => (sekunden / 60) + ":" + (sekunden % 60).ToString("00");
}
