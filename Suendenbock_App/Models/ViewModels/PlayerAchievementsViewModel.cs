using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Models.ViewModels
{
    public class PlayerAchievementsViewModel
    {
        public int UnlockedCount { get; set; }
        public int TotalCount { get; set; }
        public int TotalPoints { get; set; }
        public int MaxPoints { get; set; }
        public Dictionary<AchievementCategory, List<AchievementItem>> AchievementsByCategory { get; set; } = new();
    }

    public class AchievementItem
    {
        public Achievement Achievement { get; set; } = null!;
        public bool IsUnlocked { get; set; }
        public DateTime? UnlockedAt { get; set; }
    }
}
