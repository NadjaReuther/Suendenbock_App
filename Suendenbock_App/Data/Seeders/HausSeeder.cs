using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Data.Seeders
{
    public static class HausSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Haus>().Any())
            {
                context.Set<Haus>().AddRange(
                    new Haus { Name = "Habsburg", ImagePath = "/images/houses/drachenstein.png" },
                    new Haus { Name = "Witt und Loon", ImagePath = "/images/houses/silbermond.png" },
                    new Haus { Name = "Wellern", ImagePath = "/images/houses/eisenfaust.png" },
                    new Haus { Name = "Sönneberg", ImagePath = "/images/houses/goldrose.png" },
                    new Haus { Name = "Küenburg", ImagePath = "/images/houses/sturmkrone.png" },
                    new Haus { Name = "Lippe-Brake", ImagePath = "/images/houses/schattenklinge.png" },
                    new Haus { Name = "Schlosser", ImagePath = "/images/houses/flammenherz.png" },
                    new Haus { Name = "Feigenwinter", ImagePath = "/images/houses/waldmark.png" },
                    new Haus { Name = "Landsberg", ImagePath = "/images/houses/hohenstein.png" },
                    new Haus { Name = "Wütherich", ImagePath = "/images/houses/wuetherich.png"},
                    new Haus { Name = "Thurn und Taxis", ImagePath = "/images/houses/hohenberg.png" },
                    new Haus { Name = "Rehlingen", ImagePath = "/images/houses/hohenberg.png" },
                    new Haus { Name = "Stöckl", ImagePath = "/images/houses/hohenberg.png" },
                    new Haus { Name = "Hohenzollern", ImagePath = "/images/houses/hohenberg.png" },
                    new Haus { Name = "Arnim", ImagePath = "/images/houses/hohenberg.png" },
                    new Haus { Name = "Schwerin", ImagePath = "/images/houses/hohenberg.png" },
                    new Haus { Name = "Derffinger", ImagePath = "/images/houses/hohenberg.png" },
                    new Haus { Name = "Kristoffel", ImagePath = "/images/houses/hohenberg.png" },
                    new Haus { Name = "Chudenitz", ImagePath = "/images/houses/hohenberg.png" },
                    new Haus { Name = "Hagsen-Höveln", ImagePath = "/images/houses/hohenberg.png" },
                    new Haus { Name = "Bünder-Wirren", ImagePath = "/images/houses/hohenberg.png" },
                    new Haus { Name = "Fugger", ImagePath = "/images/houses/hohenberg.png" },
                    new Haus { Name = "Leitzsch", ImagePath = "/images/houses/hohenberg.png" },
                    new Haus { Name = "Farnese", ImagePath = "/images/houses/hohenberg.png" },
                    new Haus { Name = "Oldenburg", ImagePath = "/images/houses/hohenberg.png" },
                    new Haus { Name = "Landshut", ImagePath = "/images/houses/hohenberg.png" }
                );
                context.SaveChanges();
            }
        }

    }
}
