using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace SuendenbockDataExporter
{
    // ===============================
    // VEREINFACHTE MODELS (nur für Export)
    // ===============================

    public class ExportCharacter
    {
        public int Id { get; set; }
        public string Vorname { get; set; } = string.Empty;
        public string Nachname { get; set; } = string.Empty;
        public string Rufname { get; set; } = string.Empty;
        public string Geschlecht { get; set; } = string.Empty;
        public string? Geburtsdatum { get; set; }
        public string? ImagePath { get; set; }
        public int CompletionLevel { get; set; }
        public int RasseId { get; set; }
        public int LebensstatusId { get; set; }
        public int EindruckId { get; set; }
        public int? VaterId { get; set; }
        public int? MutterId { get; set; }
    }

    public class ExportCharacterDetails
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public string? quote { get; set; }
        public string? urheber { get; set; }
        public string? Description { get; set; }
        public string? ProcessedDescription { get; set; }
        public string? Beruf { get; set; }
        public int? BodyHeight { get; set; }
        public int? StandId { get; set; }
        public int? BlutgruppeId { get; set; }
        public int? HausId { get; set; }
        public int? HerkunftslandId { get; set; }
    }

    public class ExportCharacterAffiliation
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public int? GuildId { get; set; }
        public int? RegimentId { get; set; }
        public int? InfanterierangId { get; set; }
        public int? ReligionId { get; set; }
    }

    public class ExportCharacterMagicClass
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public int MagicClassId { get; set; }
        public int? MagicClassSpecializationId { get; set; }
    }

    public class ExportMagicClass
    {
        public int Id { get; set; }
        public string Bezeichnung { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public int ObermagieId { get; set; }
    }

    public class ExportMagicClassSpecialization
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int MagicClassId { get; set; }
    }

    public class ExportGuild
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ProcessedDescription { get; set; }
        public string? quote { get; set; }
        public string? urheber { get; set; }
        public int LightCardId { get; set; }
        public int AbenteuerrangId { get; set; }
        public int AnmeldungsstatusId { get; set; }
        public int? LeaderId { get; set; }
        public int? VertreterId { get; set; }
    }

    public class ExportInfanterie
    {
        public int Id { get; set; }
        public string Bezeichnung { get; set; } = string.Empty;
        public string? description { get; set; }
        public string? ProcessedDescription { get; set; }
        public string? ImagePath { get; set; }
        public string Sitz { get; set; } = string.Empty;
        public int? LightCardId { get; set; }
        public int? LeaderId { get; set; }
        public int? VertreterId { get; set; }
    }

    public class ExportRegiment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ProcessedDescription { get; set; }
        public int? RegimentsleiterId { get; set; }
        public int? AdjutantId { get; set; }
        public int InfanterieId { get; set; }
    }

    // Separate Lookup Models - viel klarer!
    public class ExportRasse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ExportLebensstatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ExportEindruck
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ExportStand
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ExportBlutgruppe
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ExportHaus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
    }

    public class ExportHerkunftsland
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ExportReligion
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ExportInfanterierang
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ExportAbenteuerrang
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ExportAnmeldungsstatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ExportLightCard
    {
        public int Id { get; set; }
        public string Bezeichnung { get; set; } = string.Empty;
        public string CssClass { get; set; } = string.Empty;
        public string Farbcode { get; set; } = string.Empty;
    }

    public class ExportLizenz
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ExportObermagie
    {
        public int Id { get; set; }
        public string Bezeichnung { get; set; } = string.Empty;
        public int LightCardId { get; set; }
    }

    public class ExportZaubertyp
    {
        public int Id { get; set; }
        public string Bezeichnung { get; set; } = string.Empty;
    }

    public class ExportGrundzauber
    {
        public int Id { get; set; }
        public string Spruch { get; set; } = string.Empty;
        public string Wirkung { get; set; } = string.Empty;
        public int Stufe { get; set; }
        public int Slots { get; set; }
        public string Effekt { get; set; } = string.Empty;
        public int ZaubertypID { get; set; }
        public int MagicClassId { get; set; }
    }

    public class ExportSpecialZauber
    {
        public int Id { get; set; }
        public string Spruch { get; set; } = string.Empty;
        public string Wirkung { get; set; } = string.Empty;
        public int Stufe { get; set; }
        public int Slots { get; set; }
        public string Effekt { get; set; } = string.Empty;
        public int ZaubertypID { get; set; }
        public int MagicClassSpecializationId { get; set; }
    }

    public class ExportGildenlizenz
    {
        public int Id { get; set; }
        public int GuildId { get; set; }
        public int LizenzenId { get; set; }
    }

    // ===============================
    // VEREINFACHTE DBCONTEXT
    // ===============================

    public class ExportDbContext : DbContext
    {
        public ExportDbContext(DbContextOptions<ExportDbContext> options) : base(options) { }

        // Haupttabellen
        public DbSet<ExportCharacter> Characters { get; set; }
        public DbSet<ExportCharacterDetails> CharacterDetails { get; set; }
        public DbSet<ExportCharacterAffiliation> CharacterAffiliations { get; set; }
        public DbSet<ExportCharacterMagicClass> CharacterMagicClasses { get; set; }

        // Magie-System
        public DbSet<ExportMagicClass> MagicClasses { get; set; }
        public DbSet<ExportMagicClassSpecialization> MagicClassSpecializations { get; set; }

        // Organisationen
        public DbSet<ExportGuild> Guilds { get; set; }
        public DbSet<ExportInfanterie> Infanterien { get; set; }
        public DbSet<ExportRegiment> Regiments { get; set; }

        // Lookup-Tabellen (alle separat)
        public DbSet<ExportRasse> Rassen { get; set; }
        public DbSet<ExportLebensstatus> Lebensstati { get; set; }
        public DbSet<ExportEindruck> Eindruecke { get; set; }
        public DbSet<ExportStand> Staende { get; set; }
        public DbSet<ExportBlutgruppe> Blutgruppen { get; set; }
        public DbSet<ExportHaus> Haeuser { get; set; }
        public DbSet<ExportHerkunftsland> Herkunftslaender { get; set; }
        public DbSet<ExportReligion> Religions { get; set; }
        public DbSet<ExportInfanterierang> Infanterieraenge { get; set; }
        public DbSet<ExportAbenteuerrang> Abenteuerraenge { get; set; }
        public DbSet<ExportAnmeldungsstatus> Anmeldungsstati { get; set; }
        public DbSet<ExportLightCard> LightCards { get; set; }
        public DbSet<ExportLizenz> Lizenzen { get; set; }

        // Weitere Tabellen
        public DbSet<ExportObermagie> Obermagien { get; set; }
        public DbSet<ExportZaubertyp> Zaubertypen { get; set; }
        public DbSet<ExportGrundzauber> Grundzauber { get; set; }
        public DbSet<ExportSpecialZauber> SpecialZauber { get; set; }
        public DbSet<ExportGildenlizenz> Gildenlizenzen { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Table Names mapping zu deiner echten DB
            modelBuilder.Entity<ExportCharacter>().ToTable("Characters");
            modelBuilder.Entity<ExportCharacterDetails>().ToTable("CharacterDetails");
            modelBuilder.Entity<ExportCharacterAffiliation>().ToTable("CharacterAffiliations");
            modelBuilder.Entity<ExportCharacterMagicClass>().ToTable("CharacterMagicClasses");
            modelBuilder.Entity<ExportMagicClass>().ToTable("MagicClasses");
            modelBuilder.Entity<ExportMagicClassSpecialization>().ToTable("MagicClassSpecializations");
            modelBuilder.Entity<ExportGuild>().ToTable("Guilds");
            modelBuilder.Entity<ExportInfanterie>().ToTable("Infanterien");
            modelBuilder.Entity<ExportRegiment>().ToTable("Regiments");

            // Lookup Tables - alle separat
            modelBuilder.Entity<ExportRasse>().ToTable("Rassen");
            modelBuilder.Entity<ExportLebensstatus>().ToTable("Lebensstati");
            modelBuilder.Entity<ExportEindruck>().ToTable("Eindruecke");
            modelBuilder.Entity<ExportStand>().ToTable("Staende");
            modelBuilder.Entity<ExportBlutgruppe>().ToTable("Blutgruppen");
            modelBuilder.Entity<ExportHaus>().ToTable("Haeuser");
            modelBuilder.Entity<ExportHerkunftsland>().ToTable("Herkunftslaender");
            modelBuilder.Entity<ExportReligion>().ToTable("Religions");
            modelBuilder.Entity<ExportInfanterierang>().ToTable("Infanterieraenge");
            modelBuilder.Entity<ExportAbenteuerrang>().ToTable("Abenteuerraenge");
            modelBuilder.Entity<ExportAnmeldungsstatus>().ToTable("Anmeldungsstati");
            modelBuilder.Entity<ExportLightCard>().ToTable("LightCards");
            modelBuilder.Entity<ExportLizenz>().ToTable("Lizenzen");

            // Weitere Tabellen
            modelBuilder.Entity<ExportObermagie>().ToTable("Obermagien");
            modelBuilder.Entity<ExportZaubertyp>().ToTable("Zaubertypen");
            modelBuilder.Entity<ExportGrundzauber>().ToTable("Grundzauber");
            modelBuilder.Entity<ExportSpecialZauber>().ToTable("SpecialZauber");
            modelBuilder.Entity<ExportGildenlizenz>().ToTable("Gildenlizenzen");
        }
    }

    // ===============================
    // DATA EXPORTER
    // ===============================

    public class SimpleDataExporter
    {
        private readonly ExportDbContext _context;
        private readonly string _exportPath;

        public SimpleDataExporter(ExportDbContext context, string exportPath)
        {
            _context = context;
            _exportPath = exportPath;
        }

        public async Task ExportAllData()
        {
            Console.WriteLine("🚀 Starting simplified data export...");
            Directory.CreateDirectory(_exportPath);

            // Haupttabellen
            await ExportTable("characters", _context.Characters);
            await ExportTable("character_details", _context.CharacterDetails);
            await ExportTable("character_affiliations", _context.CharacterAffiliations);
            await ExportTable("character_magic_classes", _context.CharacterMagicClasses);

            // Magie-System
            await ExportTable("magic_classes", _context.MagicClasses);
            await ExportTable("magic_specializations", _context.MagicClassSpecializations);
            await ExportTable("obermagien", _context.Obermagien);
            await ExportTable("lightcards", _context.LightCards);
            await ExportTable("zaubertypen", _context.Zaubertypen);
            await ExportTable("grundzauber", _context.Grundzauber);
            await ExportTable("spezialzauber", _context.SpecialZauber);

            // Organisationen
            await ExportTable("guilds", _context.Guilds);
            await ExportTable("infanterien", _context.Infanterien);
            await ExportTable("regiments", _context.Regiments);
            await ExportTable("gildenlizenzen", _context.Gildenlizenzen);

            // Lookup Tables
            await ExportTable("rassen", _context.Rassen);
            await ExportTable("lebensstati", _context.Lebensstati);
            await ExportTable("eindruecke", _context.Eindruecke);
            await ExportTable("staende", _context.Staende);
            await ExportTable("blutgruppen", _context.Blutgruppen);
            await ExportTable("haeuser", _context.Haeuser);
            await ExportTable("herkunftslaender", _context.Herkunftslaender);
            await ExportTable("religions", _context.Religions);
            await ExportTable("infanterieraenge", _context.Infanterieraenge);
            await ExportTable("abenteuerraenge", _context.Abenteuerraenge);
            await ExportTable("anmeldungsstati", _context.Anmeldungsstati);
            await ExportTable("lizenzen", _context.Lizenzen);

            Console.WriteLine("✅ Simple data export completed!");
            Console.WriteLine($"📁 Files saved to: {_exportPath}");
        }

        private async Task ExportTable<T>(string filename, DbSet<T> table) where T : class
        {
            Console.WriteLine($"📊 Exporting {filename}...");
            try
            {
                var data = await table.ToListAsync();
                await SaveAsJson($"{filename}.json", data);
                Console.WriteLine($"   ✅ {data.Count} records exported");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Error exporting {filename}: {ex.Message}");
            }
        }

        private async Task SaveAsJson(string filename, object data)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(data, options);
            var filepath = Path.Combine(_exportPath, filename);
            await File.WriteAllTextAsync(filepath, json);
        }
    }

    // ===============================
    // MAIN PROGRAM
    // ===============================

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🎯 Suendenbock Data Export Tool - Simplified Version");
            Console.WriteLine("====================================================");

            // ⚠️ WICHTIG: Passe deinen Connection String hier an!
            var connectionString = "Server=192.168.178.124,1433;Database=Goerdie;User ID=sa;Password=Red%Hat$1!15;TrustServerCertificate=True;";

            var options = new DbContextOptionsBuilder<ExportDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            try
            {
                using var context = new ExportDbContext(options);

                // Test DB Connection
                Console.WriteLine("🔌 Testing database connection...");
                var canConnect = await context.Database.CanConnectAsync();
                if (!canConnect)
                {
                    Console.WriteLine("❌ Cannot connect to database! Check your connection string.");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    return;
                }
                Console.WriteLine("✅ Database connection successful!");

                // Export Path
                var exportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SuendenbockDataExport");

                var exporter = new SimpleDataExporter(context, exportPath);
                await exporter.ExportAllData();

                Console.WriteLine();
                Console.WriteLine("🎉 Export completed successfully!");
                Console.WriteLine($"📁 Check your files at: {exportPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine($"Details: {ex.InnerException?.Message}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}