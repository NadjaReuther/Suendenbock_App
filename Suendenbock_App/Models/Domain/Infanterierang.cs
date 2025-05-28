namespace Suendenbock_App.Models.Domain
{
    public class Infanterierang
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Navigation Property Infanterierang has many Characters
        public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
    }
}
