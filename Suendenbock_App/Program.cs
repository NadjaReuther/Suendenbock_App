using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Suendenbock_App.Data;
using Suendenbock_App.Data.Seeders;
using Suendenbock_App.Models.Domain;
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
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() // Role Support
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Anmeldung: 14 Tage gültig, jeder Besuch verlängert sie wieder.
// Wer 14 Tage nicht vorbeischaut, wird aus Sicherheitsgründen abgemeldet.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

builder.Services.AddControllersWithViews(options =>
{
    // Beim ersten Login nach dem echten Namen fragen
    options.Filters.Add<Suendenbock_App.Filters.RequireRealNameFilter>();
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddControllers() // F�r API-Controller hinzuf�gen
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });


// addImageUpload
builder.Services.AddScoped<IImageUploadService, ImageUploadService>();
// add Cache Service
builder.Services.AddScoped<ICachedDataService, CachedDataService>();
builder.Services.AddMemoryCache();
// Achievement Service
builder.Services.AddScoped<IAchievementService, AchievementService>();
// Game Service
builder.Services.AddScoped<GameService>();
// Push Notification Service
builder.Services.AddScoped<IPushNotificationService, PushNotificationService>();
// Tägliche Erinnerungen (Monatsbeitrag, später Termine)
builder.Services.AddHostedService<ReminderBackgroundService>();

// SignalR für Real-time Combat
builder.Services.AddSignalR(options =>
{
    // Timeout-Einstellungen für bessere Performance
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
    options.HandshakeTimeout = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);

    // Größere Message Size für Battle State (falls nötig)
    options.MaximumReceiveMessageSize = 1024 * 1024; // 1MB
});

var app = builder.Build();

// Statische Dateien für Angular-Build (ohne RequestPath = alle Dateien unter root).
// Nur einbinden, wenn der Build-Ordner existiert: PhysicalFileProvider wirft sonst beim Start
// eine DirectoryNotFoundException, und die ganze Seite startet nicht (z. B. im Docker-Container,
// wo ClientApp/dist nicht mit veröffentlicht wird).
var angularOrdner = Path.Combine(builder.Environment.ContentRootPath, "ClientApp", "dist", "client-app", "browser");
if (Directory.Exists(angularOrdner))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(angularOrdner),
        RequestPath = "" // Leerer Pfad = Dateien direkt unter root verfügbar
    });
}
    
using(var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(roleManager);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    await CreateDefaultUser(userManager, "Gott", "gott@suendenbock.lore", "Adrijan1618!");
}

async Task CreateDefaultUser(UserManager<ApplicationUser> userManager, string roleName, string userName, string password)
{
    var user = await userManager.FindByNameAsync(userName);
    if (user == null)
    {
        var newUser = new ApplicationUser()
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

// NEU: Trigger-System seeden
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    TriggerSeeder.Seed(context);
    AchievementSeeder.SeedAchievements(context);
    await GameDataSeeder.SeedGameDataAsync(context);
    await WeatherDataSeeder.SeedWeatherData(context);
}

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
// Skripte und Styles immer beim Server nachfragen (kostet nur ein 304), sonst behalten
// iPhones alte Fassungen, z. B. eine push-notifications.js ohne getSupportStatus.
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var name = ctx.File.Name;
        if (name.EndsWith(".js", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
        {
            ctx.Context.Response.Headers.CacheControl = "no-cache";
        }
    }
});

app.UseRouting();

app.UseAuthorization();

// Hesparis: eigene Webseite im selben Projekt unter /Hesparis (Areas/Hesparis).
// Bewusst NUR für diese eine Area: Eine allgemeine {area:exists}-Route würde auch
// "Identity" (Login-Seiten) treffen und Links wie /Identity/Community erzeugen.
app.MapAreaControllerRoute(
    name: "hesparis",
    areaName: "Hesparis",
    pattern: "Hesparis/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers(); // F�r API-Controller hinzuf�gen
app.MapRazorPages();

// SignalR Hub Endpoints
app.MapHub<Suendenbock_App.Hubs.BattleHub>("/battlehub");
app.MapHub<Suendenbock_App.Hubs.BattleHubV2>("/battlehubv2"); // Neues Kampfsystem
app.MapHub<Suendenbock_App.Hubs.GameHub>("/gamehub");

app.Run();
