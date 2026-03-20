using System.ComponentModel.DataAnnotations.Schema;

namespace Suendenbock_App.Models.Domain
{
    public class GlossaryEntry
    {
        public int Id { get; set; }

        // Titel für freie Einträge oder Anzeigename
        public string Title { get; set; } = string.Empty;

        // Kategorie für freie Einträge (z.B. "Geschichte", "Geografie", "Kultur")
        public string? Category { get; set; }

        // Polymorphe Beziehung zu verschiedenen Entitäten
        public string? EntityType { get; set; } // z.B. "Rasse", "MagicClass", "Obermagie", "Blutgruppe", etc.
        public int? EntityId { get; set; } // ID der verknüpften Entität

        // Beschreibung mit Entity-Mentions
        public string Description { get; set; } = string.Empty;

        // Bild-Pfad
        public string? ImagePath { get; set; }

        // Navigation Properties für polymorphe Beziehungen
        // [NotMapped] verhindert, dass EF Core FK-Spalten dafür anlegt
        [NotMapped] public Rasse? Rasse { get; set; }
        [NotMapped] public MagicClass? MagicClass { get; set; }
        [NotMapped] public Obermagie? Obermagie { get; set; }
        [NotMapped] public Blutgruppe? Blutgruppe { get; set; }
        [NotMapped] public Haus? Haus { get; set; }
        [NotMapped] public Herkunftsland? Herkunftsland { get; set; }
        [NotMapped] public Religion? Religion { get; set; }
        [NotMapped] public Infanterierang? Infanterierang { get; set; }
        [NotMapped] public Stand? Stand { get; set; }
    }
}
