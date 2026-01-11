using System.ComponentModel.DataAnnotations;

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
        public int CategoryId { get; set; }
        public int AuthorCharacterId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsArchived { get; set; } = false;
        public bool IsPinned { get; set; } = false;

        //Navigation Porperties
        public ForumCategory? Category { get; set; }
        public Character? AuthorCharacter { get; set; }
        public List<ForumReply>? Replies { get; set; }
    }
}
