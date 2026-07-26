using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;

namespace Suendenbock_App.Controllers.Api
{
    [Route("api/lore")]
    [ApiController]
    public class LoreForumApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public LoreForumApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("getAll")]
        public IActionResult GetAll()
        {
            var loreEntries = _context.ForumThreads
                .Include(t => t.AuthorUser)
                .Include(t => t.AuthorCharacter)
                .Include(t => t.Category)
                .Where(t => t.CategoryId == 11) // Allgemein-Forum mit allen Threads von Funktionsweise, Regeln, etc.
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Content,
                    t.CategoryId,
                    CategoryName = t.Category.Name,
                    AuthorName = t.AuthorCharacter != null
                        ? t.AuthorCharacter.Vorname
                        : t.AuthorUser != null
                            ? t.AuthorUser.UserName
                            : "Unbekannt",
                    t.CreatedAt,
                    t.IsPinned,
                    t.IsArchived
                })
                .ToList();
            return Ok(loreEntries);
        }
    }
}
