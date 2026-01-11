namespace Suendenbock_App.Models.Domain
{
    public class EventRSVP
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int CharacterId { get; set; }
        public string Status { get; set; } = string.Empty; // enum: "yes", "maybe", "no"
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //Navigation Properties
        public MonthlyEvent? Event { get; set; }
        public Character? Character { get; set; }
    }
}
