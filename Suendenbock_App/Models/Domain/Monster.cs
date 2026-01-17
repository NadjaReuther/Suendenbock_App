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
        /// Hat die Guild dieses Monster gekauft? (schwächere Trophäe)
        /// </summary>
        public bool HasBoughtTrophy { get; set; } = false;

        /// <summary>
        /// Hat die Guild dieses Monster erlegt? (stärkere Trophäe)
        /// </summary>
        public bool HasSlainTrophy { get; set; } = false;

        /// <summary>
        /// Effekt wenn die Trophäe gekauft wurde (schwächerer Effekt)
        /// Nur relevant wenn HasBoughtTrophy = true
        /// </summary>
        [StringLength(500)]
        public string? BaseEffect { get; set; }

        /// <summary>
        /// Effekt wenn das Monster besiegt wurde (stärkerer Effekt)
        /// Nur relevant wenn HasSlainTrophy = true
        /// </summary>
        [StringLength(500)]
        public string? SlainEffect { get; set; }

        /// <summary>
        /// Ist diese Trophäe gerade von der Guild ausgerüstet?
        /// Maximum 3 Trophäen gleichzeitig ausgerüstet!
        /// </summary>
        public bool IsEquipped { get; set; } = false;

        /// <summary>
        /// Position an der Wand (1, 2, oder 3)
        /// null = nicht ausgerüstet
        /// </summary>
        public int? EquippedPosition { get; set; }

        /// <summary>
        /// Welche Variante bevorzugt die Guild, wenn beide verfügbar sind?
        /// "bought" = gekaufte Variante wird genutzt
        /// "slain" = erlegte Variante wird genutzt
        /// null = keine Präferenz gesetzt (Default: slain wenn verfügbar)
        /// </summary>
        [StringLength(50)]
        public string? PreferredVariant { get; set; }

        /// <summary>
        /// Berechneter Status basierend auf HasBoughtTrophy und HasSlainTrophy
        /// "none" = noch nicht erworben
        /// "bought" = nur gekauft ODER beide vorhanden + Präferenz "bought"
        /// "slain" = nur erlegt ODER beide vorhanden + Präferenz "slain"
        /// "both" = gekauft UND erlegt (wird nur intern verwendet)
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Status
        {
            get
            {
                if (HasBoughtTrophy && HasSlainTrophy)
                {
                    // Beide vorhanden → Präferenz nutzen (Default: slain)
                    return PreferredVariant == "bought" ? "bought" : "slain";
                }
                if (HasSlainTrophy) return "slain";
                if (HasBoughtTrophy) return "bought";
                return "none";
            }
        }

        /// <summary>
        /// Hat die Guild beide Varianten? (für UI-Toggle-Button)
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public bool HasBothVariants => HasBoughtTrophy && HasSlainTrophy;

        /// <summary>
        /// Ist diese Trophäe überhaupt in der Truhe? (mindestens eine Variante erworben)
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public bool IsInTrophyChest => HasBoughtTrophy || HasSlainTrophy;

        // Foreign Key
        public int MonstertypId { get; set; }
        // Navigation Property Monster has a Monstertyp
        public virtual Monstertyp? Monstertyp { get; set; }
        // Foreign Key
    }
}
