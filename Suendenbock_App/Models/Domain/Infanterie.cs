namespace Suendenbock_App.Models.Domain
{
    public class Infanterie
    {
        public int Id { get; set; }
        public string Bezeichnung { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string? ImagePath { get; set; } = string.Empty;
        public string Sitz {  get; set; } = string.Empty;
        public int? LightCardId { get; set; } // Foreign Key to LightCard
        public int? leader { get; set; } // Character ID of the leader
        public int? vertreter { get; set; } // Character ID of the representative

        //Navigation Property
        public virtual ICollection<Regiment> Regiments { get; set; } = new List<Regiment>();
    }
}
