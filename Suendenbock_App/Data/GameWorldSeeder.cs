using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Data
{
    public class GameWorldSeeder
    {
        public static async Task SeedGameWorldsAsync(ApplicationDbContext context)
        {
            // Prüfe ob GameWorlds bereits existieren
            if (await context.GameWorlds.AnyAsync())
            {
                await OriginalweltSicherstellenAsync(context);
                return; // Bereits initialisiert
            }

            var gameWorlds = new List<GameWorld>
            {
                new GameWorld
                {
                    Name = "Sündenbock 1618",
                    Description = "Die Hauptspielwelt im 30-jährigen Krieg",
                    Era = "1618",
                    IsOriginWorld = true,
                    ColorCode = "#8B4513", // Braun für historisches Setting
                    IsActive = true,
                    SortOrder = 1
                },
                new GameWorld
                {
                    Name = "1810 Season Zwei",
                    Description = "Zeitalter der Industrialisierung mit Dampfmaschinen",
                    Era = "1810",
                    ColorCode = "#4A5568", // Grau/Stahl für industrielles Setting
                    IsActive = true,
                    SortOrder = 2
                }
            };

            await context.GameWorlds.AddRangeAsync(gameWorlds);
            await context.SaveChangesAsync();

            // Hole alle GameWorlds aus der DB (jetzt mit IDs)
            var world1618 = await context.GameWorlds.FirstAsync(gw => gw.Name == "Sündenbock 1618");
            var world1810 = await context.GameWorlds.FirstAsync(gw => gw.Name == "1810 Season Zwei");

            // WICHTIG: Alle bestehenden Characters zu BEIDEN Welten hinzufügen
            var existingCharacters = await context.Characters.ToListAsync();

            if (existingCharacters.Any())
            {
                var characterGameWorlds = new List<CharacterGameWorld>();

                foreach (var character in existingCharacters)
                {
                    // Character zu 1618 hinzufügen (Origin World = true)
                    characterGameWorlds.Add(new CharacterGameWorld
                    {
                        CharacterId = character.Id,
                        GameWorldId = world1618.Id,
                        IsOriginWorld = true, // Bestehende Characters stammen ursprünglich aus 1618
                        AddedAt = DateTime.UtcNow
                    });

                    // Character auch zu 1810 hinzufügen (Origin World = false)
                    characterGameWorlds.Add(new CharacterGameWorld
                    {
                        CharacterId = character.Id,
                        GameWorldId = world1810.Id,
                        IsOriginWorld = false,
                        AddedAt = DateTime.UtcNow
                    });
                }

                await context.CharacterGameWorlds.AddRangeAsync(characterGameWorlds);
                await context.SaveChangesAsync();
            }
        }
    
        /// <summary>
        /// Ist noch keine Originalwelt markiert (z.B. nach der Migration), wird "Sündenbock 1618" gesetzt,
        /// sonst die erste Welt nach Sortierung. Die Zuordnungen bekommen den passenden IsOriginWorld-Wert.
        /// </summary>
        public static async Task OriginalweltSicherstellenAsync(ApplicationDbContext context)
        {
            if (await context.GameWorlds.AnyAsync(gw => gw.IsOriginWorld))
            {
                return;
            }

            var original = await context.GameWorlds.FirstOrDefaultAsync(gw => gw.Name == "Sündenbock 1618")
                           ?? await context.GameWorlds.OrderBy(gw => gw.SortOrder).FirstOrDefaultAsync();
            if (original == null)
            {
                return;
            }

            original.IsOriginWorld = true;
            foreach (var cgw in await context.CharacterGameWorlds.ToListAsync())
            {
                cgw.IsOriginWorld = cgw.GameWorldId == original.Id;
            }
            await context.SaveChangesAsync();
        }
    }
}
