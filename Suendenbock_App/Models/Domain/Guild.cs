namespace Suendenbock_App.Models.Domain
{
    public class Guild
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        //Foreign Key
        public int LightCardId { get; set; }

        //Navigation Properties
        public LightCard? LightCard { get; set; }  // Navigation Property Guild has a LightCard
        public virtual ICollection<Character> Characters { get; set; } = new List<Character>(); // Guild has many Characters
    }
}
