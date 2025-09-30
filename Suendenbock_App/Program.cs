using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Data.Seeders;
using Suendenbock_App.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// use UTF-8 as Standard Encoding
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// integrates Authentication with roles (RequireConfirmedAccount = false, no Mail needed), Registration offline
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() // Role Support
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddControllers(); // Für API-Controller hinzufügen


// addImageUpload 
builder.Services.AddScoped<IImageUploadService, ImageUploadService>();
// add Cache Service
builder.Services.AddScoped<ICachedDataService, CachedDataService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    await CreateRole(roleManager, "Administrator");
    await CreateDefaultUser(userManager, "Administrator", "gott@suendenbock.lore", "Adrijan1618!");
}
async Task CreateRole(RoleManager<IdentityRole> roleManager, string roleName)
{
    // exists the used role
    if(!await roleManager.RoleExistsAsync(roleName))
    {
        // then new administrator role
        await roleManager.CreateAsync(new IdentityRole(roleName));
    }
}

async Task CreateDefaultUser(UserManager<IdentityUser> userManager, string roleName, string userName, string password)
{
    var user = await userManager.FindByNameAsync(userName);
    if (user == null)
    {
        var newUser = new IdentityUser()
        {
            UserName = userName,
            Email = userName
        };

        var result = await userManager.CreateAsync(newUser, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newUser, roleName);
        }
    }
}

DatabaseSeeder.Seed(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers(); // Für API-Controller hinzufügen
app.MapRazorPages();

app.Run();
