namespace Suendenbock_App.Models.Domain
{
    public class CharacterMagicClass
    {
        public int Id { get; set; }
                // Foreign Key for Character
        public int CharacterId { get; set; } // Foreign Key for Character
        // Navigation Property for Character
        public virtual Character? Character { get; set; } // Navigation property for Character
        // Foreign Key for MagicClass
        public int MagicClassId { get; set; } // Foreign Key for MagicClass
        // Navigation Property for MagicClass
        public virtual MagicClass? MagicClass { get; set; } // Navigation property for MagicClass

        // Foreign Key for MagicSpecialization (optional)
        public int? MagicClassSpecializationId { get; set; } // Foreign Key for MagicSpecialization
        // Navigation Property for MagicSpecialization (optional)
        public virtual MagicClassSpecialization? MagicClassSpecialization { get; set; } // Navigation property for MagicSpecialization
    }
}
