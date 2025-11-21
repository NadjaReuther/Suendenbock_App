namespace Suendenbock_App.Models.Domain
{
    public class Monster
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string? Basics { get; set; }
        public string? Description { get; set; }
        public string? ProcessedDescription { get; set; }
        public bool meet { get; set; } = false;
        public bool encounter { get; set; } = false;
        public bool perfected { get; set; } = false;
        // Foreign Key
        public int MonstertypId { get; set; }
        // Navigation Property Monster has a Monstertyp
        public virtual Monstertyp? Monstertyp { get; set; }
        // Foreign Key
    }
}
