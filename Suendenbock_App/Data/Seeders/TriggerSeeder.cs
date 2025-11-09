using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Data.Seeders
{
    public static class TriggerSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Prüfen ob bereits Daten vorhanden sind
            if (context.TriggerCategories.Any())
            {
                return; // Daten bereits vorhanden, nicht erneut seeden
            }

            // ===============================
            // KATEGORIEN ERSTELLEN
            // ===============================

            var categories = new List<TriggerCategory>
            {
                new TriggerCategory { Name = "Körperliche Gewalt & Kriegsgräuel", SortOrder = 1 },
                new TriggerCategory { Name = "Kinder & Familie", SortOrder = 2 },
                new TriggerCategory { Name = "Psychologische Themen & Zwischenmenschliches", SortOrder = 3 },
                new TriggerCategory { Name = "Sexuelle Gewalt & Ausbeutung", SortOrder = 4 },
                new TriggerCategory { Name = "Religiöse & Kulturelle Konflikte", SortOrder = 5 },
                new TriggerCategory { Name = "Tierleid", SortOrder = 6 },
                new TriggerCategory { Name = "Körperliches & Medizinisches", SortOrder = 7 }
            };

            context.TriggerCategories.AddRange(categories);
            context.SaveChanges();

            // ===============================
            // THEMEN ERSTELLEN
            // ===============================

            var topics = new List<TriggerTopic>();

            // Kategorie 1: Körperliche Gewalt & Kriegsgräuel
            var cat1 = categories[0];
            topics.AddRange(new[]
            {
                new TriggerTopic { CategoryId = cat1.Id, Name = "Grafische Beschreibung von Schlachten & Verletzungen", SortOrder = 1 },
                new TriggerTopic { CategoryId = cat1.Id, Name = "Folter & Verstümmelung", SortOrder = 2 },
                new TriggerTopic { CategoryId = cat1.Id, Name = "Hinrichtung (Rädern, Vierteilen, Erhängen, ...)", SortOrder = 3 },
                new TriggerTopic { CategoryId = cat1.Id, Name = "Massaker an Zivilisten (z.B. Plünderungen)", SortOrder = 4 },
                new TriggerTopic { CategoryId = cat1.Id, Name = "Verweseung, Leichenberge, Seuchen", SortOrder = 5 },
                new TriggerTopic { CategoryId = cat1.Id, Name = "Amputationen & frühe Medizin (ohne Narkose)", SortOrder = 6 }
            });

            // Kategorie 2: Kinder & Familie
            var cat2 = categories[1];
            topics.AddRange(new[]
            {
                new TriggerTopic { CategoryId = cat2.Id, Name = "Gewalt gegen Kinder", SortOrder = 1 },
                new TriggerTopic { CategoryId = cat2.Id, Name = "Tod oder schwere Krankheit von Kindern", SortOrder = 2 },
                new TriggerTopic { CategoryId = cat2.Id, Name = "Verwaiste Kinder", SortOrder = 3 },
                new TriggerTopic { CategoryId = cat2.Id, Name = "Kinder als Soldaten (Marketenderkinder)", SortOrder = 4 }
            });

            // Kategorie 3: Psychologische Themen & Zwischenmenschliches
            var cat3 = categories[2];
            topics.AddRange(new[]
            {
                new TriggerTopic { CategoryId = cat3.Id, Name = "Psychische Erkrankungen (PTBS, \"Kriegsgezitter\")", SortOrder = 1 },
                new TriggerTopic { CategoryId = cat3.Id, Name = "Extreme Einsamkeit & Verlust", SortOrder = 2 },
                new TriggerTopic { CategoryId = cat3.Id, Name = "Verrat durch enge Vertraute", SortOrder = 3 },
                new TriggerTopic { CategoryId = cat3.Id, Name = "Erpressung", SortOrder = 4 },
                new TriggerTopic { CategoryId = cat3.Id, Name = "Geiselnahme", SortOrder = 5 }
            });

            // Kategorie 4: Sexuelle Gewalt & Ausbeutung
            var cat4 = categories[3];
            topics.AddRange(new[]
            {
                new TriggerTopic { CategoryId = cat4.Id, Name = "Grafische Darstellung sexueller Gewalt", SortOrder = 1 },
                new TriggerTopic { CategoryId = cat4.Id, Name = "Sexuelle Belästigung", SortOrder = 2 },
                new TriggerTopic { CategoryId = cat4.Id, Name = "Zwangsprostitution", SortOrder = 3 },
                new TriggerTopic { CategoryId = cat4.Id, Name = "Prostitution", SortOrder = 4 }
            });

            // Kategorie 5: Religiöse & Kulturelle Konflikte
            var cat5 = categories[4];
            topics.AddRange(new[]
            {
                new TriggerTopic { CategoryId = cat5.Id, Name = "Antisemitismus", SortOrder = 1 },
                new TriggerTopic { CategoryId = cat5.Id, Name = "Detailierte Darstellung von \"Hexenverfolgung\"", SortOrder = 2 },
                new TriggerTopic { CategoryId = cat5.Id, Name = "Religiöser Fanatismus", SortOrder = 3 },
                new TriggerTopic { CategoryId = cat5.Id, Name = "Religiöse Verunglimpfung / Blasphemie", SortOrder = 4 }
            });

            // Kategorie 6: Tierleid
            var cat6 = categories[5];
            topics.AddRange(new[]
            {
                new TriggerTopic { CategoryId = cat6.Id, Name = "Verwahrloste Tiere", SortOrder = 1 },
                new TriggerTopic { CategoryId = cat6.Id, Name = "Gewalt gegen / Tod von Tieren (Pferde, Zugtiere)", SortOrder = 2 }
            });

            // Kategorie 7: Körperliches & Medizinisches
            var cat7 = categories[6];
            topics.AddRange(new[]
            {
                new TriggerTopic { CategoryId = cat7.Id, Name = "Ausführliche Beschreibung von Krankheiten", SortOrder = 1 },
                new TriggerTopic { CategoryId = cat7.Id, Name = "Hunger, Durst, Kannibalismus", SortOrder = 2 }
            });

            context.TriggerTopics.AddRange(topics);
            context.SaveChanges();
        }
    }
}