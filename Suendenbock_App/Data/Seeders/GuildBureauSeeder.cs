using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Data.Seeders
{
        public static class AbenteuerrangSeeder
        {
            public static void Seed(ApplicationDbContext context)
            {
                if (!context.Set<Abenteuerrang>().Any())
                {
                    context.Set<Abenteuerrang>().AddRange(
                        new Abenteuerrang { Name = "E-Rang" },
                        new Abenteuerrang { Name = "D-Rang" },
                        new Abenteuerrang { Name = "C-Rang" },
                        new Abenteuerrang { Name = "B-Rang" },
                        new Abenteuerrang { Name = "A-Rang" },
                        new Abenteuerrang { Name = "S-Rang" },
                        new Abenteuerrang { Name = "SS-Rang" },
                        new Abenteuerrang { Name = "SSS-Rang" }
                    );
                    context.SaveChanges();
                }
            }
        }

        public static class AnmeldungsstatusSeeder
        {
            public static void Seed(ApplicationDbContext context)
            {
                if (!context.Set<Anmeldungsstatus>().Any())
                {
                    context.Set<Anmeldungsstatus>().AddRange(                        
                        new Anmeldungsstatus { Name = "Geschlossen" },
                        new Anmeldungsstatus { Name = "Suchend ohne Anmeldung" },
                        new Anmeldungsstatus { Name = "Suchend mit Anmeldung" },
                        new Anmeldungsstatus { Name = "Anwerbend" }
                    );
                    context.SaveChanges();
                }
            }
        }

        public static class EindruckSeeder
        {
            public static void Seed(ApplicationDbContext context)
            {
                if (!context.Set<Eindruck>().Any())
                {
                    context.Set<Eindruck>().AddRange(
                        new Eindruck { Name = "Verbündeter" },
                        new Eindruck { Name = "Feind" },
                        new Eindruck { Name = "Neutral" },
                        new Eindruck { Name = "Angespannt" },
                        new Eindruck { Name = "Zugeneigt" }
                    );
                    context.SaveChanges();
                }
            }
        }
}
