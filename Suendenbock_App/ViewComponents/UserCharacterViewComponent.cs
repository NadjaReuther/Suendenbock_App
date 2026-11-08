using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.ViewComponents
{
    public class UserCharacterViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserCharacterViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return View("Default", new UserCharacterViewModel());
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return View("Default", new UserCharacterViewModel());
            }

            // Hauptcharakter (Sündenbock): nur er trägt Character.UserId
            var currentCharacter = await _context.Characters
                .Include(c => c.CharacterGameWorlds)
                    .ThenInclude(cgw => cgw.GameWorld)
                .Where(c => c.UserId == user.Id)
                .OrderBy(c => c.Id)
                .FirstOrDefaultAsync();

            var currentGameWorld = currentCharacter?.CharacterGameWorlds
                .Select(cgw => cgw.GameWorld)
                .FirstOrDefault(gw => gw.IsOriginWorld);

            // Link "Zu Hesparis": wer dort einen Nebencharakter hat (oder Gott)
            bool hatHesparis = User.IsInRole("Gott") || await _context.CharacterGameWorlds
                .AnyAsync(cgw => cgw.UserId == user.Id && cgw.GameWorld.Name.Contains("Hesparis"));

            var viewModel = new UserCharacterViewModel
            {
                CharacterId = currentCharacter?.Id,
                CharacterImagePath = currentCharacter?.ImagePath,
                UserInitial = user.Name?.Substring(0, 1) ?? user.UserName?.Substring(0, 1) ?? "?",
                CurrentCharacter = currentCharacter,
                CurrentGameWorld = currentGameWorld,
                HatHesparis = hatHesparis
            };

            return View("Default", viewModel);
        }
    }

    public class UserCharacterViewModel
    {
        public int? CharacterId { get; set; }
        public string? CharacterImagePath { get; set; }
        public string UserInitial { get; set; } = "?";
        public Character? CurrentCharacter { get; set; }
        public GameWorld? CurrentGameWorld { get; set; }
        /// <summary>Zeigt im Menü den Link "Zu Hesparis".</summary>
        public bool HatHesparis { get; set; }
    }
}
