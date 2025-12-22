using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.Domain
{
    public class Monster
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string? Basics { get; set; }
        public string? Description { get; set; }
        public string? ProcessedDescription { get; set; }
        public bool meet { get; set; } = false;
        public bool encounter { get; set; } = false;
        public bool perfected { get; set; } = false;

        /// <summary>
        /// Effekt wenn die Trophäe gekauft wurde (schwächerer Effekt)
        /// </summary>
        [Required]
        [StringLength(200)]
        public string BaseEffect { get; set; } = string.Empty;

        /// <summary>
        /// Effekt wenn das Monster besiegt wurde (stärkerer Effekt)
        /// </summary>
        [Required]
        [StringLength(200)]
        public string SlainEffect { get; set; } = string.Empty;

        // ===== STATUS & AUSRÜSTUNG (für die Guild) =====

        /// <summary>
        /// Status für die Guild "Wolkenbruch":
        /// "none" = noch nicht erworben
        /// "bought" = gekauft
        /// "slain" = besiegt
        /// "both" = gekauft UND später besiegt (seltener Fall)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "none"; // "none", "bought", "slain", "both"

        /// <summary>
        /// Ist diese Trophäe gerade von der Guild ausgerüstet?
        /// Maximum 3 Trophäen gleichzeitig ausgerüstet!
        /// </summary>
        public bool IsEquipped { get; set; } = false;

        // Foreign Key
        public int MonstertypId { get; set; }
        // Navigation Property Monster has a Monstertyp
        public virtual Monstertyp? Monstertyp { get; set; }
        // Foreign Key
    }
}
