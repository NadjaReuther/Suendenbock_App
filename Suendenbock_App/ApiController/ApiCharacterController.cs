using Microsoft.AspNetCore.Mvc;
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
            var characters = _context.Characters.ToList();
            return Ok(characters);
        }
        // GET: api/characters/{id}
        [HttpGet("{id}")]
        public IActionResult GetCharacter(int id)
        {
            var character = _context.Characters.Find(id);
            if (character == null)
            {
                return NotFound();
            }
            return Ok(character);
        }
    }
}
