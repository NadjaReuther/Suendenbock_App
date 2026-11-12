namespace Suendenbock_App.Models.Domain.Hesparis;

/// <summary>
/// Mitschüler der Klasse (JS: state.klasse). Die Spielfiguren stehen hier ebenfalls
/// drin, verknüpft über <see cref="SpielfigurId"/>.
/// </summary>
public class Mitschueler : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;

    /// <summary>Der Hesparis-Charakter (Art Lehrkraft bzw. Mitschüler) zu diesen Zusatzdaten.</summary>
    public int? HesparisCharakterId { get; set; }
    public HesparisCharakter? HesparisCharakter { get; set; }

    /// <summary>Kennung aus dem Frontend, z. B. "kl0". Eindeutig je Kampagne.</summary>
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    /// <summary>JS: spitz.</summary>
    public string? Spitzname { get; set; }

    /// <summary>JS: spott.</summary>
    public string? Spottname { get; set; }
    public string? Magie { get; set; }
    public string? Herkunft { get; set; }

    public int? LieblingsfachId { get; set; }
    public Fach? Lieblingsfach { get; set; }
    public int? HassfachId { get; set; }
    public Fach? Hassfach { get; set; }

    public string? Patronus { get; set; }
    public string? Rolle { get; set; }
    public string? Notiz { get; set; }

    /// <summary>Notenschnitt, z. B. 1,5 (JS: schnitt).</summary>
    public decimal? Notenschnitt { get; set; }
    public DateOnly? Geburtstag { get; set; }

    /// <summary>Farbindex für die Darstellung (JS: farbe, 0–7).</summary>
    public int? Farbe { get; set; }

    /// <summary>Steht unter Leistungsdruck (JS: druck).</summary>
    public bool Leistungsdruck { get; set; }

    /// <summary>Platz im Sitzplan (JS: klSitz[code]).</summary>
    public int? Sitzplatz { get; set; }

    /// <summary>Gesetzt, wenn dieser Mitschüler eine Spielfigur ist (JS: spieler).</summary>
    public int? SpielfigurId { get; set; }
    public Spielfigur? Spielfigur { get; set; }

    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<MitschuelerMeinung> Meinungen { get; set; } = new List<MitschuelerMeinung>();
    public ICollection<MitschuelerAg> Ags { get; set; } = new List<MitschuelerAg>();
    public ICollection<Beziehung> Beziehungen { get; set; } = new List<Beziehung>();
}

/// <summary>Haltung zu einem Schulthema, −1 / 0 / +1 (JS: meinung).</summary>
public class MitschuelerMeinung
{
    public int MitschuelerId { get; set; }
    public Mitschueler Mitschueler { get; set; } = null!;

    /// <summary>geld, ordnung, lehrer, freizeit, sport, essen.</summary>
    public string Thema { get; set; } = "";
    public int Wert { get; set; }
}

/// <summary>AG eines Mitschülers, der keine Spielfigur ist (JS: ags). Spielfiguren stehen in <see cref="AgMitglied"/>.</summary>
public class MitschuelerAg
{
    public int MitschuelerId { get; set; }
    public Mitschueler Mitschueler { get; set; } = null!;
    public string AgCode { get; set; } = "";
    public Ag Ag { get; set; } = null!;
}

/// <summary>
/// Beziehung zwischen einem Mitschüler und einer Spielfigur (JS: bez[spielerIndex]).
/// Zwei Richtungen mit je fünf Kategorien.
/// </summary>
public class Beziehung
{
    public int MitschuelerId { get; set; }
    public Mitschueler Mitschueler { get; set; } = null!;
    public int SpielfigurId { get; set; }
    public Spielfigur Spielfigur { get; set; } = null!;

    /// <summary>Wie die Spielfigur zum Mitschüler steht (JS: o).</summary>
    public Beziehungswerte SpielfigurZuMitschueler { get; set; } = new();

    /// <summary>Wie der Mitschüler zur Spielfigur steht (JS: i).</summary>
    public Beziehungswerte MitschuelerZuSpielfigur { get; set; } = new();

    /// <summary>Vor den Spielern verborgen (JS: hid).</summary>
    public bool Verborgen { get; set; }
}

/// <summary>Lehrkraft (JS: LEHRER, lehrer.__extra und lehrer[name]).</summary>
public class Lehrer : IKampagnenDaten
{
    public int Id { get; set; }
    public int KampagneId { get; set; }
    public Kampagne Kampagne { get; set; } = null!;

    /// <summary>Der Hesparis-Charakter (Art Lehrkraft bzw. Mitschüler) zu diesen Zusatzdaten.</summary>
    public int? HesparisCharakterId { get; set; }
    public HesparisCharakter? HesparisCharakter { get; set; }
    public string Name { get; set; } = "";

    /// <summary>Freitext zu Fächern/Ämtern (JS: __extra.faecher), z. B. "Schulleiterin".</summary>
    public string? Aufgaben { get; set; }

    /// <summary>Haltung zur Gruppe 0–4: feindselig … Fan der Gruppe (JS: rel).</summary>
    public int? Haltung { get; set; }

    // Steckbrief (JS: steck)
    public string? Spitzname { get; set; }
    public string? Erkennungszeichen { get; set; }
    public string? TrifftManMeist { get; set; }
    public string? Mag { get; set; }
    public string? MagNicht { get; set; }
    public string? WunderPunkt { get; set; }
    public string? SoPunktetMan { get; set; }
    public string? Geheimnis { get; set; }

    public ICollection<LehrerFach> Faecher { get; set; } = new List<LehrerFach>();
    public ICollection<LehrerFakt> Fakten { get; set; } = new List<LehrerFakt>();
}

/// <summary>Welche Lehrkraft welches Fach unterrichtet (JS: LEHRER).</summary>
public class LehrerFach
{
    public int LehrerId { get; set; }
    public Lehrer Lehrer { get; set; } = null!;
    public int FachId { get; set; }
    public Fach Fach { get; set; } = null!;
}

/// <summary>Gesammeltes Wissen über eine Lehrkraft (JS: facts).</summary>
public class LehrerFakt
{
    public int Id { get; set; }
    public int LehrerId { get; set; }
    public Lehrer Lehrer { get; set; } = null!;
    public string Text { get; set; } = "";

    /// <summary>Wer es herausgefunden hat (Spielfigur-Name oder "Spielleitung").</summary>
    public string? Von { get; set; }

    /// <summary>Spieldatum als Text, z. B. "3. September".</summary>
    public string? DatumText { get; set; }
}
