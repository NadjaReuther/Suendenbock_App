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

            // Charakter des Users laden
            var character = await _context.Characters
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            var viewModel = new UserCharacterViewModel
            {
                CharacterId = character?.Id,
                CharacterImagePath = character?.ImagePath,
                UserInitial = user.Name?.Substring(0, 1) ?? user.UserName?.Substring(0, 1) ?? "?"
            };

            return View("Default", viewModel);
        }
    }

    public class UserCharacterViewModel
    {
        public int? CharacterId { get; set; }
        public string? CharacterImagePath { get; set; }
        public string UserInitial { get; set; } = "?";
    }
}
