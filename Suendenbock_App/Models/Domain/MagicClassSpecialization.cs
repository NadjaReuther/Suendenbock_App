namespace Suendenbock_App.Models.Domain
{
    public class MagicClassSpecialization
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;

        // Foreign Key
        public int MagicClassId { get; set; }
        // Navigation Property MagicClassSpecialization has a MagicClass

        public virtual MagicClass? MagicClass { get; set; }

    }
}
