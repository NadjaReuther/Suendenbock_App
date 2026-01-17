using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using System.Security.Claims;

namespace Suendenbock_App.Controllers
{
    [Authorize]
    [Route("api/news")]
    [ApiController]
    public class NewsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NewsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/news - Create News
        [HttpPost]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> CreateNews([FromBody] CreateNewsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userName = User.Identity?.Name ?? "Unbekannt";

            // Generate excerpt (first 100 chars)
            var excerpt = request.Content.Length > 100
                ? request.Content.Substring(0, 97) + "..."
                : request.Content;

            // Set icon based on category
            var icon = request.Category switch
            {
                "Spiel-Update" => "campaign",
                "Technik" => "history_edu",
                "Events" => "military_tech",
                _ => "campaign"
            };

            var newsItem = new NewsItem
            {
                Title = request.Title,
                Content = request.Content,
                Excerpt = excerpt,
                Category = request.Category,
                Icon = icon,
                Author = userName,
                CreatedAt = DateTime.Now,
                Comments = new List<NewsComment>()
            };

            _context.NewsItems.Add(newsItem);
            await _context.SaveChangesAsync();

            return Ok(new { id = newsItem.Id, message = "Neuigkeit erfolgreich erstellt." });
        }

        // PUT: api/news/{id} - Update News
        [HttpPut("{id}")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> UpdateNews(int id, [FromBody] UpdateNewsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newsItem = await _context.NewsItems.FindAsync(id);
            if (newsItem == null)
            {
                return NotFound(new { message = "Neuigkeit nicht gefunden." });
            }

            // Update fields
            newsItem.Title = request.Title;
            newsItem.Content = request.Content;
            newsItem.Category = request.Category;

            // Update excerpt
            newsItem.Excerpt = request.Content.Length > 100
                ? request.Content.Substring(0, 97) + "..."
                : request.Content;

            // Update icon based on category
            newsItem.Icon = request.Category switch
            {
                "Spiel-Update" => "campaign",
                "Technik" => "history_edu",
                "Events" => "military_tech",
                _ => "campaign"
            };

            _context.NewsItems.Update(newsItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Neuigkeit erfolgreich aktualisiert." });
        }

        // DELETE: api/news/{id} - Delete News
        [HttpDelete("{id}")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var newsItem = await _context.NewsItems
                .Include(n => n.Comments)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (newsItem == null)
            {
                return NotFound(new { message = "Neuigkeit nicht gefunden." });
            }

            _context.NewsItems.Remove(newsItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Neuigkeit erfolgreich gelöscht." });
        }

        // GET: api/news/{id} - Get single news (for editing)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNews(int id)
        {
            var newsItem = await _context.NewsItems.FindAsync(id);

            if (newsItem == null)
            {
                return NotFound(new { message = "Neuigkeit nicht gefunden." });
            }

            return Ok(new
            {
                id = newsItem.Id,
                title = newsItem.Title,
                content = newsItem.Content,
                category = newsItem.Category
            });
        }

        // POST: api/news/comment - Add Comment
        [HttpPost("comment")]
        public async Task<IActionResult> AddComment([FromBody] AddCommentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newsItem = await _context.NewsItems.FindAsync(request.NewsItemId);
            if (newsItem == null)
            {
                return NotFound(new { message = "Neuigkeit nicht gefunden." });
            }

            var userName = User.Identity?.Name ?? "Bürger";

            var comment = new NewsComment
            {
                Text = request.Text,
                Author = userName,
                CreatedAt = DateTime.Now,
                NewsItemId = request.NewsItemId
            };

            _context.NewsComments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(new { id = comment.Id, message = "Kommentar erfolgreich hinzugefügt." });
        }

        // DELETE: api/news/comment/{id} - Delete Comment
        [HttpDelete("comment/{id}")]
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.NewsComments.FindAsync(id);

            if (comment == null)
            {
                return NotFound(new { message = "Kommentar nicht gefunden." });
            }

            _context.NewsComments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kommentar erfolgreich gelöscht." });
        }
    }

    // ===== REQUEST MODELS =====

    public class CreateNewsRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = "Spiel-Update";
    }

    public class UpdateNewsRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = "Spiel-Update";
    }

    public class AddCommentRequest
    {
        public int NewsItemId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
