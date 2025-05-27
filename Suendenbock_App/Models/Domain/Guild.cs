namespace Suendenbock_App.Models.Domain
{
    public class Guild
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Abenteuerrang { get; set; } = 0; // Default value for Abenteuerrang
        public int Anmeldungsstatus { get; set; } = 0; // Default value for Anmeldungstatus
        public string ImagePath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        //Foreign Key
        public int LightCardId { get; set; }
        public int AbenteuerrangId { get; set; } // Foreign Key for Abenteuerrang
        public int AnmeldungsstatusId { get; set; } // Foreign Key for Anmeldungstatus

        //Navigation Properties
        public LightCard? LightCard { get; set; }  // Navigation Property Guild has a LightCard
        public Abenteuerrang? AbenteuerrangNavigation { get; set; } // Navigation Property Guild has an Abenteuerrang
        public Anmeldungsstatus? AnmeldungsstatusNavigation { get; set; } // Navigation Property Guild has an Anmeldungsstatus

        public virtual ICollection<Character> Characters { get; set; } = new List<Character>(); // Guild has many Characters
    }
}
