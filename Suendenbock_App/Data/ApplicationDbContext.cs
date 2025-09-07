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
        public DbSet<Beruf> Berufe { get; set; }
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

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //Character - Pflichtfelder definieren
            // Character Basis-Beziehungen (bestehend - nicht ändern)
            builder.Entity<Character>()
                .HasOne(c => c.Rasse)
                .WithMany(r => r.Characters)
                .HasForeignKey(c => c.RasseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Character>()
                .HasOne(c => c.Lebensstatus)
                .WithMany(l => l.Characters)
                .HasForeignKey(c => c.LebensstatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Character>()
                .HasOne(c => c.Eindruck)
                .WithMany(e => e.Charaktere)
                .HasForeignKey(c => c.EindruckId)
                .OnDelete(DeleteBehavior.Restrict);

            // Character Eltern-Beziehungen (bestehend - nicht ändern)
            builder.Entity<Character>()
                .HasOne(c => c.Vater)
                .WithMany()
                .HasForeignKey(c => c.VaterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Character>()
                .HasOne(c => c.Mutter)
                .WithMany()
                .HasForeignKey(c => c.MutterId)
                .OnDelete(DeleteBehavior.Restrict);

            // CharacterDetails - 1:1 Beziehung (bestehend - nicht ändern)
            builder.Entity<CharacterDetails>()
                .HasOne(cd => cd.Character)
                .WithOne(c => c.Details)
                .HasForeignKey<CharacterDetails>(cd => cd.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // CharacterAffiliation - 1:1 Beziehung (bestehend - nicht ändern)
            builder.Entity<CharacterAffiliation>()
                .HasOne(ca => ca.Character)
                .WithOne(c => c.Affiliation)
                .HasForeignKey<CharacterAffiliation>(ca => ca.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // MagicClass Beziehungen (bestehend - nicht ändern)
            builder.Entity<MagicClassSpecialization>()
               .HasOne(mcs => mcs.MagicClass)
               .WithMany(mc => mc.MagicClassSpecializations)
               .HasForeignKey(mcs => mcs.MagicClassId);

            // CharacterMagicClass (bestehend - nicht ändern)
            builder.Entity<CharacterMagicClass>()
                .HasKey(cm => new { cm.CharacterId, cm.MagicClassId });

            builder.Entity<CharacterMagicClass>()
                .HasOne(cm => cm.Character)
                .WithMany(c => c.CharacterMagicClasses)
                .HasForeignKey(cm => cm.CharacterId);

            builder.Entity<CharacterMagicClass>()
                .HasOne(cm => cm.MagicClass)
                .WithMany(m => m.CharacterMagicClasses)
                .HasForeignKey(cm => cm.MagicClassId);

            builder.Entity<CharacterMagicClass>()
                .HasOne(cm => cm.MagicClassSpecialization)
                .WithMany()
                .HasForeignKey(cm => cm.MagicClassSpecializationId)
                .IsRequired(false);

            // NEUE Guild Leader/Vertreter Beziehungen - ClientSetNull
            builder.Entity<Guild>()
                .HasOne(g => g.LeaderCharacter)
                .WithMany()
                .HasForeignKey(g => g.leader)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired(false);

            builder.Entity<Guild>()
                .HasOne(g => g.VertreterCharacter)
                .WithMany()
                .HasForeignKey(g => g.vertreter)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired(false);

            // NEUE Regiment Beziehungen - ClientSetNull
            builder.Entity<Regiment>()
                .HasOne(r => r.RegimentsCharacter)
                .WithMany()
                .HasForeignKey(r => r.Regimentsleiter)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired(false);

            builder.Entity<Regiment>()
                .HasOne(r => r.AdjutantCharacter)
                .WithMany()
                .HasForeignKey(r => r.Adjutant)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired(false);

            // Regiment zu Infanterie - Required Beziehung
            builder.Entity<Regiment>()
                .HasOne(r => r.Infanterie)
                .WithMany(i => i.Regiments)
                .HasForeignKey(r => r.InfanterieId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Infanterie>()
                .HasOne(i => i.LeaderCharacter)
                .WithMany()
                .HasForeignKey(i => i.leader)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired(false);

            builder.Entity<Infanterie>()
                .HasOne(i => i.VertreterCharacter)
                .WithMany()
                .HasForeignKey(i => i.vertreter)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired(false);
        }
    }
}
