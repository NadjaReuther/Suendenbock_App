using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using System.Text.Json;

namespace Suendenbock_App.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RiddleCalendarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RiddleCalendarController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Holt alle aktiven Rätsel-Türchen (ohne Lösungen und Hinweise!)
        /// </summary>
        [HttpGet("doors")]
        public async Task<IActionResult> GetDoors()
        {
            var doors = await _context.RiddleDoors
                .Where(d => d.IsActive)
                .OrderBy(d => d.DayNumber)
                .Select(d => new
                {
                    d.Id,
                    d.DayNumber,
                    d.ImagePath,
                    d.Description
                })
                .ToListAsync();

            return Ok(doors);
        }

        /// <summary>
        /// Holt den User-Fortschritt für alle Türchen
        /// </summary>
        [HttpGet("progress")]
        public async Task<IActionResult> GetUserProgress()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var attempts = await _context.UserRiddleAttempts
                .Where(a => a.UserId == userId)
                .Select(a => new
                {
                    a.RiddleDoorId,
                    a.HintsUsed,
                    a.AttemptCount,
                    a.IsSolved,
                    a.FirstOpenedAt,
                    a.SolvedAt
                })
                .ToListAsync();

            return Ok(attempts);
        }

        /// <summary>
        /// Holt einen spezifischen Hinweis für ein Türchen
        /// </summary>
        [HttpPost("hint")]
        public async Task<IActionResult> GetHint([FromBody] GetHintRequest request)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var door = await _context.RiddleDoors.FindAsync(request.DoorId);
            if (door == null) return NotFound("Türchen nicht gefunden");

            // Hole oder erstelle User-Attempt
            var attempt = await _context.UserRiddleAttempts
                .FirstOrDefaultAsync(a => a.UserId == userId && a.RiddleDoorId == request.DoorId);

            if (attempt == null)
            {
                attempt = new UserRiddleAttempt
                {
                    UserId = userId,
                    RiddleDoorId = request.DoorId,
                    FirstOpenedAt = DateTime.UtcNow
                };
                _context.UserRiddleAttempts.Add(attempt);
            }

            // Erhöhe Hinweis-Counter
            if (attempt.HintsUsed < 3)
            {
                attempt.HintsUsed++;
                await _context.SaveChangesAsync();
            }

            // Gib den entsprechenden Hinweis zurück
            string hint = attempt.HintsUsed switch
            {
                1 => door.Hint1,
                2 => door.Hint2,
                3 => door.Hint3,
                _ => "Keine weiteren Hinweise verfügbar"
            };

            return Ok(new { hint, hintsUsed = attempt.HintsUsed });
        }

        /// <summary>
        /// Prüft eine Lösungseingabe
        /// </summary>
        [HttpPost("check")]
        public async Task<IActionResult> CheckSolution([FromBody] CheckSolutionRequest request)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var door = await _context.RiddleDoors.FindAsync(request.DoorId);
            if (door == null) return NotFound("Türchen nicht gefunden");

            // Hole oder erstelle User-Attempt
            var attempt = await _context.UserRiddleAttempts
                .FirstOrDefaultAsync(a => a.UserId == userId && a.RiddleDoorId == request.DoorId);

            if (attempt == null)
            {
                attempt = new UserRiddleAttempt
                {
                    UserId = userId,
                    RiddleDoorId = request.DoorId,
                    FirstOpenedAt = DateTime.UtcNow
                };
                _context.UserRiddleAttempts.Add(attempt);
            }

            // Erhöhe Attempt-Counter
            attempt.AttemptCount++;

            // Speichere den Versuch in der Liste
            var submittedAnswers = string.IsNullOrEmpty(attempt.SubmittedAnswers)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(attempt.SubmittedAnswers) ?? new List<string>();

            submittedAnswers.Add(request.Answer);
            attempt.SubmittedAnswers = JsonSerializer.Serialize(submittedAnswers);

            // Vergleiche Lösung (case-insensitive, trim whitespace)
            bool isCorrect = string.Equals(
                door.Solution.Trim(),
                request.Answer.Trim(),
                StringComparison.OrdinalIgnoreCase
            );

            if (isCorrect && !attempt.IsSolved)
            {
                attempt.IsSolved = true;
                attempt.SolvedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                correct = isCorrect,
                attemptCount = attempt.AttemptCount,
                hintsUsed = attempt.HintsUsed,
                isSolved = attempt.IsSolved
            });
        }
    }

    // DTOs
    public class GetHintRequest
    {
        public int DoorId { get; set; }
    }

    public class CheckSolutionRequest
    {
        public int DoorId { get; set; }
        public string Answer { get; set; } = string.Empty;
    }
}
