namespace Suendenbock_App.Models.Domain
{
    public class Regiment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ProcessedDescription { get; set; }

        // Nullable Foreign Keys
        public int? RegimentsleiterId { get; set; }
        public int? AdjutantId { get; set; }
        public int InfanterieId { get; set; } 

        // Navigation Properties
        public virtual Character? Regimentsleiter { get; set; }
        public virtual Character? Adjutant { get; set; }
        public virtual Infanterie? Infanterie { get; set; }
    }
}
