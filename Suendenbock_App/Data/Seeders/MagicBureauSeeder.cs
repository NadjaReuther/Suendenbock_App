using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Data.Seeders
{
    public static class ZaubertypSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Zaubertyp>().Any())
            {
                context.Set<Zaubertyp>().AddRange(
                    new Zaubertyp { Name = "Ritual", Beschreibung = "dauern einen gewissen Moment" },
                    new Zaubertyp { Name = "Blickkontakt", Beschreibung = "Zauber, nur wenn Gegner sichtbar" },
                    new Zaubertyp { Name = "Berührung", Beschreibung = "Zauber, nur wenn in der Nähe" },
                    new Zaubertyp { Name = "Gespräch", Beschreibung = "Zauber, wichtig für Hiro" },
                    new Zaubertyp { Name = "Reaktion", Beschreibung = "Zauber, nur wenn Gegner vorher agiert" }
                );
                context.SaveChanges();
            }
        }
    }
    public static class ObermagicSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Obermagie>().Any())
            {
                context.Set<Obermagie>().AddRange(
                    new Obermagie { Bezeichnung = "Dunkel", LightCardId = 1 },
                    new Obermagie { Bezeichnung = "Luft", LightCardId = 2 },
                    new Obermagie { Bezeichnung = "Wasser", LightCardId = 3 },
                    new Obermagie { Bezeichnung = "Seele", LightCardId = 4 },
                    new Obermagie { Bezeichnung = "Flora", LightCardId = 5 },
                    new Obermagie { Bezeichnung = "Fauna", LightCardId = 6 },
                    new Obermagie { Bezeichnung = "Licht", LightCardId = 7 },
                    new Obermagie { Bezeichnung = "Schmerz", LightCardId = 8 },
                    new Obermagie { Bezeichnung = "Kunst", LightCardId = 9 },
                    new Obermagie { Bezeichnung = "Geist", LightCardId = 10 },
                    new Obermagie { Bezeichnung = "Feuer", LightCardId = 11 },
                    new Obermagie { Bezeichnung = "Zeit", LightCardId = 12 },
                    new Obermagie { Bezeichnung = "Mantik", LightCardId = 13 },
                    new Obermagie { Bezeichnung = "Dämmerung", LightCardId = 14 },
                    new Obermagie { Bezeichnung = "Unbegabt", LightCardId = 15 }
                );
                context.SaveChanges();
            }
        }
    }
    public static class  MagicClassSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<MagicClass>().Any())
            {
                context.Set<MagicClass>().AddRange(
                    new MagicClass { Bezeichnung = "Tür", ImagePath = "/images/magicclass/door.png", ObermagieId = 1 },
                    new MagicClass { Bezeichnung = "Sieg", ImagePath = "/images/magicclass/air.png", ObermagieId = 9 },
                    new MagicClass { Bezeichnung = "Elektro", ImagePath = "/images/magicclass/electric.png", ObermagieId = 12 },
                    new MagicClass { Bezeichnung = "Nebel", ImagePath = "/images/magicclass/fog.png", ObermagieId = 3 },
                    new MagicClass { Bezeichnung = "Holz", ImagePath = "/images/magicclass/wood.png", ObermagieId = 5 },
                    new MagicClass { Bezeichnung = "Fluch", ImagePath = "/images/magicclass/fire.png", ObermagieId = 8 },
                    new MagicClass { Bezeichnung = "Heilig", ImagePath = "/images/magicclass/holy.png", ObermagieId = 9 },
                    new MagicClass { Bezeichnung = "Toxika", ImagePath = "/images/magicclass/wood.png", ObermagieId = 5 },
                    new MagicClass { Bezeichnung = "Schall", ImagePath = "/images/magicclass/shadow.png", ObermagieId = 2 },
                    new MagicClass { Bezeichnung = "Eis", ImagePath = "/images/magicclass/ice.png", ObermagieId = 3 },
                    new MagicClass { Bezeichnung = "Wind", ImagePath = "/images/magicclass/wind.png", ObermagieId = 2 },
                    new MagicClass { Bezeichnung = "Gefallen", ImagePath = "/images/magicclass/shadow.png", ObermagieId = 8 },
                    new MagicClass { Bezeichnung = "Barriere", ImagePath = "/images/magicclass/barrier.png", ObermagieId = 12 },
                    new MagicClass { Bezeichnung = "Schlamm", ImagePath = "/images/magicclass/water.png", ObermagieId = 3 },
                    new MagicClass { Bezeichnung = "Feuerwerk", ImagePath = "/images/magicclass/shadow.png", ObermagieId = 11 },
                    new MagicClass { Bezeichnung = "Dampf", ImagePath = "/images/magicclass/fog.png", ObermagieId = 3 },
                    new MagicClass { Bezeichnung = "Tierbegleiter", ImagePath = "/images/magicclass/shadow.png", ObermagieId = 6 },
                    new MagicClass { Bezeichnung = "Zwielicht", ImagePath = "/images/magicclass/dark.png", ObermagieId = 14 },
                    new MagicClass { Bezeichnung = "Dunkel", ImagePath = "/images/magicclass/dark.png", ObermagieId = 1 },
                    new MagicClass { Bezeichnung = "Teer", ImagePath = "/images/magicclass/dark.png", ObermagieId = 6 },
                    new MagicClass { Bezeichnung = "Rhetorika", ImagePath = "/images/magicclass/shadow.png", ObermagieId = 9 },
                    new MagicClass { Bezeichnung = "Illusion", ImagePath = "/images/magicclass/illusion.png", ObermagieId = 7 },
                    new MagicClass { Bezeichnung = "Glück", ImagePath = "/images/magicclass/pain.png", ObermagieId = 9 },
                    new MagicClass { Bezeichnung = "Erde", ImagePath = "/images/magicclass/earth.png", ObermagieId = 5 },
                    new MagicClass { Bezeichnung = "Genesung", ImagePath = "/images/magicclass/pain.png", ObermagieId = 3 },
                    new MagicClass { Bezeichnung = "Schutz", ImagePath = "/images/magicclass/pain.png", ObermagieId = 12 },
                    new MagicClass { Bezeichnung = "Profane", ImagePath = "/images/magicclass/dark.png", ObermagieId = 15 },
                    new MagicClass { Bezeichnung = "Beschränkte", ImagePath = "/images/magicclass/dark.png", ObermagieId = 15 },
                    new MagicClass { Bezeichnung = "Unbegabte", ImagePath = "/images/magicclass/dark.png", ObermagieId = 15 }
                );
                context.SaveChanges();
            }
        }
    }
    public static class MagicClassSpecializationSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<MagicClassSpecialization>().Any())
            {
                context.Set<MagicClassSpecialization>().AddRange(
                    new MagicClassSpecialization { Name = "Lehre der tausend Wege", MagicClassId = 1 },
                    new MagicClassSpecialization { Name = "Lehre der Heimkehr", MagicClassId = 1 },

                    new MagicClassSpecialization { Name = "Lehre der heiligen Sonne", MagicClassId = 2 },
                    new MagicClassSpecialization { Name = "Lehre des ewigen Sieges", MagicClassId = 2 },

                    new MagicClassSpecialization { Name = "Lehre der Spannung", MagicClassId = 3 },
                    new MagicClassSpecialization { Name = "Lehre des Widerstandes", MagicClassId = 3 },

                    new MagicClassSpecialization { Name = "Lehre des Bergnebels", MagicClassId = 4 },
                    new MagicClassSpecialization { Name = "Lehre des Talnebels", MagicClassId = 4 },

                    new MagicClassSpecialization { Name = "Lehre der Wurzeln", MagicClassId = 5 },
                    new MagicClassSpecialization { Name = "Lehre der Zweige", MagicClassId = 5 },

                    new MagicClassSpecialization { Name = "Lehre der Pein", MagicClassId = 6 },
                    new MagicClassSpecialization { Name = "Lehre des Zweifels", MagicClassId = 6 },

                    new MagicClassSpecialization { Name = "Lehre des heiligen Ritters", MagicClassId = 7 },
                    new MagicClassSpecialization { Name = "Lehre des heiligen Lichts", MagicClassId = 7 },

                    new MagicClassSpecialization { Name = "Lehre der Mykoniden", MagicClassId = 8 },
                    new MagicClassSpecialization { Name = "Lehre der Flora", MagicClassId = 8 },

                    new MagicClassSpecialization { Name = "Lehre der Resonanz", MagicClassId = 9 },
                    new MagicClassSpecialization { Name = "Lehre der Frequenz", MagicClassId = 9 },

                    new MagicClassSpecialization { Name = "Lehre der Gletscher", MagicClassId = 10 },
                    new MagicClassSpecialization { Name = "Lehre der Tundra", MagicClassId = 10 },
                    new MagicClassSpecialization { Name = "Lehre des Vakuums", MagicClassId = 11 },
                    new MagicClassSpecialization { Name = "Lehre des Sturms", MagicClassId = 11 },

                    new MagicClassSpecialization { Name = "Lehre des Eidbrechers", MagicClassId = 12 },
                    new MagicClassSpecialization { Name = "Lehre des Samael", MagicClassId = 12 },

                    new MagicClassSpecialization { Name = "Lehre des ewigen Käfigs", MagicClassId = 13 },
                    new MagicClassSpecialization { Name = "Lehre der endlosen Fessel", MagicClassId = 13 },

                    new MagicClassSpecialization { Name = "Lehre des Morast", MagicClassId = 14 },
                    new MagicClassSpecialization { Name = "Lehre des Fango", MagicClassId = 14 },

                    new MagicClassSpecialization { Name = "Lehre der Farben", MagicClassId = 15 },
                    new MagicClassSpecialization { Name = "Lehre des Kometen ", MagicClassId = 15 },

                    new MagicClassSpecialization { Name = "Lehre des Unterdrucks", MagicClassId = 16 },
                    new MagicClassSpecialization { Name = "Lehre des Überdrucks", MagicClassId = 16 },

                    new MagicClassSpecialization { Name = "Ruf der Nähe", MagicClassId = 17 },
                    new MagicClassSpecialization { Name = "Ruf der Ferne", MagicClassId = 17 },

                    new MagicClassSpecialization { Name = "Lehre des Lichts", MagicClassId = 18 },
                    new MagicClassSpecialization { Name = "Lehre des Schattens", MagicClassId = 18 },

                    new MagicClassSpecialization { Name = "Lehre des Mondritters", MagicClassId = 19 },
                    new MagicClassSpecialization { Name = "Lehre des Nachtgeistes", MagicClassId = 19 },

                    new MagicClassSpecialization { Name = "Lehre der Zähigkeit", MagicClassId = 20 },
                    new MagicClassSpecialization { Name = "Lehre des Brodelns", MagicClassId = 20 },
                    new MagicClassSpecialization { Name = "Lehre der Tat", MagicClassId = 21 },
                    new MagicClassSpecialization { Name = "Lehre des Wortes", MagicClassId = 21 },

                    new MagicClassSpecialization { Name = "Lehre der Erkenntnis", MagicClassId = 22 },
                    new MagicClassSpecialization { Name = "Lehre der Verschleierung", MagicClassId = 22 },

                    new MagicClassSpecialization { Name = "Lehre der Gewissenheit", MagicClassId = 23 },
                    new MagicClassSpecialization { Name = "Lehre des Lust", MagicClassId = 23 },

                    new MagicClassSpecialization { Name = "Lehre des Felsenherzes", MagicClassId = 24 },
                    new MagicClassSpecialization { Name = "Lehre der Eisenlunge", MagicClassId = 24 },

                    new MagicClassSpecialization { Name = "Lehre der Massen und Räume", MagicClassId = 25 },
                    new MagicClassSpecialization { Name = "Lehre der vier Säfte", MagicClassId = 25 },

                    new MagicClassSpecialization { Name = "Lehre des Wellernschen Pokus", MagicClassId = 26 },
                    new MagicClassSpecialization { Name = "Lehre des Wellernschen Opfers", MagicClassId = 26 }
                );
                context.SaveChanges();
            }
        }
    }
}