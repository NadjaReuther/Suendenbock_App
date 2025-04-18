namespace Suendenbock_App.Models
{
    public class LightCards
    {
        public int Id { get; set; }
        public string cssClass { get; set; }
        public ICollection<MagicClassModel> MagicClasses { get; set; } // LightCard has many MagicClasses
        public ICollection<GuildModel> Guilds { get; set; } // LightCard has many Guilds
    }
}
