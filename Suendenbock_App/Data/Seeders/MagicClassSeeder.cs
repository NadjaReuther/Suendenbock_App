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
    public static class MagicClassSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.MagicClasses.Any())
            {
                context.MagicClasses.AddRange(
                    // Elementare Magie - Rot (LightCardId = 1)
                    new MagicClass { Bezeichnung = "Feuer", ImagePath = "/images/magicclass/fire.png", LightCardId = 1 },
                    new MagicClass { Bezeichnung = "Blitz", ImagePath = "/images/magicclass/lightning.png", LightCardId = 1 },
                    new MagicClass { Bezeichnung = "Lava", ImagePath = "/images/magicclass/lava.png", LightCardId = 1 },

                    // Wasser/Eis Magie - Blau (LightCardId = 2)
                    new MagicClass { Bezeichnung = "Wasser", ImagePath = "/images/magicclass/water.png", LightCardId = 2 },
                    new MagicClass { Bezeichnung = "Eis", ImagePath = "/images/magicclass/ice.png", LightCardId = 2 },
                    new MagicClass { Bezeichnung = "Dampf", ImagePath = "/images/magicclass/steam.png", LightCardId = 2 },
                    new MagicClass { Bezeichnung = "Barriere", ImagePath = "/images/magicclass/barrier.png", LightCardId = 2 },

                    // Natur Magie - Grün (LightCardId = 4)
                    new MagicClass { Bezeichnung = "Holz", ImagePath = "/images/magicclass/wood.png", LightCardId = 4 },
                    new MagicClass { Bezeichnung = "Erde", ImagePath = "/images/magicclass/earth.png", LightCardId = 4 },
                    new MagicClass { Bezeichnung = "Gift", ImagePath = "/images/magicclass/poison.png", LightCardId = 4 },
                    new MagicClass { Bezeichnung = "Heilung", ImagePath = "/images/magicclass/healing.png", LightCardId = 4 },

                    // Licht Magie - Gelb (LightCardId = 5)
                    new MagicClass { Bezeichnung = "Licht", ImagePath = "/images/magicclass/light.png", LightCardId = 5 },
                    new MagicClass { Bezeichnung = "Heilig", ImagePath = "/images/magicclass/holy.png", LightCardId = 5 },
                    new MagicClass { Bezeichnung = "Elektro", ImagePath = "/images/magicclass/electric.png", LightCardId = 5 },

                    // Mystische Magie - Braun (LightCardId = 6)
                    new MagicClass { Bezeichnung = "Tür", ImagePath = "/images/magicclass/door.png", LightCardId = 6 },
                    new MagicClass { Bezeichnung = "Illusion", ImagePath = "/images/magicclass/illusion.png", LightCardId = 6 },
                    new MagicClass { Bezeichnung = "Verwandlung", ImagePath = "/images/magicclass/transform.png", LightCardId = 6 },

                    // Dunkle Magie - Schatten (LightCardId = 7)
                    new MagicClass { Bezeichnung = "Schatten", ImagePath = "/images/magicclass/shadow.png", LightCardId = 7 },
                    new MagicClass { Bezeichnung = "Zwielicht", ImagePath = "/images/magicclass/twilight.png", LightCardId = 7 },
                    new MagicClass { Bezeichnung = "Nekromantie", ImagePath = "/images/magicclass/necromancy.png", LightCardId = 7 },
                    new MagicClass { Bezeichnung = "Wind", ImagePath = "/images/magicclass/wind.png", LightCardId = 7 },
                    new MagicClass { Bezeichnung = "Nebel", ImagePath = "/images/magicclass/fog.png", LightCardId = 7 }
                );
                context.SaveChanges();
            }
        }
    }
}