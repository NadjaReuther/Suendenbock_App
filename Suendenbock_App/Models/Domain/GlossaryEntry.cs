namespace Suendenbock_App.Models.Domain
{
    public class GlossaryEntry
    {
        public int Id { get; set; }

        // Titel für freie Einträge oder Anzeigename
        public string Title { get; set; } = string.Empty;

        // Polymorphe Beziehung zu verschiedenen Entitäten
        public string? EntityType { get; set; } // z.B. "Rasse", "MagicClass", "Obermagie", "Blutgruppe", etc.
        public int? EntityId { get; set; } // ID der verknüpften Entität

        // Beschreibung mit Entity-Mentions
        public string Description { get; set; } = string.Empty;

        // Bild-Pfad
        public string? ImagePath { get; set; }

        // Navigation Properties für polymorphe Beziehungen
        // Diese werden nicht direkt in der DB gemappt, sondern zur Laufzeit geladen
        public Rasse? Rasse { get; set; }
        public MagicClass? MagicClass { get; set; }
        public Obermagie? Obermagie { get; set; }
        public Blutgruppe? Blutgruppe { get; set; }
        public Haus? Haus { get; set; }
        public Herkunftsland? Herkunftsland { get; set; }
        public Religion? Religion { get; set; }
        public Infanterierang? Infanterierang { get; set; }
        public Stand? Stand { get; set; }
    }
}
