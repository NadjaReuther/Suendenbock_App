using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Data.Seeders
{
    public static class MonsterwuerfelSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Monsterwuerfel>().Any())
            {
                context.Set<Monsterwuerfel>().AddRange(
                    new Monsterwuerfel { Wuerfel = "1" },
                    new Monsterwuerfel { Wuerfel = "2" },
                    new Monsterwuerfel { Wuerfel = "3" },
                    new Monsterwuerfel { Wuerfel = "4" },
                    new Monsterwuerfel { Wuerfel = "5" }
                );
                context.SaveChanges();
            }
        }
    }
    public static class MonsterintelligenzSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Monsterintelligenz>().Any())
            {
                context.Set<Monsterintelligenz>().AddRange(
                    new Monsterintelligenz { Name = "Tierhaft" },
                    new Monsterintelligenz { Name = "Dumm" },
                    new Monsterintelligenz { Name = "Klug" },
                    new Monsterintelligenz { Name = "Sehr Klug" },
                    new Monsterintelligenz { Name = "Berechnend" }
                );
                context.SaveChanges();
            }
        }
    }
    public static class MonstervorkommenSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Monstervorkommen>().Any())
            {
                context.Set<Monstervorkommen>().AddRange(
                    new Monstervorkommen { Name = "Wälder" },
                    new Monstervorkommen { Name = "Ebenen" },
                    new Monstervorkommen { Name = "Höhlen" },
                    new Monstervorkommen { Name = "Gebirge" },
                    new Monstervorkommen { Name = "Städte" },
                    new Monstervorkommen { Name = "Sümpfe" }
                );
                context.SaveChanges();
            }
        }
    }

    public static class MonstergruppenSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Monstergruppen>().Any())
            {
                context.Set<Monstergruppen>().AddRange(
                    new Monstergruppen { Name = "Allein" },
                    new Monstergruppen { Name = "1-3 Wesen" },
                    new Monstergruppen { Name = "4-10 Wesen" },
                    new Monstergruppen { Name = ">10 Wesen" }
                );
                context.SaveChanges();
            }
        }
    }

    public static class MonsterimmunitaetenSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Monsterimmunitaeten>().Any())
            {
                context.Set<Monsterimmunitaeten>().AddRange(
                    new Monsterimmunitaeten { Name = "Vergiftet" },
                    new Monsterimmunitaeten { Name = "Brennend" },
                    new Monsterimmunitaeten { Name = "Liegend" },
                    new Monsterimmunitaeten { Name = "Ohnmächtig" },
                    new Monsterimmunitaeten { Name = "Verwirrt" },
                    new Monsterimmunitaeten { Name = "Erschöpft" },
                    new Monsterimmunitaeten { Name = "Sensibel" },
                    new Monsterimmunitaeten { Name = "Verängstigt" },
                    new Monsterimmunitaeten { Name = "Halluzinierend" },
                    new Monsterimmunitaeten { Name = "Beeinflussung" },
                    new Monsterimmunitaeten { Name = "Blutend" },
                    new Monsterimmunitaeten { Name = "Verwundbar" },
                    new Monsterimmunitaeten { Name = "Übergebend" },
                    new Monsterimmunitaeten { Name = "Ergriffen" },
                    new Monsterimmunitaeten { Name = "Betrunken" },
                    new Monsterimmunitaeten { Name = "Rasend" },
                    new Monsterimmunitaeten { Name = "Verflucht" },
                    new Monsterimmunitaeten { Name = "Gesegnet" },
                    new Monsterimmunitaeten { Name = "Wuchtschaden" },
                    new Monsterimmunitaeten { Name = "Schnittschaden" },
                    new Monsterimmunitaeten { Name = "Stichschaden" },
                    new Monsterimmunitaeten { Name = "Hiebschaden" },
                    new Monsterimmunitaeten { Name = "Dimeritium" }
                );
                context.SaveChanges();
            }
        }
    }

    public static class MonsteranfaelligkeitenSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Monsteranfaelligkeiten>().Any())
            {
                context.Set<Monsteranfaelligkeiten>().AddRange(
                    new Monsteranfaelligkeiten { Name = "Gift" },
                    new Monsteranfaelligkeiten { Name = "Feuer" },
                    new Monsteranfaelligkeiten { Name = "Wasser" },
                    new Monsteranfaelligkeiten { Name = "Waffenöl" },
                    new Monsteranfaelligkeiten { Name = "Bomben" },
                    new Monsteranfaelligkeiten { Name = "Lärm" },
                    new Monsteranfaelligkeiten { Name = "Helligkeit" },
                    new Monsteranfaelligkeiten { Name = "Dimeritium" },
                    new Monsteranfaelligkeiten { Name = "Wuchtschaden" },
                    new Monsteranfaelligkeiten { Name = "Schnitt- & Schnittschaden" },
                    new Monsteranfaelligkeiten { Name = "Hiebschaden" }

                );
                context.SaveChanges();
            }
        }
    }
    public static class MonstertypSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Monstertyp>().Any())
            {
                context.Set<Monstertyp>().AddRange(
                    new Monstertyp { Name = "Tiere", MonsterwuerfelId = 1, MonsterintelligenzId = 1, MonstergruppenId = 3 },
                    new Monstertyp { Name = "Draconide", MonsterwuerfelId = 5, MonsterintelligenzId = 4, MonstergruppenId = 2 },
                    new Monstertyp { Name = "Erscheinungen", MonsterwuerfelId = 2, MonsterintelligenzId = 2, MonstergruppenId = 1 },
                    new Monstertyp { Name = "Hybride", MonsterwuerfelId = 2, MonsterintelligenzId = 1, MonstergruppenId = 2 },
                    new Monstertyp { Name = "Insektoide", MonsterwuerfelId = 2, MonsterintelligenzId = 1, MonstergruppenId = 4 },
                    new Monstertyp { Name = "Nekrophagen", MonsterwuerfelId = 1, MonsterintelligenzId = 2, MonstergruppenId = 3 },
                    new Monstertyp { Name = "Konstrukte", MonsterwuerfelId = 3, MonsterintelligenzId = 3, MonstergruppenId = 1 },
                    new Monstertyp { Name = "Relikte", MonsterwuerfelId = 4, MonsterintelligenzId = 3, MonstergruppenId = 1 },
                    new Monstertyp { Name = "Ogroide", MonsterwuerfelId = 3, MonsterintelligenzId = 2, MonstergruppenId = 2 },
                    new Monstertyp { Name = "Verfluchte", MonsterwuerfelId = 3, MonsterintelligenzId = 4, MonstergruppenId = 4 },
                    new Monstertyp { Name = "Vampire", MonsterwuerfelId = 4, MonsterintelligenzId = 5, MonstergruppenId = 3 },
                    new Monstertyp { Name = "Tulpen", MonsterwuerfelId = 5, MonsterintelligenzId = 4, MonstergruppenId = 1 },
                    new Monstertyp { Name = "Dämonen", MonsterwuerfelId = 4, MonsterintelligenzId = 3, MonstergruppenId = 3 },
                    new Monstertyp { Name = "Götter", MonsterwuerfelId = 5, MonsterintelligenzId = 5, MonstergruppenId = 1 }
                );
                context.SaveChanges();
            }
        }
    }
    public static class MonstertypvorkommenSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Monstertypvorkommen>().Any())
            {
                context.Set<Monstertypvorkommen>().AddRange(
                    new Monstertypvorkommen { MonstertypId = 1, MonstervorkommenId = 1 },
                    new Monstertypvorkommen { MonstertypId = 1, MonstervorkommenId = 2 },
                    new Monstertypvorkommen { MonstertypId = 1, MonstervorkommenId = 3 },
                    new Monstertypvorkommen { MonstertypId = 1, MonstervorkommenId = 4 },
                    new Monstertypvorkommen { MonstertypId = 1, MonstervorkommenId = 5 },
                    new Monstertypvorkommen { MonstertypId = 1, MonstervorkommenId = 6 },

                    new Monstertypvorkommen { MonstertypId = 2, MonstervorkommenId = 2 },
                    new Monstertypvorkommen { MonstertypId = 2, MonstervorkommenId = 4 },
                    new Monstertypvorkommen { MonstertypId = 2, MonstervorkommenId = 6 },

                    new Monstertypvorkommen { MonstertypId = 3, MonstervorkommenId = 1 },
                    new Monstertypvorkommen { MonstertypId = 3, MonstervorkommenId = 5 },

                    new Monstertypvorkommen { MonstertypId = 4, MonstervorkommenId = 2 },
                    new Monstertypvorkommen { MonstertypId = 4, MonstervorkommenId = 3 },
                    new Monstertypvorkommen { MonstertypId = 4, MonstervorkommenId = 4 },
                    new Monstertypvorkommen { MonstertypId = 4, MonstervorkommenId = 6 },

                    new Monstertypvorkommen { MonstertypId = 5, MonstervorkommenId = 1 },
                    new Monstertypvorkommen { MonstertypId = 5, MonstervorkommenId = 2 },
                    new Monstertypvorkommen { MonstertypId = 5, MonstervorkommenId = 3 },
                    new Monstertypvorkommen { MonstertypId = 5, MonstervorkommenId = 4 },
                    new Monstertypvorkommen { MonstertypId = 5, MonstervorkommenId = 5 },
                    new Monstertypvorkommen { MonstertypId = 5, MonstervorkommenId = 6 },

                    new Monstertypvorkommen { MonstertypId = 6, MonstervorkommenId = 1 },
                    new Monstertypvorkommen { MonstertypId = 6, MonstervorkommenId = 5 },
                    new Monstertypvorkommen { MonstertypId = 6, MonstervorkommenId = 6 },

                    new Monstertypvorkommen { MonstertypId = 7, MonstervorkommenId = 1 },
                    new Monstertypvorkommen { MonstertypId = 7, MonstervorkommenId = 2 },
                    new Monstertypvorkommen { MonstertypId = 7, MonstervorkommenId = 4 },
                    new Monstertypvorkommen { MonstertypId = 7, MonstervorkommenId = 6 },

                    new Monstertypvorkommen { MonstertypId = 8, MonstervorkommenId = 1 },
                    new Monstertypvorkommen { MonstertypId = 8, MonstervorkommenId = 2 },
                    new Monstertypvorkommen { MonstertypId = 8, MonstervorkommenId = 3 },
                    new Monstertypvorkommen { MonstertypId = 8, MonstervorkommenId = 5 },
                    new Monstertypvorkommen { MonstertypId = 8, MonstervorkommenId = 6 },

                    new Monstertypvorkommen { MonstertypId = 9, MonstervorkommenId = 1 },
                    new Monstertypvorkommen { MonstertypId = 9, MonstervorkommenId = 2 },
                    new Monstertypvorkommen { MonstertypId = 9, MonstervorkommenId = 3 },
                    new Monstertypvorkommen { MonstertypId = 9, MonstervorkommenId = 4 },
                    new Monstertypvorkommen { MonstertypId = 9, MonstervorkommenId = 6 },

                    new Monstertypvorkommen { MonstertypId = 10, MonstervorkommenId = 1 },
                    new Monstertypvorkommen { MonstertypId = 10, MonstervorkommenId = 2 },
                    new Monstertypvorkommen { MonstertypId = 10, MonstervorkommenId = 3 },
                    new Monstertypvorkommen { MonstertypId = 10, MonstervorkommenId = 4 },
                    new Monstertypvorkommen { MonstertypId = 10, MonstervorkommenId = 5 },
                    new Monstertypvorkommen { MonstertypId = 10, MonstervorkommenId = 6 },

                    new Monstertypvorkommen { MonstertypId = 11, MonstervorkommenId = 5 },

                    new Monstertypvorkommen { MonstertypId = 12, MonstervorkommenId = 5 },

                    new Monstertypvorkommen { MonstertypId = 13, MonstervorkommenId = 5 },
                    new Monstertypvorkommen { MonstertypId = 14, MonstervorkommenId = 1 },
                    new Monstertypvorkommen { MonstertypId = 14, MonstervorkommenId = 2 },
                    new Monstertypvorkommen { MonstertypId = 14, MonstervorkommenId = 3 },
                    new Monstertypvorkommen { MonstertypId = 14, MonstervorkommenId = 4 },
                    new Monstertypvorkommen { MonstertypId = 14, MonstervorkommenId = 5 },
                    new Monstertypvorkommen { MonstertypId = 14, MonstervorkommenId = 6 }
                );
                context.SaveChanges();
            }
        }
    }
    public static class MonstertypimmunitaetenSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Monstertypimmunitaeten>().Any())
            {
                context.Set<Monstertypimmunitaeten>().AddRange(
                    new Monstertypimmunitaeten { MonstertypId = 1, MonsterimmunitaetenId = 12 },
                    new Monstertypimmunitaeten { MonstertypId = 1, MonsterimmunitaetenId = 23 },

                    new Monstertypimmunitaeten { MonstertypId = 2, MonsterimmunitaetenId = 2 },
                    new Monstertypimmunitaeten { MonstertypId = 2, MonsterimmunitaetenId = 4 },
                    new Monstertypimmunitaeten { MonstertypId = 2, MonsterimmunitaetenId = 5 },
                    new Monstertypimmunitaeten { MonstertypId = 2, MonsterimmunitaetenId = 9 },
                    new Monstertypimmunitaeten { MonstertypId = 2, MonsterimmunitaetenId = 10 },
                    new Monstertypimmunitaeten { MonstertypId = 2, MonsterimmunitaetenId = 15 },
                    new Monstertypimmunitaeten { MonstertypId = 2, MonsterimmunitaetenId = 22 },
                    new Monstertypimmunitaeten { MonstertypId = 2, MonsterimmunitaetenId = 23 },

                    new Monstertypimmunitaeten { MonstertypId = 3, MonsterimmunitaetenId = 1 },
                    new Monstertypimmunitaeten { MonstertypId = 3, MonsterimmunitaetenId = 2 },
                    new Monstertypimmunitaeten { MonstertypId = 3, MonsterimmunitaetenId = 3 },
                    new Monstertypimmunitaeten { MonstertypId = 3, MonsterimmunitaetenId = 4 },
                    new Monstertypimmunitaeten { MonstertypId = 3, MonsterimmunitaetenId = 6 },
                    new Monstertypimmunitaeten { MonstertypId = 3, MonsterimmunitaetenId = 11 },
                    new Monstertypimmunitaeten { MonstertypId = 3, MonsterimmunitaetenId = 12 },
                    new Monstertypimmunitaeten { MonstertypId = 3, MonsterimmunitaetenId = 13 },
                    new Monstertypimmunitaeten { MonstertypId = 3, MonsterimmunitaetenId = 14 },
                    new Monstertypimmunitaeten { MonstertypId = 3, MonsterimmunitaetenId = 15 },
                    new Monstertypimmunitaeten { MonstertypId = 3, MonsterimmunitaetenId = 19 },
                    new Monstertypimmunitaeten { MonstertypId = 3, MonsterimmunitaetenId = 20 },
                    new Monstertypimmunitaeten { MonstertypId = 3, MonsterimmunitaetenId = 21 },
                    new Monstertypimmunitaeten { MonstertypId = 3, MonsterimmunitaetenId = 22 },

                    new Monstertypimmunitaeten { MonstertypId = 4, MonsterimmunitaetenId = 4 },
                    new Monstertypimmunitaeten { MonstertypId = 4, MonsterimmunitaetenId = 23 },
                    new Monstertypimmunitaeten { MonstertypId = 5, MonsterimmunitaetenId = 1 },
                    new Monstertypimmunitaeten { MonstertypId = 5, MonsterimmunitaetenId = 4 },
                    new Monstertypimmunitaeten { MonstertypId = 5, MonsterimmunitaetenId = 8 },
                    new Monstertypimmunitaeten { MonstertypId = 5, MonsterimmunitaetenId = 15 },
                    new Monstertypimmunitaeten { MonstertypId = 5, MonsterimmunitaetenId = 20 },
                    new Monstertypimmunitaeten { MonstertypId = 5, MonsterimmunitaetenId = 21 },

                    new Monstertypimmunitaeten { MonstertypId = 6, MonsterimmunitaetenId = 2 },
                    new Monstertypimmunitaeten { MonstertypId = 6, MonsterimmunitaetenId = 13 },
                    new Monstertypimmunitaeten { MonstertypId = 6, MonsterimmunitaetenId = 15 },

                    new Monstertypimmunitaeten { MonstertypId = 7, MonsterimmunitaetenId = 1 },
                    new Monstertypimmunitaeten { MonstertypId = 7, MonsterimmunitaetenId = 4 },
                    new Monstertypimmunitaeten { MonstertypId = 7, MonsterimmunitaetenId = 6 },
                    new Monstertypimmunitaeten { MonstertypId = 7, MonsterimmunitaetenId = 11 },
                    new Monstertypimmunitaeten { MonstertypId = 7, MonsterimmunitaetenId = 13 },
                    new Monstertypimmunitaeten { MonstertypId = 7, MonsterimmunitaetenId = 15 },
                    new Monstertypimmunitaeten { MonstertypId = 8, MonsterimmunitaetenId = 10 },
                    new Monstertypimmunitaeten { MonstertypId = 8, MonsterimmunitaetenId = 15 },
                    new Monstertypimmunitaeten { MonstertypId = 8, MonsterimmunitaetenId = 17 },
                    new Monstertypimmunitaeten { MonstertypId = 8, MonsterimmunitaetenId = 18 },
                    new Monstertypimmunitaeten { MonstertypId = 8, MonsterimmunitaetenId = 23 },
                    new Monstertypimmunitaeten { MonstertypId = 9, MonsterimmunitaetenId = 23 },
                    new Monstertypimmunitaeten { MonstertypId = 10, MonsterimmunitaetenId = 17 },
                    new Monstertypimmunitaeten { MonstertypId = 10, MonsterimmunitaetenId = 23 },
                    new Monstertypimmunitaeten { MonstertypId = 11, MonsterimmunitaetenId = 1 },
                    new Monstertypimmunitaeten { MonstertypId = 11, MonsterimmunitaetenId = 4 },
                    new Monstertypimmunitaeten { MonstertypId = 11, MonsterimmunitaetenId = 5 },
                    new Monstertypimmunitaeten { MonstertypId = 11, MonsterimmunitaetenId = 8 },
                    new Monstertypimmunitaeten { MonstertypId = 11, MonsterimmunitaetenId = 9 },
                    new Monstertypimmunitaeten { MonstertypId = 11, MonsterimmunitaetenId = 10 },
                    new Monstertypimmunitaeten { MonstertypId = 11, MonsterimmunitaetenId = 15 },
                    new Monstertypimmunitaeten { MonstertypId = 11, MonsterimmunitaetenId = 20 },
                    new Monstertypimmunitaeten { MonstertypId = 11, MonsterimmunitaetenId = 21 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 1 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 2 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 3 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 4 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 5 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 6 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 7 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 8 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 9 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 10 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 11 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 12 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 13 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 14 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 15 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 16 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 17 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 18 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 19 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 20 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 21 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 22 },
                    new Monstertypimmunitaeten { MonstertypId = 12, MonsterimmunitaetenId = 23 },
                    new Monstertypimmunitaeten { MonstertypId = 13, MonsterimmunitaetenId = 1 },
                    new Monstertypimmunitaeten { MonstertypId = 13, MonsterimmunitaetenId = 2 },
                    new Monstertypimmunitaeten { MonstertypId = 13, MonsterimmunitaetenId = 5 },
                    new Monstertypimmunitaeten { MonstertypId = 13, MonsterimmunitaetenId = 7 },
                    new Monstertypimmunitaeten { MonstertypId = 13, MonsterimmunitaetenId = 10 },
                    new Monstertypimmunitaeten { MonstertypId = 13, MonsterimmunitaetenId = 12 },
                    new Monstertypimmunitaeten { MonstertypId = 13, MonsterimmunitaetenId = 11 },
                    new Monstertypimmunitaeten { MonstertypId = 13, MonsterimmunitaetenId = 14 },
                    new Monstertypimmunitaeten { MonstertypId = 13, MonsterimmunitaetenId = 16 },
                    new Monstertypimmunitaeten { MonstertypId = 13, MonsterimmunitaetenId = 18 },
                    new Monstertypimmunitaeten { MonstertypId = 13, MonsterimmunitaetenId = 23 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 1 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 2 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 3 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 4 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 5 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 6 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 7 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 8 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 9 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 10 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 11 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 12 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 13 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 14 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 15 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 16 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 17 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 18 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 20 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 21 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 22 },
                    new Monstertypimmunitaeten { MonstertypId = 14, MonsterimmunitaetenId = 23 }
                );
                context.SaveChanges();
            }
        }
    }
    public static class MonstertypanfaelligkeitenSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Set<Monstertypanfaelligkeiten>().Any())
            {
                context.Set<Monstertypanfaelligkeiten>().AddRange(
                    new Monstertypanfaelligkeiten { MonstertypId = 1, MonsteranfaelligkeitenId = 1 },
                    new Monstertypanfaelligkeiten { MonstertypId = 1, MonsteranfaelligkeitenId = 2 },
                    new Monstertypanfaelligkeiten { MonstertypId = 1, MonsteranfaelligkeitenId = 3 },
                    new Monstertypanfaelligkeiten { MonstertypId = 1, MonsteranfaelligkeitenId = 5 },
                    new Monstertypanfaelligkeiten { MonstertypId = 1, MonsteranfaelligkeitenId = 6 },
                    new Monstertypanfaelligkeiten { MonstertypId = 1, MonsteranfaelligkeitenId = 9 },
                    new Monstertypanfaelligkeiten { MonstertypId = 2, MonsteranfaelligkeitenId = 4 },
                    new Monstertypanfaelligkeiten { MonstertypId = 2, MonsteranfaelligkeitenId = 5 },
                    new Monstertypanfaelligkeiten { MonstertypId = 2, MonsteranfaelligkeitenId = 6 },
                    new Monstertypanfaelligkeiten { MonstertypId = 2, MonsteranfaelligkeitenId = 9 },
                    new Monstertypanfaelligkeiten { MonstertypId = 3, MonsteranfaelligkeitenId = 4 },
                    new Monstertypanfaelligkeiten { MonstertypId = 3, MonsteranfaelligkeitenId = 6 },
                    new Monstertypanfaelligkeiten { MonstertypId = 3, MonsteranfaelligkeitenId = 7 },
                    new Monstertypanfaelligkeiten { MonstertypId = 3, MonsteranfaelligkeitenId = 8 },
                    new Monstertypanfaelligkeiten { MonstertypId = 4, MonsteranfaelligkeitenId = 2 },
                    new Monstertypanfaelligkeiten { MonstertypId = 4, MonsteranfaelligkeitenId = 4 },
                    new Monstertypanfaelligkeiten { MonstertypId = 4, MonsteranfaelligkeitenId = 5 },
                    new Monstertypanfaelligkeiten { MonstertypId = 4, MonsteranfaelligkeitenId = 9 },
                    new Monstertypanfaelligkeiten { MonstertypId = 4, MonsteranfaelligkeitenId = 11 },
                    new Monstertypanfaelligkeiten { MonstertypId = 5, MonsteranfaelligkeitenId = 2 },
                    new Monstertypanfaelligkeiten { MonstertypId = 5, MonsteranfaelligkeitenId = 4 },
                    new Monstertypanfaelligkeiten { MonstertypId = 5, MonsteranfaelligkeitenId = 5 },
                    new Monstertypanfaelligkeiten { MonstertypId = 5, MonsteranfaelligkeitenId = 9 },
                    new Monstertypanfaelligkeiten { MonstertypId = 5, MonsteranfaelligkeitenId = 11 },
                    new Monstertypanfaelligkeiten { MonstertypId = 6, MonsteranfaelligkeitenId = 2 },
                    new Monstertypanfaelligkeiten { MonstertypId = 6, MonsteranfaelligkeitenId = 4 },
                    new Monstertypanfaelligkeiten { MonstertypId = 6, MonsteranfaelligkeitenId = 5 },
                    new Monstertypanfaelligkeiten { MonstertypId = 6, MonsteranfaelligkeitenId = 9 },
                    new Monstertypanfaelligkeiten { MonstertypId = 6, MonsteranfaelligkeitenId = 11 },
                    new Monstertypanfaelligkeiten { MonstertypId = 7, MonsteranfaelligkeitenId = 2 },
                    new Monstertypanfaelligkeiten { MonstertypId = 7, MonsteranfaelligkeitenId = 3 },
                    new Monstertypanfaelligkeiten { MonstertypId = 7, MonsteranfaelligkeitenId = 5 },
                    new Monstertypanfaelligkeiten { MonstertypId = 7, MonsteranfaelligkeitenId = 8 },
                    new Monstertypanfaelligkeiten { MonstertypId = 7, MonsteranfaelligkeitenId = 9 },
                    new Monstertypanfaelligkeiten { MonstertypId = 8, MonsteranfaelligkeitenId = 1 },
                    new Monstertypanfaelligkeiten { MonstertypId = 8, MonsteranfaelligkeitenId = 4 },
                    new Monstertypanfaelligkeiten { MonstertypId = 8, MonsteranfaelligkeitenId = 5 },
                    new Monstertypanfaelligkeiten { MonstertypId = 8, MonsteranfaelligkeitenId = 6 },
                    new Monstertypanfaelligkeiten { MonstertypId = 8, MonsteranfaelligkeitenId = 8 },
                    new Monstertypanfaelligkeiten { MonstertypId = 9, MonsteranfaelligkeitenId = 1 },
                    new Monstertypanfaelligkeiten { MonstertypId = 9, MonsteranfaelligkeitenId = 2 },
                    new Monstertypanfaelligkeiten { MonstertypId = 9, MonsteranfaelligkeitenId = 3 },
                    new Monstertypanfaelligkeiten { MonstertypId = 9, MonsteranfaelligkeitenId = 4 },
                    new Monstertypanfaelligkeiten { MonstertypId = 9, MonsteranfaelligkeitenId = 5 },
                    new Monstertypanfaelligkeiten { MonstertypId = 9, MonsteranfaelligkeitenId = 6 },
                    new Monstertypanfaelligkeiten { MonstertypId = 9, MonsteranfaelligkeitenId = 7 },
                    new Monstertypanfaelligkeiten { MonstertypId = 9, MonsteranfaelligkeitenId = 9 },
                    new Monstertypanfaelligkeiten { MonstertypId = 10, MonsteranfaelligkeitenId = 10 },
                    new Monstertypanfaelligkeiten { MonstertypId = 11, MonsteranfaelligkeitenId = 2 },
                    new Monstertypanfaelligkeiten { MonstertypId = 11, MonsteranfaelligkeitenId = 5 },
                    new Monstertypanfaelligkeiten { MonstertypId = 11, MonsteranfaelligkeitenId = 7 },
                    new Monstertypanfaelligkeiten { MonstertypId = 11, MonsteranfaelligkeitenId = 9 },
                    new Monstertypanfaelligkeiten { MonstertypId = 11, MonsteranfaelligkeitenId = 10 },
                    new Monstertypanfaelligkeiten { MonstertypId = 13, MonsteranfaelligkeitenId = 4 },
                    new Monstertypanfaelligkeiten { MonstertypId = 13, MonsteranfaelligkeitenId = 6 }
                );
                context.SaveChanges();
            }
        }
    }
}
