using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <summary>
    /// Entfernt das Tenant-System und legt die Hesparis-Charaktertabellen an.
    ///
    /// Die Datenbank war schon halb umgebaut (erster Schritt angewendet, Migration aber nicht im Projekt).
    /// Deshalb prüft jeder Befehl vorher, ob es das Objekt gibt bzw. noch nicht gibt.
    /// So läuft die Migration unabhängig davon, was schon erledigt ist.
    /// </summary>
    public partial class tenantsystementfernt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ------------------------------------------------------------------
            // 1. Altes Tenant-System entfernen (nur, was noch da ist)
            // ------------------------------------------------------------------
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[FK_AspNetUsers_Characters_CurrentCharacterId]', N'F') IS NOT NULL
    ALTER TABLE [AspNetUsers] DROP CONSTRAINT [FK_AspNetUsers_Characters_CurrentCharacterId];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AspNetUsers_CurrentCharacterId' AND object_id = OBJECT_ID(N'[AspNetUsers]'))
    DROP INDEX [IX_AspNetUsers_CurrentCharacterId] ON [AspNetUsers];
IF COL_LENGTH(N'AspNetUsers', N'CurrentCharacterId') IS NOT NULL
    ALTER TABLE [AspNetUsers] DROP COLUMN [CurrentCharacterId];

IF OBJECT_ID(N'[FK_Kampagnen_GameWorlds_GameWorldId]', N'F') IS NOT NULL
    ALTER TABLE [Kampagnen] DROP CONSTRAINT [FK_Kampagnen_GameWorlds_GameWorldId];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Kampagnen_GameWorldId' AND object_id = OBJECT_ID(N'[Kampagnen]'))
    DROP INDEX [IX_Kampagnen_GameWorldId] ON [Kampagnen];
IF COL_LENGTH(N'Kampagnen', N'GameWorldId') IS NOT NULL
    ALTER TABLE [Kampagnen] DROP COLUMN [GameWorldId];

IF OBJECT_ID(N'[FK_Spielfiguren_Characters_CharacterId]', N'F') IS NOT NULL
    ALTER TABLE [Spielfiguren] DROP CONSTRAINT [FK_Spielfiguren_Characters_CharacterId];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Spielfiguren_CharacterId' AND object_id = OBJECT_ID(N'[Spielfiguren]'))
    DROP INDEX [IX_Spielfiguren_CharacterId] ON [Spielfiguren];
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Spielfiguren_KampagneId_CharacterId' AND object_id = OBJECT_ID(N'[Spielfiguren]'))
    DROP INDEX [IX_Spielfiguren_KampagneId_CharacterId] ON [Spielfiguren];
IF COL_LENGTH(N'Spielfiguren', N'CharacterId') IS NOT NULL
    ALTER TABLE [Spielfiguren] DROP COLUMN [CharacterId];

IF OBJECT_ID(N'[CharacterGameWorlds]', N'U') IS NOT NULL
    DROP TABLE [CharacterGameWorlds];
IF OBJECT_ID(N'[GameWorlds]', N'U') IS NOT NULL
    DROP TABLE [GameWorlds];
");

            // ------------------------------------------------------------------
            // 2. Hesparis-Charaktere (nur anlegen, was noch fehlt)
            // ------------------------------------------------------------------
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[stamm].[Charakterarten]', N'U') IS NULL
BEGIN
    CREATE TABLE [stamm].[Charakterarten] (
        [Id] int NOT NULL,
        [Name] nvarchar(40) NOT NULL,
        CONSTRAINT [PK_Charakterarten] PRIMARY KEY ([Id])
    );
END;
IF NOT EXISTS (SELECT 1 FROM [stamm].[Charakterarten] WHERE [Id] = 1) INSERT INTO [stamm].[Charakterarten] ([Id], [Name]) VALUES (1, N'Spieler');
IF NOT EXISTS (SELECT 1 FROM [stamm].[Charakterarten] WHERE [Id] = 2) INSERT INTO [stamm].[Charakterarten] ([Id], [Name]) VALUES (2, N'Lehrkraft');
IF NOT EXISTS (SELECT 1 FROM [stamm].[Charakterarten] WHERE [Id] = 3) INSERT INTO [stamm].[Charakterarten] ([Id], [Name]) VALUES (3, N'Mitschüler');

