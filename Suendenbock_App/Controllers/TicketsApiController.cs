using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using System.Security.Claims;

namespace Suendenbock_App.Controllers
{
    [Authorize]
    [Route("api/tickets")]
    [ApiController]
    public class TicketsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TicketsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/tickets
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Titel erforderlich.");
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                return BadRequest("Beschreibung erforderlich.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var characterIdStr = User.FindFirstValue("CharacterId");

            int? characterId = null;
            if (!string.IsNullOrEmpty(characterIdStr) && int.TryParse(characterIdStr, out int parsedCharId))
            {
                characterId = parsedCharId;
            }

            var ticket = new Ticket
            {
                Title = request.Title,
                Description = request.Description,
                Category = request.Category ?? "Other",
                Status = "Pending",
                ReporterUserId = userId,
                ReporterCharacterId = characterId,
                CreatedAt = DateTime.Now
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return Ok(new { id = ticket.Id, message = "Ticket erfolgreich erstellt" });
        }

        // PUT: api/tickets/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] UpdateTicketRequest request)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound("Ticket nicht gefunden.");
            }

            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                ticket.Title = request.Title;
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                ticket.Description = request.Description;
            }

            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                ticket.Category = request.Category;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Ticket aktualisiert" });
        }

        // PUT: api/tickets/{id}/resolve
        [HttpPut("{id}/resolve")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> ResolveTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound("Ticket nicht gefunden.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ticket.Status = "Resolved";
            ticket.ResolvedAt = DateTime.Now;
            ticket.ResolvedByUserId = userId;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Ticket gelöst" });
        }

        // PUT: api/tickets/{id}/reopen
        [HttpPut("{id}/reopen")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> ReopenTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound("Ticket nicht gefunden.");
            }

            ticket.Status = "Pending";
            ticket.ResolvedAt = null;
            ticket.ResolvedByUserId = null;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Ticket wieder geöffnet" });
        }

        // DELETE: api/tickets/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound("Ticket nicht gefunden.");
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ticket gelöscht" });
        }
    }

    // Request Models
    public class CreateTicketRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Category { get; set; }
    }

    public class UpdateTicketRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
    }
}
