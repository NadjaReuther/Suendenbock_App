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
                    new LightCard { CssClass = "dunkel" },
                    new LightCard { CssClass = "luft" },
                    new LightCard { CssClass = "wasser" },
                    new LightCard { CssClass = "seele" },
                    new LightCard { CssClass = "flora" },
                    new LightCard { CssClass = "fauna" },
                    new LightCard { CssClass = "licht" },
                    new LightCard { CssClass = "schmerz" },
                    new LightCard { CssClass = "kunst" },
                    new LightCard { CssClass = "geist" },
                    new LightCard { CssClass = "feuer" },
                    new LightCard { CssClass = "zeit" },
                    new LightCard { CssClass = "mantik" },
                    new LightCard { CssClass = "daemmerung" }
                );
                context.SaveChanges();
            }
        }
    }
}
