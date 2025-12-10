namespace Suendenbock_App.Models.Domain
{
    public class Achievement
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty; // Eindeutiger Schlüssel z.B. "character_basics_filled"
        public string Name { get; set; } = string.Empty; // z.B. "Grundlagen gelegt"
        public string Description { get; set; } = string.Empty; // Beschreibung des Achievements
        public string Icon { get; set; } = "🏆"; // Emoji oder Icon
        public AchievementCategory Category { get; set; }
        public AchievementScope Scope { get; set; } // User oder Guild
        public int Points { get; set; } = 10; // Achievement-Punkte
        public bool IsSecret { get; set; } = false; // Versteckte Achievements

        // Für gestaffelte Achievements (z.B. Bronze, Silber, Gold)
        public int? RequiredCount { get; set; } // z.B. 10 Monster für "Monsterjäger"
        public string? EntityType { get; set; } // z.B. "Monstertyp" für typ-spezifische Achievements
        public int? EntityId { get; set; } // z.B. ID des Monstertyps

        // Navigation Properties
        public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
        public ICollection<GuildAchievement> GuildAchievements { get; set; } = new List<GuildAchievement>();
    }

    public enum AchievementCategory
    {
        CharacterCompletion,    // Steckbrief-Ausfüllung
        CharacterRelations,     // Familie & Beziehungen
        GuildSize,              // Gilden-Größe
        Bestiary,               // Bestiarium
        BestiaryType,           // Typ-spezifische Monster
        AdventCalendar,         // Weihnachtsabenteuer
        Knowledge               // Wissens-Achievements
    }

    public enum AchievementScope
    {
        User,   // Spieler-Achievement
        Guild   // Gilden-Achievement
    }
}
