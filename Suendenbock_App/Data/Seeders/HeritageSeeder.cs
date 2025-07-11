﻿using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Data.Seeders
{  
    public static class StandSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Stand>().Any())
            {
                context.Set<Stand>().AddRange(
                    new Stand { Name = "Klerus" },
                    new Stand { Name = "Medikus" },
                    new Stand { Name = "Hochadel" },
                    new Stand { Name = "Niederadel" },
                    new Stand { Name = "Wachen" },
                    new Stand { Name = "Bürger" },
                    new Stand { Name = "Kaufmann" },
                    new Stand { Name = "Gesindel" },
                    new Stand { Name = "Mätressen" },
                    new Stand { Name = "Schausteller" },
                    new Stand { Name = "Händler" },
                    new Stand { Name = "Handwerker" },
                    new Stand { Name = "Bauer" },
                    new Stand { Name = "Pilger" },
                    new Stand { Name = "Kriminelle" },
                    new Stand { Name = "Mittellose" },
                    new Stand { Name = "Soldaten" },
                    new Stand { Name = "Leibeigene" }
                );
                context.SaveChanges();
            }
        }
    }
    public static class BlutgruppeSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Blutgruppe>().Any())
            {
                context.Set<Blutgruppe>().AddRange(
                    new Blutgruppe { Name = "Wellernsche Brut", Besonderheiten = "" },
                    new Blutgruppe { Name = "Mensch", Besonderheiten = "" },
                    new Blutgruppe { Name = "Hexer", Besonderheiten = "" },
                    new Blutgruppe { Name = "Magier", Besonderheiten = "" },
                    new Blutgruppe { Name = "Profaner", Besonderheiten = "" },
                    new Blutgruppe { Name = "Druide", Besonderheiten = "" }
                );
                context.SaveChanges();
            }
        }
    }
    public static class RasseSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Rasse>().Any())
            {
                context.Set<Rasse>().AddRange(
                    new Rasse { Name = "Mensch"},
                    new Rasse { Name = "Upir" },
                    new Rasse { Name = "Lykantrop" },
                    new Rasse { Name = "Ulfhedinn" },
                    new Rasse { Name = "Konstrukteur" }                    
                );
                context.SaveChanges();
            }
        }
    }
    public static class LebensstatusSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Lebensstatus>().Any())
            {
                context.Set<Lebensstatus>().AddRange(
                    new Lebensstatus { Name = "tot (im Alles)" },
                    new Lebensstatus { Name = "tot (in der Leere)" },
                    new Lebensstatus { Name = "lebend" },
                    new Lebensstatus { Name = "wiedererweckt" },
                    new Lebensstatus { Name = "unsterblich" },
                    new Lebensstatus { Name = "Patron" },
                    new Lebensstatus { Name = "unbekannt" }
                );
                context.SaveChanges();
            }
        }
    }
}
