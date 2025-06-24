using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Data.Seeders
{
    public static class LightCardSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.LightCards.Any())
            {
                context.LightCards.AddRange(
                    new LightCard { CssClass = "dunkel", Farbcode = "#372334", Bezeichnung = "Dunkel" },
                    new LightCard { CssClass = "luft", Farbcode = "#abb8d4", Bezeichnung = "Luft" },
                    new LightCard { CssClass = "wasser", Farbcode = "#19879f", Bezeichnung = "Wasser" },
                    new LightCard { CssClass = "seele", Farbcode = "#d46aab", Bezeichnung = "Seele" },
                    new LightCard { CssClass = "flora", Farbcode = "#226d22", Bezeichnung = "Flora" },
                    new LightCard { CssClass = "fauna", Farbcode = "#604136", Bezeichnung = "Fauna" },
                    new LightCard { CssClass = "licht", Farbcode = "#f8ecd0", Bezeichnung = "Licht" },
                    new LightCard { CssClass = "schmerz", Farbcode = "#963742", Bezeichnung = "Schmerz" },
                    new LightCard { CssClass = "kunst", Farbcode = "#f6c780", Bezeichnung = "Kunst" },
                    new LightCard { CssClass = "geist", Farbcode = "#431b92", Bezeichnung ="Geist" },
                    new LightCard { CssClass = "feuer", Farbcode = "#bb2929", Bezeichnung = "Feuer" },
                    new LightCard { CssClass = "zeit", Farbcode = "#bd9a4c", Bezeichnung = "Zeit" },
                    new LightCard { CssClass = "mantik", Farbcode = "#597476", Bezeichnung = "Mantik" },
                    new LightCard { CssClass = "daemmerung", Farbcode = "#989693", Bezeichnung = "Dämmerung" }
                );
                context.SaveChanges();
            }
        }
    }
}
