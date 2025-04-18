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
    }
}
