// Models/Act.cs
using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models
{
    /// <summary>
    /// Ein Akt im Spiel (z.B. "Akt 1: Der Aufbruch")
    /// Jeder Akt hat eine eigene Karte
    /// </summary>
    public class Act
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Nummer des Akts (1, 2, 3, ...)
        /// </summary>
        public int ActNumber { get; set; }

        /// <summary>
        /// Ist dieser Akt gerade aktiv?
        /// </summary>
        public bool IsActive { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ===== BEZIEHUNGEN =====

        /// <summary>
        /// Die Karte für diesen Akt (One-to-One)
        /// </summary>
        public Map? Map { get; set; }
    }

    /// <summary>
    /// Eine Karte für einen Akt
    /// </summary>
    public class Map
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// URL zum Kartenbild
        /// </summary>
        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Zu welchem Akt gehört diese Karte? (One-to-One)
        /// </summary>
        public int ActId { get; set; }
        public Act Act { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ===== BEZIEHUNGEN =====

        /// <summary>
        /// Orte/Marker auf dieser Karte
        /// </summary>
        public List<MapMarker> Markers { get; set; } = new();
    }

    /// <summary>
    /// Ein Marker/Ort auf einer Karte
    /// </summary>
    public class MapMarker
    {
        public int Id { get; set; }

        /// <summary>
        /// Zu welcher Karte gehört dieser Marker?
        /// </summary>
        public int MapId { get; set; }
        public Map Map { get; set; } = null!;

        /// <summary>
        /// X-Position in Prozent (0-100)
        /// </summary>
        [Range(0, 100)]
        public double XPercent { get; set; }

        /// <summary>
        /// Y-Position in Prozent (0-100)
        /// </summary>
        [Range(0, 100)]
        public double YPercent { get; set; }

        [Required]
        [StringLength(200)]
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Marker-Typ: "quest", "info", "danger", "settlement"
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = "info"; // "quest", "info", "danger", "settlement"

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ===== BEZIEHUNGEN =====

        /// <summary>
        /// Quests, die mit diesem Marker verbunden sind
        /// </summary>
        public List<Quest> Quests { get; set; } = new();
    }
}