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
                        ImagePath = "/images/fire.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Wasser",
                        ImagePath = "/images/water.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Eis",
                        ImagePath = "/images/ice.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Tür",
                        ImagePath = "/images/door.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Barriere",
                        ImagePath = "/images/barrier.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Holz",
                        ImagePath = "/images/wood.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Schatten",
                        ImagePath = "/images/shadow.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Dunkel",
                        ImagePath = "/images/dark.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Licht",
                        ImagePath = "/images/light.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Wind",
                        ImagePath = "/images/wind.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Erde",
                        ImagePath = "/images/earth.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Elektro",
                        ImagePath = "/images/electric.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Nebel",
                        ImagePath = "/images/fog.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Illusion",
                        ImagePath = "/images/illusion.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Nekromantie",
                        ImagePath = "/images/necromancy.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Heilig",
                        ImagePath = "/images/holy.png"
                    }
                );
            }
        }
    }
}
