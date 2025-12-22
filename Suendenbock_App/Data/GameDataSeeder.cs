using Suendenbock_App.Data;
using Suendenbock_App.Models;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Data
{
    public static class GameDataSeeder
    {
        public static async Task SeedGameDataAsync(ApplicationDbContext context)
        {
            // 1. Rest Food
            if (!context.RestFoods.Any())
            {
                var foods = new List<RestFood>
                {
                    new RestFood
                    {
                        Name = "verdorben",
                        HealthBonus = -40
                    },
                    new RestFood
                    {
                        Name = "nichts",
                        HealthBonus = -20
                    },
                    new RestFood
                    {
                        Name = "schlecht",
                        HealthBonus = 0                        
                    },
                    new RestFood
                    {
                        Name = "billig",
                        HealthBonus = 10                        
                    },
                    new RestFood
                    {
                        Name = "gut",
                        HealthBonus = 30                        
                    },
                    new RestFood
                    {
                        Name = "edel",
                        HealthBonus = 50
                    }
                };
                context.RestFoods.AddRange(foods);
            }

            // 2. Acts und Maps
            if (!context.Acts.Any())
            {
                var act1 = new Act
                {
                    Name = "Der Graben",
                    Description = "Die Gruppe kommt in den Graben",
                    ActNumber = 8,
                    IsActive = true
                };
                context.Acts.Add(act1);
                await context.SaveChangesAsync(); // Act braucht Id!

                var map1 = new Map
                {
                    Name = "Graben",
                    ImageUrl = "/images/maps/graben.jpg",
                    ActId = act1.Id
                };
                context.Maps.Add(map1);
                await context.SaveChangesAsync(); // Map braucht Id!

                var markers = new List<MapMarker>
                {
                    new MapMarker
                    {
                        MapId = map1.Id,
                        XPercent = 50,
                        YPercent = 30,
                        Label = "Prag",
                        Type = "settlement",
                        Description = "Die goldene Stadt"
                    },
                    new MapMarker
                    {
                        MapId = map1.Id,
                        XPercent = 65,
                        YPercent = 55,
                        Label = "Dunkler Wald",
                        Type = "danger",
                        Description = "Gerüchten zufolge lauern hier Banditen"
                    }
                };
                context.MapMarkers.AddRange(markers);
            }

            await context.SaveChangesAsync();
        }
    }
}