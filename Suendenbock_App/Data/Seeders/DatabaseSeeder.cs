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

            SeedMagicClass(context);

            context.SaveChanges();
        }

        private static void SeedMagicClass(ApplicationDbContext context)
        {
            if (!context.MagicClasses.Any())
            {
                context.MagicClasses.AddRange(
                    new MagicClassModel
                    {
                        Bezeichnung = "Feuer",
                        ImagePath = "/images/magicclass/fire.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Wasser",
                        ImagePath = "/images/magicclass/water.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Eis",
                        ImagePath = "/images/magicclass/ice.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Tür",
                        ImagePath = "/images/magicclass/door.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Barriere",
                        ImagePath = "/images/magicclass/barrier.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Holz",
                        ImagePath = "/images/magicclass/wood.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Schatten",
                        ImagePath = "/images/magicclass/shadow.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Dunkel",
                        ImagePath = "/images/magicclass/dark.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Licht",
                        ImagePath = "/images/magicclass/light.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Wind",
                        ImagePath = "/images/magicclass/wind.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Erde",
                        ImagePath = "/images/magicclass/earth.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Elektro",
                        ImagePath = "/images/magicclass/electric.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Nebel",
                        ImagePath = "/images/magicclass/fog.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Illusion",
                        ImagePath = "/images/magicclass/illusion.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Nekromantie",
                        ImagePath = "/images/magicclass/necromancy.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Heilig",
                        ImagePath = "/images/magicclass/holy.png"
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
