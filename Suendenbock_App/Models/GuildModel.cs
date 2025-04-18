namespace Suendenbock_App.Models
{
    public class GuildModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }

        //Foreign Key
        public int LightCardsId { get; set; }
        public LightCards LightCard { get; set; }  // Navigation Property Guild has a LightCard

        //Navigation Properties
        public virtual ICollection<CharacterModel> Characters { get; set; } // Guild has many Characters
    }
}
