namespace Suendenbock_App.Models.Domain
{
    public class Monstertyp
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ProcessedDescription { get; set; }
        // Foreign Key
        public int MonsterwuerfelId { get; set; }
        public int MonsterintelligenzId { get; set; }
        public int MonstergruppenId { get; set; }
        // Navigation Property 
        public virtual Monsterwuerfel? Monsterwuerfel { get; set; }
        public virtual Monsterintelligenz? Monsterintelligenz { get; set; }
        public virtual Monstergruppen? Monstergruppen { get; set; }
        public virtual ICollection<Monstertypvorkommen> MonstertypenVorkommen { get; set; } = new List<Monstertypvorkommen>();
        public virtual ICollection<Monstertypimmunitaeten> MonstertypImmunitaeten { get; set; } = new List<Monstertypimmunitaeten>();
        public virtual ICollection<Monstertypanfaelligkeiten> MonstertypAnfaelligkeiten { get; set; } = new List<Monstertypanfaelligkeiten>();
        public virtual ICollection<Monster> Monster { get; set; } = new List<Monster>();
    }
}
