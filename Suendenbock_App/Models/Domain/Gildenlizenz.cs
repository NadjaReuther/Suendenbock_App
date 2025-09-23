namespace Suendenbock_App.Models.Domain
{
    public class Gildenlizenz
    {
        public int Id { get; set; }
        //Foreign Keys
        public int GuildId { get; set; }
        public int LizenzenId { get; set; }

        //Property Navigation
        public virtual Guild? Guild { get; set; }
        public virtual Lizenzen? Lizenzen { get; set; }
    }
}
