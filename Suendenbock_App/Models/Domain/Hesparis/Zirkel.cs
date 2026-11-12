namespace Suendenbock_App.Models.Domain.Hesparis;

/// <summary>
/// Person im Zirkel-Rätsel (JS: zirkel.el[nr].p bzw. .s). Zwölf Paare,
/// je mit Prior/Paar-Seite und Spiegel-Seite.
/// </summary>
public class ZirkelPerson : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;

    /// <summary>Platz im Zirkel, 1–12.</summary>
    public int Nr { get; set; }
    public ZirkelSeite Seite { get; set; }
    public string? Vorname { get; set; }
    public string? Nachname { get; set; }
    public string? Geschlecht { get; set; }
    public string? Magie { get; set; }

    /// <summary>Schachfigur, z. B. springer, bauer.</summary>
    public string? Figur { get; set; }
    public string? Beruf { get; set; }
}

/// <summary>
/// Rätselstand je Feld (JS: zirkel.found["nr|seite|feld"] und zirkel.lock[…]).
/// </summary>
public class ZirkelFeld : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public int Nr { get; set; }
    public ZirkelSeite Seite { get; set; }

    /// <summary>vn, nn, ge, mag, fig, beruf.</summary>
    public string Feld { get; set; } = "";
    public bool Gefunden { get; set; }

    /// <summary>Nach einem Fehlversuch gesperrt bis (Echtzeit).</summary>
    public DateTime? GesperrtBis { get; set; }
}

/// <summary>Verlauf der Rätselversuche (JS: zirkel.log).</summary>
public class ZirkelVersuch : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public int Nr { get; set; }
    public ZirkelSeite Seite { get; set; }
    public string Feld { get; set; } = "";
    public string Wert { get; set; } = "";
    public DateTime Zeitpunkt { get; set; }
    public string? SpieltagText { get; set; }
}

/// <summary>Hinweis-Notiz der Spieler (JS: zirkel.notes).</summary>
public class ZirkelNotiz : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public int Nr { get; set; }

    /// <summary>Zu wem: Paar- oder Spiegelseite (JS: r).</summary>
    public ZirkelSeite? Seite { get; set; }
    public string? Typ { get; set; }

    /// <summary>Von wem (JS: pl, Name einer Spielfigur).</summary>
    public int? SpielfigurId { get; set; }
    public Spielfigur? Spielfigur { get; set; }
    public string Text { get; set; } = "";

    /// <summary>Von der Spielleitung bestätigt (JS: ok).</summary>
    public bool Bestaetigt { get; set; }
    public DateTime ErstelltAm { get; set; }
}

/// <summary>Der Mordfall im Zirkel (JS: zirkel.mord). Genau einer je Kampagne.</summary>
public class ZirkelMordfall : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;

    /// <summary>Erlaubte Versuche je Frage.</summary>
    public int MaxVersuche { get; set; } = 3;
    public int RestVersucheOpfer { get; set; } = 3;
    public int RestVersucheTaeter { get; set; } = 3;

    /// <summary>Platz des Opfers (JS: tot).</summary>
    public int? OpferNr { get; set; }

    /// <summary>Täter (JS: moe = "nr|seite").</summary>
    public int? TaeterNr { get; set; }
    public ZirkelSeite? TaeterSeite { get; set; }
    public string? Motiv { get; set; }
    public bool OpferGeloest { get; set; }
    public bool TaeterGeloest { get; set; }
    public bool MotivAufgedeckt { get; set; }
}

/// <summary>Kiste im Archiv (JS: archiv.kats).</summary>
public class ArchivKategorie : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;
    public string Code { get; set; } = "";
    public int Position { get; set; }
    public string Name { get; set; } = "";
    public string? Signatur { get; set; }
}

/// <summary>Dokument im Archiv (JS: archiv.docs).</summary>
public class ArchivDokument : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;

    /// <summary>Brief, Bericht, Telegramm …</summary>
    public string? Art { get; set; }
    public string Titel { get; set; } = "";
    public string? DatumText { get; set; }
    public string? Signatur { get; set; }
    public string? Text { get; set; }
    public string? Unterschrift { get; set; }
    public bool Versiegelt { get; set; }

    /// <summary>Für Spieler noch nicht sichtbar.</summary>
    public bool Verborgen { get; set; }

    public ICollection<ArchivDokumentKategorie> Kategorien { get; set; } = new List<ArchivDokumentKategorie>();
}

public class ArchivDokumentKategorie
{
    public int ArchivDokumentId { get; set; }
    public ArchivDokument ArchivDokument { get; set; } = null!;
    public int ArchivKategorieId { get; set; }
    public ArchivKategorie ArchivKategorie { get; set; } = null!;
}
