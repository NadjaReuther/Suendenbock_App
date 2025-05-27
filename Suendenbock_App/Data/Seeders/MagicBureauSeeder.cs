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
                    new Obermagie { Bezeichnung = "Dämmerung", LightCardId = 14 }
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
                    new MagicClass { Bezeichnung = "Rhetorika", ImagePath = "/images/magicclass/shadow.png", ObermagieId = 9 },
                    new MagicClass { Bezeichnung = "Illusion", ImagePath = "/images/magicclass/illusion.png", ObermagieId = 7 },
                    new MagicClass { Bezeichnung = "Glück", ImagePath = "/images/magicclass/pain.png", ObermagieId = 9 },
                    new MagicClass { Bezeichnung = "Erde", ImagePath = "/images/magicclass/earth.png", ObermagieId = 5 },
                    new MagicClass { Bezeichnung = "Genesung", ImagePath = "/images/magicclass/pain.png", ObermagieId = 3 },
                    new MagicClass { Bezeichnung = "Schutz", ImagePath = "/images/magicclass/pain.png", ObermagieId = 12 }
                );
                context.SaveChanges();
            }
        }
    }
}