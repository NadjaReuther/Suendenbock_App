using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suendenbock_App.Models.Domain
{
    public class ForumReply
    {
        public int Id { get; set; }
        [Required]
        public int ThreadId { get; set; }
        [Required(ErrorMessage = "Die Beschreibung ist erforderlich")]
        public string Content { get; set; } = string.Empty;
        public int? AuthorCharacterId { get; set; }
        public int? AuthorUserId { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsArchived { get; set; } = false;

        //Navigation Properties
        public ForumThread Thread { get; set; } = null!;
        public Character? AuthorCharacter { get; set; }
        public ApplicationUser? AuthorUser { get; set; }

        // Computed Property
        [NotMapped]
        public string AuthorName => AuthorCharacter?.Vorname ?? AuthorUser?.UserName ?? "Unbekannt";

    }
}
