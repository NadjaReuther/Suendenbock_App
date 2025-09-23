namespace Suendenbock_App.Models.Domain
{
    public class Guild
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ProcessedDescription { get; set; }
        public string? quote { get; set; }
        public string? urheber { get; set; }
        public int LightCardId { get; set; }
        public int AbenteuerrangId { get; set; }
        public int AnmeldungsstatusId { get; set; }

        public int? leader { get; set; }    // Character ID des Leaders
        public int? vertreter { get; set; } // Character ID des Stellvertreters

        // Navigation Properties
        public LightCard? LightCard { get; set; }
        public Abenteuerrang? AbenteuerrangNavigation { get; set; }
        public Anmeldungsstatus? AnmeldungsstatusNavigation { get; set; }
        public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
        public virtual ICollection<Gildenlizenz>? Gildenlizenzen {  get; set; }
        public virtual Character? LeaderCharacter { get; set; }
        public virtual Character? VertreterCharacter { get; set; }
    }
}
