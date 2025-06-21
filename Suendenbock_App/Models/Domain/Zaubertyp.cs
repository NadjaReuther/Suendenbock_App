namespace Suendenbock_App.Models.Domain
{
    public class Zaubertyp
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Beschreibung { get; set; } = string.Empty;
        // Navigation Property
        public virtual ICollection<Grundzauber> Grundzaubers { get; set; } = new List<Grundzauber>(); // Zaubertyp has many Grundzauber
        public virtual ICollection<SpecialZauber> SpecialZaubers { get; set; } = new List<SpecialZauber>(); // Zaubertyp has many SpecialZauber
    }
}
