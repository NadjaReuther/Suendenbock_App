namespace Suendenbock_App.Models.Domain
{
    public class Zaubertyp
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Beschreibung { get; set; } = string.Empty;
        // Navigation Property
        public virtual ICollection<Zauber> Zauber { get; set; } = new List<Zauber>(); // Zaubertyp has many Zauber
    }
}
