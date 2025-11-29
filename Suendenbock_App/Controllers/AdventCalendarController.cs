using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using System.Security.Claims;

namespace Suendenbock_App.Controllers
{
    [Authorize]
    public class AdventCalendarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public AdventCalendarController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        /// <summary>
        /// Lädt den Inhalt eines Türchens basierend auf dem Tag und DoorType
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetContent(int day)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Türchen aus DB laden
            var door = await _context.AdventDoors
                .Include(d => d.UserChoices)
                .FirstOrDefaultAsync(d => d.DayNumber == day);

            if (door == null)
            {
                return NotFound(new { message = "Türchen nicht gefunden" });
            }

            // Prüfen ob User in Gott-Rolle ist
            var user = await _userManager.GetUserAsync(User);
            var isGod = user != null && await _userManager.IsInRoleAsync(user, "Gott");

            // Zeitbegrenzung prüfen (außer für Gods)
            if (!isGod && !IsDayAvailable(day))
            {
                return BadRequest(new { message = "Dieses Türchen ist noch nicht verfügbar" });
            }

            // Türchen-Öffnung tracken (falls noch nicht geöffnet)
            await TrackDoorOpeningAsync(door, userId);

            // Je nach DoorType unterschiedliche Antwort
            switch (door.DoorType)
            {
                case DoorType.Simple:
                    return await HandleSimpleDoor(door, userId);

                case DoorType.Choice:
                    return await HandleChoiceDoor(door, userId);

                case DoorType.DirectAudio:
                    return await HandleDirectAudioDoor(door, userId);

                default:
                    return BadRequest(new { message = "Unbekannter Türchen-Typ" });
            }
        }

        /// <summary>
        /// Speichert die User-Auswahl für Choice-Türchen
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveChoice([FromBody] SaveChoiceRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var door = await _context.AdventDoors
                .Include(d => d.UserChoices)
                .FirstOrDefaultAsync(d => d.DayNumber == request.Day);

            if (door == null || door.DoorType != DoorType.Choice)
            {
                return NotFound(new { message = "Türchen nicht gefunden oder falscher Typ" });
            }

            // Prüfen ob User schon gewählt hat
            var existingChoice = await _context.UserAdventChoices
                .FirstOrDefaultAsync(c => c.UserId == userId && c.AdventDoorId == door.Id);

            if (existingChoice != null && existingChoice.ChoiceIndex.HasValue)
            {
                return BadRequest(new { message = "Du hast bereits gewählt!" });
            }

            // Auswahl speichern oder updaten
            if (existingChoice != null)
            {
                // Eintrag existiert bereits (von TrackDoorOpening), nur ChoiceIndex setzen
                existingChoice.ChoiceIndex = request.ChoiceIndex;
                existingChoice.ChosenAt = DateTime.Now;
            }
            else
            {
                // Neuer Eintrag (sollte eigentlich nicht passieren, da TrackDoorOpening schon aufgerufen wurde)
                var choice = new UserAdventChoice
                {
                    UserId = userId,
                    AdventDoorId = door.Id,
                    ChoiceIndex = request.ChoiceIndex,
                    ChosenAt = DateTime.Now
                };
                _context.UserAdventChoices.Add(choice);
            }

            await _context.SaveChangesAsync();

            // Audio-Pfad basierend auf Wahl zurückgeben
            var audioPath = request.ChoiceIndex == 0 ? door.EmmaAudioPath : door.KasimirAudioPath;

            return Ok(new
            {
                success = true,
                choiceIndex = request.ChoiceIndex,
                audioPath = audioPath,
                message = "Auswahl gespeichert!"
            });
        }

        /// <summary>
        /// Prüft ob User die Gott-Rolle hat (für Frontend)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> IsGod()
        {
            var user = await _userManager.GetUserAsync(User);
            var isGod = user != null && await _userManager.IsInRoleAsync(user, "Gott");
            return Ok(new { isGod });
        }

        /// <summary>
        /// Gibt alle geöffneten Türchen für den aktuellen User zurück
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetOpenedDoors()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var openedDoors = await _context.UserAdventChoices
                .Where(c => c.UserId == userId)
                .Select(c => c.AdventDoorId)
                .ToListAsync();

            // Türchen-Nummern zurückgeben
            var doorNumbers = await _context.AdventDoors
                .Where(d => openedDoors.Contains(d.Id))
                .Select(d => d.DayNumber)
                .ToListAsync();

            return Ok(new { openedDoors = doorNumbers });
        }

        // ========================================
        // ADMIN BEREICH
        // ========================================

        /// <summary>
        /// Admin-Dashboard für Adventskalender-Übersicht
        /// </summary>
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> AdminDashboard()
        {
            // Alle Türchen mit UserChoices laden
            var doors = await _context.AdventDoors
                .Include(d => d.UserChoices)
                .OrderBy(d => d.DayNumber)
                .ToListAsync();

            // Alle User-Choices laden
            var allChoices = await _context.UserAdventChoices
                .Where(c => c.ChoiceIndex.HasValue)
                .ToListAsync();

            // Player mit Rolle "Spieler" laden
            var playerUsers = await _userManager.GetUsersInRoleAsync("Spieler");

            // Nur Player mit zugewiesenem Character laden (mit Character-Farbe)
            var playerUserIds = playerUsers.Select(u => u.Id).ToList();
            var charactersWithColor = await _context.Characters
                .Where(c => playerUserIds.Contains(c.UserId))
                .ToDictionaryAsync(c => c.UserId, c => c.UserColor);

            // Für jeden Player die Statistik berechnen (nur mit Character)
            var playerStats = new List<dynamic>();
            foreach (var player in playerUsers.Where(p => charactersWithColor.ContainsKey(p.Id)).OrderBy(p => p.UserName))
            {
                var playerChoices = allChoices.Where(c => c.UserId == player.Id).ToList();
                var emmaCount = playerChoices.Count(c => c.ChoiceIndex == 0);
                var kasimirCount = playerChoices.Count(c => c.ChoiceIndex == 1);
                var total = emmaCount + kasimirCount;

                // Alle Player anzeigen (auch ohne Auswahlen)
                playerStats.Add(new
                {
                    UserName = player.UserName ?? "Unbekannt",
                    Farbcode = charactersWithColor[player.Id] ?? "#6c757d", // Character-Farbe
                    EmmaCount = emmaCount,
                    KasimirCount = kasimirCount,
                    Total = total,
                    Preference = total == 0 ? "Keine Auswahl" :
                                emmaCount > kasimirCount ? "Emma" :
                                kasimirCount > emmaCount ? "Kasimir" : "Gleich"
                });
            }

            // Ersten Öffner für jedes Türchen finden
            var firstOpeners = new Dictionary<int, dynamic>();
            foreach (var door in doors)
            {
                var firstChoice = door.UserChoices.OrderBy(c => c.ChosenAt).FirstOrDefault();
                if (firstChoice != null)
                {
                    var user = await _context.Users.FindAsync(firstChoice.UserId);
                    firstOpeners[door.Id] = new
                    {
                        UserName = user?.UserName ?? "Unbekannt",
                        OpenedAt = firstChoice.ChosenAt
                    };
                }
            }

            ViewBag.PlayerStats = playerStats;
            ViewBag.Doors = doors;
            ViewBag.FirstOpeners = firstOpeners;

            return View();
        }

        /// <summary>
        /// Zeigt detaillierte User-Aktivitäten für ein bestimmtes Türchen
        /// </summary>
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> DoorActivity(int doorId)
        {
            var door = await _context.AdventDoors
                .Include(d => d.UserChoices)
                .FirstOrDefaultAsync(d => d.Id == doorId);

            if (door == null)
            {
                return NotFound();
            }

            // User-Informationen laden
            var userChoices = await _context.UserAdventChoices
                .Where(c => c.AdventDoorId == doorId)
                .OrderBy(c => c.ChosenAt)
                .ToListAsync();

            var userIds = userChoices.Select(c => c.UserId).ToList();
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName ?? "Unbekannt");

            ViewBag.Door = door;
            ViewBag.UserChoices = userChoices;
            ViewBag.Users = users;

            return View();
        }

        /// <summary>
        /// API-Endpoint für Statistiken eines Türchens
        /// </summary>
        [Authorize(Roles = "Gott")]
        [HttpGet]
        public async Task<IActionResult> GetDoorStatistics(int doorId)
        {
            var door = await _context.AdventDoors
                .Include(d => d.UserChoices)
                .FirstOrDefaultAsync(d => d.Id == doorId);

            if (door == null)
            {
                return NotFound();
            }

            var totalOpenings = door.UserChoices.Count;
            var emmaCount = door.UserChoices.Count(c => c.ChoiceIndex == 0);
            var kasimirCount = door.UserChoices.Count(c => c.ChoiceIndex == 1);

            return Ok(new
            {
                doorNumber = door.DayNumber,
                doorType = door.DoorType.ToString(),
                totalOpenings = totalOpenings,
                emmaCount = emmaCount,
                kasimirCount = kasimirCount,
                emmaPercentage = totalOpenings > 0 ? (emmaCount * 100.0 / totalOpenings) : 0,
                kasimirPercentage = totalOpenings > 0 ? (kasimirCount * 100.0 / totalOpenings) : 0
            });
        }

        // ========================================
        // PRIVATE HELPER METHODEN
        // ========================================

        /// <summary>
        /// Trackt dass ein User ein Türchen geöffnet hat
        /// </summary>
        private async Task TrackDoorOpeningAsync(AdventDoor door, string userId)
        {
            // Prüfen ob schon ein Eintrag existiert
            var existingEntry = await _context.UserAdventChoices
                .FirstOrDefaultAsync(c => c.UserId == userId && c.AdventDoorId == door.Id);

            if (existingEntry == null)
            {
                // Neuer Eintrag für Türchen-Öffnung (ChoiceIndex = NULL für Simple/DirectAudio)
                var entry = new UserAdventChoice
                {
                    UserId = userId,
                    AdventDoorId = door.Id,
                    ChoiceIndex = null, // Wird bei SaveChoice gesetzt falls Choice-Türchen
                    ChosenAt = DateTime.Now
                };

                _context.UserAdventChoices.Add(entry);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<IActionResult> HandleSimpleDoor(AdventDoor door, string userId)
        {
            if (string.IsNullOrEmpty(door.HtmlContentPath))
            {
                return BadRequest(new { message = "Kein HTML-Inhalt definiert" });
            }

            // HTML-Datei laden
            var htmlPath = Path.Combine(_env.WebRootPath, door.HtmlContentPath.TrimStart('/'));

            if (!System.IO.File.Exists(htmlPath))
            {
                return NotFound(new { message = "HTML-Datei nicht gefunden" });
            }

            var htmlContent = await System.IO.File.ReadAllTextAsync(htmlPath);

            return Ok(new
            {
                doorType = "simple",
                htmlContent = htmlContent
            });
        }

        private async Task<IActionResult> HandleChoiceDoor(AdventDoor door, string userId)
        {
            // Hat der User bereits gewählt?
            var existingChoice = door.UserChoices
                .FirstOrDefault(c => c.UserId == userId);

            // Nur wenn ChoiceIndex gesetzt ist, hat der User tatsächlich gewählt
            if (existingChoice != null && existingChoice.ChoiceIndex.HasValue)
            {
                // User hat bereits gewählt - zeige Ergebnis
                var audioPath = existingChoice.ChoiceIndex == 0
                    ? door.EmmaAudioPath
                    : door.KasimirAudioPath;

                return Ok(new
                {
                    doorType = "choice",
                    alreadyChosen = true,
                    choiceIndex = existingChoice.ChoiceIndex,
                    choiceName = existingChoice.ChoiceIndex == 0 ? "Emma" : "Kasimir",
                    audioPath = audioPath
                });
            }

            // User hat noch nicht gewählt - zeige Auswahlmöglichkeiten
            return Ok(new
            {
                doorType = "choice",
                alreadyChosen = false,
                emmaAudioPath = door.EmmaAudioPath,
                kasimirAudioPath = door.KasimirAudioPath
            });
        }

        private Task<IActionResult> HandleDirectAudioDoor(AdventDoor door, string userId)
        {
            if (string.IsNullOrEmpty(door.AudioPath))
            {
                return Task.FromResult<IActionResult>(
                    BadRequest(new { message = "Keine Audio-Datei definiert" }));
            }

            return Task.FromResult<IActionResult>(Ok(new
            {
                doorType = "directAudio",
                audioPath = door.AudioPath
            }));
        }

        private bool IsDayAvailable(int dayNumber)
        {
            // TESTING: Alle Türchen immer verfügbar
            return true;

            /* ORIGINAL CODE - SPÄTER WIEDER AKTIVIEREN:
            var today = DateTime.Now;

            // Nur im Dezember
            if (today.Month != 12)
            {
                return false;
            }

            // Tag muss erreicht oder überschritten sein
            return today.Day >= dayNumber;
            */
        }
    }

    // ========================================
    // REQUEST MODELS
    // ========================================

    public class SaveChoiceRequest
    {
        public int Day { get; set; }
        public int ChoiceIndex { get; set; } // 0 = Emma, 1 = Kasimir
    }
}
