using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;

namespace Suendenbock_App.Controllers.Api
{
    [Route("api/infanterie")]
    [ApiController]
    public class InfanterieApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InfanterieApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{infanterieId}/regiments")]
        public async Task<IActionResult> GetRegimentsByInfanterie(int infanterieId)
        {
            try
            {
                var regiments = await _context.Regiments
                    .Where(r => r.InfanterieId == infanterieId)
                    .Select(r => new
                    {
                        r.Id,
                        r.Name,
                        r.Description,
                        RegimentsCharacter = r.RegimentsCharacter != null ? new
                        {
                            r.RegimentsCharacter.Vorname,
                            r.RegimentsCharacter.Nachname
                        } : null,
                        AdjutantCharacter = r.AdjutantCharacter != null ? new
                        {
                            r.AdjutantCharacter.Vorname,
                            r.AdjutantCharacter.Nachname
                        } : null
                    })
                    .ToListAsync();

                return Ok(regiments);
            }
            catch (Exception ex)
            {
                return BadRequest($"Fehler beim Laden der Regimente: {ex.Message}");
            }
        }
    }
}