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
        /// Helper: Get current character for the logged-in user
        /// Returns null if user has no current character selected
        /// </summary>
        protected async Task<Character?> GetCurrentCharacterAsync()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return null;

            var user = await _context.Users
                .Include(u => u.CurrentCharacter)
                    .ThenInclude(c => c.CharacterGameWorlds)
                        .ThenInclude(cgw => cgw.GameWorld)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.CurrentCharacter;
        }

        /// <summary>
        /// Helper: Get current GameWorld based on active character
        /// Returns null if no character is selected or character has no world
        /// WICHTIG: Wenn Character in mehreren Welten ist, wird "Sündenbock 1618" bevorzugt
        /// </summary>
        protected async Task<GameWorld?> GetCurrentGameWorldAsync()
        {
            var character = await GetCurrentCharacterAsync();
            if (character == null)
                return null;

            if (character.CharacterGameWorlds == null || !character.CharacterGameWorlds.Any())
                return null;

            // REGEL: Wenn Character in mehreren Welten existiert, bevorzuge "Sündenbock 1618"
            var hauptwelt = character.CharacterGameWorlds
                .FirstOrDefault(cgw => cgw.GameWorld?.Name == "Sündenbock 1618");

            if (hauptwelt != null)
                return hauptwelt.GameWorld;

            // Fallback: Erste verfügbare Welt (z.B. nur 1810)
            return character.CharacterGameWorlds.FirstOrDefault()?.GameWorld;
        }

        /// <summary>
        /// Helper: Get all characters for the current user (for character switcher)
        /// </summary>
        protected async Task<List<Character>> GetUserCharactersAsync()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return new List<Character>();

            return await _context.Characters
                .Include(c => c.CharacterGameWorlds)
                    .ThenInclude(cgw => cgw.GameWorld)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Helper: Switch to a different character
        /// Updates ApplicationUser.CurrentCharacterId
        /// </summary>
        protected async Task<bool> SwitchCharacterAsync(int characterId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return false;

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            // Prüfe ob Character dem User gehört
            var character = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == characterId && c.UserId == userId);

            if (character == null)
                return false;

            user.CurrentCharacterId = characterId;
            await _context.SaveChangesAsync();
            return true;
        }

        // ===============================
        // AUTHORIZATION HELPERS
        // ===============================

        /// <summary>
        /// Helper: Get default GameWorld für nicht-eingeloggte User oder Gott
        /// Standard: "Sündenbock 1618"
        /// </summary>
        protected async Task<GameWorld?> GetDefaultGameWorldAsync()
        {
            return await _context.GameWorlds
                .FirstOrDefaultAsync(gw => gw.Name == "Sündenbock 1618");
        }

        /// <summary>
        /// Helper: Get GameWorld für Anzeige (mit Fallback auf Default)
        /// - Eingeloggte Spieler: Ihre aktuelle Welt basierend auf aktivem Character
        /// - Nicht eingeloggt oder Gott: "Sündenbock 1618"
        /// </summary>
        protected async Task<GameWorld?> GetEffectiveGameWorldAsync()
        {
            // Gott sieht standardmäßig "Sündenbock 1618" (kann später alle sehen)
            if (IsGott())
                return await GetDefaultGameWorldAsync();

            // Wenn nicht eingeloggt → Standard
            if (User.Identity?.IsAuthenticated != true)
                return await GetDefaultGameWorldAsync();

            // Eingeloggte Spieler → Ihre aktuelle Welt
            var currentWorld = await GetCurrentGameWorldAsync();
            if (currentWorld != null)
                return currentWorld;

            // Fallback wenn kein Character ausgewählt
            return await GetDefaultGameWorldAsync();
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
        /// Prüft ob ein Character zu einer bestimmten GameWorld gehört
        /// </summary>
        protected async Task<bool> IsCharacterInGameWorldAsync(int characterId, int gameWorldId)
        {
            return await _context.CharacterGameWorlds
                .AnyAsync(cgw => cgw.CharacterId == characterId && cgw.GameWorldId == gameWorldId);
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
        /// Setzt ViewBag für Multi-Character/GameWorld System
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

                // Multi-Character/GameWorld System
                ViewBag.IsGott = IsGott();
                ViewBag.CurrentCharacter = await GetCurrentCharacterAsync();
                ViewBag.CurrentGameWorld = await GetCurrentGameWorldAsync();
                ViewBag.UserCharacters = await GetUserCharactersAsync();
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
