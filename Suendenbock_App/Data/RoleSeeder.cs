using Microsoft.AspNetCore.Identity;

namespace Suendenbock_App.Data
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // Admin-Rolle
            if(!await roleManager.RoleExistsAsync("Gott"))
            {
                await roleManager.CreateAsync(new IdentityRole("Gott"));
            }

            // Spieler-Rolle (volle Spielrechte)
            if(!await roleManager.RoleExistsAsync("Spieler"))
            {
                await roleManager.CreateAsync(new IdentityRole("Spieler"));
            }

            // Moderator-Rolle (Spieler-Rechte + Moderations-Rechte)
            if(!await roleManager.RoleExistsAsync("Moderator"))
            {
                await roleManager.CreateAsync(new IdentityRole("Moderator"));
            }

            // Gast-Rolle (nur Lesezugriff, kein Spielmodus)
            if(!await roleManager.RoleExistsAsync("Gast"))
            {
                await roleManager.CreateAsync(new IdentityRole("Gast"));
            }
        }
    }
}
