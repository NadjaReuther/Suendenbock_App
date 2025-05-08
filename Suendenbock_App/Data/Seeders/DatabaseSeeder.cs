using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models.Domain;

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
            context.SaveChanges();

            SeedMagicClass(context);
            SeedGuilds(context);
            SeedReligions(context);

            context.SaveChanges();
        }

        private static void SeedLightCards(ApplicationDbContext context)
        {
            if (!context.LightCards.Any())
            {
                context.LightCards.AddRange(
                    new LightCard
                    {
                        CssClass = "card-hover-red",
                    },
                    new LightCard
                    {
                        CssClass = "card-hover-blue",
                    },
                    new LightCard
                    {
                        CssClass = "card-hover-violett",
                    },
                    new LightCard
                    {
                        CssClass = "card-hover-green",
                    },
                    new LightCard
                    {
                        CssClass = "card-hover-yellow",
                    },
                    new LightCard
                    {
                        CssClass = "card-hover-brown",
                    },
                    new LightCard
                    {
                        CssClass = "card-hover-shadow",
                    }
                );
            }
        }

        private static void SeedMagicClass(ApplicationDbContext context)
        {
            if (!context.MagicClasses.Any())
            {
                context.MagicClasses.AddRange(
                    new MagicClass
                    {
                        Bezeichnung = "Feuer",
                        ImagePath = "/images/magicclass/fire.png",
                        LightCardId = 1
                    },
                    new MagicClass
                    {
                        Bezeichnung = "Wasser",
                        ImagePath = "/images/magicclass/water.png",
                        LightCardId = 2
                    },
                    new MagicClass
                    {
                        Bezeichnung = "Eis",
                        ImagePath = "/images/magicclass/ice.png",
                        LightCardId = 2
                    },
                    new MagicClass
                    {
                        Bezeichnung = "Tür",
                        ImagePath = "/images/magicclass/door.png",
                        LightCardId = 6
                    },
                    new MagicClass
                    {
                        Bezeichnung = "Barriere",
                        ImagePath = "/images/magicclass/barrier.png",
                        LightCardId = 2
                    },
                    new MagicClass
                    {
                        Bezeichnung = "Holz",
                        ImagePath = "/images/magicclass/wood.png",
                        LightCardId = 6
                    },
                    new MagicClass
                    {
                        Bezeichnung = "Schatten",
                        ImagePath = "/images/magicclass/shadow.png",
                        LightCardId = 7
                    },
                    new MagicClass
                    {
                        Bezeichnung = "Zwielicht",
                        ImagePath = "/images/magicclass/dark.png",
                        LightCardId = 7
                    },
                    new MagicClass
                    {
                        Bezeichnung = "Wind",
                        ImagePath = "/images/magicclass/wind.png",
                        LightCardId = 7
                    },
                    new MagicClass
                    {
                        Bezeichnung = "Erde",
                        ImagePath = "/images/magicclass/earth.png",
                        LightCardId = 6
                    },
                    new MagicClass
                    {
                        Bezeichnung = "Elektro",
                        ImagePath = "/images/magicclass/electric.png",
                        LightCardId = 5
                    },
                    new MagicClass
                    {
                        Bezeichnung = "Nebel",
                        ImagePath = "/images/magicclass/fog.png",
                        LightCardId = 7
                    },
                    new MagicClass
                    {
                        Bezeichnung = "Illusion",
                        ImagePath = "/images/magicclass/illusion.png",
                        LightCardId = 6
                    },
                    new MagicClass
                    {
                        Bezeichnung = "Nekromantie",
                        ImagePath = "/images/magicclass/necromancy.png",
                        LightCardId = 7
                    },
                    new MagicClass
                    {
                        Bezeichnung = "Heilig",
                        ImagePath = "/images/magicclass/holy.png",
                        LightCardId = 5
                    }
                );
            }
        }

        private static void SeedGuilds(ApplicationDbContext context)
        {
            if (!context.Guilds.Any())
            {
                context.Guilds.AddRange(
                    new Guild
                    {
                        Name = "Bärenklaue",
                        ImagePath = "/images/guild/baerenklaue.png",
                        LightCardId = 6,
                        Description = "ist die Gilde von Brönn"
                    },
                    new Guild
                    {
                        Name = "Wolkenbruch",
                        ImagePath = "/images/guild/wolkenbruch.png",
                        LightCardId = 7,
                        Description = "sind wir"
                    },
                    new Guild
                    {
                        Name = "Schneesturm",
                        ImagePath = "/images/guild/schneesturm.png",
                        LightCardId = 2,
                        Description = "krasse Scheisser"
                    },
                    new Guild
                    {
                        Name = "Schwerthieb",
                        ImagePath = "/images/guild/schwerthieb.png",
                        LightCardId = 7,
                        Description = "irgendwas mit Upiren"
                    }
                );
            }
        }
        private static void SeedReligions(ApplicationDbContext context)
        {
            if (!context.Religions.Any())
            {
                context.Religions.AddRange(
                    new Religion
                    {
                        Type = "Lutheranisch"
                    },
                    new Religion
                    {
                        Type = "Katholisch"
                    },
                    new Religion
                    {
                        Type = "alte Götter"
                    },
                    new Religion
                    {
                        Type = "calvinistisch"
                    },
                    new Religion
                    {
                        Type = "orthodox"
                    }
                );
            }
        }
    }
}
