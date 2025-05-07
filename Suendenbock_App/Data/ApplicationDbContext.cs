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

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
