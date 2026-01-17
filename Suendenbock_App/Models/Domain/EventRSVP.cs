using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suendenbock_App.Models.Domain
{
    public class EventRSVP
    {
        public int Id { get; set; }
        [Required]
        public int EventId { get; set; }
        public int? CharacterId { get; set; }
        public string? UserId { get; set; }
        [Required]
        public string Status { get; set; } = string.Empty; // enum: "yes", "maybe", "no"
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //Navigation Properties
        public MonthlyEvent Event { get; set; } = null!;
        public Character? Character { get; set; }
        public ApplicationUser? User { get; set; }

        // Computed Property
        [NotMapped]
        public string ParticipantName => Character?.Vorname ?? User?.UserName ?? "Unbekannt";
    }
}
