using System.Runtime.CompilerServices;

namespace Suendenbock_App.Models.Domain
{
    public class ForumThread
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }
        public int AuthorCharacterId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsArchived { get; set; }
        public bool IsPinned { get; set; }

        //Navigation Porperties
        ForumCategory Category { get; set; }
        Character AuthorCharacter { get; set; }
        List<ForumReply> Replies { get; set; }
    }
}
