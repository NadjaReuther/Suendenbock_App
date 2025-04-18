using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models;

namespace Suendenbock_App.Data.Seeders
{
    public static class DatabaseSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            SeedLightCards(context);
            SeedMagicClass(context);

            context.SaveChanges();
        }

        private static void SeedLightCards(ApplicationDbContext context)
        {
            if (!context.LightCards.Any())
            {
                context.LightCards.AddRange(
                    new LightCards
                    {
                        cssClass = "card-hover-red",
                    },
                    new LightCards
                    {
                        cssClass = "card-hover-blue",
                    },
                    new LightCards
                    {
                        cssClass = "card-hover-violett",
                    },
                    new LightCards
                    {
                        cssClass = "card-hover-green",
                    },
                    new LightCards
                    {
                        cssClass = "card-hover-yellow",
                    },
                    new LightCards
                    {
                        cssClass = "card-hover-brown",
                    },
                    new LightCards
                    {
                        cssClass = "card-hover-shadow",
                    }
                );
            }
        }

        private static void SeedMagicClass(ApplicationDbContext context)
        {
            if (!context.MagicClasses.Any())
            {
                context.MagicClasses.AddRange(
                    new MagicClassModel
                    {
                        Bezeichnung = "Feuer",
                        ImagePath = "/images/magicclass/fire.png",
                        LightCardsId = 1
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Wasser",
                        ImagePath = "/images/magicclass/water.png",
                        LightCardsId = 2
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Eis",
                        ImagePath = "/images/magicclass/ice.png",
                        LightCardsId = 3
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Tür",
                        ImagePath = "/images/magicclass/door.png",
                        LightCardsId = 6
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Barriere",
                        ImagePath = "/images/magicclass/barrier.png",
                        LightCardsId = 7
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Holz",
                        ImagePath = "/images/magicclass/wood.png",
                        LightCardsId = 4
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Schatten",
                        ImagePath = "/images/magicclass/shadow.png",
                        LightCardsId = 7
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Dunkel",
                        ImagePath = "/images/magicclass/dark.png",
                        LightCardsId = 7
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Licht",
                        ImagePath = "/images/magicclass/light.png",
                        LightCardsId = 5
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Wind",
                        ImagePath = "/images/magicclass/wind.png",
                        LightCardsId = 7
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Erde",
                        ImagePath = "/images/magicclass/earth.png",
                        LightCardsId = 6
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Elektro",
                        ImagePath = "/images/magicclass/electric.png",
                        LightCardsId = 3
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Nebel",
                        ImagePath = "/images/magicclass/fog.png",
                        LightCardsId = 7
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Illusion",
                        ImagePath = "/images/magicclass/illusion.png",
                        LightCardsId = 3
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Nekromantie",
                        ImagePath = "/images/magicclass/necromancy.png",
                        LightCardsId = 7
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Heilig",
                        ImagePath = "/images/magicclass/holy.png",
                        LightCardsId = 4
                    }
                );
            }
        }

        private static void SeedGuilds(ApplicationDbContext context)
        {
            if (!context.Guilds.Any())
            {
                context.Guilds.AddRange(
                    new GuildModel
                    {
                        Name = "Bärenklaue",
                        ImagePath = "/images/guild/baerenklaue.png"
                    },
                    new GuildModel
                    {
                        Name = "Feuerhand",
                        ImagePath = "/images/guild/feuerhand.png"
                    }
                );
            }
        }
    }
}
