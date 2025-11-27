using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Helpers;
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
            // Basis-Daten laden
            var allMagicClasses = _context.MagicClasses
                .Include(mc => mc.Obermagie)
                .ThenInclude(o => o.LightCard)
                .ToList();
            var allGuilds = _context.Guilds.ToList();
            var allInfanteries = _context.Infanterien.ToList();
            var characters = _context.Characters
                .Include(c => c.Lebensstatus)
                .Include(c => c.Eindruck)
                .Include(c => c.Rasse)
                .Include(c => c.CharacterMagicClasses)
                .ToList();
            var monstertypen = _context.MonsterTypes.ToList();
            var monsters = _context.Monsters.ToList();

            // Mapping: MagicClass ID -> Obermagie Bezeichnung
            var magicClassToObermagie = _context.MagicClasses
                .Include(mc => mc.Obermagie)
                .ToDictionary(
                    mc => mc.Id,
                    mc => mc.Obermagie?.Bezeichnung ?? "Unbekannt"
                );

            // Obermagien-Statistik: Zähle Charaktere pro Obermagie
            var obermagienStats = _context.CharacterMagicClasses
                .AsEnumerable() // Zur Client-Evaluierung wechseln
                .GroupBy(cmc => magicClassToObermagie.ContainsKey(cmc.MagicClassId)
                    ? magicClassToObermagie[cmc.MagicClassId]
                    : "Unbekannt")
                .ToDictionary(g => g.Key, g => g.Count());

            // Detaillierte Magieklassen-Statistik pro Obermagie für Tooltips
            var magicClassDetails = _context.CharacterMagicClasses
                .Include(cmc => cmc.MagicClass)
                    .ThenInclude(mc => mc.Obermagie)
                .AsEnumerable()
                .GroupBy(cmc => cmc.MagicClass.Obermagie?.Bezeichnung ?? "Unbekannt")
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(cmc => cmc.MagicClass.Bezeichnung)
                          .ToDictionary(
                              mg => mg.Key,
                              mg => mg.Count()
                          )
                );

            // Geschlechter-Statistik
            var genderStats = _context.Characters
                .GroupBy(c => c.Geschlecht)
                .ToDictionary(g => g.Key, g => g.Count());

            // Sternzeichen-Statistik
            var zodiacGroups = characters
                .Where(c => !string.IsNullOrEmpty(c.Geburtsdatum))
                .GroupBy(c => ZodiacHelper.GetZodiacElement(c.Geburtsdatum))
                .Where(g => g.Key != null)
                .ToDictionary(
                    g => g.Key!,
                    g => g.Select(c => $"{c.Vorname} {c.Nachname}").ToList()
                );

            // Spielercharaktere und Begleitcharakter laden
            var playerCharacters = _context.Characters
                .Where(c => c.UserId != null)
                .Include(c => c.CharacterMagicClasses)
                    .ThenInclude(cmc => cmc.MagicClass)
                .ToList();

            var companionCharacters = _context.Characters
                .Where(c => c.IsCompanion)
                .Include(c => c.CharacterMagicClasses)
                    .ThenInclude(cmc => cmc.MagicClass)
                .ToList();


            // ViewModel erstellen
            var viewModel = new HomeViewModel
            {
                Characters = characters,
                MagicClasses = allMagicClasses,
                Guilds = allGuilds,
                Infanteries = allInfanteries,
                GenderStats = genderStats,
                Monstertyps = monstertypen,
                ZodiacStats = zodiacGroups,
                Obermagien = obermagienStats,
                MagicClassDetails = magicClassDetails,
                PlayerCharacters = playerCharacters,
                CompanionCharacters = companionCharacters,
                Monsters = monsters
            };

            return View(viewModel);
        }

        //Einbau Familienstammbaum
        public IActionResult FamilyTree(int characterId = 0)
        {
            //wenn kein Chracter angegeben ist, nimm einen zuf�lligen
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
                        .Where(c => c.Id != characterId) // ausgew�hlten Charakter ausschlie�en
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
