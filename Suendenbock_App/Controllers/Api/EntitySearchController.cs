using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;

namespace Suendenbock_App.Controllers.Api
{
    [Route("api/entity")]
    [ApiController]
    public class EntitySearchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EntitySearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchEntities(string query = "", string type = "")
        {
            if (string.IsNullOrEmpty(query) || query.Length < 1)
            {
                return Ok(new { results = new List<object>() });
            }

            var results = new List<object>();

            switch (type.ToLower())
            {
                case "character":
                case "@":
                    results.AddRange(await SearchCharacters(query));
                    break;
                case "guild":
                case "#":
                    results.AddRange(await SearchGuilds(query));
                    break;
                case "infanterie":
                case "§":
                    results.AddRange(await SearchInfanterie(query));
                    break;
                case "monster":
                case "&":
                    results.AddRange(await SearchMonster(query));
                    break;
                case "magicclass":
                case "%":
                    results.AddRange(await SearchMagicClasses(query));
                    break;
                default:
                    // Suche in allen Entitäten
                    results.AddRange(await SearchAll(query));
                    break;
            }

            return Ok(new { results = results });
        }

        private async Task<List<object>> SearchCharacters(string query)
        {
            return await _context.Characters
                .Where(c => (c.Vorname + " " + c.Nachname).Contains(query) ||
                           c.Rufname.Contains(query))
                .Take(10)
                .Select(c => new
                {
                    id = c.Id,
                    name = c.Vorname + " " + c.Nachname,
                    type = "character",
                    icon = "👤",
                    url = $"/Character/CharacterSheet/{c.Id}",
                    subtitle = c.Rufname
                })
                .ToListAsync<object>();
        }

        private async Task<List<object>> SearchGuilds(string query)
        {
            return await _context.Guilds
                .Where(g => g.Name.Contains(query))
                .Take(10)
                .Select(g => new
                {
                    id = g.Id,
                    name = g.Name,
                    type = "guild",
                    icon = "🏰",
                    url = $"/Guild/GuildSheet/{g.Id}",
                    subtitle = "Gilde"
                })
                .ToListAsync<object>();
        }

        private async Task<List<object>> SearchInfanterie(string query)
        {
            return await _context.Infanterien
                .Where(i => i.Bezeichnung.Contains(query) ||
                           (i.Bezeichnung + ". Infanterie").Contains(query))
                .Take(10)
                .Select(i => new
                {
                    id = i.Id,
                    name = i.Bezeichnung + ". Infanterie",
                    type = "infanterie",
                    icon = "⚔️",
                    url = $"/Infanterie/Overview?id={i.Id}",
                    subtitle = i.Sitz
                })
                .ToListAsync<object>();
        }

        private async Task<List<object>> SearchMonster(string query)
        {
            return await _context.Monsters
                .Where(m => m.Name.Contains(query))
                .Take(10)
                .Select(m => new
                {
                    id = m.Id,
                    name = m.Name,
                    type = "monster",
                    icon = "👹",
                    url = $"/Monster/Overview?monsterId={m.Id}",
                    subtitle = "Monster"
                })
                .ToListAsync<object>();
        }

        private async Task<List<object>> SearchMagicClasses(string query)
        {
            return await _context.MagicClasses
                .Include(mc => mc.Obermagie)
                .Where(mc => mc.Bezeichnung.Contains(query))
                .Take(10)
                .Select(mc => new
                {
                    id = mc.Id,
                    name = mc.Bezeichnung,
                    type = "magicclass",
                    icon = "🔮",
                    url = $"/MagicClass/MagicClassSheet/{mc.Id}",
                    subtitle = mc.Obermagie.Bezeichnung
                })
                .ToListAsync<object>();
        }

        private async Task<List<object>> SearchAll(string query)
        {
            var allResults = new List<object>();

            allResults.AddRange(await SearchCharacters(query));
            allResults.AddRange(await SearchGuilds(query));
            allResults.AddRange(await SearchInfanterie(query));
            allResults.AddRange(await SearchMonster(query));
            allResults.AddRange(await SearchMagicClasses(query));

            return allResults.Take(15).ToList();
        }
    }
}