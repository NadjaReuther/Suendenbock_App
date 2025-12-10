using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Data
{
    public static class AchievementSeeder
    {
        public static void SeedAchievements(ApplicationDbContext context)
        {
            // Nur seeden, wenn noch keine Achievements vorhanden sind
            if (context.Achievements.Any())
            {
                return;
            }

            var achievements = new List<Achievement>
            {
                // ===========================
                // CHARACTER-ACHIEVEMENTS
                // ===========================

                // Steckbrief-Completion
                new Achievement
                {
                    Key = "character_basics_filled",
                    Name = "Grundlagen gelegt",
                    Description = "Basis-Felder des Charakterbogens ausgefüllt",
                    Icon = "📝",
                    Category = AchievementCategory.CharacterCompletion,
                    Scope = AchievementScope.User,
                    Points = 10
                },
                new Achievement
                {
                    Key = "character_50_percent",
                    Name = "Detailverliebt",
                    Description = "50% des Charakterbogens ausgefüllt",
                    Icon = "📋",
                    Category = AchievementCategory.CharacterCompletion,
                    Scope = AchievementScope.User,
                    Points = 25,
                    RequiredCount = 50
                },
                new Achievement
                {
                    Key = "character_80_percent",
                    Name = "Perfektionist",
                    Description = "80% des Charakterbogens ausgefüllt",
                    Icon = "⭐",
                    Category = AchievementCategory.CharacterCompletion,
                    Scope = AchievementScope.User,
                    Points = 50,
                    RequiredCount = 80
                },

                // Familie & Beziehungen
                new Achievement
                {
                    Key = "family_one_relation",
                    Name = "Familienbande",
                    Description = "Mindestens eine familiäre Beziehung eingetragen (Vater, Mutter oder Partner)",
                    Icon = "👨‍👩‍👧",
                    Category = AchievementCategory.CharacterRelations,
                    Scope = AchievementScope.User,
                    Points = 15
                },
                new Achievement
                {
                    Key = "family_complete",
                    Name = "Stammbaum",
                    Description = "Alle drei familiären Beziehungen eingetragen (Vater, Mutter und Partner)",
                    Icon = "🌳",
                    Category = AchievementCategory.CharacterRelations,
                    Scope = AchievementScope.User,
                    Points = 30
                },

                // ===========================
                // GILDEN-ACHIEVEMENTS
                // ===========================

                // Gilden-Größe
                new Achievement
                {
                    Key = "guild_10_members",
                    Name = "Wachsende Gemeinschaft",
                    Description = "Gilde hat 10 Mitglieder erreicht",
                    Icon = "👥",
                    Category = AchievementCategory.GuildSize,
                    Scope = AchievementScope.Guild,
                    Points = 25,
                    RequiredCount = 10
                },
                new Achievement
                {
                    Key = "guild_25_members",
                    Name = "Große Gemeinschaft",
                    Description = "Gilde hat 25 Mitglieder erreicht",
                    Icon = "👨‍👨‍👧‍👦",
                    Category = AchievementCategory.GuildSize,
                    Scope = AchievementScope.Guild,
                    Points = 50,
                    RequiredCount = 25
                },
                new Achievement
                {
                    Key = "guild_50_members",
                    Name = "Sehr große Gilde",
                    Description = "Gilde hat 50 Mitglieder erreicht",
                    Icon = "🏰",
                    Category = AchievementCategory.GuildSize,
                    Scope = AchievementScope.Guild,
                    Points = 100,
                    RequiredCount = 50
                },

                // Bestiarium
                new Achievement
                {
                    Key = "bestiary_first_encounter",
                    Name = "Erste Begegnung",
                    Description = "Erstes Monster freigeschaltet",
                    Icon = "👹",
                    Category = AchievementCategory.Bestiary,
                    Scope = AchievementScope.Guild,
                    Points = 10,
                    RequiredCount = 1
                },
                new Achievement
                {
                    Key = "bestiary_10_monsters",
                    Name = "Monsterjäger",
                    Description = "10 Monster freigeschaltet",
                    Icon = "⚔️",
                    Category = AchievementCategory.Bestiary,
                    Scope = AchievementScope.Guild,
                    Points = 30,
                    RequiredCount = 10
                },
                new Achievement
                {
                    Key = "bestiary_25_monsters",
                    Name = "Bestienkenner",
                    Description = "25 Monster freigeschaltet",
                    Icon = "🗡️",
                    Category = AchievementCategory.Bestiary,
                    Scope = AchievementScope.Guild,
                    Points = 75,
                    RequiredCount = 25
                },
                new Achievement
                {
                    Key = "bestiary_all_monsters",
                    Name = "Meister des Bestiariums",
                    Description = "Alle Monster freigeschaltet",
                    Icon = "🏆",
                    Category = AchievementCategory.Bestiary,
                    Scope = AchievementScope.Guild,
                    Points = 200
                },

                // ===========================
                // WEIHNACHTSABENTEUER
                // ===========================

                new Achievement
                {
                    Key = "advent_first_door",
                    Name = "Türchen-Öffner",
                    Description = "Erstes Türchen des Adventskalenders geöffnet",
                    Icon = "🎄",
                    Category = AchievementCategory.AdventCalendar,
                    Scope = AchievementScope.User,
                    Points = 5,
                    RequiredCount = 1
                },
                new Achievement
                {
                    Key = "advent_12_doors",
                    Name = "Fleißiger Adventskalender-Leser",
                    Description = "12 Türchen des Adventskalenders geöffnet",
                    Icon = "🎅",
                    Category = AchievementCategory.AdventCalendar,
                    Scope = AchievementScope.User,
                    Points = 25,
                    RequiredCount = 12
                },
                new Achievement
                {
                    Key = "advent_all_doors",
                    Name = "Advent-Enthusiast",
                    Description = "Alle 24 Türchen des Adventskalenders geöffnet",
                    Icon = "🌟",
                    Category = AchievementCategory.AdventCalendar,
                    Scope = AchievementScope.User,
                    Points = 50,
                    RequiredCount = 24
                },

                // ===========================
                // WISSENS-ACHIEVEMENTS
                // ===========================

                new Achievement
                {
                    Key = "knowledge_5_entries",
                    Name = "Wissensdurstig",
                    Description = "5 Glossar-Einträge angesehen",
                    Icon = "📚",
                    Category = AchievementCategory.Knowledge,
                    Scope = AchievementScope.User,
                    Points = 10,
                    RequiredCount = 5
                },
                new Achievement
                {
                    Key = "knowledge_25_entries",
                    Name = "Gelehrter",
                    Description = "25 Glossar-Einträge angesehen",
                    Icon = "🎓",
                    Category = AchievementCategory.Knowledge,
                    Scope = AchievementScope.User,
                    Points = 30,
                    RequiredCount = 25
                }
            };

            context.Achievements.AddRange(achievements);
            context.SaveChanges();
        }
    }
}
