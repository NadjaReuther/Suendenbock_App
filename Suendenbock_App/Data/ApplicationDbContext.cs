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
        public DbSet<MagicClassSpecialization> Specializations { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
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
