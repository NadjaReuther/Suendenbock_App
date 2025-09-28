namespace Suendenbock_App.Models.Domain
{
    public class Infanterie
    {
        public int Id { get; set; }
        public string Bezeichnung { get; set; } = string.Empty;
        public string? description { get; set; }
        public string? ProcessedDescription { get; set; }
        public string? ImagePath { get; set; } = string.Empty;
        public string Sitz {  get; set; } = string.Empty;
        public int? LightCardId { get; set; } // Foreign Key to LightCard
        public int? LeaderId { get; set; } // Character ID of the leader
        public int? VertreterId { get; set; } // Character ID of the representative

        //Navigation Property
        public virtual ICollection<Regiment> Regiments { get; set; } = new List<Regiment>();

        public virtual Character? LeaderCharacter { get; set; }
        public virtual Character? VertreterCharacter { get; set; }
    }
}
