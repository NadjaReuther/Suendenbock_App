using Suendenbock_App.Models.Domain.Hesparis;

namespace Suendenbock_App.Models.ViewModels.Hesparis;

// =====================================================================
// Akademie · Krankenflügel (JS: MODS.kranken)
// =====================================================================

/// <summary>Feste Inhalte und kleine Regeln des Krankenflügels (JS: KF_BEDS, KF_KIND, kfLeft …).</summary>
public static class KrankenfluegelRegeln
{
    /// <summary>Betten im Saal (JS: KF_BEDS).</summary>
    public const int Betten = 6;

    /// <summary>Was ist passiert? Art und Symbol (JS: KF_KIND).</summary>
    public static readonly (string Art, string Symbol)[] Arten =
    {
        ("Unfall", "✹"), ("Verletzung", "✚"), ("Fluch", "☾"), ("Erschöpfung", "≈"),
        ("Vergiftung", "☠"), ("Trankunfall", "⚗"), ("Krankheit", "❋"),
    };

    public static string Symbol(string? art) => Arten.FirstOrDefault(a => a.Art == art).Symbol ?? "✚";

    /// <summary>Farbe für Patienten, die keine Spielfigur sind (JS: kfCol).</summary>
    public static readonly HesparisSigille.Aussehen Fremd = new("#9fc0ff", "159,192,255", "");

    /// <summary>Freies Bett: grau (JS: ['#5a5a6a', '90,90,106']).</summary>
    public static readonly HesparisSigille.Aussehen Frei = new("#5a5a6a", "90,90,106", "");

    /// <summary>Noch so viele Tage im Bett (JS: kfLeft).</summary>
    public static int Rest(Krankheitsfall f, DateOnly heute) =>
        Math.Max(0, (f.Tage ?? 0) - Math.Max(0, heute.DayNumber - f.Aufnahme.DayNumber));

    /// <summary>"1 Tag" / "3 Tage" (JS: kfDays).</summary>
    public static string Tage(int n) => n + (n == 1 ? " Tag" : " Tage");

    /// <summary>Kurzes Datum "3.11." (JS: kfShort).</summary>
    public static string Kurz(DateOnly d) => d.Day + "." + d.Month + ".";
}

/// <summary>Ein Fall mit Anzeigename und Farbe.</summary>
public class KrankenZeile
{
    public Krankheitsfall Fall { get; set; } = null!;
    public string Name { get; set; } = "";
    public HesparisSigille.Aussehen Aussehen { get; set; } = KrankenfluegelRegeln.Fremd;
    public int Rest { get; set; }
}

public class KrankenfluegelViewModel
{
    public DateOnly Heute { get; set; }

    /// <summary>Die sechs Betten, null = frei (JS: kfBeds).</summary>
    public KrankenZeile?[] Betten { get; set; } = new KrankenZeile?[KrankenfluegelRegeln.Betten];
    public int Belegt => Betten.Count(b => b != null);

    /// <summary>Gewähltes Bett (JS: KF.sel).</summary>
    public int Bett { get; set; }

    /// <summary>Aufnahmebuch: alle Fälle nach Aufnahmetag (JS: list).</summary>
    public List<KrankenZeile> Buch { get; set; } = new();

    /// <summary>Buch gleich offen, mit diesem Eintrag bzw. dem Aufnahmeformular.</summary>
    public bool BuchOffen { get; set; }
    public int? AuswahlId { get; set; }
    public bool Formular { get; set; }

    /// <summary>Vorausgewähltes freies Bett im Formular (JS: KF.formBed).</summary>
    public int? FormularBett { get; set; }

    /// <summary>Spielfiguren zur Auswahl im Formular.</summary>
    public List<FigurChip> Figuren { get; set; } = new();

    public bool IstSpielleitung { get; set; }
}
