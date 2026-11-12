using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Models.Domain.Hesparis;

/// <summary>Art eines Hesparis-Charakters: Spieler, Lehrkraft, Mitschüler (Tabelle stamm.Charakterarten).</summary>
public class Charakterart
{
    public const int Spieler = 1;
    public const int Lehrkraft = 2;
    public const int Mitschueler = 3;

    public int Id { get; set; }
    public string Name { get; set; } = "";

    public ICollection<HesparisCharakter> Charaktere { get; set; } = new List<HesparisCharakter>();
}

/// <summary>
/// Ein Charakter der Hesparis-Welt. Eigene Tabelle, unabhängig von den Sündenbock-Characters.
/// Gibt es dieselbe Person auch in Sündenbock, steht das in CharakterVerknuepfung.
/// </summary>
public class HesparisCharakter
{
    public int Id { get; set; }
    public string Vorname { get; set; } = "";
    public string? Nachname { get; set; }
    public string? Rufname { get; set; }

    /// <summary>Pfad zum Bild (wie Character.ImagePath, z. B. /images/characters/…).</summary>
    public string? BildPfad { get; set; }

    public int CharakterartId { get; set; }
    public Charakterart Charakterart { get; set; } = null!;

    /// <summary>
    /// Spieler (Benutzer), dem dieser Charakter gehört. Nur zur Anzeige ("deine Figur");
    /// bearbeiten dürfen in Hesparis alle Spieler alle Spielercharaktere.
    /// </summary>
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public CharakterVerknuepfung? Verknuepfung { get; set; }

    /// <summary>Spielwerte (Talente, Noten, Tasche …), nur bei Art Spieler.</summary>
    public Spielfigur? Spielfigur { get; set; }

    /// <summary>Anzeigename in Hesparis: immer der Vorname (der Rufname bleibt als Info gespeichert).</summary>
    public string Anzeigename => Vorname;

    public string VollerName => (Vorname + " " + Nachname).Trim();
}

/// <summary>
/// Zwischentabelle: Dieser Hesparis-Charakter ist dieselbe Person wie dieser Sündenbock-Character.
/// Jeder Charakter kommt hier höchstens einmal vor (1:1).
/// </summary>
public class CharakterVerknuepfung
{
    public int Id { get; set; }

    public int HesparisCharakterId { get; set; }
    public HesparisCharakter HesparisCharakter { get; set; } = null!;

    public int CharacterId { get; set; }
    public Character Character { get; set; } = null!;
}
