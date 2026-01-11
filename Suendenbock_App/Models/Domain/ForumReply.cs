namespace Suendenbock_App.Models.Domain
{
    public class ForumReply
    {
        public int Id { get; set; }
        public int ThreadId { get; set; }
        public int AuthorCharacterId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsArchived { get; set; } = false;

        //Navigation Properties
        public ForumThread Thread { get; set; }
        public Character AuthorCharacter { get; set; }
    }
}
