namespace Suendenbock_App.Models.Domain
{
    public class Blutgruppe
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Name der Blutgruppe, z.B. "A+", "B-", etc.
        // Optional: Weitere Eigenschaften, die für die Blutgruppe relevant sein könnten
        public string? Besonderheiten { get; set; } // z.B. "Kann nur mit A+ transfundiert werden"
        // Navigation Property für Charaktere, die diese Blutgruppe haben
        public virtual ICollection<Character> Charaktere { get; set; } = new List<Character>();
    }
}
