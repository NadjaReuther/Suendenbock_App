using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;


// Connection Strings
var sourceConnectionString = "Server=NB2512\\SQLEXPRESS;Database=Goerdie;User Id=sa;Password=Red%Hat$1!15;TrustServerCertificate=true;Connection Timeout=30;";
var targetConnectionString = "Server=192.168.178.124,1433;Database=Goerdie;User Id=sa;Password=Red%Hat$1!15;TrustServerCertificate=true;Connection Timeout=30;";

Console.WriteLine("=== Suendenbock Datenübertragung ===");

try
{
    // Entity Framework Contexts für Many-to-Many Tabellen
    var sourceOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(sourceConnectionString).Options;
    var targetOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(targetConnectionString).Options;
    using var sourceContext = new ApplicationDbContext(sourceOptions);
    using var targetContext = new ApplicationDbContext(targetOptions);

    // Ziel-Datenbank leeren
    await ClearDatabase(targetConnectionString);

    Console.WriteLine("Übertrage Daten...");

    // Basis-Tabellen
    await TransferTable(sourceConnectionString, targetConnectionString, "LightCards");
    await TransferTable(sourceConnectionString, targetConnectionString, "Religions");
    await TransferTable(sourceConnectionString, targetConnectionString, "Staende");
    await TransferTable(sourceConnectionString, targetConnectionString, "Haeuser");
    await TransferTable(sourceConnectionString, targetConnectionString, "Blutgruppen");
    await TransferTable(sourceConnectionString, targetConnectionString, "Rassen");
    await TransferTable(sourceConnectionString, targetConnectionString, "Lebensstati");
    await TransferTable(sourceConnectionString, targetConnectionString, "Herkunftslaender");
    await TransferTable(sourceConnectionString, targetConnectionString, "Berufe");
    await TransferTable(sourceConnectionString, targetConnectionString, "Eindruecke");
    await TransferTable(sourceConnectionString, targetConnectionString, "Infanterieraenge");
    await TransferTable(sourceConnectionString, targetConnectionString, "Abenteuerraenge");
    await TransferTable(sourceConnectionString, targetConnectionString, "Anmeldungsstati");

    // Magic System
    await TransferTable(sourceConnectionString, targetConnectionString, "Obermagien");
    await TransferTable(sourceConnectionString, targetConnectionString, "MagicClasses");
    await TransferTable(sourceConnectionString, targetConnectionString, "MagicClassSpecializations");
    await TransferTable(sourceConnectionString, targetConnectionString, "Zaubertypen");

    // Organisationen
    await TransferTable(sourceConnectionString, targetConnectionString, "Guilds");
    await TransferTable(sourceConnectionString, targetConnectionString, "Infanterien");

    // Charaktere
    await TransferTable(sourceConnectionString, targetConnectionString, "Characters");
    await TransferTable(sourceConnectionString, targetConnectionString, "CharacterDetails");
    await TransferTable(sourceConnectionString, targetConnectionString, "CharacterAffiliations");

    // Many-to-Many
    await TransferManyToMany<CharacterMagicClass>(sourceContext, targetContext, "CharacterMagicClasses");

    // Zauber
    await TransferTable(sourceConnectionString, targetConnectionString, "Grundzauber");
    await TransferTable(sourceConnectionString, targetConnectionString, "SpecialZauber");

    Console.WriteLine("🎉 Transfer abgeschlossen!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Fehler: {ex.Message}");
}

Console.ReadKey();

// ====================== HILFSMETHODEN ======================

static async Task ClearDatabase(string connectionString)
{
    using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();

    var disableFk = new SqlCommand("EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'", connection);
    await disableFk.ExecuteNonQueryAsync();

    string[] tables = {
        "SpecialZauber", "Grundzauber", "CharacterMagicClasses", "CharacterAffiliations", "CharacterDetails",
        "Characters", "Guilds", "Infanterien", "MagicClassSpecializations", "MagicClasses", "Obermagien",
        "Zaubertypen", "Anmeldungsstati", "Abenteuerraenge", "Infanterieraenge", "Eindruecke", "Berufe",
        "Herkunftslaender", "Lebensstati", "Rassen", "Blutgruppen", "Haeuser", "Staende", "Religions", "LightCards"
    };

    foreach (var table in tables)
    {
        var deleteCmd = new SqlCommand($"DELETE FROM {table}", connection);
        await deleteCmd.ExecuteNonQueryAsync();
    }

    var enableFk = new SqlCommand("EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'", connection);
    await enableFk.ExecuteNonQueryAsync();
}

