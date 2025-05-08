using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;

namespace Suendenbock_App.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiCharacterController: ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ApiCharacterController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/characters
        [HttpGet]
        public IActionResult GetCharacters()
        {
            var characters = _context.Characters
                .Include(c => c.CharacterMagicClasses)
                    .ThenInclude(cmc => cmc.MagicClass)
                .Include(c => c.Religion)
                .Include(c => c.Guild)
                .ToList();

            return Ok(characters);
        }
        // GET: api/characters/{id}
        [HttpGet("{id}")]
        public IActionResult GetCharacter(int id)
        {
            var character = _context.Characters
                .Include(c => c.CharacterMagicClasses)
                    .ThenInclude(cmc => cmc.MagicClass)
                .Include(c => c.Religion)
                .Include(c => c.Guild)
                .FirstOrDefault(c => c.Id == id);

            if (character == null)
            {
                return NotFound();
            }
            return Ok(character);
        }
    }
}
