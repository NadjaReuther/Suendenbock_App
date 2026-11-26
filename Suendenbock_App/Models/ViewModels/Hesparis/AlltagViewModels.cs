namespace Suendenbock_App.Models.ViewModels.Hesparis;

// ------------------------------------------------------------------ Stundenplan

/// <summary>Stundenplan: Fächer-Übersicht und Wochenraster (JS: renderStundenplan).</summary>
public class StundenplanViewModel
{
    public List<StundenplanFach> Faecher { get; set; } = new();

    /// <summary>Heute (0 = Montag) und laufende Zeile im Raster; −1 = keine.</summary>
    public int HeuteTag { get; set; } = -1;
    public int JetztZeile { get; set; } = -1;

    public StundenplanFach Fach(int id) => Faecher.First(f => f.Id == id);
}

public class StundenplanFach
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Kurzname { get; set; } = "";
    public string? Lehrkraft { get; set; }
    public string Farbe { get; set; } = "";
    public string Rgb { get; set; } = "";

    /// <summary>Räume, in denen das Fach stattfindet, z. B. "Raum 211 · Trainingsplatz".</summary>
    public string Orte { get; set; } = "";
}
