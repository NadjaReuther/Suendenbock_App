namespace Suendenbock_App.Models.Domain
{
    public class MagicClass
    {
        public int Id { get; set; }
        public string Bezeichnung { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;

        //Foreign Key
        public int LightCardId { get; set; }

        //Navigation Properties
        public virtual LightCard? LightCard { get; set; }  // Navigation Property Magicclass has a LightCard
        public virtual ICollection<Specialization> Specializations { get; set; } = new List<Specialization>(); // MagicClass has many Specializations
        public virtual ICollection<CharacterMagicClass> CharacterMagicClasses{ get; set; } = new List<CharacterMagicClass>(); // MagicClass has many Characters
    }
}
