using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly ApplicationDbContext _context;

        public AchievementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CheckCharacterAchievements(string userId)
        {
            // Finde den Character des Users
            var character = await _context.Characters
                .Include(c => c.Rasse)
                .Include(c => c.Lebensstatus)
                .Include(c => c.Eindruck)
                .Include(c => c.Details)
                .Include(c => c.Vater)
                .Include(c => c.Mutter)
                .Include(c => c.Partner)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (character == null) return;

            // Berechne Steckbrief-Vollständigkeit
            var completionPercentage = CalculateCharacterCompletionPercentage(character);

            // Prüfe Achievements
            await CheckAndAwardAchievement(userId, "character_basics_filled", completionPercentage >= 20);
            await CheckAndAwardAchievement(userId, "character_50_percent", completionPercentage >= 50);
            await CheckAndAwardAchievement(userId, "character_80_percent", completionPercentage >= 80);

            // Prüfe Familien-Achievements
            var familyRelationsCount = 0;
            if (character.VaterId.HasValue) familyRelationsCount++;
            if (character.MutterId.HasValue) familyRelationsCount++;
            if (character.PartnerId.HasValue) familyRelationsCount++;

            await CheckAndAwardAchievement(userId, "family_one_relation", familyRelationsCount >= 1);
            await CheckAndAwardAchievement(userId, "family_complete", familyRelationsCount >= 3);
        }

        public async Task CheckGuildAchievements(int guildId)
        {
            var guild = await _context.Guilds
                .FirstOrDefaultAsync(g => g.Id == guildId);

            if (guild == null) return;

            // Nur für die Wolkenbruch-Gilde prüfen
            if (guild.Name != "Wolkenbruch") return;

            // Zähle Mitglieder über CharacterAffiliations
            var memberCount = await _context.CharacterAffiliations
                .CountAsync(ca => ca.GuildId == guildId);

            // Prüfe Gilden-Größen-Achievements
            await CheckAndAwardGuildAchievement(guildId, "guild_10_members", memberCount >= 10);
            await CheckAndAwardGuildAchievement(guildId, "guild_25_members", memberCount >= 25);
            await CheckAndAwardGuildAchievement(guildId, "guild_50_members", memberCount >= 50);

            // Prüfe Bestiarium-Achievements
            var unlockedMonstersCount = await _context.Monsters.CountAsync(m => m.meet);

            await CheckAndAwardGuildAchievement(guildId, "bestiary_first_encounter", unlockedMonstersCount >= 1);
            await CheckAndAwardGuildAchievement(guildId, "bestiary_10_monsters", unlockedMonstersCount >= 10);
            await CheckAndAwardGuildAchievement(guildId, "bestiary_25_monsters", unlockedMonstersCount >= 25);

            var totalMonsters = await _context.Monsters.CountAsync();
            await CheckAndAwardGuildAchievement(guildId, "bestiary_all_monsters", unlockedMonstersCount >= totalMonsters && totalMonsters > 0);
        }

        public async Task CheckAdventCalendarAchievements(string userId)
        {
            // Zähle geöffnete Türchen (über AdventDoor.DayNumber)
            var openedDoors = await _context.UserAdventChoices
                .Include(uac => uac.AdventDoor)
                .Where(uac => uac.UserId == userId)
                .Select(uac => uac.AdventDoor.DayNumber)
                .Distinct()
                .ToListAsync();

            var openedDoorsCount = openedDoors.Count;

            await CheckAndAwardAchievement(userId, "advent_first_door", openedDoorsCount >= 1);
            await CheckAndAwardAchievement(userId, "advent_12_doors", openedDoorsCount >= 12);
            await CheckAndAwardAchievement(userId, "advent_all_doors", openedDoorsCount >= 24);
        }

        public async Task CheckKnowledgeAchievements(string userId)
        {
            // Hinweis: Wir brauchen ein Tracking für "angesehene" Glossar-Einträge
            // Das könnte in einer separaten Tabelle UserGlossaryViews gespeichert werden
            // Für jetzt lassen wir das offen, kann später implementiert werden

            // Platzhalter für zukünftige Implementierung
            await Task.CompletedTask;
        }

        public async Task<List<UserAchievement>> GetUserAchievements(string userId)
        {
            return await _context.UserAchievements
                .Include(ua => ua.Achievement)
                .Where(ua => ua.UserId == userId)
                .OrderByDescending(ua => ua.UnlockedAt)
                .ToListAsync();
        }

        public async Task<List<GuildAchievement>> GetGuildAchievements(int guildId)
        {
            return await _context.GuildAchievements
                .Include(ga => ga.Achievement)
                .Where(ga => ga.GuildId == guildId)
                .OrderByDescending(ga => ga.UnlockedAt)
                .ToListAsync();
        }

        public async Task<List<Achievement>> GetNewlyUnlockedAchievements(string userId)
        {
            // Achievements die in den letzten 5 Minuten freigeschaltet wurden
            var fiveMinutesAgo = DateTime.Now.AddMinutes(-5);

            return await _context.UserAchievements
                .Include(ua => ua.Achievement)
                .Where(ua => ua.UserId == userId && ua.UnlockedAt >= fiveMinutesAgo)
                .Select(ua => ua.Achievement)
                .ToListAsync();
        }

        // ============= PRIVATE HELPER METHODS =============

        private async Task<bool> CheckAndAwardAchievement(string userId, string achievementKey, bool condition)
        {
            if (!condition) return false;

            // Prüfe ob Achievement bereits vergeben wurde
            var achievement = await _context.Achievements
                .FirstOrDefaultAsync(a => a.Key == achievementKey);

            if (achievement == null) return false;

            var existingUserAchievement = await _context.UserAchievements
                .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementId == achievement.Id);

            if (existingUserAchievement != null) return false;

            // Achievement vergeben
            var userAchievement = new UserAchievement
            {
                UserId = userId,
                AchievementId = achievement.Id,
                UnlockedAt = DateTime.Now
            };

            _context.UserAchievements.Add(userAchievement);
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<bool> CheckAndAwardGuildAchievement(int guildId, string achievementKey, bool condition)
        {
            if (!condition) return false;

            var achievement = await _context.Achievements
                .FirstOrDefaultAsync(a => a.Key == achievementKey);

            if (achievement == null) return false;

            var existingGuildAchievement = await _context.GuildAchievements
                .FirstOrDefaultAsync(ga => ga.GuildId == guildId && ga.AchievementId == achievement.Id);

            if (existingGuildAchievement != null) return false;

            var guildAchievement = new GuildAchievement
            {
                GuildId = guildId,
                AchievementId = achievement.Id,
                UnlockedAt = DateTime.Now
            };

            _context.GuildAchievements.Add(guildAchievement);
            await _context.SaveChangesAsync();

            return true;
        }

        private int CalculateCharacterCompletionPercentage(Character character)
        {
            int totalFields = 0;
            int filledFields = 0;

            // Basis-Felder (immer ausgefüllt)
            totalFields += 5; // Vorname, Nachname, Rufname, Geschlecht, RasseId
            filledFields += 5;

            // Optionale Character-Felder
            var characterOptionalFields = new List<object?>
            {
                character.Geburtsdatum,
                character.ImagePath,
                character.VaterId,
                character.MutterId,
                character.PartnerId
            };

            totalFields += characterOptionalFields.Count;
            filledFields += characterOptionalFields.Count(f => f != null && (f is not string || !string.IsNullOrWhiteSpace((string)f)));

            // CharacterDetails-Felder
            if (character.Details != null)
            {
                var detailsFields = new List<object?>
                {
                    character.Details.quote,
                    character.Details.urheber,
                    character.Details.Description,
                    character.Details.Beruf,
                    character.Details.BodyHeight,
                    character.Details.StandId,
                    character.Details.BlutgruppeId,
                    character.Details.HausId,
                    character.Details.HerkunftslandId
                };

                totalFields += detailsFields.Count;
                filledFields += detailsFields.Count(f => f != null && (f is not string || !string.IsNullOrWhiteSpace((string)f)));
            }
            else
            {
                totalFields += 9; // Anzahl der Details-Felder
            }

            return totalFields > 0 ? (int)((filledFields * 100.0) / totalFields) : 0;
        }
    }
}
