namespace Suendenbock_App.Models.Domain
{
    public class LightCard
    {
        public int Id { get; set; }
        public string CssClass { get; set; } = string.Empty;
        public virtual ICollection<MagicClass> MagicClasses { get; set; } = new List<MagicClass>(); // LightCard has many MagicClasses
        public virtual ICollection<Guild> Guilds { get; set; } = new List<Guild>(); // LightCard has many Guilds
    }
}
