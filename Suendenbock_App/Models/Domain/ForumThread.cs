using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suendenbock_App.Models.Domain
{
    public class ForumThread
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Der Titel ist erforderlich")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Die Beschreibung ist erforderlich")]
        public string Content { get; set; } = string.Empty ;
        [Required]
        public int CategoryId { get; set; }
        public int? AuthorCharacterId { get; set; }
        public string? AuthorUserId { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsArchived { get; set; } = false;
        public bool IsPinned { get; set; } = false;

        //Navigation Porperties
        public ForumCategory Category { get; set; } = null!;
        public Character? AuthorCharacter { get; set; }
        public ApplicationUser? AuthorUser { get; set; }
        public List<ForumReply> Replies { get; set; } = new List<ForumReply>();

        // computed Property für Anzeige: nicht in DB gespeichert, nur für Anzeige
        [NotMapped]
        public string AuthorName => AuthorCharacter?.Vorname ?? AuthorUser?.UserName ?? "Unbekannt";
    }
}
