using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    /// <summary>
    /// Repräsentiert eine Spielwelt (z.B. "Sündenbock 1618" oder "1810 Season Zwei")
    /// Characters können einer oder mehreren GameWorlds zugeordnet werden
    /// </summary>
    public class GameWorld
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Beschreibung")]
        [MaxLength(500)]
        public string? Description { get; set; }

        [Display(Name = "Jahr/Epoche")]
        [MaxLength(50)]
        public string? Era { get; set; }

        [Display(Name = "Farbcode")]
        [MaxLength(7)] // z.B. #FF5733
        public string? ColorCode { get; set; }

        [Display(Name = "Aktiv")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Erstellt am")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Sortierung")]
        public int SortOrder { get; set; } = 0;

        /// <summary>
        /// Originalwelt (z.B. "Sündenbock 1618"). Es gibt genau eine; festgelegt in der Spielwelt-Übersicht.
        /// Wer einer anderen Welt zugeordnet wird, sieht dort, ob er auch in der Originalwelt existiert.
        /// </summary>
        [Display(Name = "Originalwelt")]
        public bool IsOriginWorld { get; set; } = false;

        // Navigation Properties
        public virtual ICollection<CharacterGameWorld> CharacterGameWorlds { get; set; } = new List<CharacterGameWorld>();
    }
}
