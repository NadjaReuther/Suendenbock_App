using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<MagicClass> MagicClasses { get; set; }
        public DbSet<Guild> Guilds { get; set; }
        public DbSet<LightCard> LightCards { get; set; }
        public DbSet<Religion> Religions { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterMagicClass> CharacterMagicClasses { get; set; }
        public DbSet<MagicClassSpecialization> MagicClassSpecializations { get; set; }
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
        public DbSet<Zauber> Zauber { get; set; }
        public DbSet<Zaubertyp> Zaubertypen { get; set; }
        public DbSet<Stand> Staende { get; set; }
        public DbSet<Rasse> Rassen { get; set; }
        public DbSet<Infanterie> Infanterien { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Define the MagicClassSpecialization relationship
            builder.Entity<MagicClassSpecialization>()
               .HasOne(mcs => mcs.MagicClass)
               .WithMany(mc => mc.MagicClassSpecializations)
               .HasForeignKey(mcs => mcs.MagicClassId);
            // Define the composite key for CharacterMagicClass
            builder.Entity<CharacterMagicClass>()
                .HasKey(cm => new { cm.CharacterId, cm.MagicClassId });
            // Define the relationships
            builder.Entity<CharacterMagicClass>()
                .HasOne(cm => cm.Character)
                .WithMany(c => c.CharacterMagicClasses)
                .HasForeignKey(cm => cm.CharacterId);
            builder.Entity<CharacterMagicClass>()
                .HasOne(cm => cm.MagicClass)
                .WithMany(m => m.CharacterMagicClasses)
                .HasForeignKey(cm => cm.MagicClassId);
            //Define the optional relationship for MagicClassSpecialization
            builder.Entity<CharacterMagicClass>()
                .HasOne(cm => cm.MagicClassSpecialization)
                .WithMany()
                .HasForeignKey(cm => cm.MagicClassSpecializationId)
                .IsRequired(false); // Make it optional
            // define the father relationship
            builder.Entity<Character>()
                .HasOne(c => c.Vater)
                .WithMany()
                .HasForeignKey(c => c.VaterId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
            // define the mother relationship
            builder.Entity<Character>()
                .HasOne(c => c.Mutter)
                .WithMany()
                .HasForeignKey(c => c.MutterId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
        }
    }
}
