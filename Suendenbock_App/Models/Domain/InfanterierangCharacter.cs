namespace Suendenbock_App.Models.Domain
{
    public class InfanterierangCharacter
    {
        public int Id { get; set; }
        public int CharacterId { get; set; } // Foreign key to Character
        public int InfanterierangId { get; set; } // Foreign key to Infanterierang
        // Navigation properties
        public virtual Character Character { get; set; } = null!; // character has one infanterierang
        public virtual Infanterierang Infanterierang { get; set; } = null!; // infanterierang has many characters
    }
}
