using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models
{
    /// <summary>
    /// Klickbarer Polygon-Bereich auf einer Weltkarte
    /// Repräsentiert eine Region, die zu einer Detail-Karte führt
    /// </summary>
    public class MapRegion
    {
        public int Id { get; set; }

        /// <summary>
        /// Auf welcher Karte ist dieser Bereich? (Weltkarte)
        /// </summary>
        public int MapId { get; set; }
        public Map Map { get; set; } = null!;

        /// <summary>
        /// Zu welcher Detail-Karte führt dieser Bereich?
        /// </summary>
        public int LinkedMapId { get; set; }
        public Map LinkedMap { get; set; } = null!;

        /// <summary>
        /// Name der Region (z.B. "Königreich Nord")
        /// </summary>
        [Required]
        [StringLength(200)]
        public string RegionName { get; set; } = string.Empty;

        /// <summary>
        /// Polygon-Punkte als JSON
        /// Format: [{"x":10.5,"y":20.3},{"x":30.1,"y":40.8},...]
        /// Koordinaten sind in Prozent (0-100)
        /// </summary>
        [Required]
        public string PolygonPoints { get; set; } = "[]";

        /// <summary>
        /// Farbe für Rand und Füllung des Polygons (z.B. "#B89E68" für Gold)
        /// </summary>
        [StringLength(50)]
        public string? BorderColor { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
