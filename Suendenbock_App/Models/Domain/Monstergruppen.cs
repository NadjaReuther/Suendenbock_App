namespace Suendenbock_App.Models.Domain
{
    public class Monstergruppen
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Monstertyp> Monstertypen { get; set; } = new List<Monstertyp>();
    }
}
