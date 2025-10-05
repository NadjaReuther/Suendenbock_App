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
                    new Obermagie { Bezeichnung = "Zyklus", LightCardId = 15 },
                    new Obermagie { Bezeichnung = "Unbegabt", LightCardId = 16 }
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
                        new MagicClass { Bezeichnung = "Tür", ImagePath = "/images/magicclass/dunkel.png", ObermagieId = 1 },
                        new MagicClass { Bezeichnung = "Dunkel", ImagePath = "/images/magicclass/dunkel .png", ObermagieId = 1 },
                        new MagicClass { Bezeichnung = "Leere", ImagePath = "/images/magicclass/dunkel .png", ObermagieId = 1 },
                        new MagicClass { Bezeichnung = "Barriere", ImagePath = "/images/magicclass/dunkel .png", ObermagieId = 1 },
                        new MagicClass { Bezeichnung = "Raum", ImagePath = "/images/magicclass/dunkel .png", ObermagieId = 1 },
                        new MagicClass { Bezeichnung = "Albtraum", ImagePath = "/images/magicclass/dunkel .png", ObermagieId = 1 },
                        new MagicClass { Bezeichnung = "Kosmos", ImagePath = "/images/magicclass/dunkel .png", ObermagieId = 1 },

                        new MagicClass { Bezeichnung = "Wind", ImagePath = "/images/magicclass/luft.png", ObermagieId = 2 },
                        new MagicClass { Bezeichnung = "Schall", ImagePath = "/images/magicclass/luft.png", ObermagieId = 2 },
                        new MagicClass { Bezeichnung = "Elektro", ImagePath = "/images/magicclass/luft.png", ObermagieId = 2 },
                        new MagicClass { Bezeichnung = "Aroma", ImagePath = "/images/magicclass/luft.png", ObermagieId = 2 },
                        new MagicClass { Bezeichnung = "Wetter", ImagePath = "/images/magicclass/luft.png", ObermagieId = 2 },
                        new MagicClass { Bezeichnung = "Gravitation", ImagePath = "/images/magicclass/luft.png", ObermagieId = 2 },
                        new MagicClass { Bezeichnung = "Äther", ImagePath = "/images/magicclass/luft.png", ObermagieId = 2 },

                        new MagicClass { Bezeichnung = "Nebel", ImagePath = "/images/magicclass/wasser.png", ObermagieId = 3 },
                        new MagicClass { Bezeichnung = "Gezeiten", ImagePath = "/images/magicclass/wasser.png", ObermagieId = 3 },
                        new MagicClass { Bezeichnung = "Dampf", ImagePath = "/images/magicclass/wasser.png", ObermagieId = 3 },
                        new MagicClass { Bezeichnung = "Blut", ImagePath = "/images/magicclass/wasser.png", ObermagieId = 3 },
                        new MagicClass { Bezeichnung = "Eis", ImagePath = "/images/magicclass/wasser.png", ObermagieId = 3 },
                        new MagicClass { Bezeichnung = "Tiefe", ImagePath = "/images/magicclass/wasser.png", ObermagieId = 3 },
                        new MagicClass { Bezeichnung = "Schleim", ImagePath = "/images/magicclass/wasser.png", ObermagieId = 3 },

                        new MagicClass { Bezeichnung = "Karma", ImagePath = "/images/magicclass/seele.png", ObermagieId = 4 },
                        new MagicClass { Bezeichnung = "Wahrsagung", ImagePath = "/images/magicclass/seele.png", ObermagieId = 4 },
                        new MagicClass { Bezeichnung = "Libero", ImagePath = "/images/magicclass/seele.png", ObermagieId = 4 },
                        new MagicClass { Bezeichnung = "Traum", ImagePath = "/images/magicclass/seele.png", ObermagieId = 4 },
                        new MagicClass { Bezeichnung = "Charme", ImagePath = "/images/magicclass/seele.png", ObermagieId = 4 },
                        new MagicClass { Bezeichnung = "Astral", ImagePath = "/images/magicclass/seele.png", ObermagieId = 4 },
                        new MagicClass { Bezeichnung = "Metamorphose", ImagePath = "/images/magicclass/seele.png", ObermagieId = 4 },
                        new MagicClass { Bezeichnung = "Toxika", ImagePath = "/images/magicclass/flora.png", ObermagieId = 5 },
                        new MagicClass { Bezeichnung = "Holz", ImagePath = "/images/magicclass/flora.png", ObermagieId = 5 },
                        new MagicClass { Bezeichnung = "Alge", ImagePath = "/images/magicclass/flora.png", ObermagieId = 5 },
                        new MagicClass { Bezeichnung = "Erde", ImagePath = "/images/magicclass/flora.png", ObermagieId = 5 },
                        new MagicClass { Bezeichnung = "Schlamm", ImagePath = "/images/magicclass/flora.png", ObermagieId = 5 },
                        new MagicClass { Bezeichnung = "Sand", ImagePath = "/images/magicclass/flora.png", ObermagieId = 5 },
                        new MagicClass { Bezeichnung = "Staub", ImagePath = "/images/magicclass/flora.png", ObermagieId = 5 },

                        new MagicClass { Bezeichnung = "Avi", ImagePath = "/images/magicclass/fauna.png", ObermagieId = 6 },
                        new MagicClass { Bezeichnung = "Mammalia", ImagePath = "/images/magicclass/fauna.png", ObermagieId = 6 },
                        new MagicClass { Bezeichnung = "Herpeto", ImagePath = "/images/magicclass/fauna.png", ObermagieId = 6 },
                        new MagicClass { Bezeichnung = "Ichtyo", ImagePath = "/images/magicclass/fauna.png", ObermagieId = 6 },
                        new MagicClass { Bezeichnung = "Arthropoda", ImagePath = "/images/magicclass/fauna.png", ObermagieId = 6 },
                        new MagicClass { Bezeichnung = "Chimero", ImagePath = "/images/magicclass/fauna.png", ObermagieId = 6 },
                        new MagicClass { Bezeichnung = "Draconi", ImagePath = "/images/magicclass/fauna.png", ObermagieId = 6 },

                        new MagicClass { Bezeichnung = "Illusion", ImagePath = "/images/magicclass/licht.png", ObermagieId = 7 },
                        new MagicClass { Bezeichnung = "Sonne", ImagePath = "/images/magicclass/licht.png", ObermagieId = 7 },
                        new MagicClass { Bezeichnung = "Spiegel", ImagePath = "/images/magicclass/licht.png", ObermagieId = 7 },
                        new MagicClass { Bezeichnung = "Feuerwerk", ImagePath = "/images/magicclass/licht.png", ObermagieId = 7 },
                        new MagicClass { Bezeichnung = "Aurora", ImagePath = "/images/magicclass/licht.png", ObermagieId = 7 },
                        new MagicClass { Bezeichnung = "Neon", ImagePath = "/images/magicclass/licht.png", ObermagieId = 7 },
                        new MagicClass { Bezeichnung = "Kristalka", ImagePath = "/images/magicclass/licht.png", ObermagieId = 7 },

                        new MagicClass { Bezeichnung = "Agonie", ImagePath = "/images/magicclass/schmerz.png", ObermagieId = 8 },
                        new MagicClass { Bezeichnung = "Pech", ImagePath = "/images/magicclass/schmerz.png", ObermagieId = 8 },
                        new MagicClass { Bezeichnung = "Schwäche", ImagePath = "/images/magicclass/schmerz.png", ObermagieId = 8 },
                        new MagicClass { Bezeichnung = "Hass", ImagePath = "/images/magicclass/schmerz.png", ObermagieId = 8 },
                        new MagicClass { Bezeichnung = "Verlust", ImagePath = "/images/magicclass/schmerz.png", ObermagieId = 8 },
                        new MagicClass { Bezeichnung = "Bann", ImagePath = "/images/magicclass/schmerz.png", ObermagieId = 8 },
                        new MagicClass { Bezeichnung = "Gommage", ImagePath = "/images/magicclass/schmerz.png", ObermagieId = 8 },
                        new MagicClass { Bezeichnung = "Euphorie", ImagePath = "/images/magicclass/kunst.png", ObermagieId = 9 },
                        new MagicClass { Bezeichnung = "Glück", ImagePath = "/images/magicclass/kunst.png", ObermagieId = 9 },
                        new MagicClass { Bezeichnung = "Schutz", ImagePath = "/images/magicclass/kunst.png", ObermagieId = 9 },
                        new MagicClass { Bezeichnung = "Liebe", ImagePath = "/images/magicclass/kunst.png", ObermagieId = 9 },
                        new MagicClass { Bezeichnung = "Sieg", ImagePath = "/images/magicclass/kunst.png", ObermagieId = 9 },
                        new MagicClass { Bezeichnung = "Sygill", ImagePath = "/images/magicclass/kunst.png", ObermagieId = 9 },
                        new MagicClass { Bezeichnung = "Restauration", ImagePath = "/images/magicclass/kunst.png", ObermagieId = 9 },

                        new MagicClass { Bezeichnung = "Echo", ImagePath = "/images/magicclass/geist.png", ObermagieId = 10 },
                        new MagicClass { Bezeichnung = "Medium", ImagePath = "/images/magicclass/geist.png", ObermagieId = 10 },
                        new MagicClass { Bezeichnung = "Knochen", ImagePath = "/images/magicclass/geist.png", ObermagieId = 10 },
                        new MagicClass { Bezeichnung = "Credo", ImagePath = "/images/magicclass/geist.png", ObermagieId = 10 },
                        new MagicClass { Bezeichnung = "Schicksal", ImagePath = "/images/magicclass/geist.png", ObermagieId = 10 },
                        new MagicClass { Bezeichnung = "Zerfall", ImagePath = "/images/magicclass/geist.png", ObermagieId = 10 },
                        new MagicClass { Bezeichnung = "Asche", ImagePath = "/images/magicclass/geist.png", ObermagieId = 10 },

                        new MagicClass { Bezeichnung = "Rauch", ImagePath = "/images/magicclass/feuer.png", ObermagieId = 11 },
                        new MagicClass { Bezeichnung = "Plasma", ImagePath = "/images/magicclass/feuer.png", ObermagieId = 11 },
                        new MagicClass { Bezeichnung = "Teer", ImagePath = "/images/magicclass/feuer.png", ObermagieId = 11 },
                        new MagicClass { Bezeichnung = "Explosion", ImagePath = "/images/magicclass/feuer.png", ObermagieId = 11 },
                        new MagicClass { Bezeichnung = "Glut", ImagePath = "/images/magicclass/feuer.png", ObermagieId = 11 },
                        new MagicClass { Bezeichnung = "Wärme", ImagePath = "/images/magicclass/feuer.png", ObermagieId = 11 },
                        new MagicClass { Bezeichnung = "Lava", ImagePath = "/images/magicclass/feuer.png", ObermagieId = 11 },

                        new MagicClass { Bezeichnung = "Reversion", ImagePath = "/images/magicclass/zeit.png", ObermagieId = 12 },
                        new MagicClass { Bezeichnung = "Prokursion", ImagePath = "/images/magicclass/zeit.png", ObermagieId = 12 },
                        new MagicClass { Bezeichnung = "Mnestik", ImagePath = "/images/magicclass/zeit.png", ObermagieId = 12 },
                        new MagicClass { Bezeichnung = "Partie", ImagePath = "/images/magicclass/zeit.png", ObermagieId = 12 },
                        new MagicClass { Bezeichnung = "Zyklus", ImagePath = "/images/magicclass/zeit.png", ObermagieId = 12 },
                        new MagicClass { Bezeichnung = "Juvenil", ImagePath = "/images/magicclass/zeit.png", ObermagieId = 12 },
                        new MagicClass { Bezeichnung = "Senium", ImagePath = "/images/magicclass/zeit.png", ObermagieId = 12 },

                        new MagicClass { Bezeichnung = "Mantik", ImagePath = "/images/magicclass/mantik.png", ObermagieId = 13 },
                        new MagicClass { Bezeichnung = "Nekromantie", ImagePath = "/images/magicclass/mantik.png", ObermagieId = 13 },
                        new MagicClass { Bezeichnung = "Zwielicht", ImagePath = "/images/magicclass/dämmerung.png", ObermagieId = 14 },
                        new MagicClass { Bezeichnung = "Chaos", ImagePath = "/images/magicclass/dämmerung.png", ObermagieId = 14 },

                        new MagicClass { Bezeichnung = "Heilig", ImagePath = "/images/magicclass/zyklus.png", ObermagieId = 15 },
                        new MagicClass { Bezeichnung = "Fluch", ImagePath = "/images/magicclass/zyklus.png", ObermagieId = 15 }
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
                    // Dunkelmagie OM
                    // Tür
                    new MagicClassSpecialization { Name = "Lehre der tausend Wege ", MagicClassId = 1 },
                    new MagicClassSpecialization { Name = "Lehre der Heimkehr ", MagicClassId = 1 },

                    // Dunkel
                    new MagicClassSpecialization { Name = "Lehre des Mondritters", MagicClassId = 2 },
                    new MagicClassSpecialization { Name = "Lehre des Nachtgeistes ", MagicClassId = 2 },

                    // Leere
                    new MagicClassSpecialization { Name = "Lehre des Skeptizismus", MagicClassId = 3 },
                    new MagicClassSpecialization { Name = "Lehre des Fatalismus ", MagicClassId = 3 },

                    // Barriere
                    new MagicClassSpecialization { Name = "Lehre der endlosen Fessel", MagicClassId = 4 },
                    new MagicClassSpecialization { Name = "Lehre des ewigen Käfigs", MagicClassId = 4 },

                    // Raum
                    new MagicClassSpecialization { Name = "Lehre der Alkoven", MagicClassId = 5 },
                    new MagicClassSpecialization { Name = "Lehre des Séparée ", MagicClassId = 5 },

                    // Albtraum
                    new MagicClassSpecialization { Name = "Lehre der Schlaflosigkeit", MagicClassId = 6 },
                    new MagicClassSpecialization { Name = "Lehre der Paralyse ", MagicClassId = 6 },

                    // Kosmos
                    new MagicClassSpecialization { Name = "Lehre des Sternenmeers", MagicClassId = 7 },
                    new MagicClassSpecialization { Name = "Lehre des Galaxienstrudels ", MagicClassId = 7 },

                    // Luftmagie OM
                    // Wind
                    new MagicClassSpecialization { Name = "Lehre des Vakuums", MagicClassId = 8 },
                    new MagicClassSpecialization { Name = "Lehre des Sturms ", MagicClassId = 8 },

                    // Schall
                    new MagicClassSpecialization { Name = "Lehre der Resonanz", MagicClassId = 9 },
                    new MagicClassSpecialization { Name = "Lehre der Frequenz ", MagicClassId = 9 },

                    // Elektro
                    new MagicClassSpecialization { Name = "Lehre der Spannung", MagicClassId = 10 },
                    new MagicClassSpecialization { Name = "Lehre des Widerstandes ", MagicClassId = 10 },

                    // Aroma
                    new MagicClassSpecialization { Name = "Lehre des vollen Buketts", MagicClassId = 11 },
                    new MagicClassSpecialization { Name = "Lehre der scharfen Note", MagicClassId = 11 },

                    // Wetter
                    new MagicClassSpecialization { Name = "Lehre der Dürre", MagicClassId = 12 },
                    new MagicClassSpecialization { Name = "Lehre des Zenitals ", MagicClassId = 12 },

                    // Gravitation
                    new MagicClassSpecialization { Name = "Lehre der Anziehung", MagicClassId = 13 },
                    new MagicClassSpecialization { Name = "Lehre der Abstoßung ", MagicClassId = 13 },

                    // Äther
                    new MagicClassSpecialization { Name = "Lehre des Ozon", MagicClassId = 14 },
                    new MagicClassSpecialization { Name = "Lehre des Smog ", MagicClassId = 14 },

                    // Wassermagie OM
                    // Nebel
                    new MagicClassSpecialization { Name = "Lehre des Bergnebels", MagicClassId = 15 },
                    new MagicClassSpecialization { Name = "Lehre des Talnebels", MagicClassId = 15 },

                    // Gezeiten
                    new MagicClassSpecialization { Name = "Lehre der Ebbe", MagicClassId = 16 },
                    new MagicClassSpecialization { Name = "Lehre der Flut ", MagicClassId = 16 },

                    // Dampf
                    new MagicClassSpecialization { Name = "Lehre der Spannung", MagicClassId = 17 },
                    new MagicClassSpecialization { Name = "Lehre des Widerstandes ", MagicClassId = 17 },

                    // Blut
                    new MagicClassSpecialization { Name = "Lehre der Massen und Räume", MagicClassId = 18 },
                    new MagicClassSpecialization { Name = "Lehre der vier Säfte", MagicClassId = 18 },

                    // Eis
                    new MagicClassSpecialization { Name = "Lehre der Gletscher", MagicClassId = 19 },
                    new MagicClassSpecialization { Name = "Lehre der Tundra", MagicClassId = 19 },

                    // Tiefe
                    new MagicClassSpecialization { Name = "Lehre des Bathypel", MagicClassId = 20 },
                    new MagicClassSpecialization { Name = "Lehre der Abyss ", MagicClassId = 20 },

                    // Schleim
                    new MagicClassSpecialization { Name = "Lehre des Ätzen", MagicClassId = 21 },
                    new MagicClassSpecialization { Name = "Lehre des Kühlen ", MagicClassId = 21 },

                    // Seelemagie OM
                    // Karma
                    new MagicClassSpecialization { Name = "Lehre des Sanchita", MagicClassId = 22 },
                    new MagicClassSpecialization { Name = "Lehre des Kriyamana", MagicClassId = 22 },

                    // Wahrsagung
                    new MagicClassSpecialization { Name = "Lehre des Orakel", MagicClassId = 23 },
                    new MagicClassSpecialization { Name = "Lehre der Divination ", MagicClassId = 23 },

                    // Libero
                    new MagicClassSpecialization { Name = "Lehre der Möglichkeiten", MagicClassId = 24 },
                    new MagicClassSpecialization { Name = "Lehre der Unabhängigkeiten ", MagicClassId = 24 },

                    // Traum
                    new MagicClassSpecialization { Name = "Lehre der Inzeption", MagicClassId = 25 },
                    new MagicClassSpecialization { Name = "Lehre der Exzeption", MagicClassId = 25 },

                    // Charme
                    new MagicClassSpecialization { Name = "Lehre der Faszination", MagicClassId = 26 },
                    new MagicClassSpecialization { Name = "Lehre der Liasion", MagicClassId = 26 },

                    // Astral
                    new MagicClassSpecialization { Name = "Lehre des Alles", MagicClassId = 27 },
                    new MagicClassSpecialization { Name = "Lehre des Nichts ", MagicClassId = 27 },

                    // Metamorphose
                    new MagicClassSpecialization { Name = "Lehre der Anpassung", MagicClassId = 28 },
                    new MagicClassSpecialization { Name = "Lehre der Spezialisierung ", MagicClassId = 28 },

                    // Floramagie OM
                    // Toxika
                    new MagicClassSpecialization { Name = "Lehre der Mykoniden", MagicClassId = 29 },
                    new MagicClassSpecialization { Name = "Lehre der Botanik", MagicClassId = 29 },

                    // Holz
                    new MagicClassSpecialization { Name = "Lehre der Zweige", MagicClassId = 30 },
                    new MagicClassSpecialization { Name = "Lehre der Wurzeln ", MagicClassId = 30 },

                    // Alge
                    new MagicClassSpecialization { Name = "Lehre der Symbiose", MagicClassId = 31 },
                    new MagicClassSpecialization { Name = "Lehre der Autonomie ", MagicClassId = 31 },

                    // Erde
                    new MagicClassSpecialization { Name = "Lehre des Felsenherzes", MagicClassId = 32 },
                    new MagicClassSpecialization { Name = "Lehre der Eisenlunge", MagicClassId = 32 },

                    // Schlamm
                    new MagicClassSpecialization { Name = "Lehre des Morast", MagicClassId = 33 },
                    new MagicClassSpecialization { Name = "Lehre des Fango", MagicClassId = 33 },

                    // Sand
                    new MagicClassSpecialization { Name = "Lehre des Schlicks", MagicClassId = 34 },
                    new MagicClassSpecialization { Name = "Lehre des Sediments ", MagicClassId = 34 },

                    // Staub
                    new MagicClassSpecialization { Name = "Lehre des Puders", MagicClassId = 35 },
                    new MagicClassSpecialization { Name = "Lehre der Kluster ", MagicClassId = 35 },

                    // Faunamagie OM
                    // Avi
                    new MagicClassSpecialization { Name = "Ruf des Nests", MagicClassId = 36 },
                    new MagicClassSpecialization { Name = "Ruf der Weite", MagicClassId = 36 },

                    // Mammalia
                    new MagicClassSpecialization { Name = "Ruf der Nähe", MagicClassId = 37 },
                    new MagicClassSpecialization { Name = "Ruf der Ferne ", MagicClassId = 37 },

                    // Herpeto
                    new MagicClassSpecialization { Name = "Ruf der Lungenatmer", MagicClassId = 38 },
                    new MagicClassSpecialization { Name = "Ruf der Kiemenatmen", MagicClassId = 38 },

                    // Ichtyo
                    new MagicClassSpecialization { Name = "Ruf der Wale", MagicClassId = 39 },
                    new MagicClassSpecialization { Name = "Ruf der Haie", MagicClassId = 39 },

                    // Arthropoda
                    new MagicClassSpecialization { Name = "Ruf der Witwe", MagicClassId = 40 },
                    new MagicClassSpecialization { Name = "Ruf der Familie", MagicClassId = 40 },

                    // Chimero
                    new MagicClassSpecialization { Name = "Ruf des Himmels", MagicClassId = 41 },
                    new MagicClassSpecialization { Name = "Ruf der Hölle", MagicClassId = 41 },

                    // Draconi
                    new MagicClassSpecialization { Name = "Ruf des Hauchs", MagicClassId = 42 },
                    new MagicClassSpecialization { Name = "Ruf der Schwingen", MagicClassId = 42 },

                    // Lichtmagie OM
                    // Illusion
                    new MagicClassSpecialization { Name = "Lehre der Erkenntnis", MagicClassId = 43 },
                    new MagicClassSpecialization { Name = "Lehre der Verschleierung", MagicClassId = 43 },

                    // Sonne
                    new MagicClassSpecialization { Name = "Lehre des großen Helio", MagicClassId = 44 },
                    new MagicClassSpecialization { Name = "Lehre des schweren Sol", MagicClassId = 44 },

                    // Spiegel
                    new MagicClassSpecialization { Name = "Lehre der Reflexion", MagicClassId = 45 },
                    new MagicClassSpecialization { Name = "Lehre der Absorption", MagicClassId = 45 },

                    // Feuerwerk
                    new MagicClassSpecialization { Name = "Lehre der Farben", MagicClassId = 46 },
                    new MagicClassSpecialization { Name = "Lehre des Kometen", MagicClassId = 46 },

                    // Aurora
                    new MagicClassSpecialization { Name = "Lehre der Nordlichter", MagicClassId = 47 },
                    new MagicClassSpecialization { Name = "Lehre des Alpenglühen", MagicClassId = 47 },

                    // Neon
                    new MagicClassSpecialization { Name = "Lehre der Warnung", MagicClassId = 48 },
                    new MagicClassSpecialization { Name = "Lehre der Verlockung", MagicClassId = 48 },

                    // Kristalka
                    new MagicClassSpecialization { Name = "Lehre der Konvexität", MagicClassId = 49 },
                    new MagicClassSpecialization { Name = "Lehre der Konkavität", MagicClassId = 49 },

                    // Schmerzmagie OM
                    // Agonie
                    new MagicClassSpecialization { Name = "Lehre des Kummers", MagicClassId = 50 },
                    new MagicClassSpecialization { Name = "Lehre der Qual", MagicClassId = 50 },

                    // Pech
                    new MagicClassSpecialization { Name = "Lehre des Malheurs", MagicClassId = 51 },
                    new MagicClassSpecialization { Name = "Lehre des Disasters", MagicClassId = 51 },

                    // Schwäche
                    new MagicClassSpecialization { Name = "Lehre der Asthenie", MagicClassId = 52 },
                    new MagicClassSpecialization { Name = "Lehre der Erschöpfung", MagicClassId = 52 },

                    // Hass
                    new MagicClassSpecialization { Name = "Lehre der Groll", MagicClassId = 53 },
                    new MagicClassSpecialization { Name = "Lehre des Abscheu", MagicClassId = 53 },

                    // Verlust
                    new MagicClassSpecialization { Name = "Lehre des Mangels", MagicClassId = 54 },
                    new MagicClassSpecialization { Name = "Lehre des Defizits", MagicClassId = 54 },

                    // Bann
                    new MagicClassSpecialization { Name = "Lehre des Verbots", MagicClassId = 55 },
                    new MagicClassSpecialization { Name = "Lehre der Ausrottung", MagicClassId = 55 },

                    // Gommage
                    new MagicClassSpecialization { Name = "Lehre der Ausradierung", MagicClassId = 56 },
                    new MagicClassSpecialization { Name = "Lehre der Austreibung", MagicClassId = 56 },

                    //Kunstmagie OM
                    // Euphorie
                    new MagicClassSpecialization { Name = "Lehre der Ekstase", MagicClassId = 57 },
                    new MagicClassSpecialization { Name = "Lehre des Rauschs", MagicClassId = 57 },

                    // Glück
                    new MagicClassSpecialization { Name = "Lehre des Wohlbefindens", MagicClassId = 58 },
                    new MagicClassSpecialization { Name = "Lehre der Glückseligkeit", MagicClassId = 58 },

                    // Schutz
                    new MagicClassSpecialization { Name = "Lehre des Wellernschen Pokus", MagicClassId = 59 },
                    new MagicClassSpecialization { Name = "Lehre des Wellernschen Opfers", MagicClassId = 59 },

                    // Liebe
                    new MagicClassSpecialization { Name = "Lehre des Eros", MagicClassId = 60 },
                    new MagicClassSpecialization { Name = "Lehre des Agape", MagicClassId = 60 },

                    // Sieg
                    new MagicClassSpecialization { Name = "Lehre des Triumphs", MagicClassId = 61 },
                    new MagicClassSpecialization { Name = "Lehre des Erfolgs", MagicClassId = 61 },

                    // Sygill
                    new MagicClassSpecialization { Name = "Lehre des Symbole", MagicClassId = 62 },
                    new MagicClassSpecialization { Name = "Lehre der Schrift", MagicClassId = 62 },

                    // Restauration
                    new MagicClassSpecialization { Name = "Lehre der Konservierung", MagicClassId = 63 },
                    new MagicClassSpecialization { Name = "Lehre der Renovierung", MagicClassId = 63 },

                    //Geistmagie OM
                    // Echo
                    new MagicClassSpecialization { Name = "Lehre des Figments", MagicClassId = 64 },
                    new MagicClassSpecialization { Name = "Lehre des Nachhalls", MagicClassId = 64 },

                    // Medium
                    new MagicClassSpecialization { Name = "Lehre der Anrufung", MagicClassId = 65 },
                    new MagicClassSpecialization { Name = "Lehre der Heimsuchung", MagicClassId = 65 },

                    // Knochen
                    new MagicClassSpecialization { Name = "Lehre der Epiphyse", MagicClassId = 66 },
                    new MagicClassSpecialization { Name = "Lehre der Diaphyse", MagicClassId = 66 },

                    // Credo
                    new MagicClassSpecialization { Name = "Lehre des Monotheismus", MagicClassId = 67 },
                    new MagicClassSpecialization { Name = "Lehre des Polytheismus", MagicClassId = 67 },

                    // Schicksal
                    new MagicClassSpecialization { Name = "Lehre des Bestimmung", MagicClassId = 68 },
                    new MagicClassSpecialization { Name = "Lehre des Vorhersehung", MagicClassId = 68 },

                    // Zerfall
                    new MagicClassSpecialization { Name = "Lehre des Untergangs", MagicClassId = 69 },
                    new MagicClassSpecialization { Name = "Lehre der Zerlegung", MagicClassId = 69 },

                    // Asche
                    new MagicClassSpecialization { Name = "Lehre der Vergänglichkeit", MagicClassId = 70 },
                    new MagicClassSpecialization { Name = "Lehre der Reinigung", MagicClassId = 70 },

                    //Feuermagie OM
                    // Rauch
                    new MagicClassSpecialization { Name = "Lehre des Qualms", MagicClassId = 71 },
                    new MagicClassSpecialization { Name = "Lehre des Ruß", MagicClassId = 71 },

                    // Plasma
                    new MagicClassSpecialization { Name = "Lehre des Albumin ", MagicClassId = 72 },
                    new MagicClassSpecialization { Name = "Lehre der Globuline", MagicClassId = 72 },

                    // Teer
                    new MagicClassSpecialization { Name = "Lehre der Zähigkeit", MagicClassId = 73 },
                    new MagicClassSpecialization { Name = "Lehre des Brodelns", MagicClassId = 73 },

                    // Explosion
                    new MagicClassSpecialization { Name = "Lehre der Deflagration", MagicClassId = 74 },
                    new MagicClassSpecialization { Name = "Lehre der Detonation", MagicClassId = 74 },

                    // Glut
                    new MagicClassSpecialization { Name = "Lehre der Kohle", MagicClassId = 75 },
                    new MagicClassSpecialization { Name = "Lehre des Schwellens", MagicClassId = 75 },

                    // Wärme
                    new MagicClassSpecialization { Name = "Lehre des Kaminfeuers", MagicClassId = 76 },
                    new MagicClassSpecialization { Name = "Lehre der Decken und Kissen", MagicClassId = 76 },

                    // Lava
                    new MagicClassSpecialization { Name = "Lehre des Magmas", MagicClassId = 77 },
                    new MagicClassSpecialization { Name = "Lehre der Pelé", MagicClassId = 77 },

                    //Zeitmagie OM
                    // Reversion
                    new MagicClassSpecialization { Name = "Lehre der Entropie", MagicClassId = 78 },
                    new MagicClassSpecialization { Name = "Lehre des Vortex", MagicClassId = 78 },

                    // Prokursion
                    new MagicClassSpecialization { Name = "Lehre des Prognose ", MagicClassId = 79 },
                    new MagicClassSpecialization { Name = "Lehre des Futur", MagicClassId = 79 },

                    // Mnestik
                    new MagicClassSpecialization { Name = "Lehre der Amnesie", MagicClassId = 80 },
                    new MagicClassSpecialization { Name = "Lehre des Deklaration", MagicClassId = 80 },

                    // Partie
                    new MagicClassSpecialization { Name = "Lehre der Deflagration", MagicClassId = 81 },
                    new MagicClassSpecialization { Name = "Lehre der Detonation", MagicClassId = 81 },

                    // Zyklus
                    new MagicClassSpecialization { Name = "Lehre der Ovulation", MagicClassId = 82 },
                    new MagicClassSpecialization { Name = "Lehre des Luteal", MagicClassId = 82 },

                    // Juvenil
                    new MagicClassSpecialization { Name = "Lehre der ewigen Jugend", MagicClassId = 83 },
                    new MagicClassSpecialization { Name = "Lehre des hellen Geistes", MagicClassId = 83 },

                    // Senium
                    new MagicClassSpecialization { Name = "Lehre des schmerzenden Leibs", MagicClassId = 84 },
                    new MagicClassSpecialization { Name = "Lehre der geistigen Unendlichkeit", MagicClassId = 84 },

                    //Horizontliniemagie OM
                    // Mantik
                    new MagicClassSpecialization { Name = "Lehre des zähen Blutes", MagicClassId = 85 },
                    new MagicClassSpecialization { Name = "Lehre der Ahnen", MagicClassId = 85 },

                    // Nekromantie
                    new MagicClassSpecialization { Name = "Lehre des Puppenspielers ", MagicClassId = 86 },
                    new MagicClassSpecialization { Name = "Lehre des Präperators", MagicClassId = 86 },

                    //Vertikalliniemagie OM
                    // Zwielicht
                    new MagicClassSpecialization { Name = "Lehre des Lichts", MagicClassId = 87 },
                    new MagicClassSpecialization { Name = "Lehre des Schattens", MagicClassId = 87 },

                    // Chaos
                    new MagicClassSpecialization { Name = "Lehre des Wirrwarr ", MagicClassId = 88 },
                    new MagicClassSpecialization { Name = "Lehre der Anarchie", MagicClassId = 88 },

                    //Zyklusmagie OM
                    // Heilig
                    new MagicClassSpecialization { Name = "Lehre des Lichtritters", MagicClassId = 89 },
                    new MagicClassSpecialization { Name = "Lehre des heiligen Wortes", MagicClassId = 89 },

                    // Fluch
                    new MagicClassSpecialization { Name = "Lehre der Zweifel ", MagicClassId = 90 },
                    new MagicClassSpecialization { Name = "Lehre der Pein", MagicClassId = 90 }
                );
                context.SaveChanges();
            }
        }
    }
}