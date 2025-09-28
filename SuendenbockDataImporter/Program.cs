using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace SuendenbockDataImporter
{
    // ===============================
    // VEREINFACHTE MODELS (nur Characters)
    // ===============================

    public class ImportCharacter
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

    public class ImportCharacterDetails
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public string? Quote { get; set; }
        public string? Urheber { get; set; }
        public string? Description { get; set; }
        public string? ProcessedDescription { get; set; }
        public string? Beruf { get; set; }
        public int? BodyHeight { get; set; }
        public int? StandId { get; set; }
        public int? BlutgruppeId { get; set; }
        public int? HausId { get; set; }
        public int? HerkunftslandId { get; set; }
    }

    public class ImportCharacterAffiliation
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public int? GuildId { get; set; }
        public int? RegimentId { get; set; }
        public int? InfanterierangId { get; set; }
        public int? ReligionId { get; set; }
    }

    // ===============================
    // VEREINFACHTE DBCONTEXT
    // ===============================

    public class ImportDbContext : DbContext
    {
        public ImportDbContext(DbContextOptions<ImportDbContext> options) : base(options) { }

        public DbSet<ImportCharacter> Characters { get; set; }
        public DbSet<ImportCharacterDetails> CharacterDetails { get; set; }
        public DbSet<ImportCharacterAffiliation> CharacterAffiliations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Table Names mapping
            modelBuilder.Entity<ImportCharacter>().ToTable("Characters");
            modelBuilder.Entity<ImportCharacterDetails>().ToTable("CharacterDetails");
            modelBuilder.Entity<ImportCharacterAffiliation>().ToTable("CharacterAffiliations");

            // IDs werden automatisch generiert (Standard-Verhalten)
            // KEINE ValueGeneratedNever() Konfiguration
        }
    }

    // ===============================
    // DATA IMPORTER (nur Characters)
    // ===============================

    public class CharacterDataImporter
    {
        private readonly ImportDbContext _context;

        public CharacterDataImporter(ImportDbContext context)
        {
            _context = context;
        }

        public async Task<ImportResult> ImportFromJsonFileAsync(string filePath)
        {
            var result = new ImportResult();

            try
            {
                Console.WriteLine($"📁 Lade JSON-Datei: {Path.GetFileName(filePath)}");
                string jsonContent = await File.ReadAllTextAsync(filePath);

                // Character-Daten importieren
                await ProcessJsonContentCharacters(jsonContent, result, Path.GetFileName(filePath));

                if (result.SuccessCount > 0)
                {
                    Console.WriteLine("💾 Speichere Änderungen in Datenbank...");
                    try
                    {
                        await _context.SaveChangesAsync();
                        Console.WriteLine("✅ Änderungen erfolgreich gespeichert!");
                    }
                    catch (Exception saveEx)
                    {
                        var errorMessage = GetDetailedErrorMessage(saveEx);
                        result.AddError($"Datenbankfehler: {errorMessage}");
                        Console.WriteLine($"❌ Fehler beim Speichern: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = GetDetailedErrorMessage(ex);
                result.AddError($"Unerwarteter Fehler: {errorMessage}");
                Console.WriteLine($"❌ Fehler: {errorMessage}");
            }

            return result;
        }

        private async Task ProcessJsonContentCharacters(string jsonContent, ImportResult result, string fileName)
        {
            if (fileName.ToLower().Contains("character") && !fileName.ToLower().Contains("detail") && !fileName.ToLower().Contains("affiliation"))
            {
                Console.WriteLine("👤 Erkannt: Character-Basis-Daten");
                await ImportCharactersSimpleAsync(jsonContent, result);
            }
            else if (fileName.ToLower().Contains("characterdetails"))
            {
                Console.WriteLine("📝 Erkannt: Character-Details");
                await ImportCharacterDetailsSimpleAsync(jsonContent, result);
            }
            else if (fileName.ToLower().Contains("characteraffiliation"))
            {
                Console.WriteLine("🏛️ Erkannt: Character-Zugehörigkeiten");
                await ImportCharacterAffiliationsSimpleAsync(jsonContent, result);
            }
            else
            {
                result.AddWarning($"Datei '{fileName}' wird übersprungen - nur Character-Daten werden importiert");
            }
        }

        private async Task ImportCharactersSimpleAsync(string jsonContent, ImportResult result)
        {
            try
            {
                using var document = JsonDocument.Parse(jsonContent);

                foreach (var element in document.RootElement.EnumerateArray())
                {
                    var vorname = element.TryGetProperty("vorname", out var vornameProp) ? vornameProp.GetString() ?? "" : "";
                    var nachname = element.TryGetProperty("nachname", out var nachnameProp) ? nachnameProp.GetString() ?? "" : "";

                    if (string.IsNullOrEmpty(vorname) && string.IsNullOrEmpty(nachname))
                    {
                        result.AddWarning("Character ohne Vor- und Nachname übersprungen");
                        continue;
                    }

                    // Prüfen ob bereits vorhanden (anhand Vor- und Nachname)
                    var existing = await _context.Characters.FirstOrDefaultAsync(c =>
                        c.Vorname == vorname && c.Nachname == nachname);
                    if (existing != null)
                    {
                        result.AddWarning($"Character '{vorname} {nachname}' existiert bereits - übersprungen");
                        continue;
                    }

                    // WICHTIG: Referenzen müssen existieren!
                    // Für Pflichtfelder nehmen wir Default-Werte
                    var character = new ImportCharacter
                    {
                        Vorname = vorname,
                        Nachname = nachname,
                        Rufname = element.TryGetProperty("rufname", out var rufnameProp) ? rufnameProp.GetString() ?? "" : "",
                        Geschlecht = element.TryGetProperty("geschlecht", out var geschlechtProp) ? geschlechtProp.GetString() ?? "" : "",
                        Geburtsdatum = element.TryGetProperty("geburtsdatum", out var gebProp) ? gebProp.GetString() : null,
                        ImagePath = element.TryGetProperty("imagePath", out var imgProp) ? imgProp.GetString() : null,
                        CompletionLevel = element.TryGetProperty("completionLevel", out var compProp) ? compProp.GetInt32() : 1,

                        // PFLICHT-REFERENZEN: Standard-Werte falls nicht vorhanden
                        RasseId = element.TryGetProperty("rasseId", out var rasseProp) ? rasseProp.GetInt32() : 1,
                        LebensstatusId = element.TryGetProperty("lebensstatusId", out var lebenProp) ? lebenProp.GetInt32() : 1,
                        EindruckId = element.TryGetProperty("eindruckId", out var eindruckProp) ? eindruckProp.GetInt32() : 1,

                        // OPTIONALE ELTERN-REFERENZEN: Vorerst null (müssen später verknüpft werden)
                        VaterId = null, // element.TryGetProperty("vaterId", out var vaterProp) ? vaterProp.GetInt32() : null,
                        MutterId = null // element.TryGetProperty("mutterId", out var mutterProp) ? mutterProp.GetInt32() : null
                    };

                    _context.Characters.Add(character);
                    result.AddSuccess($"Character '{vorname} {nachname}' importiert (ohne Eltern-Verknüpfungen)");
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Fehler beim Import der Characters: {GetDetailedErrorMessage(ex)}");
            }
        }

        private async Task ImportCharacterDetailsSimpleAsync(string jsonContent, ImportResult result)
        {
            try
            {
                using var document = JsonDocument.Parse(jsonContent);

                foreach (var element in document.RootElement.EnumerateArray())
                {
                    // CharacterId ist Pflicht für Details
                    if (!element.TryGetProperty("characterId", out var charIdProp))
                    {
                        result.AddWarning("CharacterDetails ohne CharacterId übersprungen");
                        continue;
                    }

                    var characterId = charIdProp.GetInt32();

                    // Prüfen ob Character existiert
                    var characterExists = await _context.Characters.AnyAsync(c => c.Id == characterId);
                    if (!characterExists)
                    {
                        result.AddWarning($"Character mit ID {characterId} nicht gefunden - Details übersprungen");
                        continue;
                    }

                    // Prüfen ob Details bereits existieren
                    var existing = await _context.CharacterDetails.FirstOrDefaultAsync(cd => cd.CharacterId == characterId);
                    if (existing != null)
                    {
                        result.AddWarning($"CharacterDetails für Character {characterId} existieren bereits - übersprungen");
                        continue;
                    }

                    var details = new ImportCharacterDetails
                    {
                        CharacterId = characterId,
                        Quote = element.TryGetProperty("quote", out var quoteProp) ? quoteProp.GetString() : null,
                        Urheber = element.TryGetProperty("urheber", out var urheberProp) ? urheberProp.GetString() : null,
                        Description = element.TryGetProperty("description", out var descProp) ? descProp.GetString() : null,
                        ProcessedDescription = element.TryGetProperty("processedDescription", out var procProp) ? procProp.GetString() : null,
                        Beruf = element.TryGetProperty("beruf", out var berufProp) ? berufProp.GetString() : null,
                        BodyHeight = element.TryGetProperty("bodyHeight", out var heightProp) ? heightProp.GetInt32() : null,

                        // OPTIONALE REFERENZEN: Null lassen wenn nicht vorhanden
                        StandId = element.TryGetProperty("standId", out var standProp) ? standProp.GetInt32() : null,
                        BlutgruppeId = element.TryGetProperty("blutgruppeId", out var blutProp) ? blutProp.GetInt32() : null,
                        HausId = element.TryGetProperty("hausId", out var hausProp) ? hausProp.GetInt32() : null,
                        HerkunftslandId = element.TryGetProperty("herkunftslandId", out var herkunftProp) ? herkunftProp.GetInt32() : null
                    };

                    _context.CharacterDetails.Add(details);
                    result.AddSuccess($"CharacterDetails für Character {characterId} importiert");
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Fehler beim Import der CharacterDetails: {GetDetailedErrorMessage(ex)}");
            }
        }

        private async Task ImportCharacterAffiliationsSimpleAsync(string jsonContent, ImportResult result)
        {
            try
            {
                using var document = JsonDocument.Parse(jsonContent);

                foreach (var element in document.RootElement.EnumerateArray())
                {
                    // CharacterId ist Pflicht für Affiliations
                    if (!element.TryGetProperty("characterId", out var charIdProp))
                    {
                        result.AddWarning("CharacterAffiliation ohne CharacterId übersprungen");
                        continue;
                    }

                    var characterId = charIdProp.GetInt32();

                    // Prüfen ob Character existiert
                    var characterExists = await _context.Characters.AnyAsync(c => c.Id == characterId);
                    if (!characterExists)
                    {
                        result.AddWarning($"Character mit ID {characterId} nicht gefunden - Affiliation übersprungen");
                        continue;
                    }

                    // Prüfen ob Affiliation bereits existiert
                    var existing = await _context.CharacterAffiliations.FirstOrDefaultAsync(ca => ca.CharacterId == characterId);
                    if (existing != null)
                    {
                        result.AddWarning($"CharacterAffiliation für Character {characterId} existiert bereits - übersprungen");
                        continue;
                    }

                    var affiliation = new ImportCharacterAffiliation
                    {
                        CharacterId = characterId,

                        // ALLE ZUGEHÖRIGKEITEN SIND OPTIONAL
                        GuildId = element.TryGetProperty("guildId", out var guildProp) ? guildProp.GetInt32() : null,
                        RegimentId = element.TryGetProperty("regimentId", out var regimentProp) ? regimentProp.GetInt32() : null,
                        InfanterierangId = element.TryGetProperty("infanterierangId", out var infRangProp) ? infRangProp.GetInt32() : null,
                        ReligionId = element.TryGetProperty("religionId", out var religionProp) ? religionProp.GetInt32() : null
                    };

                    _context.CharacterAffiliations.Add(affiliation);
                    result.AddSuccess($"CharacterAffiliation für Character {characterId} importiert");
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Fehler beim Import der CharacterAffiliations: {GetDetailedErrorMessage(ex)}");
            }
        }

        private string GetDetailedErrorMessage(Exception ex)
        {
            var messages = new List<string>();
            var current = ex;

            while (current != null)
            {
                if (!string.IsNullOrEmpty(current.Message) && !messages.Contains(current.Message))
                {
                    messages.Add(current.Message);
                }
                current = current.InnerException;
            }

            return string.Join(" → ", messages);
        }
    }

    // ===============================
    // IMPORT RESULT
    // ===============================

    public class ImportResult
    {
        private readonly List<string> _errors = new();
        private readonly List<string> _warnings = new();
        private readonly List<string> _successes = new();

        public IReadOnlyList<string> Errors => _errors.AsReadOnly();
        public IReadOnlyList<string> Warnings => _warnings.AsReadOnly();
        public IReadOnlyList<string> Successes => _successes.AsReadOnly();

        public int ErrorCount => _errors.Count;
        public int WarningCount => _warnings.Count;
        public int SuccessCount => _successes.Count;

        public bool HasErrors => _errors.Any();
        public bool IsSuccess => !HasErrors;

        public void AddError(string message) => _errors.Add(message);
        public void AddWarning(string message) => _warnings.Add(message);
        public void AddSuccess(string message) => _successes.Add(message);
    }

    // ===============================
    // MAIN PROGRAM
    // ===============================

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("⬆️ Character Data Import Tool");
            Console.WriteLine("===============================================");
            Console.WriteLine();

            var exportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SuendenbockDataExport");

            if (!Directory.Exists(exportPath))
            {
                Console.WriteLine($"❌ Export-Ordner wurde nicht gefunden: {exportPath}");
                Console.WriteLine("Stelle sicher, dass der SuendenbockDataExporter zuerst ausgeführt wurde.");
                Console.WriteLine();
                Console.WriteLine("Drücken Sie eine Taste zum Beenden...");
                Console.ReadKey();
                return;
            }

            var connectionString = "Server=192.168.178.124,1433;Database=Goerdie;User ID=sa;Password=Red%Hat$1!15;TrustServerCertificate=True;";

            var options = new DbContextOptionsBuilder<ImportDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            try
            {
                using var context = new ImportDbContext(options);

                Console.WriteLine("🔌 Teste Datenbankverbindung...");
                var canConnect = await context.Database.CanConnectAsync();
                if (!canConnect)
                {
                    Console.WriteLine("❌ Kann nicht zur Datenbank verbinden!");
                    Console.WriteLine("Drücken Sie eine Taste zum Beenden...");
                    Console.ReadKey();
                    return;
                }
                Console.WriteLine("✅ Datenbankverbindung erfolgreich!");
                Console.WriteLine();

                // NUR die Character-Dateien importieren
                var characterFiles = new[] { "characters.json", "characterdetails.json", "characteraffiliations.json" };

                var importer = new CharacterDataImporter(context);
                var totalResult = new ImportResult();

                Console.WriteLine("🚀 Starte Character-Import...");
                Console.WriteLine();

                foreach (var fileName in characterFiles)
                {
                    var filePath = Path.Combine(exportPath, fileName);

                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine($"⏭️ Datei {fileName} nicht gefunden - übersprungen");
                        continue;
                    }

                    Console.WriteLine($"📂 Importiere {fileName}...");
                    var result = await importer.ImportFromJsonFileAsync(filePath);

                    foreach (var success in result.Successes) totalResult.AddSuccess(success);
                    foreach (var warning in result.Warnings) totalResult.AddWarning(warning);
                    foreach (var error in result.Errors) totalResult.AddError(error);

                    if (result.HasErrors)
                    {
                        Console.WriteLine($"❌ Fehler beim Import von {fileName}");
                    }
                    else
                    {
                        Console.WriteLine($"✅ {fileName} erfolgreich importiert ({result.SuccessCount} Datensätze)");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("===============================");
                Console.WriteLine("📊 Gesamte Import-Ergebnisse:");
                Console.WriteLine($"✅ Erfolgreich: {totalResult.SuccessCount}");
                Console.WriteLine($"⚠️ Warnungen: {totalResult.WarningCount}");
                Console.WriteLine($"❌ Fehler: {totalResult.ErrorCount}");

                if (totalResult.Warnings.Any())
                {
                    Console.WriteLine();
                    Console.WriteLine("⚠️ Warnungen:");
                    foreach (var warning in totalResult.Warnings)
                    {
                        Console.WriteLine($"   • {warning}");
                    }
                }

                if (totalResult.Errors.Any())
                {
                    Console.WriteLine();
                    Console.WriteLine("❌ Fehler:");
                    foreach (var error in totalResult.Errors)
                    {
                        Console.WriteLine($"   • {error}");
                    }
                }

                Console.WriteLine();
                if (totalResult.IsSuccess)
                {
                    Console.WriteLine("🎉 Character-Import erfolgreich!");
                    Console.WriteLine("📝 Hinweis: Eltern-Verknüpfungen (VaterId/MutterId) wurden übersprungen.");
                    Console.WriteLine("   Diese müssen nach dem Import manuell verknüpft werden.");
                }
                else
                {
                    Console.WriteLine("⚠️ Import mit Fehlern abgeschlossen.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unerwarteter Fehler: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Drücken Sie eine Taste zum Beenden...");
            Console.ReadKey();
        }
    }
}