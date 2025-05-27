using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Data.Seeders
{
    public static class DatabaseSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            try
            {
                //Basis-Entitäten zuerst (ohne Abhängigkeiten)
                Console.WriteLine("Seeding Basis-Entitäten...");
                LightCardSeeder.Seed(context);
                ReligionSeeder.Seed(context);
                AbenteuerrangSeeder.Seed(context);
                AnmeldungsstatusSeeder.Seed(context);
                EindruckSeeder.Seed(context);
                StandSeeder.Seed(context);
                HausSeeder.Seed(context);
                BlutgruppeSeeder.Seed(context);
                HerkunftslandSeeder.Seed(context);

                // 2. MagicClasses (abhängig von LightCards)
                Console.WriteLine("Seeding MagicClasses...");
                ObermagicSeeder.Seed(context);
                MagicClassSeeder.Seed(context);
                ZaubertypSeeder.Seed(context);

                // 3. Guilds (abhängig von LightCards)
                Console.WriteLine("Seeding Guilds...");
                AbenteuerrangSeeder.Seed(context);
                AnmeldungsstatusSeeder.Seed(context);
                EindruckSeeder.Seed(context);

                Console.WriteLine("Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during database seeding: {ex.Message}");
                throw;
            }
        }
    }
}
