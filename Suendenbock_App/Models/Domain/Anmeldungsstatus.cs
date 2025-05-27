namespace Suendenbock_App.Models.Domain
{
    public class Anmeldungsstatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        // Navigation Properties
        public virtual ICollection<Guild> Guilds { get; set; } = new List<Guild>(); // Anmeldungsstatus has many Guilds
    }
}
