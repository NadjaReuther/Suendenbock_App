namespace Suendenbock_App.Models.Domain
{
    public class Guild
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

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

        public virtual Character? LeaderCharacter { get; set; }
        public virtual Character? VertreterCharacter { get; set; }
    }
}