static async Task TransferTable(string sourceConnectionString, string targetConnectionString, string tableName)
{
    Console.Write($"Übertrage {tableName}... ");

    using var sourceConnection = new SqlConnection(sourceConnectionString);
    using var targetConnection = new SqlConnection(targetConnectionString);

    await sourceConnection.OpenAsync();
    await targetConnection.OpenAsync();

    using var transaction = targetConnection.BeginTransaction();

    try
    {
        var sourceCmd = new SqlCommand($"SELECT * FROM {tableName}", sourceConnection);
        using var reader = await sourceCmd.ExecuteReaderAsync();

        if (!reader.HasRows)
        {
            Console.WriteLine("✓ Keine Daten");
            return;
        }

        var schemaTable = reader.GetSchemaTable();
        var columns = new List<string>();
        var hasIdentity = false;

        foreach (System.Data.DataRow row in schemaTable.Rows)
        {
            columns.Add(row["ColumnName"].ToString());
            if ((bool)row["IsIdentity"]) hasIdentity = true;
        }

        if (hasIdentity)
        {
            var identityOnCmd = new SqlCommand($"SET IDENTITY_INSERT {tableName} ON", targetConnection, transaction);
            await identityOnCmd.ExecuteNonQueryAsync();
        }

        var insertedCount = 0;
        var columnList = string.Join(",", columns.Select(c => $"[{c}]"));

        while (await reader.ReadAsync())
        {
            var values = new List<string>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var value = reader.GetValue(i);
                if (value == null || value == DBNull.Value)
                {
                    values.Add("NULL");
                }
                else if (value is string stringValue)
                {
                    values.Add($"'{stringValue.Replace("'", "''")}'");
                }
                else if (value is DateTime dateTime)
                {
                    values.Add($"'{dateTime:yyyy-MM-dd HH:mm:ss.fff}'");
                }
                else if (value is bool boolValue)
                {
                    values.Add(boolValue ? "1" : "0");
                }
                else if (value is decimal || value is double || value is float)
                {
                    values.Add(value.ToString().Replace(",", "."));
                }
                else
                {
                    values.Add(value.ToString());
                }
            }

            var valueList = string.Join(",", values);
            var insertSql = $"INSERT INTO {tableName} ({columnList}) VALUES ({valueList})";

            var insertCmd = new SqlCommand(insertSql, targetConnection, transaction);
            await insertCmd.ExecuteNonQueryAsync();
            insertedCount++;
        }

        if (hasIdentity)
        {
            var identityOffCmd = new SqlCommand($"SET IDENTITY_INSERT {tableName} OFF", targetConnection, transaction);
            await identityOffCmd.ExecuteNonQueryAsync();
        }

        transaction.Commit();
        Console.WriteLine($"✓ {insertedCount}");
    }
    catch (Exception)
    {
        transaction.Rollback();
        throw;
    }
}

static async Task TransferManyToMany<T>(ApplicationDbContext source, ApplicationDbContext target, string tableName) where T : class
{
    Console.Write($"Übertrage {tableName}... ");

    var sourceData = await source.Set<T>().AsNoTracking().ToListAsync();
    if (!sourceData.Any())
    {
        Console.WriteLine("✓ Keine Daten");
        return;
    }

    using var transaction = await target.Database.BeginTransactionAsync();

    try
    {
        await target.Set<T>().AddRangeAsync(sourceData);
        await target.SaveChangesAsync();
        await transaction.CommitAsync();

        Console.WriteLine($"✓ {sourceData.Count}");
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        throw;
    }
}