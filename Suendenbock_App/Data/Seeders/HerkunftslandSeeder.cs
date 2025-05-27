using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Data.Seeders
{
    public static class HerkunftslandSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Herkunftsland>().Any())
            {
                context.Set<Herkunftsland>().AddRange(
                    new Herkunftsland { Name = "Dänemark" },
                    new Herkunftsland { Name = "Schweden" },
                    new Herkunftsland { Name = "Finnland" },
                    new Herkunftsland { Name = "Norwegen" },
                    new Herkunftsland { Name = "Holstein" },
                    new Herkunftsland { Name = "Schleswig" },
                    new Herkunftsland { Name = "Mecklenburg" },
                    new Herkunftsland { Name = "Pommern" },
                    new Herkunftsland { Name = "Polen" },
                    new Herkunftsland { Name = "Preußen" },
                    new Herkunftsland { Name = "Brandenburg" },
                    new Herkunftsland { Name = "Schlesien" },
                    new Herkunftsland { Name = "Braunschweig-Lüneburg" },
                    new Herkunftsland { Name = "Sachsen" },
                    new Herkunftsland { Name = "Böhmen" },
                    new Herkunftsland { Name = "Mähren" },
                    new Herkunftsland { Name = "Österreich" },
                    new Herkunftsland { Name = "Ungarn" },
                    new Herkunftsland { Name = "Schweizerische Eidschaft" },
                    new Herkunftsland { Name = "Münster" },
                    new Herkunftsland { Name = "Flandern" },
                    new Herkunftsland { Name = "Niederlande" },
                    new Herkunftsland { Name = "England" },
                    new Herkunftsland { Name = "Brabant" },
                    new Herkunftsland { Name = "Frankreich" },
                    new Herkunftsland { Name = "Spanien" },
                    new Herkunftsland { Name = "Kölln" },
                    new Herkunftsland { Name = "Luxemburg" },
                    new Herkunftsland { Name = "Hessen" },
                    new Herkunftsland { Name = "Franken" },
                    new Herkunftsland { Name = "Bayern" },
                    new Herkunftsland { Name = "Württemberg" },
                    new Herkunftsland { Name = "Ruppin" },
                    new Herkunftsland { Name = "Leipzick" },
                    new Herkunftsland { Name = "Lippe" },
                    new Herkunftsland { Name = "Lausitz" },
                    new Herkunftsland { Name = "Salzburg" },
                    new Herkunftsland { Name = "Burgund" },
                    new Herkunftsland { Name = "Lothringen" },
                    new Herkunftsland { Name = "Savoyen" },
                    new Herkunftsland { Name = "Genua" },
                    new Herkunftsland { Name = "Mailand" },
                    new Herkunftsland { Name = "Venezien" },
                    new Herkunftsland { Name = "Este" },
                    new Herkunftsland { Name = "Manua" },
                    new Herkunftsland { Name = "Löwenstein" },
                    new Herkunftsland { Name = "Baden" },
                    new Herkunftsland { Name = "Speyer" },
                    new Herkunftsland { Name = "Lucca" },
                    new Herkunftsland { Name = "Florenz" },
                    new Herkunftsland { Name = "Trient" },
                    new Herkunftsland { Name = "Erbach" },
                    new Herkunftsland { Name = "Trier" },
                    new Herkunftsland { Name = "Saarbrücken" }

                );
                context.SaveChanges();
            }
        }
    }
}
