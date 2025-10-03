using Microsoft.AspNetCore.Identity;

namespace Suendenbock_App.Data
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if(!await roleManager.RoleExistsAsync("Gott"))
            {
                await roleManager.CreateAsync(new IdentityRole("Gott"));
            }
            if(!await roleManager.RoleExistsAsync("Spieler"))
            {
                await roleManager.CreateAsync(new IdentityRole("Spieler"));
            }
        }
    }
}
