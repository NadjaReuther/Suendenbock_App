namespace Suendenbock_App.Models.Domain
{
    public class Monster
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ProcessedDescription { get; set; }
        public bool encounter { get; set; } = false;
        // Foreign Key
        public int MonstertypId { get; set; }
        // Navigation Property Monster has a Monstertyp
        public virtual Monstertyp? Monstertyp { get; set; }
        // Foreign Key
    }
}
