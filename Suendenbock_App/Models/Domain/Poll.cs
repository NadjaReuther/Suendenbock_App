using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suendenbock_App.Models.Domain
{
    public class Poll
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Die Frage ist erforderlich")]
        [StringLength(500)]
        public string Question { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public string Status { get; set; } = "active"; // "active" or "closed"
        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty; // "Events", "Balancing", "Community"
        public bool AllowMultipleChoices { get; set; } = false;
        [Required]
        public string CreatedByUserId { get; set; } = string.Empty;
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        //Navigation Properties
        public ApplicationUser CreatedBy { get; set; } = null!;
        public List<PollOption> Options { get; set; } = new List<PollOption>();
        public List<PollVote> Votes { get; set; } = new List<PollVote>();

        // Computed Property
        [NotMapped]
        public int TotalVoters => Votes.Select(v => v.CharacterId ?? v.UserId.GetHashCode()).Distinct().Count();
    }
}
