using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using System.Security.Claims;

namespace Suendenbock_App.Controllers
{
    /// <summary>
    /// Base Controller, der von allen Controllern geerbt wird
    /// Setzt automatisch ViewBag.CurrentActId für SignalR
    /// </summary>
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Helper: Get Current User ID
        /// </summary>
        protected string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }

        /// <summary>
        /// Helper: Check if current user is Gott (Admin)
        /// </summary>
        protected bool IsGott()
        {
            return User.IsInRole("Gott");
        }

        /// <summary>
        /// Helper: Character des eingeloggten Spielers in Sündenbock.
        /// (Hesparis hat eigene Charaktere, siehe Models/Domain/Hesparis/Charaktere.cs.)
        /// </summary>
        protected async Task<Character?> GetCurrentCharacterAsync()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return null;

            return await _context.Characters
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Id)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Helper: Characters des Spielers in Sündenbock (normalerweise genau einer)
        /// </summary>
        protected async Task<List<Character>> GetUserCharactersAsync()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return new List<Character>();

            return await _context.Characters
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Prüft ob der aktuelle User Zugriff auf einen bestimmten Character hat
        /// Gott hat immer Zugriff, normale User nur auf ihre eigenen Characters
        /// </summary>
        protected async Task<bool> HasCharacterAccessAsync(int characterId)
        {
            // Gott hat immer Zugriff
            if (IsGott())
                return true;

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return false;

            // Prüfe ob Character dem User gehört
            return await _context.Characters
                .AnyAsync(c => c.Id == characterId && c.UserId == userId);
        }

        /// <summary>
        /// Prüft ob der aktuelle User einen Character bearbeiten darf
        /// </summary>
        protected async Task<bool> CanEditCharacterAsync(int characterId)
        {
            return await HasCharacterAccessAsync(characterId);
        }

        /// <summary>
        /// Validiert Character-Zugriff und gibt Forbid() zurück wenn nicht erlaubt
        /// Nützlich für Action-Methoden
        /// </summary>
        protected async Task<IActionResult?> ValidateCharacterAccessAsync(int characterId, string? errorMessage = null)
        {
            if (!await HasCharacterAccessAsync(characterId))
            {
                TempData["Error"] = errorMessage ?? "Du hast keinen Zugriff auf diesen Character.";
                return Forbid();
            }
            return null;
        }

        /// <summary>
        /// Erzwingt dass ein aktueller Character ausgewählt ist
        /// Gibt Redirect zurück wenn kein Character ausgewählt
        /// </summary>
        protected async Task<IActionResult?> RequireCurrentCharacterAsync(string? errorMessage = null)
        {
            var character = await GetCurrentCharacterAsync();
            if (character == null)
            {
                TempData["Error"] = errorMessage ?? "Bitte wähle zuerst einen Character aus.";
                return RedirectToAction("Index", "Player");
            }
            return null;
        }

        /// <summary>
        /// Wird vor jeder Action ausgeführt
        /// Setzt ViewBag.CurrentActId mit dem aktiven Act
        /// </summary>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Nur wenn User eingeloggt ist
            if (User.Identity?.IsAuthenticated == true)
            {
                // Finde den aktiven Act
                var activeAct = await _context.Acts.FirstOrDefaultAsync(a => a.IsActive);

                if (activeAct != null)
                {
                    // WICHTIG: Verwende ActNumber (logische Nummer) statt Id (Datenbank-ID)
                    ViewBag.CurrentActId = activeAct.ActNumber;
                }
                else
                {
                    ViewBag.CurrentActId = null;
                }

                ViewBag.IsGott = IsGott();
                ViewBag.CurrentCharacter = await GetCurrentCharacterAsync();
                ViewBag.UserCharacters = await GetUserCharactersAsync();
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
