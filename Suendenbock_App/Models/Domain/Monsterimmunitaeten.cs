namespace Suendenbock_App.Models.Domain
{
    public class Monsterimmunitaeten
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Monstertypimmunitaeten>? Monstertypimmunitaeten { get; set; }
    }
}
