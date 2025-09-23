using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Obermagie> Obermagien { get; set; }
        public DbSet<Anmeldungsstatus> Anmeldungsstati { get; set; }
        public DbSet<Abenteuerrang> Abenteuerraenge { get; set; }
        public DbSet<Blutgruppe> Blutgruppen { get; set; }
        public DbSet<Eindruck> Eindruecke { get; set; }
        public DbSet<Haus> Haeuser { get; set; }
        public DbSet<Herkunftsland> Herkunftslaender { get; set; }
        public DbSet<Infanterierang> Infanterieraenge { get; set; }
        public DbSet<Lebensstatus> Lebensstati { get; set; }
        public DbSet<LightCard> LightCards { get; set; }
        public DbSet<Religion> Religions { get; set; }
        public DbSet<Stand> Staende { get; set; }
        public DbSet<Rasse> Rassen { get; set; }
        public DbSet<Guild> Guilds { get; set; }
        public DbSet<Lizenzen> Lizenzen {  get; set; }
        public DbSet<Gildenlizenz> Gildenlizenzen { get; set; }
        public DbSet<Infanterie> Infanterien { get; set; }
        public DbSet<Regiment> Regiments { get; set; }
        public DbSet<MagicClass> MagicClasses { get; set; }
        public DbSet<MagicClassSpecialization> MagicClassSpecializations { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterDetails> CharacterDetails { get; set; }
        public DbSet<CharacterAffiliation> CharacterAffiliations { get; set; }
        public DbSet<Zaubertyp> Zaubertypen { get; set; }
        public DbSet<Grundzauber> Grundzauber { get; set; }
        public DbSet<SpecialZauber> SpecialZauber { get; set; }
        public DbSet<CharacterMagicClass> CharacterMagicClasses { get; set; }
        public DbSet<Monsterimmunitaeten> Monsterimmunitaeten { get; set; }
        public DbSet<Monsteranfaelligkeiten> Monsteranfaelligkeiten { get; set; }
        public DbSet<Monstergruppen> Monstergruppen { get; set; }
        public DbSet<Monsterintelligenz> Monsterintelligenzen { get; set; }
        public DbSet<Monstervorkommen> Monstervorkommen { get; set; }
        public DbSet<Monsterwuerfel> Monsterwuerfel { get; set; }
        public DbSet<Monstertyp> MonsterTypes { get; set; }
        public DbSet<Monstertypanfaelligkeiten> Monstertypanfaelligkeiten { get; set; }
        public DbSet<Monstertypimmunitaeten> Monstertypimmunitaeten { get; set; }
        public DbSet<Monstertypvorkommen> Monstertypvorkommen { get; set; }
        public DbSet<Monster> Monsters { get; set; }
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

            // Character -> Rasse (1:n)
            builder.Entity<Character>()
                .HasOne(c => c.Rasse)
                .WithMany(r => r.Characters)
                .HasForeignKey(c => c.RasseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Character -> Lebensstatus (1:n)
            builder.Entity<Character>()
                .HasOne(c => c.Lebensstatus)
                .WithMany(l => l.Characters)
                .HasForeignKey(c => c.LebensstatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Character -> Eindruck (1:n)
            builder.Entity<Character>()
                .HasOne(c => c.Eindruck)
                .WithMany(e => e.Charaktere)
                .HasForeignKey(c => c.EindruckId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // CHARACTER ELTERN-BEZIEHUNGEN
            // ===============================

            // Character -> Vater (selbstreferenziell)
            builder.Entity<Character>()
                .HasOne(c => c.Vater)
                .WithMany()
                .HasForeignKey(c => c.VaterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Character -> Mutter (selbstreferenziell)
            builder.Entity<Character>()
                .HasOne(c => c.Mutter)
                .WithMany()
                .HasForeignKey(c => c.MutterId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // CHARACTER DETAIL-BEZIEHUNGEN (1:1)
            // ===============================

            // CharacterDetails -> Character (1:1)
            builder.Entity<CharacterDetails>()
                .HasOne(cd => cd.Character)
                .WithOne(c => c.Details)
                .HasForeignKey<CharacterDetails>(cd => cd.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // CharacterDetails -> Stand
            builder.Entity<CharacterDetails>()
                .HasOne(cd => cd.Stand)
                .WithMany()
                .HasForeignKey(cd => cd.StandId)
                .OnDelete(DeleteBehavior.Restrict);

            // CharacterDetails -> Blutgruppe
            builder.Entity<CharacterDetails>()
                .HasOne(cd => cd.Blutgruppe)
                .WithMany()
                .HasForeignKey(cd => cd.BlutgruppeId)
                .OnDelete(DeleteBehavior.Restrict);

            // CharacterDetails -> Haus
            builder.Entity<CharacterDetails>()
                .HasOne(cd => cd.Haus)
                .WithMany()
                .HasForeignKey(cd => cd.HausId)
                .OnDelete(DeleteBehavior.Restrict);

            // CharacterDetails -> Herkunftsland
            builder.Entity<CharacterDetails>()
                .HasOne(cd => cd.Herkunftsland)
                .WithMany()
                .HasForeignKey(cd => cd.HerkunftslandId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // CHARACTER ZUGEHÖRIGKEITS-BEZIEHUNGEN (1:1)
            // ===============================

            // CharacterAffiliation -> Character (1:1)
            builder.Entity<CharacterAffiliation>()
                .HasOne(ca => ca.Character)
                .WithOne(c => c.Affiliation)
                .HasForeignKey<CharacterAffiliation>(ca => ca.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // CharacterAffiliation -> Guild
            builder.Entity<CharacterAffiliation>()
                .HasOne(ca => ca.Guild)
                .WithMany()  // Guild hat keine direkte Characters Collection
                .HasForeignKey(ca => ca.GuildId)
                .OnDelete(DeleteBehavior.Restrict);

            // CharacterAffiliation -> Regiment
            builder.Entity<CharacterAffiliation>()
                .HasOne(ca => ca.Regiment)
                .WithMany()  // Regiment Characters werden indirekt über CharacterAffiliation erreicht
                .HasForeignKey(ca => ca.RegimentsId)
                .OnDelete(DeleteBehavior.Restrict);

            // CharacterAffiliation -> Infanterierang
            builder.Entity<CharacterAffiliation>()
                .HasOne(ca => ca.Infanterierang)
                .WithMany()  // Infanterierang Characters werden indirekt über CharacterAffiliation erreicht
                .HasForeignKey(ca => ca.InfanterierangId)
                .OnDelete(DeleteBehavior.Restrict);

            // CharacterAffiliation -> Religion
            builder.Entity<CharacterAffiliation>()
                .HasOne(ca => ca.Religion)
                .WithMany()  // Religion Characters werden indirekt über CharacterAffiliation erreicht
                .HasForeignKey(ca => ca.ReligionId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // GUILD BEZIEHUNGEN
            // ===============================

            // Guild -> LightCard
            builder.Entity<Guild>()
                .HasOne(g => g.LightCard)
                .WithMany()
                .HasForeignKey(g => g.LightCardId)
                .OnDelete(DeleteBehavior.Restrict);

            // Guild Leader/Vertreter Beziehungen
            builder.Entity<Guild>()
                .HasOne(g => g.LeaderCharacter)
                .WithMany()
                .HasForeignKey(g => g.leader)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Guild>()
                .HasOne(g => g.VertreterCharacter)
                .WithMany()
                .HasForeignKey(g => g.vertreter)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // INFANTERIE BEZIEHUNGEN
            // ===============================

            // Infanterie Leader/Vertreter Beziehungen
            builder.Entity<Infanterie>()
                .HasOne(i => i.LeaderCharacter)
                .WithMany()
                .HasForeignKey(i => i.leader)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Infanterie>()
                .HasOne(i => i.VertreterCharacter)
                .WithMany()
                .HasForeignKey(i => i.vertreter)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // REGIMENT BEZIEHUNGEN
            // ===============================

            // Regiment -> Infanterie
            builder.Entity<Regiment>()
                .HasOne(r => r.Infanterie)
                .WithMany(i => i.Regiments)
                .HasForeignKey(r => r.InfanterieId)
                .OnDelete(DeleteBehavior.Restrict);

            // Regiment Leader/Adjutant Beziehungen
            builder.Entity<Regiment>()
                .HasOne(r => r.RegimentsCharacter)
                .WithMany()
                .HasForeignKey(r => r.Regimentsleiter)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Regiment>()
                .HasOne(r => r.AdjutantCharacter)
                .WithMany()
                .HasForeignKey(r => r.Adjutant)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // MAGIC CLASS BEZIEHUNGEN
            // ===============================

            // MagicClass -> Obermagie
            builder.Entity<MagicClass>()
                .HasOne(mc => mc.Obermagie)
                .WithMany()
                .HasForeignKey(mc => mc.ObermagieId)
                .OnDelete(DeleteBehavior.Restrict);

            // MagicClassSpecialization -> MagicClass
            builder.Entity<MagicClassSpecialization>()
                .HasOne(mcs => mcs.MagicClass)
                .WithMany(mc => mc.MagicClassSpecializations)
                .HasForeignKey(mcs => mcs.MagicClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // Obermagie -> LightCard
            builder.Entity<Obermagie>()
                .HasOne(o => o.LightCard)
                .WithMany(lc => lc.Obermagie)
                .HasForeignKey(o => o.LightCardId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // CHARACTER MAGIC CLASS BEZIEHUNGEN (n:m)
            // ===============================

            // CharacterMagicClass -> Character
            builder.Entity<CharacterMagicClass>()
                .HasOne(cmc => cmc.Character)
                .WithMany(c => c.CharacterMagicClasses)
                .HasForeignKey(cmc => cmc.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // CharacterMagicClass -> MagicClass
            builder.Entity<CharacterMagicClass>()
                .HasOne(cmc => cmc.MagicClass)
                .WithMany(mc => mc.CharacterMagicClasses)
                .HasForeignKey(cmc => cmc.MagicClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // CharacterMagicClass -> MagicClassSpecialization (optional)
            builder.Entity<CharacterMagicClass>()
                .HasOne(cmc => cmc.MagicClassSpecialization)
                .WithMany()
                .HasForeignKey(cmc => cmc.MagicClassSpecializationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===============================
            // ZAUBER BEZIEHUNGEN
            // ===============================

            // Grundzauber -> MagicClass
            builder.Entity<Grundzauber>()
                .HasOne(gz => gz.MagicClass)
                .WithMany()
                .HasForeignKey(gz => gz.MagicClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // SpecialZauber -> MagicClassSpecialization
            builder.Entity<SpecialZauber>()
                .HasOne(sz => sz.MagicClassSpecialization)
                .WithMany()
                .HasForeignKey(sz => sz.MagicClassSpecializationId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===============================
            // MONSTER BEZIEHUNGEN
            // ===============================

            // Monster -> Monstertyp
            builder.Entity<Monster>()
                .HasOne(m => m.Monstertyp)
                .WithMany(mt => mt.Monster)
                .HasForeignKey(m => m.MonstertypId)
                .OnDelete(DeleteBehavior.Restrict);           
        }
    }
}
