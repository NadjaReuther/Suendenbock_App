using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;

namespace Suendenbock_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetCharactersForDropdown(string search = "", int page= 1)
        {
            var query = _context.Characters
                .Select(c => new
                {
                    c.Id,
                    FullName = c.Vorname + " " + c.Nachname
                });
                
            //Suche anwenden
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.FullName.Contains(search));
            }

            //Paginierung anwenden
            var results = await query
                .Skip((page - 1) * 20)
                .Take(20)
                .ToListAsync();

            return Ok(new {results, pagination = new {more = results.Count == 20}});
        }
    }
}
