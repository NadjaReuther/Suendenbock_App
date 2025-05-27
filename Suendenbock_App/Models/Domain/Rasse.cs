namespace Suendenbock_App.Models.Domain
{
    public class Rasse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Navigation Properties
        public virtual ICollection<Character> Characters { get; set; } = new List<Character>(); // Rasse has many Characters
    }
}
