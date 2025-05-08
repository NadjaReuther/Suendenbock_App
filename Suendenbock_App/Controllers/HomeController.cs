using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Models.ViewModels;

namespace Suendenbock_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var allMagicClasses = _context.MagicClasses
                .Include(mc => mc.LightCard)
                .ToList();
            var allGuilds = _context.Guilds.ToList();

            // die 6 neuesten Charaktere
            var recentCharacters = _context.Characters
                .Include(c => c.Guild)
                .Include(c => c.Religion)
                .OrderByDescending(c => c.Id)
                .Take(6)
                .ToList();

            // Magieklassen-Statistik
            var magicClassStats = _context.CharacterMagicClasses
                .GroupBy(cmc => cmc.MagicClassId)
                .Select(g => new { MagicClassId = g.Key, Count = g.Count() })
                .ToDictionary(
                    x => _context.MagicClasses.FirstOrDefault(mc => mc.Id == x.MagicClassId)?.Bezeichnung ?? "Unbekannt",
                x => x.Count);

            // Charaktere ohne Magieklasse hinzufügen
            var charactersWithoutMagic = _context.Characters.Count(c => !c.CharacterMagicClasses.Any());
            if (charactersWithoutMagic > 0)
            {
                magicClassStats.Add("Keine Magieklasse", charactersWithoutMagic);
            }
            // Geschlechter-Statistik
            var genderStats = _context.Characters
                .GroupBy(c => c.Geschlecht)
                .ToDictionary(g => g.Key, g => g.Count());

            // Erstelle das ViewModel
            var viewModel = new HomeViewModel
            {
                MagicClassStats = magicClassStats,
                Characters = recentCharacters,
                MagicClasses = allMagicClasses,
                Guilds = allGuilds,
                GenderStats = genderStats
            };      
            return View(viewModel);
        }

        //Einbau Familienstammbaum
        public IActionResult FamilyTree(int characterId = 0)
        {
            //wenn kein Chracter angegeben ist, nimm einen zufälligen
            if (characterId == 0)
            {
                var randomCharacter = _context.Characters.FirstOrDefault();
                if (randomCharacter != null)
                {
                    characterId = randomCharacter.Id;
                }
                else
                {
                    return NotFound("Keine Charaktere in der Datenbank gefunden.");
                }
            }
            var character = _context.Characters
                .Include(c => c.Guild)
                .Include(c => c.Religion)
                .FirstOrDefault(c => c.Id == characterId);

            if (character == null)
            {
                return NotFound();
            }

            // Eltern und Vorfahren laden
            var ancestors = new List<Character>();
                if (character.VaterId.HasValue)
                {
                    var father = _context.Characters.Find(character.VaterId.Value);
                    if (father != null)
                    {
                        ancestors.Add(father);
                    }
                }
                if (character.MutterId.HasValue)
                {
                    var mother = _context.Characters.Find(character.MutterId.Value);
                    if (mother != null)
                    {
                        ancestors.Add(mother);
                    }
                }

                //Geschwister laden
                var siblings = new List<Character>();
                if (character.VaterId.HasValue || character.MutterId.HasValue)
                {
                    siblings = _context.Characters
                        .Where(c => 
                            (character.VaterId.HasValue && c.VaterId == character.VaterId) ||
                            (character.MutterId.HasValue && c.MutterId == character.MutterId))
                        .Where(c => c.Id != characterId) // ausgewählten Charakter ausschließen
                        .ToList();
                }

                // Kinder laden
                var descendants = _context.Characters
                    .Where(c => c.VaterId == character.Id || c.MutterId == character.Id)
                    .ToList();
                var viewModel = new FamilyTreeViewModel
                {
                    RootCharacter = character,
                    Ancestors = ancestors,
                    Siblings = siblings,
                    Descendants = descendants
                };

                return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
