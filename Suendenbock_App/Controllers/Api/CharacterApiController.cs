using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Controllers.Api
{
    [Route("api/character")]
    [ApiController]

    public class CharacterApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CharacterApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetCharactersForDropdown(string search = "", int page= 1, string gender = "", int excludeId = 0)
        {
            var query = _context.Characters
                .Select(c => new
                {
                    c.Id,
                    FullName = c.Vorname + " " + c.Nachname,
                    geschlecht = c.Geschlecht
                });
                

            //Geschlecht filtern, falls angegeben
            if (!string.IsNullOrEmpty(gender))
            {
                query = query.Where(c => c.geschlecht == gender);
            }

            if (excludeId > 0)
            {
                query = query.Where(c => c.Id != excludeId);
            }

            //Suche anwenden
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.FullName.Contains(search));
                var searchResults = await query
                    .Take(100)
                    .ToListAsync();

                return Ok(new
                {
                    results = searchResults,
                    pagination = new { more = false }
                });
            }
            //Paginierung anwenden
            var pageSize = 20;
            var results = await query
                .OrderBy(c => c.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize + 1)
                .ToListAsync();

            var hasMore = results.Count > pageSize;
            if(hasMore)
            {
                results.Take(pageSize).ToList();
            }
            
            return Ok(new {
                results = results,
                pagination = new {more = hasMore}
            });
        }
    }
}
