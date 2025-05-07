namespace Suendenbock_App.Models.Domain
{
    public class Specialization
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;

        // Foreign Key
        public int MagicClassId { get; set; }
        // Navigation Property
        public virtual MagicClass MagicClass { get; set; } = null!;
        public virtual ICollection<Character> Characters { get; set; } = new List<Character>(); // Specialization has many Characters
    }
}
