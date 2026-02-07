using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    /// <summary>
    /// Feldeffekt-Masterdata für das Kampfsystem.
    /// Wird von Gott über das Admin-Dashboard verwaltet.
    /// Aktive Feldeffekte werden in der CombatSession gespeichert.
    /// </summary>
    public class FeldEffekt
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Der Name ist erforderlich")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Beschreibung { get; set; }

        /// <summary>
        /// Fremdschlüssel zur LightCard für Farbdarstellung
        /// </summary>
        [Required]
        public int LightCardId { get; set; }

        /// <summary>
        /// Navigationseigenschaft zur LightCard
        /// </summary>
        public virtual LightCard? LightCard { get; set; }
    }
}
