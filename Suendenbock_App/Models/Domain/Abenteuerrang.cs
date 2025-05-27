namespace Suendenbock_App.Models.Domain
{
    public class Abenteuerrang
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Navigation Properties
        public virtual ICollection<Guild> Guilds { get; set; } = new List<Guild>(); // Abenteuerrang has many Guilds
    }
}
