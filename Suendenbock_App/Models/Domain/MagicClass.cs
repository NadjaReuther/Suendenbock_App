namespace Suendenbock_App.Models.Domain
{
    public class MagicClass
    {
        public int Id { get; set; }
        public string Bezeichnung { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;

        //Foreign Key
        public int ObermagieId { get; set; }
        // Navigation Property
        // MagicClass has a Obermagie
        public virtual Obermagie? Obermagie { get; set; }
        // MagicClass has many CharacterMagicClasses
        public virtual ICollection<CharacterMagicClass> CharacterMagicClasses { get; set; } = new List<CharacterMagicClass>();
        //MagicClass has many MagicClassSpecializations
        public virtual ICollection<MagicClass> MagicClasses { get; set;} = new List<MagicClass>();

    }
}
