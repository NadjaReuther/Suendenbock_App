namespace Suendenbock_App.Models.Domain
{
    public class MagicClass
    {
        public int Id { get; set; }
        public string Bezeichnung { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;

        //Foreign Key
        public int LightCardsId { get; set; }

        //Navigation Properties
        public virtual LightCard? LightCard { get; set; }  // Navigation Property Magicclass has a LightCard
        public virtual ICollection<Character> Characters { get; set; } = new List<Character>(); // MagicClass has many Characters
    }
}
