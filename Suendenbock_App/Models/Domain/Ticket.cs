using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suendenbock_App.Models.Domain
{
    public class Ticket
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Der Titel ist erforderlich")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Die Beschreibung ist erforderlich")]
        public string Description { get; set; } = string.Empty;
        public int? ReporterCharacterId { get; set; }
        public int? ReporterUserId { get; set; }
        [Required]
        public string Category { get; set; } = string.Empty; // enum: "Bug", "Support", "Suggestion", "Other"
        [Required]
        public string Status { get; set; } = string.Empty; // enum: "Pending", "Resolved"
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? ResolvedByUserId { get; set; }

        //Navigation Properties
        public Character? ReporterCharacter { get; set; }
        public ApplicationUser? ReporterUser { get; set; }
        public ApplicationUser? ResolvedBy {  get; set; }

        // computed Property
        [NotMapped]
        public string ReporterName => ReporterCharacter?.Vorname ?? ReporterUser?.UserName ?? "Unbekannt";
    }
}
