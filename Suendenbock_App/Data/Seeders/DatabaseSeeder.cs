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
                        Description = "Feuer ist ein Element, das mit Zerstörung und Leidenschaft assoziiert wird.",
                        ImagePath = "/images/fire.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Wasser",
                        Description = "Wasser steht für Fluss, Anpassungsfähigkeit und Heilung.",
                        ImagePath = "/images/water.png"
                    },
                    new MagicClassModel
                    {
                        Bezeichnung = "Eis",
                        Description = "Eis ist ein Element, das mit Zerstörung und Kälte assoziiert wird.",
                        ImagePath = "/images/fire.png"
                    }
                );
            }
        }
    }
}
