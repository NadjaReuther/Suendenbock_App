using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Suendenbock_App.Models;
using Suendenbock_App.Models.Domain;
using System.Reflection.Emit;

namespace Suendenbock_App.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // ===============================
        // DBSETS - ALLE ENTITÄTEN
        // ===============================

        // NEU: Trigger-System
        public DbSet<TriggerCategory> TriggerCategories { get; set; }
        public DbSet<TriggerTopic> TriggerTopics { get; set; }
        public DbSet<UserTriggerPreference> UserTriggerPreferences { get; set; }


        // Kern-Entitäten
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterDetails> CharacterDetails { get; set; }
        public DbSet<CharacterAffiliation> CharacterAffiliations { get; set; }
        public DbSet<CharacterMagicClass> CharacterMagicClasses { get; set; }

        // Magie-System
        public DbSet<MagicClass> MagicClasses { get; set; }
        public DbSet<MagicClassSpecialization> MagicClassSpecializations { get; set; }
        public DbSet<Obermagie> Obermagien { get; set; }
        public DbSet<Grundzauber> Grundzauber { get; set; }
        public DbSet<SpecialZauber> SpecialZauber { get; set; }
        public DbSet<Zaubertyp> Zaubertypen { get; set; }

        // Organisationen
        public DbSet<Guild> Guilds { get; set; }
        public DbSet<Gildenlizenz> Gildenlizenzen { get; set; }
        public DbSet<Lizenzen> Lizenzen { get; set; }
        public DbSet<Infanterie> Infanterien { get; set; }
        public DbSet<Regiment> Regiments { get; set; }

        // Lookup-Tabellen
        public DbSet<Rasse> Rassen { get; set; }
        public DbSet<Lebensstatus> Lebensstati { get; set; }
        public DbSet<Eindruck> Eindruecke { get; set; }
        public DbSet<Stand> Staende { get; set; }
        public DbSet<Blutgruppe> Blutgruppen { get; set; }
        public DbSet<Haus> Haeuser { get; set; }
        public DbSet<Herkunftsland> Herkunftslaender { get; set; }
        public DbSet<Religion> Religions { get; set; }
        public DbSet<Infanterierang> Infanterieraenge { get; set; }
        public DbSet<Abenteuerrang> Abenteuerraenge { get; set; }
        public DbSet<Anmeldungsstatus> Anmeldungsstati { get; set; }
        public DbSet<LightCard> LightCards { get; set; }

        // Monster-System
        public DbSet<Monster> Monsters { get; set; }
        public DbSet<Monstertyp> MonsterTypes { get; set; }
        public DbSet<Monstergruppen> Monstergruppen { get; set; }
        public DbSet<Monsterintelligenz> Monsterintelligenzen { get; set; }
        public DbSet<Monsterwuerfel> Monsterwuerfel { get; set; }
        public DbSet<Monstervorkommen> Monstervorkommen { get; set; }
        public DbSet<Monsterimmunitaeten> Monsterimmunitaeten { get; set; }
        public DbSet<Monsteranfaelligkeiten> Monsteranfaelligkeiten { get; set; }
        public DbSet<Monstertypimmunitaeten> Monstertypimmunitaeten { get; set; }
        public DbSet<Monstertypanfaelligkeiten> Monstertypanfaelligkeiten { get; set; }
        public DbSet<Monstertypvorkommen> Monstertypvorkommen { get; set; }

        //Weihnachtsabenteuer
        public DbSet<AdventDoor> AdventDoors { get; set; }
        public DbSet<UserAdventChoice> UserAdventChoices { get; set; }

        // Glossar/Wiki-System
        public DbSet<GlossaryEntry> GlossaryEntries { get; set; }

        // Achievement-System
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<GuildAchievement> GuildAchievements { get; set; }

        // GameSession-System
        public DbSet<Quest> Quests { get; set; }
        public DbSet<Act> Acts { get; set; }
        public DbSet<Map> Maps { get; set; }
        public DbSet<MapMarker> MapMarkers { get; set; }
        public DbSet<RestFood> RestFoods { get; set; }

        // Weather-System
        public DbSet<WeatherOption> WeatherOptions { get; set; }
        public DbSet<WeatherForecastDay> WeatherForecastDays { get; set; }

        // Combat-System
        public DbSet<CombatSession> CombatSessions { get; set; }

        // Versammlungsort System
        public DbSet<ForumCategory> ForumCategories { get; set; }
        public DbSet<ForumThread> ForumThreads { get; set; }
        public DbSet<ForumReply> forumReplies { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<PollOption> PollOptions { get; set; }
        public DbSet<PollVote> PollVotes { get; set; }
        public DbSet<MonthlyEvent> MonthlyEvents { get; set; }
        public DbSet<EventChore> EventChores { get; set; }
        public DbSet<EventRSVP> EventRSVPs { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<NewsItem> NewsItems { get; set; }
        public DbSet<NewsComment> NewsComments { get; set; }
        public DbSet<MonthlyPayment> MonthlyPayments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ===============================
            // CHARACTER BASIS-BEZIEHUNGEN
            // ===============================

            // Character -> Rasse (PFLICHT)
            builder.Entity<Character>()
                .HasOne(c => c.Rasse)
                .WithMany(r => r.Characters)
                .HasForeignKey(c => c.RasseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Character -> Lebensstatus (PFLICHT)
            builder.Entity<Character>()
                .HasOne(c => c.Lebensstatus)
                .WithMany(l => l.Characters)
                .HasForeignKey(c => c.LebensstatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Character -> Eindruck (PFLICHT)
            builder.Entity<Character>()
                .HasOne(c => c.Eindruck)
                .WithMany(e => e.Charaktere)
                .HasForeignKey(c => c.EindruckId)
                .OnDelete(DeleteBehavior.Restrict);

            // Character -> Vater (Selbstreferenz, optional)
            builder.Entity<Character>()
                .HasOne(c => c.Vater)
                .WithMany()
                .HasForeignKey(c => c.VaterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Character -> Mutter (Selbstreferenz, optional)
            builder.Entity<Character>()
                .HasOne(c => c.Mutter)
                .WithMany()
                .HasForeignKey(c => c.MutterId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // CHARACTER DETAILS (1:1)
            // ===============================

            // CharacterDetails -> Character (1:1, CASCADE)
            builder.Entity<CharacterDetails>()
                .HasOne(cd => cd.Character)
                .WithOne(c => c.Details)
                .HasForeignKey<CharacterDetails>(cd => cd.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // CharacterDetails -> Stand (optional)
            builder.Entity<CharacterDetails>()
                .HasOne(cd => cd.Stand)
                .WithMany()
                .HasForeignKey(cd => cd.StandId)
                .OnDelete(DeleteBehavior.Restrict);

            // CharacterDetails -> Blutgruppe (optional)
            builder.Entity<CharacterDetails>()
                .HasOne(cd => cd.Blutgruppe)
                .WithMany()
                .HasForeignKey(cd => cd.BlutgruppeId)
                .OnDelete(DeleteBehavior.Restrict);

            // CharacterDetails -> Haus (optional)
            builder.Entity<CharacterDetails>()
                .HasOne(cd => cd.Haus)
                .WithMany()
                .HasForeignKey(cd => cd.HausId)
                .OnDelete(DeleteBehavior.Restrict);

            // CharacterDetails -> Herkunftsland (optional)
            builder.Entity<CharacterDetails>()
                .HasOne(cd => cd.Herkunftsland)
                .WithMany()
                .HasForeignKey(cd => cd.HerkunftslandId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // CHARACTER AFFILIATION (1:1)
            // ===============================

            // CharacterAffiliation -> Character (1:1, CASCADE)
            builder.Entity<CharacterAffiliation>()
                .HasOne(ca => ca.Character)
                .WithOne(c => c.Affiliation)
                .HasForeignKey<CharacterAffiliation>(ca => ca.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // CharacterAffiliation -> Guild (optional)
            builder.Entity<CharacterAffiliation>()
                .HasOne(ca => ca.Guild)
                .WithMany()
                .HasForeignKey(ca => ca.GuildId)
                .OnDelete(DeleteBehavior.Restrict);

            // CharacterAffiliation -> Regiment (optional) - KORRIGIERTER FK NAME
            builder.Entity<CharacterAffiliation>()
                .HasOne(ca => ca.Regiment)
                .WithMany()
                .HasForeignKey(ca => ca.RegimentId)  // ← KORRIGIERT von RegimentsId
                .OnDelete(DeleteBehavior.Restrict);

            // CharacterAffiliation -> Infanterierang (optional)
            builder.Entity<CharacterAffiliation>()
                .HasOne(ca => ca.Infanterierang)
                .WithMany()
                .HasForeignKey(ca => ca.InfanterierangId)
                .OnDelete(DeleteBehavior.Restrict);

            // CharacterAffiliation -> Religion (optional)
            builder.Entity<CharacterAffiliation>()
                .HasOne(ca => ca.Religion)
                .WithMany()
                .HasForeignKey(ca => ca.ReligionId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // CHARACTER MAGIC CLASSES (N:M)
            // ===============================

            // CharacterMagicClass -> Character (CASCADE)
            builder.Entity<CharacterMagicClass>()
                .HasOne(cmc => cmc.Character)
                .WithMany(c => c.CharacterMagicClasses)
                .HasForeignKey(cmc => cmc.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // CharacterMagicClass -> MagicClass (RESTRICT)
            builder.Entity<CharacterMagicClass>()
                .HasOne(cmc => cmc.MagicClass)
                .WithMany(mc => mc.CharacterMagicClasses)
                .HasForeignKey(cmc => cmc.MagicClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // CharacterMagicClass -> MagicClassSpecialization (optional, RESTRICT)
            builder.Entity<CharacterMagicClass>()
                .HasOne(cmc => cmc.MagicClassSpecialization)
                .WithMany()
                .HasForeignKey(cmc => cmc.MagicClassSpecializationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // MAGIE-SYSTEM
            // ===============================

            // MagicClass -> Obermagie (RESTRICT)
            builder.Entity<MagicClass>()
                .HasOne(mc => mc.Obermagie)
                .WithMany()
                .HasForeignKey(mc => mc.ObermagieId)
                .OnDelete(DeleteBehavior.Restrict);

            // MagicClassSpecialization -> MagicClass (CASCADE)
            builder.Entity<MagicClassSpecialization>()
                .HasOne(mcs => mcs.MagicClass)
                .WithMany(mc => mc.MagicClassSpecializations)
                .HasForeignKey(mcs => mcs.MagicClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // Obermagie -> LightCard (RESTRICT)
            builder.Entity<Obermagie>()
                .HasOne(o => o.LightCard)
                .WithMany(lc => lc.Obermagie)
                .HasForeignKey(o => o.LightCardId)
                .OnDelete(DeleteBehavior.Restrict);

            // Grundzauber -> MagicClass (CASCADE)
            builder.Entity<Grundzauber>()
                .HasOne(gz => gz.MagicClass)
                .WithMany()
                .HasForeignKey(gz => gz.MagicClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // SpecialZauber -> MagicClassSpecialization (CASCADE)
            builder.Entity<SpecialZauber>()
                .HasOne(sz => sz.MagicClassSpecialization)
                .WithMany()
                .HasForeignKey(sz => sz.MagicClassSpecializationId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===============================
            // GUILD-SYSTEM
            // ===============================

            // Guild -> LightCard (RESTRICT)
            builder.Entity<Guild>()
                .HasOne(g => g.LightCard)
                .WithMany()
                .HasForeignKey(g => g.LightCardId)
                .OnDelete(DeleteBehavior.Restrict);

            // Guild -> Abenteuerrang (RESTRICT)
            builder.Entity<Guild>()
                .HasOne(g => g.AbenteuerrangNavigation)
                .WithMany(a => a.Guilds)
                .HasForeignKey(g => g.AbenteuerrangId)
                .OnDelete(DeleteBehavior.Restrict);

            // Guild -> Anmeldungsstatus (RESTRICT)
            builder.Entity<Guild>()
                .HasOne(g => g.AnmeldungsstatusNavigation)
                .WithMany(a => a.Guilds)
                .HasForeignKey(g => g.AnmeldungsstatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Guild -> Leader Character (optional, RESTRICT) - KORRIGIERTER FK NAME
            builder.Entity<Guild>()
                .HasOne(g => g.LeaderCharacter)
                .WithMany()
                .HasForeignKey(g => g.LeaderId)  // ← KORRIGIERT von leader
                .OnDelete(DeleteBehavior.Restrict);

            // Guild -> Vertreter Character (optional, RESTRICT) - KORRIGIERTER FK NAME
            builder.Entity<Guild>()
                .HasOne(g => g.VertreterCharacter)
                .WithMany()
                .HasForeignKey(g => g.VertreterId)  // ← KORRIGIERT von vertreter
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // GILDENLIZENZ (N:M)
            // ===============================

            // Gildenlizenz -> Guild (CASCADE)
            builder.Entity<Gildenlizenz>()
                .HasOne(gl => gl.Guild)
                .WithMany(g => g.Gildenlizenzen)
                .HasForeignKey(gl => gl.GuildId)
                .OnDelete(DeleteBehavior.Cascade);

            // Gildenlizenz -> Lizenzen (RESTRICT)
            builder.Entity<Gildenlizenz>()
                .HasOne(gl => gl.Lizenzen)
                .WithMany(l => l.Gildenlizenzen)
                .HasForeignKey(gl => gl.LizenzenId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // INFANTERIE-SYSTEM
            // ===============================

            // Infanterie -> Leader Character (optional, RESTRICT) - KORRIGIERTER FK NAME
            builder.Entity<Infanterie>()
                .HasOne(i => i.LeaderCharacter)
                .WithMany()
                .HasForeignKey(i => i.LeaderId)  // ← KORRIGIERT von leader
                .OnDelete(DeleteBehavior.Restrict);

            // Infanterie -> Vertreter Character (optional, RESTRICT) - KORRIGIERTER FK NAME
            builder.Entity<Infanterie>()
                .HasOne(i => i.VertreterCharacter)
                .WithMany()
                .HasForeignKey(i => i.VertreterId)  // ← KORRIGIERT von vertreter
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // REGIMENT-SYSTEM
            // ===============================

            // Regiment -> Infanterie (RESTRICT)
            builder.Entity<Regiment>()
                .HasOne(r => r.Infanterie)
                .WithMany(i => i.Regiments)
                .HasForeignKey(r => r.InfanterieId)
                .OnDelete(DeleteBehavior.Restrict);

            // Regiment -> Regimentsleiter Character (optional, RESTRICT) - KORRIGIERTER FK NAME
            builder.Entity<Regiment>()
                .HasOne(r => r.Regimentsleiter)  // ← KORRIGIERT Navigation Property Name
                .WithMany()
                .HasForeignKey(r => r.RegimentsleiterId)  // ← KORRIGIERT von Regimentsleiter
                .OnDelete(DeleteBehavior.Restrict);

            // Regiment -> Adjutant Character (optional, RESTRICT) - KORRIGIERTER FK NAME
            builder.Entity<Regiment>()
                .HasOne(r => r.Adjutant)  // ← KORRIGIERT Navigation Property Name
                .WithMany()
                .HasForeignKey(r => r.AdjutantId)  // ← KORRIGIERT von Adjutant
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // MONSTER-SYSTEM
            // ===============================

            // Monster -> Monstertyp (RESTRICT)
            builder.Entity<Monster>()
                .HasOne(m => m.Monstertyp)
                .WithMany(mt => mt.Monster)
                .HasForeignKey(m => m.MonstertypId)
                .OnDelete(DeleteBehavior.Restrict);

            // Monstertyp -> Monstergruppen (RESTRICT)
            builder.Entity<Monstertyp>()
                .HasOne(mt => mt.Monstergruppen)
                .WithMany(mg => mg.Monstertypen)
                .HasForeignKey(mt => mt.MonstergruppenId)
                .OnDelete(DeleteBehavior.Restrict);

            // Monstertyp -> Monsterintelligenz (RESTRICT)
            builder.Entity<Monstertyp>()
                .HasOne(mt => mt.Monsterintelligenz)
                .WithMany(mi => mi.Monstertypen)
                .HasForeignKey(mt => mt.MonsterintelligenzId)
                .OnDelete(DeleteBehavior.Restrict);

            // Monstertyp -> Monsterwuerfel (RESTRICT)
            builder.Entity<Monstertyp>()
                .HasOne(mt => mt.Monsterwuerfel)
                .WithMany(mw => mw.Monstertypen)
                .HasForeignKey(mt => mt.MonsterwuerfelId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // MONSTER N:M BEZIEHUNGEN
            // ===============================

            // Monstertypimmunitaeten (N:M zwischen Monstertyp und Monsterimmunitaeten)
            builder.Entity<Monstertypimmunitaeten>()
                .HasOne(mti => mti.Monstertyp)
                .WithMany(mt => mt.MonstertypImmunitaeten)
                .HasForeignKey(mti => mti.MonstertypId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Monstertypimmunitaeten>()
                .HasOne(mti => mti.Monsterimmunitaeten)
                .WithMany(mi => mi.Monstertypimmunitaeten)
                .HasForeignKey(mti => mti.MonsterimmunitaetenId)
                .OnDelete(DeleteBehavior.Cascade);

            // Monstertypanfaelligkeiten (N:M zwischen Monstertyp und Monsteranfaelligkeiten)
            builder.Entity<Monstertypanfaelligkeiten>()
                .HasOne(mta => mta.Monstertyp)
                .WithMany(mt => mt.MonstertypAnfaelligkeiten)
                .HasForeignKey(mta => mta.MonstertypId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Monstertypanfaelligkeiten>()
                .HasOne(mta => mta.Monsteranfaelligkeiten)
                .WithMany(ma => ma.Monstertypanfaelligkeiten)
                .HasForeignKey(mta => mta.MonsteranfaelligkeitenId)
                .OnDelete(DeleteBehavior.Cascade);

            // Monstertypvorkommen (N:M zwischen Monstertyp und Monstervorkommen)
            builder.Entity<Monstertypvorkommen>()
                .HasOne(mtv => mtv.Monstertyp)
                .WithMany(mt => mt.MonstertypenVorkommen)
                .HasForeignKey(mtv => mtv.MonstertypId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Monstertypvorkommen>()
                .HasOne(mtv => mtv.Monstervorkommen)
                .WithMany(mv => mv.MonstertypenVorkommen)
                .HasForeignKey(mtv => mtv.MonstervorkommenId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===============================
            // TRIGGER-SYSTEM
            // ===============================

            // TriggerTopic -> TriggerCategory (CASCADE)
            builder.Entity<TriggerTopic>()
                .HasOne(tt => tt.Category)
                .WithMany(tc => tc.Topics)
                .HasForeignKey(tt => tt.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserTriggerPreference -> ApplicationUser (CASCADE)
            builder.Entity<UserTriggerPreference>()
                .HasOne(utp => utp.User)
                .WithMany(u => u.TriggerPreferences)
                .HasForeignKey(utp => utp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserTriggerPreference -> TriggerTopic (CASCADE)
            builder.Entity<UserTriggerPreference>()
                .HasOne(utp => utp.Topic)
                .WithMany(tt => tt.UserPreferences)
                .HasForeignKey(utp => utp.TopicId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique Constraint: Ein User kann pro Topic nur eine Präferenz haben
            builder.Entity<UserTriggerPreference>()
                .HasIndex(utp => new { utp.UserId, utp.TopicId })
                .IsUnique();

            // ===== NEUE BEZIEHUNGEN KONFIGURIEREN =====

            // Character -> Quest (One-to-Many, optional)
            // Individual-Quests haben einen Character, Group-Quests nicht
            builder.Entity<Quest>()
                .HasOne(q => q.Character)
                .WithMany(c => c.IndividualQuests)
                .HasForeignKey(q => q.CharacterId)
                .OnDelete(DeleteBehavior.SetNull); // Wenn Character gelöscht, Quest bleibt

            // Quest -> MapMarker (Optional)
            builder.Entity<Quest>()
                .HasOne(q => q.MapMarker)
                .WithMany(m => m.Quests)
                .HasForeignKey(q => q.MapMarkerId)
                .OnDelete(DeleteBehavior.SetNull); // Wenn Marker gelöscht, Quest bleibt

            // Quest -> Act (Required)
            builder.Entity<Quest>()
                .HasOne(q => q.Act)
                .WithMany(a => a.Quests)
                .HasForeignKey(q => q.ActId)
                .OnDelete(DeleteBehavior.Restrict); // Act kann nicht gelöscht werden, wenn Quests existieren

            // Quest -> PreviousQuest (Self-referencing, Optional)
            // Für Questfolgen: Eine Quest kann eine Vorgänger-Quest haben
            builder.Entity<Quest>()
                .HasOne(q => q.PreviousQuest)
                .WithMany(q => q.FollowingQuests)
                .HasForeignKey(q => q.PreviousQuestId)
                .OnDelete(DeleteBehavior.Restrict); // Vorgänger-Quest kann nicht gelöscht werden, wenn Folge-Quests existieren

            // Act -> Map (One-to-One)
            builder.Entity<Map>()
                .HasOne(m => m.Act)
                .WithOne(a => a.Map)
                .HasForeignKey<Map>(m => m.ActId)
                .OnDelete(DeleteBehavior.Cascade); // Wenn Act gelöscht, Map auch

            // Map -> MapMarker (One-to-Many)
            builder.Entity<MapMarker>()
                .HasOne(mm => mm.Map)
                .WithMany(m => m.Markers)
                .HasForeignKey(mm => mm.MapId)
                .OnDelete(DeleteBehavior.Cascade); // Wenn Map gelöscht, Marker auch

            // ===== INDICES FÜR PERFORMANCE =====

            // Quest-Status und Type werden oft gesucht
            builder.Entity<Quest>()
                .HasIndex(q => q.Status);

            builder.Entity<Quest>()
                .HasIndex(q => q.Type);

            // Monster-Trophäen-Filterung
            builder.Entity<Monster>()
                .HasIndex(m => m.HasBoughtTrophy);

            builder.Entity<Monster>()
                .HasIndex(m => m.HasSlainTrophy);

            // IsEquipped für schnelle Abfrage der ausgerüsteten Trophäen
            builder.Entity<Monster>()
                .HasIndex(m => m.IsEquipped);

            // Nur ein aktiver Akt gleichzeitig
            builder.Entity<Act>()
                .HasIndex(a => a.IsActive);

            // ===== WEATHER-SYSTEM BEZIEHUNGEN =====

            // WeatherOption -> WeatherForecastDay (One-to-Many)
            builder.Entity<WeatherForecastDay>()
                .HasOne(wfd => wfd.WeatherOption)
                .WithMany(wo => wo.ForecastDays)
                .HasForeignKey(wfd => wfd.WeatherOptionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index für schnelle Monat-Suche
            builder.Entity<WeatherOption>()
                .HasIndex(wo => wo.Month);

            // ===== COMBAT-SYSTEM BEZIEHUNGEN =====

            // CombatSession -> Act (Required)
            builder.Entity<CombatSession>()
                .HasOne(cs => cs.Act)
                .WithMany()
                .HasForeignKey(cs => cs.ActId)
                .OnDelete(DeleteBehavior.Restrict); // Act kann nicht gelöscht werden, wenn aktive Kämpfe existieren

            // Index für schnelle Suche nach aktiven Kämpfen
            builder.Entity<CombatSession>()
                .HasIndex(cs => cs.IsActive);

            // Nur ein aktiver Kampf pro Akt
            builder.Entity<CombatSession>()
                .HasIndex(cs => new { cs.ActId, cs.IsActive });

            // ===== POLL-SYSTEM BEZIEHUNGEN =====

            // PollVote -> Poll: NO ACTION (wird indirekt über PollOption gelöscht)
            builder.Entity<PollVote>()
                .HasOne(pv => pv.Poll)
                .WithMany(p => p.Votes)
                .HasForeignKey(pv => pv.PollId)
                .OnDelete(DeleteBehavior.NoAction);

            // PollVote -> PollOption: CASCADE (wenn Option gelöscht wird, Votes auch)
            builder.Entity<PollVote>()
                .HasOne(pv => pv.PollOption)
                .WithMany(po => po.Votes)
                .HasForeignKey(pv => pv.PollOptionId)
                .OnDelete(DeleteBehavior.Cascade);

            // PollOption -> Poll: CASCADE (wenn Poll gelöscht wird, Options auch)
            builder.Entity<PollOption>()
                .HasOne(po => po.Poll)
                .WithMany(p => p.Options)
                .HasForeignKey(po => po.PollId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== NEWS-SYSTEM BEZIEHUNGEN =====

            // NewsComment -> NewsItem: CASCADE (wenn NewsItem gelöscht wird, Comments auch)
            builder.Entity<NewsComment>()
                .HasOne(nc => nc.NewsItem)
                .WithMany(ni => ni.Comments)
                .HasForeignKey(nc => nc.NewsItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}