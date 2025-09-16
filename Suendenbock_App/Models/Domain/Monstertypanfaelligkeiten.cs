namespace Suendenbock_App.Models.Domain
{
    public class Monstertypanfaelligkeiten
    {
        public int Id { get; set; }
        public int MonstertypId { get; set; }
        public int MonsteranfaelligkeitenId { get; set; }
        public virtual Monstertyp? Monstertyp { get; set; }
        public virtual Monsteranfaelligkeiten? Monsteranfaelligkeiten { get; set; }
    }
}
