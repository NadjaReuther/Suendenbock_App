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

            // AKTUELLEN Character des Users laden (mit CurrentCharacterId)
            var currentCharacter = user.CurrentCharacterId.HasValue
                ? await _context.Characters
                    .Include(c => c.CharacterGameWorlds)
                        .ThenInclude(cgw => cgw.GameWorld)
                    .FirstOrDefaultAsync(c => c.Id == user.CurrentCharacterId.Value)
                : null;

            // ALLE Characters des Users laden
            var allUserCharacters = await _context.Characters
                .Include(c => c.CharacterGameWorlds)
                    .ThenInclude(cgw => cgw.GameWorld)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            // Aktuelle GameWorld ermitteln (priorisiert "Sündenbock 1618")
            GameWorld? currentGameWorld = null;
            if (currentCharacter?.CharacterGameWorlds != null && currentCharacter.CharacterGameWorlds.Any())
            {
                // REGEL: Bevorzuge "Sündenbock 1618" wenn Character in mehreren Welten ist
                var hauptwelt = currentCharacter.CharacterGameWorlds
                    .FirstOrDefault(cgw => cgw.GameWorld?.Name == "Sündenbock 1618");

                currentGameWorld = hauptwelt?.GameWorld
                    ?? currentCharacter.CharacterGameWorlds.FirstOrDefault()?.GameWorld;
            }

            var viewModel = new UserCharacterViewModel
            {
                CharacterId = currentCharacter?.Id,
                CharacterImagePath = currentCharacter?.ImagePath,
                UserInitial = user.Name?.Substring(0, 1) ?? user.UserName?.Substring(0, 1) ?? "?",
                CurrentCharacter = currentCharacter,
                CurrentGameWorld = currentGameWorld,
                AllUserCharacters = allUserCharacters
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
        public List<Character> AllUserCharacters { get; set; } = new List<Character>();
    }
}
