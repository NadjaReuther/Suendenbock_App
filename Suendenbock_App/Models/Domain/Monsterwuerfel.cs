namespace Suendenbock_App.Models.Domain
{
    public class Monsterwuerfel
    {
        public int Id { get; set; }
        public string Wuerfel { get; set; } = string.Empty;
        public virtual ICollection<Monstertyp> Monstertypen { get; set; } = new List<Monstertyp>();
    }
}
