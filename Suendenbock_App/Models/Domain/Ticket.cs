namespace Suendenbock_App.Models.Domain
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ReporterCharacterId { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ResolvedAt { get; set; }
        public string ResolvedByUserId { get; set; }

        //Navigation Properties
        public Character ReporterCharacter { get; set; }
        public ApplicationUser ResolvedBy {  get; set; }
    }
}
