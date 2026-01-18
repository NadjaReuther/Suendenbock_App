using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using System.Security.Claims;

namespace Suendenbock_App.Controllers
{
    [Authorize]
    [Route("api/polls")]
    [ApiController]
    public class PollsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PollsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/polls
        [HttpPost]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> CreatePoll([FromBody] CreatePollRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest("Frage erforderlich.");
            }

            if (request.Options == null || request.Options.Count < 2)
            {
                return BadRequest("Mindestens 2 Optionen erforderlich.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var poll = new Poll
            {
                Question = request.Question,
                Category = "Allgemein",
                AllowMultipleChoices = request.AllowMultipleChoices,
                Status = "active",
                CreatedByUserId = userId,
                CreatedAt = DateTime.Now
            };

            // Add options
            for (int i = 0; i < request.Options.Count; i++)
            {
                poll.Options.Add(new PollOption
                {
                    Text = request.Options[i],
                    SortOrder = i
                });
            }

            _context.Polls.Add(poll);
            await _context.SaveChangesAsync();

            return Ok(new { id = poll.Id });
        }

        // POST: api/polls/vote
        [HttpPost("vote")]
        public async Task<IActionResult> Vote([FromBody] VoteRequest request)
        {
            if (request.OptionIds == null || request.OptionIds.Count == 0)
            {
                return BadRequest("Mindestens eine Option erforderlich.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Load poll with options
            var poll = await _context.Polls
                .Include(p => p.Options)
                .Include(p => p.Votes)
                .FirstOrDefaultAsync(p => p.Id == request.PollId);

            if (poll == null)
            {
                return NotFound("Umfrage nicht gefunden.");
            }

            if (poll.Status != "active")
            {
                return BadRequest("Diese Umfrage ist nicht mehr aktiv.");
            }

            // Validate option IDs
            var validOptionIds = poll.Options.Select(o => o.Id).ToList();
            if (!request.OptionIds.All(id => validOptionIds.Contains(id)))
            {
                return BadRequest("Ungültige Option ausgewählt.");
            }

            // Check multiple choice restriction
            if (!poll.AllowMultipleChoices && request.OptionIds.Count > 1)
            {
                return BadRequest("Nur eine Option erlaubt.");
            }

            // Remove existing votes from this user for this poll
            var existingVotes = poll.Votes.Where(v => v.UserId == userId).ToList();
            _context.PollVotes.RemoveRange(existingVotes);

            // Add new votes
            foreach (var optionId in request.OptionIds)
            {
                var vote = new PollVote
                {
                    PollId = poll.Id,
                    PollOptionId = optionId,
                    UserId = userId,
                    VotedAt = DateTime.Now
                };

                _context.PollVotes.Add(vote);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        // PUT: api/polls/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> UpdatePoll(int id, [FromBody] CreatePollRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest("Frage erforderlich.");
            }

            if (request.Options == null || request.Options.Count < 2)
            {
                return BadRequest("Mindestens 2 Optionen erforderlich.");
            }

            var poll = await _context.Polls
                .Include(p => p.Options)
                    .ThenInclude(o => o.Votes)
                .Include(p => p.Votes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (poll == null)
            {
                return NotFound("Umfrage nicht gefunden.");
            }

            // Update question
            poll.Question = request.Question;

            // Delete all existing votes and options
            var allVotes = poll.Votes.ToList();
            _context.PollVotes.RemoveRange(allVotes);

            var allOptions = poll.Options.ToList();
            _context.PollOptions.RemoveRange(allOptions);

            // Update multiple choice setting
            poll.AllowMultipleChoices = request.AllowMultipleChoices;

            // Add new options
            poll.Options.Clear();
            for (int i = 0; i < request.Options.Count; i++)
            {
                poll.Options.Add(new PollOption
                {
                    Text = request.Options[i],
                    SortOrder = i,
                    PollId = poll.Id
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new { id = poll.Id });
        }

        // PUT: api/polls/{id}/close
        [HttpPut("{id}/close")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> ClosePoll(int id)
        {
            var poll = await _context.Polls.FindAsync(id);

            if (poll == null)
            {
                return NotFound("Umfrage nicht gefunden.");
            }

            poll.Status = "closed";
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/polls/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> DeletePoll(int id)
        {
            var poll = await _context.Polls
                .Include(p => p.Options)
                    .ThenInclude(o => o.Votes)
                .Include(p => p.Votes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (poll == null)
            {
                return NotFound("Umfrage nicht gefunden.");
            }

            // Delete all votes first
            _context.PollVotes.RemoveRange(poll.Votes);

            // Delete all options (this will cascade delete their votes too)
            _context.PollOptions.RemoveRange(poll.Options);

            // Delete the poll
            _context.Polls.Remove(poll);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    // Request Models
    public class CreatePollRequest
    {
        public string Question { get; set; } = string.Empty;
        public bool AllowMultipleChoices { get; set; }
        public List<string> Options { get; set; } = new();
    }

    public class VoteRequest
    {
        public int PollId { get; set; }
        public List<int> OptionIds { get; set; } = new();
    }
}