IF OBJECT_ID(N'[HesparisCharaktere]', N'U') IS NULL
BEGIN
    CREATE TABLE [HesparisCharaktere] (
        [Id] int NOT NULL IDENTITY(1, 1),
        [Vorname] nvarchar(100) NOT NULL,
        [Nachname] nvarchar(100) NULL,
        [Rufname] nvarchar(100) NULL,
        [BildPfad] nvarchar(400) NULL,
        [CharakterartId] int NOT NULL,
        [UserId] nvarchar(450) NULL,
        CONSTRAINT [PK_HesparisCharaktere] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HesparisCharaktere_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]),
        CONSTRAINT [FK_HesparisCharaktere_Charakterarten_CharakterartId] FOREIGN KEY ([CharakterartId]) REFERENCES [stamm].[Charakterarten] ([Id])
    );
    CREATE INDEX [IX_HesparisCharaktere_CharakterartId] ON [HesparisCharaktere] ([CharakterartId]);
    CREATE INDEX [IX_HesparisCharaktere_UserId] ON [HesparisCharaktere] ([UserId]);
END;

IF OBJECT_ID(N'[CharakterVerknuepfungen]', N'U') IS NULL
BEGIN
    CREATE TABLE [CharakterVerknuepfungen] (
        [Id] int NOT NULL IDENTITY(1, 1),
        [HesparisCharakterId] int NOT NULL,
        [CharacterId] int NOT NULL,
        CONSTRAINT [PK_CharakterVerknuepfungen] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CharakterVerknuepfungen_Characters_CharacterId] FOREIGN KEY ([CharacterId]) REFERENCES [Characters] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CharakterVerknuepfungen_HesparisCharaktere_HesparisCharakterId] FOREIGN KEY ([HesparisCharakterId]) REFERENCES [HesparisCharaktere] ([Id]) ON DELETE CASCADE
    );
    CREATE UNIQUE INDEX [IX_CharakterVerknuepfungen_CharacterId] ON [CharakterVerknuepfungen] ([CharacterId]);
    CREATE UNIQUE INDEX [IX_CharakterVerknuepfungen_HesparisCharakterId] ON [CharakterVerknuepfungen] ([HesparisCharakterId]);
END;
");

            // Spalte HesparisCharakterId in Spielfiguren, Lehrer, Mitschueler (eigener Batch, weil die Spalte
            // im selben Batch nicht für Index/Fremdschlüssel verwendet werden kann)
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'Spielfiguren', N'HesparisCharakterId') IS NULL ALTER TABLE [Spielfiguren] ADD [HesparisCharakterId] int NULL;
IF COL_LENGTH(N'Lehrer', N'HesparisCharakterId') IS NULL ALTER TABLE [Lehrer] ADD [HesparisCharakterId] int NULL;
IF COL_LENGTH(N'Mitschueler', N'HesparisCharakterId') IS NULL ALTER TABLE [Mitschueler] ADD [HesparisCharakterId] int NULL;
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Spielfiguren_HesparisCharakterId' AND object_id = OBJECT_ID(N'[Spielfiguren]'))
    CREATE UNIQUE INDEX [IX_Spielfiguren_HesparisCharakterId] ON [Spielfiguren] ([HesparisCharakterId]) WHERE [HesparisCharakterId] IS NOT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Lehrer_HesparisCharakterId' AND object_id = OBJECT_ID(N'[Lehrer]'))
    CREATE INDEX [IX_Lehrer_HesparisCharakterId] ON [Lehrer] ([HesparisCharakterId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Mitschueler_HesparisCharakterId' AND object_id = OBJECT_ID(N'[Mitschueler]'))
    CREATE INDEX [IX_Mitschueler_HesparisCharakterId] ON [Mitschueler] ([HesparisCharakterId]);

IF OBJECT_ID(N'[FK_Spielfiguren_HesparisCharaktere_HesparisCharakterId]', N'F') IS NULL
    ALTER TABLE [Spielfiguren] ADD CONSTRAINT [FK_Spielfiguren_HesparisCharaktere_HesparisCharakterId]
        FOREIGN KEY ([HesparisCharakterId]) REFERENCES [HesparisCharaktere] ([Id]) ON DELETE CASCADE;
IF OBJECT_ID(N'[FK_Lehrer_HesparisCharaktere_HesparisCharakterId]', N'F') IS NULL
    ALTER TABLE [Lehrer] ADD CONSTRAINT [FK_Lehrer_HesparisCharaktere_HesparisCharakterId]
        FOREIGN KEY ([HesparisCharakterId]) REFERENCES [HesparisCharaktere] ([Id]);
IF OBJECT_ID(N'[FK_Mitschueler_HesparisCharaktere_HesparisCharakterId]', N'F') IS NULL
    ALTER TABLE [Mitschueler] ADD CONSTRAINT [FK_Mitschueler_HesparisCharaktere_HesparisCharakterId]
        FOREIGN KEY ([HesparisCharakterId]) REFERENCES [HesparisCharaktere] ([Id]);
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Bewusst leer: Das Tenant-System wird nicht wiederhergestellt.
        }
    }
}