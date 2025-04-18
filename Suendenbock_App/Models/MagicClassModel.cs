namespace Suendenbock_App.Models
{
    public class MagicClassModel
    {
        public int Id { get; set; }
        public string Bezeichnung { get; set; }
        public string ImagePath { get; set; }

        //Foreign Key
        public int LightCardsId { get; set; }
        public LightCards LightCard { get; set; }  // Navigation Property Magicclass has a LightCard

        //Navigation Properties
        public virtual ICollection<CharacterModel> Characters { get; set; } // MagicClass has many Characters
    }
}
