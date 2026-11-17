using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Controllers
{
    /// <summary>
    /// Echter Name des Spielers. Wird beim ersten Login abgefragt
    /// und z.B. bei Diensten, Beiträgen und Zusagen angezeigt.
    /// </summary>
    [Authorize]
    public class ProfilController : Controller
    {
        public const int MinNameLength = 2;
        public const int MaxNameLength = 40;

        private readonly UserManager<ApplicationUser> _userManager;

        public ProfilController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET /Profil/Name
        [HttpGet]
        public async Task<IActionResult> Name(string? returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.IsFirstTime = string.IsNullOrWhiteSpace(user.Name);
            ViewBag.CurrentName = user.Name ?? "";
            return View();
        }

        // POST /Profil/Name
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Name(string? name, string? returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var cleaned = (name ?? "").Trim();
            string? error = null;

            if (cleaned.Length < MinNameLength || cleaned.Length > MaxNameLength)
            {
                error = $"Bitte gib einen Namen mit {MinNameLength} bis {MaxNameLength} Zeichen ein.";
            }
            else
            {
                // Namen müssen eindeutig sein, sonst sind Dienste nicht zuzuordnen
                var lower = cleaned.ToLower();
                var taken = await _userManager.Users
                    .AnyAsync(u => u.Id != user.Id && u.Name != null && u.Name.ToLower() == lower);
                if (taken)
                {
                    error = $"„{cleaned}“ gibt es schon. Nimm zum Beispiel noch den ersten Buchstaben des Nachnamens dazu.";
                }
            }

            if (error != null)
            {
                ViewBag.Error = error;
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.IsFirstTime = string.IsNullOrWhiteSpace(user.Name);
                ViewBag.CurrentName = cleaned;
                return View();
            }

            user.Name = cleaned;
            await _userManager.UpdateAsync(user);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
