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

            var glossar = _context.GlossaryEntries.ToList();

            // Achievement-Statistiken
            var totalUserAchievements = _context.Achievements.Count(a => a.Scope == AchievementScope.User);
            var totalGuildAchievements = _context.Achievements.Count(a => a.Scope == AchievementScope.Guild);
            var unlockedUserAchievements = _context.UserAchievements.Select(ua => ua.AchievementId).Distinct().Count();
            var unlockedGuildAchievements = _context.GuildAchievements.Select(ga => ga.AchievementId).Distinct().Count();
            var totalAchievementPoints = _context.UserAchievements
                .Include(ua => ua.Achievement)
                .Sum(ua => ua.Achievement.Points) +
                _context.GuildAchievements
                .Include(ga => ga.Achievement)
                .Sum(ga => ga.Achievement.Points);

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
                Monsters = monsters,
                glossaryEntries = glossar,
                TotalUserAchievements = totalUserAchievements,
                TotalGuildAchievements = totalGuildAchievements,
                UnlockedUserAchievements = unlockedUserAchievements,
                UnlockedGuildAchievements = unlockedGuildAchievements,
                TotalAchievementPoints = totalAchievementPoints
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

        [HttpGet]
        public IActionResult GetFamilyTreeData(int characterId = 0)
        {
            if (characterId == 0)
            {
                var randomCharacter = _context.Characters.FirstOrDefault();
                if (randomCharacter != null)
                {
                    characterId = randomCharacter.Id;
                }
                else
                {
                    return NotFound(new { error = "Keine Charaktere in der Datenbank gefunden." });
                }
            }

            var character = _context.Characters.FirstOrDefault(c => c.Id == characterId);
            if (character == null)
            {
                return NotFound(new { error = "Character nicht gefunden." });
            }

            // Erstelle hierarchische Datenstruktur für D3.js
            var treeData = BuildFamilyTreeNode(character);

            return Json(treeData);
        }

        [HttpGet]
        public IActionResult GetBidirectionalFamilyTreeData(int characterId = 0)
        {
            try
            {
                if (characterId == 0)
                {
                    var randomCharacter = _context.Characters.FirstOrDefault();
                    if (randomCharacter != null)
                    {
                        characterId = randomCharacter.Id;
                    }
                    else
                    {
                        return NotFound(new { error = "Keine Charaktere in der Datenbank gefunden." });
                    }
                }

                var character = _context.Characters.FirstOrDefault(c => c.Id == characterId);
                if (character == null)
                {
                    return NotFound(new { error = "Character nicht gefunden." });
                }

                // Lade Geschwister
                var siblings = new List<Character>();
                if (character.VaterId.HasValue || character.MutterId.HasValue)
                {
                    siblings = _context.Characters
                        .Where(c =>
                            (character.VaterId.HasValue && c.VaterId == character.VaterId) ||
                            (character.MutterId.HasValue && c.MutterId == character.MutterId))
                        .Where(c => c.Id != characterId)
                        .ToList();
                }

                // HashSets zum Tracking bereits besuchter IDs (Schutz gegen Endlosschleifen)
                var visitedAncestors = new HashSet<int>();
                var visitedDescendants = new HashSet<int>();

                // Erstelle bidirektionale Datenstruktur
                var treeData = new
                {
                    root = new
                    {
                        id = character.Id,
                        name = $"{character.Vorname} {character.Nachname}",
                        geschlecht = character.Geschlecht,
                        geburtsdatum = character.Geburtsdatum,
                        imagePath = character.ImagePath,
                        isRoot = true
                    },
                    ancestors = BuildAncestorTree(character, visitedAncestors),
                    descendants = BuildDescendantTree(character, visitedDescendants),
                    siblings = siblings.Select(s => new
                    {
                        id = s.Id,
                        name = $"{s.Vorname} {s.Nachname}",
                        geschlecht = s.Geschlecht,
                        geburtsdatum = s.Geburtsdatum,
                        imagePath = s.ImagePath,
                        isSibling = true
                    }).ToList(),
                    partner = character.PartnerId.HasValue ? GetPartnerData(character.PartnerId.Value) : null
                };

                return Json(treeData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der Familiendaten für Character {CharacterId}", characterId);
                return StatusCode(500, new { error = $"Fehler beim Laden der Daten: {ex.Message}" });
            }
        }

        private object? BuildAncestorTree(Character character, HashSet<int> visitedIds)
        {
            if (character == null || visitedIds.Contains(character.Id))
            {
                return null; // Schutz gegen zirkuläre Referenzen
            }

            visitedIds.Add(character.Id);
            var parents = new List<object>();

            try
            {
                if (character.VaterId.HasValue && character.VaterId.Value > 0)
                {
                    var father = _context.Characters.Find(character.VaterId.Value);
                    if (father != null && !visitedIds.Contains(father.Id))
                    {
                        parents.Add(new
                        {
                            id = father.Id,
                            name = $"{father.Vorname} {father.Nachname}",
                            geschlecht = father.Geschlecht,
                            geburtsdatum = father.Geburtsdatum,
                            imagePath = father.ImagePath,
                            type = "Vater",
                            parents = BuildAncestorTree(father, visitedIds)
                        });
                    }
                }

                if (character.MutterId.HasValue && character.MutterId.Value > 0)
                {
                    var mother = _context.Characters.Find(character.MutterId.Value);
                    if (mother != null && !visitedIds.Contains(mother.Id))
                    {
                        parents.Add(new
                        {
                            id = mother.Id,
                            name = $"{mother.Vorname} {mother.Nachname}",
                            geschlecht = mother.Geschlecht,
                            geburtsdatum = mother.Geburtsdatum,
                            imagePath = mother.ImagePath,
                            type = "Mutter",
                            parents = BuildAncestorTree(mother, visitedIds)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der Vorfahren für Character {CharacterId}", character.Id);
            }

            return parents.Any() ? parents : null;
        }

        private object? BuildDescendantTree(Character character, HashSet<int> visitedIds)
        {
            if (character == null || visitedIds.Contains(character.Id))
            {
                return null; // Schutz gegen zirkuläre Referenzen
            }

            visitedIds.Add(character.Id);
            var descendants = new List<object>();

            try
            {
                var children = _context.Characters
                    .Where(c => (c.VaterId == character.Id || c.MutterId == character.Id) && c.Id != character.Id)
                    .ToList();

                foreach (var child in children)
                {
                    if (child != null && !visitedIds.Contains(child.Id))
                    {
                        descendants.Add(new
                        {
                            id = child.Id,
                            name = $"{child.Vorname} {child.Nachname}",
                            geschlecht = child.Geschlecht,
                            geburtsdatum = child.Geburtsdatum,
                            imagePath = child.ImagePath,
                            children = BuildDescendantTree(child, visitedIds)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der Nachkommen für Character {CharacterId}", character.Id);
            }

            return descendants.Any() ? descendants : null;
        }

        private object? GetPartnerData(int partnerId)
        {
            var partner = _context.Characters.Find(partnerId);
            if (partner == null)
                return null;

            return new
            {
                id = partner.Id,
                name = $"{partner.Vorname} {partner.Nachname}",
                geschlecht = partner.Geschlecht,
                geburtsdatum = partner.Geburtsdatum,
                imagePath = partner.ImagePath,
                isPartner = true
            };
        }

        private object BuildFamilyTreeNode(Character character)
        {
            var allChildren = new List<object>();

            // Lade Partner
            if (character.PartnerId.HasValue)
            {
                var partner = _context.Characters.Find(character.PartnerId.Value);
                if (partner != null)
                {
                    // Lade Kinder des Partners aus anderen Beziehungen
                    var partnerChildren = _context.Characters
                        .Where(c => (c.VaterId == partner.Id || c.MutterId == partner.Id) &&
                                    c.VaterId != character.Id && c.MutterId != character.Id)
                        .ToList();

                    var partnerChildNodes = partnerChildren.Select(child => BuildFamilyTreeNode(child)).ToList();

                    // Partner als spezieller Knoten
                    allChildren.Add(new
                    {
                        id = partner.Id,
                        name = $"{partner.Vorname} {partner.Nachname}",
                        geschlecht = partner.Geschlecht,
                        geburtsdatum = partner.Geburtsdatum,
                        imagePath = partner.ImagePath,
                        isPartner = true,
                        children = partnerChildNodes.Any() ? partnerChildNodes : (object?)null
                    });
                }
            }

            // Lade gemeinsame Kinder
            var children = _context.Characters
                .Where(c => c.VaterId == character.Id || c.MutterId == character.Id)
                .ToList();

            var childNodes = children.Select(child => BuildFamilyTreeNode(child)).ToList();
            allChildren.AddRange(childNodes);

            return new
            {
                id = character.Id,
                name = $"{character.Vorname} {character.Nachname}",
                geschlecht = character.Geschlecht,
                geburtsdatum = character.Geburtsdatum,
                imagePath = character.ImagePath,
                isPartner = false,
                children = allChildren.Any() ? allChildren : null
            };
        }

        /// <summary>
        /// Öffentliche Achievement-Übersicht für alle Spieler und Gilden
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> AchievementOverview()
        {
            // Alle Achievements laden
            var allAchievements = await _context.Achievements
                .OrderBy(a => a.Scope)
                .ThenBy(a => a.Category)
                .ThenBy(a => a.Points)
                .ToListAsync();

            // User-Achievements
            var userAchievements = await _context.UserAchievements
                .Include(ua => ua.Achievement)
                .Include(ua => ua.User)
                .ToListAsync();

            // Gilden-Achievements
            var guildAchievements = await _context.GuildAchievements
                .Include(ga => ga.Achievement)
                .Include(ga => ga.Guild)
                .ToListAsync();

            // Spieler mit ihren Characters laden
            var spielerUserIds = await _context.UserRoles
                .Where(ur => _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Spieler"))
                .Select(ur => ur.UserId)
                .ToListAsync();

            var users = await _context.Users
                .Where(u => spielerUserIds.Contains(u.Id))
                .ToListAsync();

            var characters = await _context.Characters
                .Where(c => spielerUserIds.Contains(c.UserId))
                .ToDictionaryAsync(c => c.UserId, c => c);

            var players = users.Select(u => new
            {
                User = u,
                Character = characters.ContainsKey(u.Id) ? characters[u.Id] : null,
                AchievementCount = userAchievements.Count(ua => ua.UserId == u.Id),
                TotalPoints = userAchievements.Where(ua => ua.UserId == u.Id).Sum(ua => ua.Achievement.Points)
            }).ToList();

            // Gilden mit Achievements
            var allGuilds = await _context.Guilds.ToListAsync();

            var guilds = allGuilds.Select(g => new
            {
                Guild = g,
                AchievementCount = guildAchievements.Count(ga => ga.GuildId == g.Id),
                TotalPoints = guildAchievements.Where(ga => ga.GuildId == g.Id).Sum(ga => ga.Achievement.Points)
            }).ToList();

            // Statistiken
            var totalUserAchievements = allAchievements.Count(a => a.Scope == AchievementScope.User);
            var totalGuildAchievements = allAchievements.Count(a => a.Scope == AchievementScope.Guild);
            var unlockedUserAchievements = userAchievements.Select(ua => ua.AchievementId).Distinct().Count();
            var unlockedGuildAchievements = guildAchievements.Select(ga => ga.AchievementId).Distinct().Count();

            ViewBag.AllAchievements = allAchievements;
            ViewBag.UserAchievements = userAchievements;
            ViewBag.GuildAchievements = guildAchievements;
            ViewBag.Players = players;
            ViewBag.Guilds = guilds;
            ViewBag.TotalUserAchievements = totalUserAchievements;
            ViewBag.TotalGuildAchievements = totalGuildAchievements;
            ViewBag.UnlockedUserAchievements = unlockedUserAchievements;
            ViewBag.UnlockedGuildAchievements = unlockedGuildAchievements;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
