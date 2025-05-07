namespace Suendenbock_App.Models.Domain
{
    public class CharacterMagicClass
    {
        public int Id { get; set; }

        // Foreign Key
        public int CharacterId { get; set; }
        public int MagicClassId { get; set; }
        public int? SpecializationId { get; set; } //Optional, da nicht jeder Character eine Spezialisierung hat

        // Navigation Properties
        public virtual Character Character { get; set; } = null!;
        public virtual MagicClass MagicClass { get; set; } = null!;
        public virtual Specialization? Specialization { get; set; }

    }
}
