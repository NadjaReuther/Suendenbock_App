namespace Suendenbock_App.Models.Domain
{
    public class Monstervorkommen
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Monstertypvorkommen> MonstertypenVorkommen { get; set; } = new List<Monstertypvorkommen>();
    }
}
