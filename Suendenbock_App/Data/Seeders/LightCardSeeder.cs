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
                    new LightCard { CssClass = "dunkel", Farbcode = "#372334" },
                    new LightCard { CssClass = "luft", Farbcode = "#abb8d4" },
                    new LightCard { CssClass = "wasser", Farbcode = "#19879f" },
                    new LightCard { CssClass = "seele", Farbcode = "#d46aab" },
                    new LightCard { CssClass = "flora", Farbcode = "#226d22" },
                    new LightCard { CssClass = "fauna", Farbcode = "#604136" },
                    new LightCard { CssClass = "licht", Farbcode = "#f8ecd0" },
                    new LightCard { CssClass = "schmerz", Farbcode = "#963742" },
                    new LightCard { CssClass = "kunst", Farbcode = "#f6c780" },
                    new LightCard { CssClass = "geist", Farbcode = "#431b92" },
                    new LightCard { CssClass = "feuer", Farbcode = "#bb2929" },
                    new LightCard { CssClass = "zeit", Farbcode = "#bd9a4c" },
                    new LightCard { CssClass = "mantik", Farbcode = "#597476" },
                    new LightCard { CssClass = "daemmerung", Farbcode = "#989693" }
                );
                context.SaveChanges();
            }
        }
    }
}
