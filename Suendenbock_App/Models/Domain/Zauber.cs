namespace Suendenbock_App.Models.Domain
{
    public class Zauber
    {
        public int Id { get; set; }
        public string Spruch { get; set; } = string.Empty;
        public string Wirkung { get; set; } = string.Empty;
        public int Stufe { get; set; } = 1; // Default value for Stufe
        public int Slots { get; set; } = 0; 
        public string Effekt { get; set; } = string.Empty;
        // Foreign Key
        public int ZaubertypID { get; set; } // Foreign for ZaubertypID
        public int MagicClassSpecializationId { get; set; } // Foreign Key for MagicSpecialization
        // Navigation Property
        public virtual MagicClassSpecialization? MagicSpecialization { get; set; } // Navigation property for MagicSpecialization
    }
}
