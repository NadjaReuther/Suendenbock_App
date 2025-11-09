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
                StandSeeder.Seed(context);
                HausSeeder.Seed(context);
                BlutgruppeSeeder.Seed(context);
                RasseSeeder.Seed(context);
                LebensstatusSeeder.Seed(context);
                HerkunftslandSeeder.Seed(context);
                InfanterieRangSeeder.Seed(context);

                // 2. MagicClasses (abhängig von LightCards)
                Console.WriteLine("Seeding MagicClasses...");
                ObermagicSeeder.Seed(context);
                MagicClassSeeder.Seed(context);
                ZaubertypSeeder.Seed(context);
                MagicClassSpecializationSeeder.Seed(context);

                // 3. Guilds (abhängig von LightCards)
                Console.WriteLine("Seeding Guilds...");
                AbenteuerrangSeeder.Seed(context);
                AnmeldungsstatusSeeder.Seed(context);
                EindruckSeeder.Seed(context);
                LizenzenSeeder.Seed(context);

                // 4. Monster
                Console.WriteLine("Seeding Monsters...");
                MonsterimmunitaetenSeeder.Seed(context);
                MonsteranfaelligkeitenSeeder.Seed(context);
                MonstergruppenSeeder.Seed(context);
                MonsterintelligenzSeeder.Seed(context);
                MonstervorkommenSeeder.Seed(context);
                MonsterwuerfelSeeder.Seed(context);   
                MonstertypSeeder.Seed(context);
                MonstertypanfaelligkeitenSeeder.Seed(context);
                MonstertypimmunitaetenSeeder.Seed(context);
                MonstertypvorkommenSeeder.Seed(context);

                // 5. Triggers
                //TriggerSeeder.Seed(context);

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
