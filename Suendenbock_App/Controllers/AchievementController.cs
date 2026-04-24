using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Models.ViewModels;

namespace Suendenbock_App.Controllers
{
    public class AchievementController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AchievementController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Achievement-Statistiken
            var totalUserAchievements = _context.Achievements.Count(a => a.Scope == AchievementScope.User);
            var totalGuildAchievements = _context.Achievements.Count(a => a.Scope == AchievementScope.Guild);
            var unlockedUserAchievements = _context.UserAchievements.Select(ua => ua.AchievementId).Distinct().Count();
            var unlockedGuildAchievements = _context.GuildAchievements.Select(ga => ga.AchievementId).Distinct().Count();
            var totalAchievementPoints = _context.UserAchievements
                .Include(ua => ua.Achievement)
                .Sum(ua => ua.Achievement.Points) +
                _context.GuildAchievements
                .Include(ga => ga.Achievement)
                .Sum(ga => ga.Achievement.Points);

            var viewModel = new AchievementStatisticsViewModel
            {
                TotalUserAchievements = totalUserAchievements,
                TotalGuildAchievements = totalGuildAchievements,
                UnlockedUserAchievements = unlockedUserAchievements,
                UnlockedGuildAchievements = unlockedGuildAchievements,
                TotalAchievementPoints = totalAchievementPoints
            };

            return View(viewModel);
        }
    }
}
