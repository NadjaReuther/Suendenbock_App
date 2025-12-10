namespace Suendenbock_App.Models.Domain
{
    public class UserAchievement
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int AchievementId { get; set; }
        public DateTime UnlockedAt { get; set; } = DateTime.Now;
        public int Progress { get; set; } = 0; // Fortschritt für gestaffelte Achievements

        // Navigation Properties
        public ApplicationUser User { get; set; } = null!;
        public Achievement Achievement { get; set; } = null!;
    }
}
