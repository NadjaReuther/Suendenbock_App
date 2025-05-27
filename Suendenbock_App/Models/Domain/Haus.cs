namespace Suendenbock_App.Models.Domain
{
    public class Haus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImagePath { get; set; } // Optional image path for the house
        // Navigation property for characters belonging to this house
        public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
    }
}
