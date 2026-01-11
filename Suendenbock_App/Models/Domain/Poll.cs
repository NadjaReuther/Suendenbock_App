using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    public class Poll
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Die Frage ist erforderlich")]
        [StringLength(500)]
        public string Question { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "active"; // "active" or "closed"
        public string Category { get; set; } = string.Empty; // "Events", "Balancing", "Community"
        public bool AllowMultipleChoices { get; set; } = false;
        public string CreatedByUserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        //Navigation Properties
        public ApplicationUser? CreatedBy { get; set; }
        public List<PollOption>? Options { get; set; }
        public List<PollVote>? Votes { get; set; }
    }
}
