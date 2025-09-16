namespace Suendenbock_App.Models.Domain
{
    public class Monstertypimmunitaeten
    {
        public int Id { get; set; }
        // Foreign Key
        public int MonstertypId { get; set; }
        // Navigation Property Monstertypimmunitaeten has a Monstertyp
        public int MonsterimmunitaetenId { get; set; }
        public virtual Monstertyp? Monstertyp { get; set; }
        public virtual Monsterimmunitaeten? Monsterimmunitaeten { get; set; }
    }
}
