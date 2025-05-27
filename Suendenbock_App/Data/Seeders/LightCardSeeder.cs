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
                    new LightCard { CssClass = "card-hover-red" },
                    new LightCard { CssClass = "card-hover-blue" },
                    new LightCard { CssClass = "card-hover-violett" },
                    new LightCard { CssClass = "card-hover-green" },
                    new LightCard { CssClass = "card-hover-yellow" },
                    new LightCard { CssClass = "card-hover-brown" },
                    new LightCard { CssClass = "card-hover-shadow" },
                    new LightCard { CssClass = "card-hover-silver" },
                    new LightCard { CssClass = "card-hover-gold" }
                );
                context.SaveChanges();
            }
        }
    }
}
