using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    /// <summary>
    /// Biom-Masterdata für das Kampfsystem.
    /// Repräsentiert Umgebungen wie Wüste, Wald, Sumpf, etc.
    /// Wird von Gott über das Admin-Dashboard verwaltet.
    /// Nur ein Biom kann gleichzeitig aktiv sein (im Gegensatz zu Feldeffekten).
    /// </summary>
    public class Biom
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Der Name ist erforderlich")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Beschreibung { get; set; }

        /// <summary>
        /// Fremdschlüssel zur LightCard für Farbdarstellung des Bioms
        /// (Hintergrundfarbe im Combat)
        /// </summary>
        [Required]
        public int LightCardId { get; set; }

        /// <summary>
        /// Navigationseigenschaft zur LightCard
        /// </summary>
        public virtual LightCard? LightCard { get; set; }
    }
}
