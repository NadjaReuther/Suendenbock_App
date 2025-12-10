using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Services
{
    public interface IAchievementService
    {
        /// <summary>
        /// Prüft und vergibt Character-Achievements für einen Benutzer
        /// </summary>
        Task CheckCharacterAchievements(string userId);

        /// <summary>
        /// Prüft und vergibt Gilden-Achievements für eine Gilde
        /// </summary>
        Task CheckGuildAchievements(int guildId);

        /// <summary>
        /// Prüft und vergibt Adventskalender-Achievements für einen Benutzer
        /// </summary>
        Task CheckAdventCalendarAchievements(string userId);

        /// <summary>
        /// Prüft und vergibt Wissens-Achievements für einen Benutzer
        /// </summary>
        Task CheckKnowledgeAchievements(string userId);

        /// <summary>
        /// Gibt alle Achievements eines Benutzers zurück
        /// </summary>
        Task<List<UserAchievement>> GetUserAchievements(string userId);

        /// <summary>
        /// Gibt alle Achievements einer Gilde zurück
        /// </summary>
        Task<List<GuildAchievement>> GetGuildAchievements(int guildId);

        /// <summary>
        /// Gibt neu freigeschaltete Achievements zurück (für Benachrichtigungen)
        /// </summary>
        Task<List<Achievement>> GetNewlyUnlockedAchievements(string userId);
    }
}
