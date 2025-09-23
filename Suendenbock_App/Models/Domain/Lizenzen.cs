namespace Suendenbock_App.Models.Domain
{
    public class Lizenzen
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Gildenlizenz>? Gildenlizenzen { get; set; }
    }
}
