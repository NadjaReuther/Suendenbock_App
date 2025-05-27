using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Data.Seeders
{
    public static class ReligionSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Religions.Any())
            {
                context.Religions.AddRange(
                    new Religion { Type = "Lutheranisch" },
                    new Religion { Type = "Katholisch" },
                    new Religion { Type = "Alte Götter" },
                    new Religion { Type = "Calvinistisch" },
                    new Religion { Type = "Orthodox" },                 
                    new Religion { Type = "Atheismus" },
                    new Religion { Type = "Agnostizismus" },
                    new Religion { Type = "Täufer" }
                );
                context.SaveChanges();
            }
        }
    }
}