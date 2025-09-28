using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        // ===============================
        // DBSETS - ALLE ENTITÄTEN
        // ===============================

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
        }
    }
}