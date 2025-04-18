using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models;

namespace Suendenbock_App.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<MagicClassModel> MagicClasses { get; set; }
        public DbSet<GuildModel> Guilds { get; set; }
        public DbSet<LightCards> LightCards { get; set; }
        public DbSet<ReligionModel> Religions { get; set; }
        public DbSet<CharacterModel> Characters { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
