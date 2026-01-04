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

        // ===== SESSION-DATEN (für GenerateSession) =====

        /// <summary>
        /// Land des Akts (z.B. "Böhmen", "Bayern", etc.)
        /// </summary>
        [StringLength(100)]
        public string? Country { get; set; }

        /// <summary>
        /// Erster Begleiter (Pflicht)
        /// </summary>
        [StringLength(100)]
        public string? Companion1 { get; set; }

        /// <summary>
        /// Zweiter Begleiter (Optional)
        /// </summary>
        [StringLength(100)]
        public string? Companion2 { get; set; }

        /// <summary>
        /// Monat des Akts (z.B. "Januar", "Februar", etc.)
        /// </summary>
        [StringLength(50)]
        public string? Month { get; set; }

        /// <summary>
        /// Aktuelles Wetter (z.B. "Kalter, klarer Himmel")
        /// </summary>
        [StringLength(200)]
        public string? Weather { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ===== BEZIEHUNGEN =====

        /// <summary>
        /// Die Karte für diesen Akt (One-to-One)
        /// </summary>
        public Map? Map { get; set; }

        /// <summary>
        /// Alle Quests, die zu diesem Akt gehören
        /// </summary>
        public List<Quest> Quests { get; set; } = new();
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
        /// Pfad zum Kartenbild (z.B. /images/maps/act-1-map.png)
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