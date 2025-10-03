using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;

namespace Suendenbock_App.Controllers
{
    [Authorize(Roles = "Spieler")]
    public class PlayerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PlayerController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            context = _context;
            _userManager = userManager;
        }

        //zeigt nur die Character die dem User zugeordnet sind
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var myCharacters = await _context.Characters
                .Where(c => c.UserId == userId)
                .Include(c => c.Rasse)
                .Include(c => c.Lebensstatus)
                .ToListAsync();
                            
            return View(myCharacters);
        }
    }
}
