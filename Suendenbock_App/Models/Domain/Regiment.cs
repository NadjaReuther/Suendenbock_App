namespace Suendenbock_App.Models.Domain
{
    public class Regiment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Nullable Foreign Keys
        public int? Regimentsleiter { get; set; }
        public int? Adjutant { get; set; }
        public int InfanterieId { get; set; } // ← Richtiger Name für FK

        // Navigation Properties
        public virtual Character? RegimentsCharacter { get; set; }
        public virtual Character? AdjutantCharacter { get; set; }
        public virtual Infanterie? Infanterie { get; set; }
        public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
    }
}
