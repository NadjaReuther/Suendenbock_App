namespace Suendenbock_App.Models.Domain
{
    public class Infanterie
    {
        public int Id { get; set; }
        public string Bezeichnung { get; set; } = string.Empty;
        public int? leader { get; set; } // Character ID of the leader
        public int? vertreter { get; set; } // Character ID of the representative
        public string? ImagePath { get; set; } = string.Empty;
        //Navigation Property
        public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
    }
}
