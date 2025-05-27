namespace Suendenbock_App.Models.Domain
{
    public class Obermagie
    {
        public int Id { get; set; }
        public string Bezeichnung { get; set; } = string.Empty;
        public int LightCardId { get; set; }
        // Navigation Properties
        public virtual LightCard? LightCard { get; set; }  // Obermagie has a LightCard
    }
}
