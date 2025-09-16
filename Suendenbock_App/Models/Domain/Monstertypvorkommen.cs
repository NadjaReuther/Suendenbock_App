namespace Suendenbock_App.Models.Domain
{
    public class Monstertypvorkommen
    {
        public int Id { get; set; }
        // Foreign Key
        public int MonstertypId { get; set; }
        public int MonstervorkommenId { get; set; }
        // Navigation Property Monstertypvorkommen has a Monstertyp and a Monstervorkommen
        public virtual Monstertyp? Monstertyp { get; set; }
        public virtual Monstervorkommen? Monstervorkommen { get; set; }
    }
}
