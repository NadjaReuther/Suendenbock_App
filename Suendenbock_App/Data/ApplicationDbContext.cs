using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models;

namespace Suendenbock_App.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<MagicClassModel> MagicClasses { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
