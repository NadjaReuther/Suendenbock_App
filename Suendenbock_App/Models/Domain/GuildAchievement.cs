namespace Suendenbock_App.Models.Domain
{
    public class GuildAchievement
    {
        public int Id { get; set; }
        public int GuildId { get; set; }
        public int AchievementId { get; set; }
        public DateTime UnlockedAt { get; set; } = DateTime.Now;
        public int Progress { get; set; } = 0; // Fortschritt für gestaffelte Achievements

        // Navigation Properties
        public Guild Guild { get; set; } = null!;
        public Achievement Achievement { get; set; } = null!;
    }
}
