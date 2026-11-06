using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class AddHesparis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "stamm");

            migrationBuilder.CreateTable(
                name: "AkademieTeams",
                schema: "stamm",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Kurz = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Ort = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Grundwert = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: true),
                    Eigen = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AkademieTeams", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Begabungen",
                schema: "stamm",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Begabungen", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Faecher",
                schema: "stamm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Kurzname = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faecher", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gerichte",
                schema: "stamm",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Mahlzeit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Wirkung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Preis = table.Column<int>(type: "int", nullable: false),
                    Art = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Lebenspunkte = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gerichte", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Kampagnen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GameWorldId = table.Column<int>(type: "int", nullable: false),
                    Spieltag = table.Column<DateOnly>(type: "date", nullable: false),
                    Kalenderjahr = table.Column<int>(type: "int", nullable: false),
                    Schuljahr = table.Column<int>(type: "int", nullable: false),
                    StufenPhase = table.Column<int>(type: "int", nullable: true),
                    HafenOffen = table.Column<bool>(type: "bit", nullable: false),
                    BrauenGesperrt = table.Column<bool>(type: "bit", nullable: false),
                    LadenNotiz = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SitzplanSpalten = table.Column<int>(type: "int", nullable: false),
                    SchwarzmarktCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SchwarzmarktRaetsel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlimSaisonNr = table.Column<int>(type: "int", nullable: false),
                    GlimKapitaen = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GlimLaufendesSpielJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlimAblaufJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kampagnen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kampagnen_GameWorlds_GameWorldId",
                        column: x => x.GameWorldId,
                        principalTable: "GameWorlds",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Sonderzauber",
                schema: "stamm",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Person = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Titel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sonderzauber", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Talente",
                schema: "stamm",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    BegabungCode = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Talente", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Talente_Begabungen_BegabungCode",
                        column: x => x.BegabungCode,
                        principalSchema: "stamm",
                        principalTable: "Begabungen",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FachStufen",
                schema: "stamm",
                columns: table => new
                {
                    FachId = table.Column<int>(type: "int", nullable: false),
                    Stufe = table.Column<int>(type: "int", nullable: false),
                    Beschreibung = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FachStufen", x => new { x.FachId, x.Stufe });
                    table.ForeignKey(
                        name: "FK_FachStufen_Faecher_FachId",
                        column: x => x.FachId,
                        principalSchema: "stamm",
                        principalTable: "Faecher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArchivDokumente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Art = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Titel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DatumText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Signatur = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unterschrift = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Versiegelt = table.Column<bool>(type: "bit", nullable: false),
                    Verborgen = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivDokumente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivDokumente_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArchivKategorien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Signatur = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivKategorien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivKategorien_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Aufgaben",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prioritaet = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Erledigt = table.Column<bool>(type: "bit", nullable: false),
                    ErstelltAm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aufgaben", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aufgaben_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlimAufstellung",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Slot = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Mitglied = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimAufstellung", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlimAufstellung_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlimBank",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Mitglied = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimBank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlimBank_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlimCupArchiv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    CupNr = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MeisterTeamCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Platz = table.Column<int>(type: "int", nullable: true),
                    Siege = table.Column<int>(type: "int", nullable: false),
                    Niederlagen = table.Column<int>(type: "int", nullable: false),
                    SpielerDesCups = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimCupArchiv", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlimCupArchiv_AkademieTeams_MeisterTeamCode",
                        column: x => x.MeisterTeamCode,
                        principalSchema: "stamm",
                        principalTable: "AkademieTeams",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_GlimCupArchiv_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlimKarrieren",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Spiele = table.Column<int>(type: "int", nullable: false),
                    Siege = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimKarrieren", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlimKarrieren_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlimPlayoffs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Runde = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    HeimTeamCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GastTeamCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimPlayoffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlimPlayoffs_AkademieTeams_GastTeamCode",
                        column: x => x.GastTeamCode,
                        principalSchema: "stamm",
                        principalTable: "AkademieTeams",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_GlimPlayoffs_AkademieTeams_HeimTeamCode",
                        column: x => x.HeimTeamCode,
                        principalSchema: "stamm",
                        principalTable: "AkademieTeams",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_GlimPlayoffs_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlimSaisonErgebnisse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Fixture = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PunkteHeim = table.Column<int>(type: "int", nullable: false),
                    PunkteGast = table.Column<int>(type: "int", nullable: false),
                    Simuliert = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimSaisonErgebnisse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlimSaisonErgebnisse_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlimSaisonStatistiken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Treffer = table.Column<int>(type: "int", nullable: false),
                    Punkte = table.Column<int>(type: "int", nullable: false),
                    Abwuerfe = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimSaisonStatistiken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlimSaisonStatistiken_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlimSpiele",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    GegnerTeamCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PunkteHesparis = table.Column<int>(type: "int", nullable: false),
                    PunkteGegner = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateOnly>(type: "date", nullable: true),
                    Heimspiel = table.Column<bool>(type: "bit", nullable: false),
                    Fixture = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Saison = table.Column<int>(type: "int", nullable: false),
                    Art = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notiz = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimSpiele", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlimSpiele_AkademieTeams_GegnerTeamCode",
                        column: x => x.GegnerTeamCode,
                        principalSchema: "stamm",
                        principalTable: "AkademieTeams",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_GlimSpiele_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlimSpieler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Jahrgang = table.Column<int>(type: "int", nullable: false),
                    Slot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Rolle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Waffe = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Wurf = table.Column<int>(type: "int", nullable: false),
                    Schlag = table.Column<int>(type: "int", nullable: false),
                    Tempo = table.Column<int>(type: "int", nullable: false),
                    Fang = table.Column<int>(type: "int", nullable: false),
                    Nerven = table.Column<int>(type: "int", nullable: false),
                    Stil = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Veteran = table.Column<bool>(type: "bit", nullable: false),
                    VorsaisonTreffer = table.Column<int>(type: "int", nullable: true),
                    VorsaisonPunkte = table.Column<int>(type: "int", nullable: true),
                    VorsaisonAbwuerfe = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimSpieler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlimSpieler_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlimTeamStaerken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    TeamCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Wert = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimTeamStaerken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlimTeamStaerken_AkademieTeams_TeamCode",
                        column: x => x.TeamCode,
                        principalSchema: "stamm",
                        principalTable: "AkademieTeams",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_GlimTeamStaerken_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlimTrikotnummern",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Mitglied = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Nummer = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimTrikotnummern", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlimTrikotnummern_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlimVerletzungen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Spiele = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimVerletzungen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlimVerletzungen_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KalenderTermine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateOnly>(type: "date", nullable: false),
                    Titel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Typ = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AutomatischAm = table.Column<DateOnly>(type: "date", nullable: true),
                    RatMonat = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KalenderTermine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KalenderTermine_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KartenNotizen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Ort = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KartenNotizen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KartenNotizen_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KartenZeiten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Schluessel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Wert = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KartenZeiten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KartenZeiten_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Klassenarbeiten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    FachId = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateOnly>(type: "date", nullable: false),
                    Schuljahr = table.Column<int>(type: "int", nullable: true),
                    Stimmung = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Klassenarbeiten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Klassenarbeiten_Faecher_FachId",
                        column: x => x.FachId,
                        principalSchema: "stamm",
                        principalTable: "Faecher",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Klassenarbeiten_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KrankenGlimGemeldet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KrankenGlimGemeldet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KrankenGlimGemeldet_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lehrer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Aufgaben = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Haltung = table.Column<int>(type: "int", nullable: true),
                    Spitzname = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Erkennungszeichen = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrifftManMeist = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Mag = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MagNicht = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    WunderPunkt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoPunktetMan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Geheimnis = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lehrer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lehrer_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Missionen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Nummer = table.Column<int>(type: "int", nullable: false),
                    Titel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Ziel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Notiz = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missionen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Missionen_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProtokollEintraege",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Titel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErstelltAm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Spieltag = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtokollEintraege", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProtokollEintraege_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RatAntraege",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Monat = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Schluessel = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Thema = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Titel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Wirkung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WirktInApp = table.Column<bool>(type: "bit", nullable: false),
                    Von = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Haltung = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Ergebnis = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AbgestimmtAm = table.Column<DateOnly>(type: "date", nullable: true),
                    Gewicht = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatAntraege", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatAntraege_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RatPlanungen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Monat = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Slot = table.Column<int>(type: "int", nullable: false),
                    Schluessel = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Thema = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Titel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Wirkung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WirktInApp = table.Column<bool>(type: "bit", nullable: false),
                    Basis = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatPlanungen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatPlanungen_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RatTagesordnungspunkte",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Von = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Erledigt = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatTagesordnungspunkte", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatTagesordnungspunkte_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RatVerbrauchteAntraege",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Schluessel = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatVerbrauchteAntraege", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatVerbrauchteAntraege_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rezepte",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Wirkung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modifikator = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezepte", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rezepte_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchuljahrBilanzen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Jahr = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchuljahrBilanzen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchuljahrBilanzen_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchwarzmarktVerkaeufe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchwarzmarktVerkaeufe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchwarzmarktVerkaeufe_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpeiseplanEintraege",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Woche = table.Column<DateOnly>(type: "date", nullable: false),
                    Wochentag = table.Column<int>(type: "int", nullable: false),
                    Mahlzeit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GerichtCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeiseplanEintraege", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpeiseplanEintraege_Gerichte_GerichtCode",
                        column: x => x.GerichtCode,
                        principalSchema: "stamm",
                        principalTable: "Gerichte",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_SpeiseplanEintraege_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Spielfiguren",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    Meilenstein2 = table.Column<bool>(type: "bit", nullable: false),
                    Meilenstein3 = table.Column<bool>(type: "bit", nullable: false),
                    Bonuspunkte = table.Column<int>(type: "int", nullable: false),
                    Geld = table.Column<int>(type: "int", nullable: false),
                    LebenspunkteAktuell = table.Column<int>(type: "int", nullable: false),
                    LebenspunkteMax = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spielfiguren", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spielfiguren_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Spielfiguren_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamNotizen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    TeamCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamNotizen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamNotizen_AkademieTeams_TeamCode",
                        column: x => x.TeamCode,
                        principalSchema: "stamm",
                        principalTable: "AkademieTeams",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_TeamNotizen_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Waffenwuerfe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Waffenart = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Schaden = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Waffenwuerfe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Waffenwuerfe_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wettbewerbe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Disziplin = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Ermittlung = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wettbewerbe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wettbewerbe_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WettkampfErgebnisse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Art = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    DatumText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Starter = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Platz = table.Column<int>(type: "int", nullable: true),
                    SiegerTeamCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Ausgewuerfelt = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WettkampfErgebnisse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WettkampfErgebnisse_AkademieTeams_SiegerTeamCode",
                        column: x => x.SiegerTeamCode,
                        principalSchema: "stamm",
                        principalTable: "AkademieTeams",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_WettkampfErgebnisse_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WettkampfNpcs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Art = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Jahrgang = table.Column<int>(type: "int", nullable: false),
                    Grundwert = table.Column<int>(type: "int", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WettkampfNpcs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WettkampfNpcs_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WettkampfStaende",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Art = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LaufendesTurnierJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LaufendesClubduellJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WettkampfStaende", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WettkampfStaende_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WettkampfZeugnisse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Stichtag = table.Column<DateOnly>(type: "date", nullable: false),
                    ErstelltAm = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WettkampfZeugnisse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WettkampfZeugnisse_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZirkelFelder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Nr = table.Column<int>(type: "int", nullable: false),
                    Seite = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Feld = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Gefunden = table.Column<bool>(type: "bit", nullable: false),
                    GesperrtBis = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZirkelFelder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZirkelFelder_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZirkelMordfaelle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    MaxVersuche = table.Column<int>(type: "int", nullable: false),
                    RestVersucheOpfer = table.Column<int>(type: "int", nullable: false),
                    RestVersucheTaeter = table.Column<int>(type: "int", nullable: false),
                    OpferNr = table.Column<int>(type: "int", nullable: true),
                    TaeterNr = table.Column<int>(type: "int", nullable: true),
                    TaeterSeite = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Motiv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpferGeloest = table.Column<bool>(type: "bit", nullable: false),
                    TaeterGeloest = table.Column<bool>(type: "bit", nullable: false),
                    MotivAufgedeckt = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZirkelMordfaelle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZirkelMordfaelle_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZirkelPersonen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Nr = table.Column<int>(type: "int", nullable: false),
                    Seite = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Vorname = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Nachname = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Geschlecht = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Magie = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Figur = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Beruf = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZirkelPersonen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZirkelPersonen_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZirkelVersuche",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Nr = table.Column<int>(type: "int", nullable: false),
                    Seite = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Feld = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Wert = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Zeitpunkt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SpieltagText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZirkelVersuche", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZirkelVersuche_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SonderzauberOptionen",
                schema: "stamm",
                columns: table => new
                {
                    SonderzauberCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Wirkung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Anwendung = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Slots = table.Column<int>(type: "int", nullable: true),
                    Schaden = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SonderzauberOptionen", x => new { x.SonderzauberCode, x.Position });
                    table.ForeignKey(
                        name: "FK_SonderzauberOptionen_Sonderzauber_SonderzauberCode",
                        column: x => x.SonderzauberCode,
                        principalSchema: "stamm",
                        principalTable: "Sonderzauber",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SonderzauberStaende",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    SonderzauberCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    GewaehltePosition = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SonderzauberStaende", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SonderzauberStaende_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SonderzauberStaende_Sonderzauber_SonderzauberCode",
                        column: x => x.SonderzauberCode,
                        principalSchema: "stamm",
                        principalTable: "Sonderzauber",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "Ags",
                schema: "stamm",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Untertitel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Kategorie = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Leitung = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BonusTalentCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    BonusWert = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ags", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Ags_Talente_BonusTalentCode",
                        column: x => x.BonusTalentCode,
                        principalSchema: "stamm",
                        principalTable: "Talente",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "LadenArtikel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Kategorie = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Preis = table.Column<int>(type: "int", nullable: false),
                    Beschreibung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bestand = table.Column<int>(type: "int", nullable: true),
                    Effekt_Art = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Effekt_TalentCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Effekt_Wert = table.Column<int>(type: "int", nullable: true),
                    Effekt_VorteilAuf = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Effekt_DauerArt = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Effekt_DauerAnzahl = table.Column<int>(type: "int", nullable: true),
                    Effekt_DauerText = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LadenArtikel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LadenArtikel_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LadenArtikel_Talente_Effekt_TalentCode",
                        column: x => x.Effekt_TalentCode,
                        principalSchema: "stamm",
                        principalTable: "Talente",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "Meisterschaften",
                schema: "stamm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    TalentCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Beschreibung = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meisterschaften", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meisterschaften_Talente_TalentCode",
                        column: x => x.TalentCode,
                        principalSchema: "stamm",
                        principalTable: "Talente",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchwarzmarktWaren",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Kategorie = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Preis = table.Column<int>(type: "int", nullable: false),
                    Bestand = table.Column<int>(type: "int", nullable: true),
                    Notiz = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Effekt_Art = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Effekt_TalentCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Effekt_Wert = table.Column<int>(type: "int", nullable: true),
                    Effekt_VorteilAuf = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Effekt_DauerArt = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Effekt_DauerAnzahl = table.Column<int>(type: "int", nullable: true),
                    Effekt_DauerText = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchwarzmarktWaren", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchwarzmarktWaren_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchwarzmarktWaren_Talente_Effekt_TalentCode",
                        column: x => x.Effekt_TalentCode,
                        principalSchema: "stamm",
                        principalTable: "Talente",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "ArchivDokumentKategorien",
                columns: table => new
                {
                    ArchivDokumentId = table.Column<int>(type: "int", nullable: false),
                    ArchivKategorieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivDokumentKategorien", x => new { x.ArchivDokumentId, x.ArchivKategorieId });
                    table.ForeignKey(
                        name: "FK_ArchivDokumentKategorien_ArchivDokumente_ArchivDokumentId",
                        column: x => x.ArchivDokumentId,
                        principalTable: "ArchivDokumente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArchivDokumentKategorien_ArchivKategorien_ArchivKategorieId",
                        column: x => x.ArchivKategorieId,
                        principalTable: "ArchivKategorien",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GlimKarriereCups",
                columns: table => new
                {
                    GlimKarriereId = table.Column<int>(type: "int", nullable: false),
                    CupNr = table.Column<int>(type: "int", nullable: false),
                    Einsaetze = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimKarriereCups", x => new { x.GlimKarriereId, x.CupNr });
                    table.ForeignKey(
                        name: "FK_GlimKarriereCups_GlimKarrieren_GlimKarriereId",
                        column: x => x.GlimKarriereId,
                        principalTable: "GlimKarrieren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlimSpielLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GlimSpielId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Trenner = table.Column<bool>(type: "bit", nullable: false),
                    Durchgang = table.Column<int>(type: "int", nullable: true),
                    Haelfte = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Seite = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Aus = table.Column<bool>(type: "bit", nullable: true),
                    Spielstand = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimSpielLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlimSpielLog_GlimSpiele_GlimSpielId",
                        column: x => x.GlimSpielId,
                        principalTable: "GlimSpiele",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlimSpielStatistiken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GlimSpielId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Treffer = table.Column<int>(type: "int", nullable: false),
                    Punkte = table.Column<int>(type: "int", nullable: false),
                    Abwuerfe = table.Column<int>(type: "int", nullable: false),
                    Aus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimSpielStatistiken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlimSpielStatistiken_GlimSpiele_GlimSpielId",
                        column: x => x.GlimSpielId,
                        principalTable: "GlimSpiele",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlimTraining",
                columns: table => new
                {
                    GlimSpielerId = table.Column<int>(type: "int", nullable: false),
                    Wert = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Steigerungen = table.Column<int>(type: "int", nullable: false),
                    Fortschritt = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimTraining", x => new { x.GlimSpielerId, x.Wert });
                    table.ForeignKey(
                        name: "FK_GlimTraining_GlimSpieler_GlimSpielerId",
                        column: x => x.GlimSpielerId,
                        principalTable: "GlimSpieler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LehrerFaecher",
                columns: table => new
                {
                    LehrerId = table.Column<int>(type: "int", nullable: false),
                    FachId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LehrerFaecher", x => new { x.LehrerId, x.FachId });
                    table.ForeignKey(
                        name: "FK_LehrerFaecher_Faecher_FachId",
                        column: x => x.FachId,
                        principalSchema: "stamm",
                        principalTable: "Faecher",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LehrerFaecher_Lehrer_LehrerId",
                        column: x => x.LehrerId,
                        principalTable: "Lehrer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LehrerFakten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LehrerId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Von = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DatumText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LehrerFakten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LehrerFakten_Lehrer_LehrerId",
                        column: x => x.LehrerId,
                        principalTable: "Lehrer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MissionHinweise",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MissionId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Erledigt = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionHinweise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MissionHinweise_Missionen_MissionId",
                        column: x => x.MissionId,
                        principalTable: "Missionen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RatRegeln",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Schluessel = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Thema = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Titel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Wirkung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WirktInApp = table.Column<bool>(type: "bit", nullable: false),
                    Seit = table.Column<DateOnly>(type: "date", nullable: true),
                    AufgehobenAm = table.Column<DateOnly>(type: "date", nullable: true),
                    RatAntragId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatRegeln", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatRegeln_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RatRegeln_RatAntraege_RatAntragId",
                        column: x => x.RatAntragId,
                        principalTable: "RatAntraege",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RatStimmen",
                columns: table => new
                {
                    RatAntragId = table.Column<int>(type: "int", nullable: false),
                    Klasse = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Ja = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatStimmen", x => new { x.RatAntragId, x.Klasse });
                    table.ForeignKey(
                        name: "FK_RatStimmen_RatAntraege_RatAntragId",
                        column: x => x.RatAntragId,
                        principalTable: "RatAntraege",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RezeptZutaten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RezeptId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Menge = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RezeptZutaten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RezeptZutaten_Rezepte_RezeptId",
                        column: x => x.RezeptId,
                        principalTable: "Rezepte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Braueintraege",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    SpielfigurId = table.Column<int>(type: "int", nullable: true),
                    Ergebnis = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Braueintraege", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Braueintraege_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Braueintraege_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DuellClubBilanzen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Typ = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SpielfigurId = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Siege = table.Column<int>(type: "int", nullable: false),
                    Niederlagen = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuellClubBilanzen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DuellClubBilanzen_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DuellClubBilanzen_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Essenskaeufe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateOnly>(type: "date", nullable: false),
                    Mahlzeit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GerichtCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Essenskaeufe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Essenskaeufe_Gerichte_GerichtCode",
                        column: x => x.GerichtCode,
                        principalSchema: "stamm",
                        principalTable: "Gerichte",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_Essenskaeufe_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Essenswirkungen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateOnly>(type: "date", nullable: false),
                    Gericht = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Wirkung = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Essenswirkungen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Essenswirkungen_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gegenstaende",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Kategorie = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Menge = table.Column<int>(type: "int", nullable: false),
                    Notiz = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quelle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Preis = table.Column<int>(type: "int", nullable: true),
                    Neu = table.Column<bool>(type: "bit", nullable: false),
                    Effekt_Art = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Effekt_TalentCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Effekt_Wert = table.Column<int>(type: "int", nullable: true),
                    Effekt_VorteilAuf = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Effekt_DauerArt = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Effekt_DauerAnzahl = table.Column<int>(type: "int", nullable: true),
                    Effekt_DauerText = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gegenstaende", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gegenstaende_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Gegenstaende_Talente_Effekt_TalentCode",
                        column: x => x.Effekt_TalentCode,
                        principalSchema: "stamm",
                        principalTable: "Talente",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "Geldbuchungen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Betrag = table.Column<int>(type: "int", nullable: false),
                    Grund = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Geldbuchungen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Geldbuchungen_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KlassenarbeitSpielfiguren",
                columns: table => new
                {
                    KlassenarbeitId = table.Column<int>(type: "int", nullable: false),
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    MussNachschreiben = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KlassenarbeitSpielfiguren", x => new { x.KlassenarbeitId, x.SpielfigurId });
                    table.ForeignKey(
                        name: "FK_KlassenarbeitSpielfiguren_Klassenarbeiten_KlassenarbeitId",
                        column: x => x.KlassenarbeitId,
                        principalTable: "Klassenarbeiten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KlassenarbeitSpielfiguren_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Krankheitsfaelle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    SpielfigurId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Art = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Diagnose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Behandlung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Malus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notiz = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tage = table.Column<int>(type: "int", nullable: true),
                    Aufnahme = table.Column<DateOnly>(type: "date", nullable: false),
                    Entlassung = table.Column<DateOnly>(type: "date", nullable: true),
                    Bett = table.Column<int>(type: "int", nullable: true),
                    Simuliert = table.Column<bool>(type: "bit", nullable: false),
                    Quelle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Krankheitsfaelle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Krankheitsfaelle_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Krankheitsfaelle_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Mitschueler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Spitzname = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Spottname = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Magie = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Herkunft = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LieblingsfachId = table.Column<int>(type: "int", nullable: true),
                    HassfachId = table.Column<int>(type: "int", nullable: true),
                    Patronus = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Rolle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notiz = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notenschnitt = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: true),
                    Geburtstag = table.Column<DateOnly>(type: "date", nullable: true),
                    Farbe = table.Column<int>(type: "int", nullable: true),
                    Leistungsdruck = table.Column<bool>(type: "bit", nullable: false),
                    Sitzplatz = table.Column<int>(type: "int", nullable: true),
                    SpielfigurId = table.Column<int>(type: "int", nullable: true),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mitschueler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mitschueler_Faecher_HassfachId",
                        column: x => x.HassfachId,
                        principalSchema: "stamm",
                        principalTable: "Faecher",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Mitschueler_Faecher_LieblingsfachId",
                        column: x => x.LieblingsfachId,
                        principalSchema: "stamm",
                        principalTable: "Faecher",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Mitschueler_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mitschueler_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Nachsitzen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    LehrerId = table.Column<int>(type: "int", nullable: true),
                    LehrerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Grund = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Strafe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ErstelltAm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MissionId = table.Column<int>(type: "int", nullable: true),
                    Verfallen = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nachsitzen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nachsitzen_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Nachsitzen_Lehrer_LehrerId",
                        column: x => x.LehrerId,
                        principalTable: "Lehrer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Nachsitzen_Missionen_MissionId",
                        column: x => x.MissionId,
                        principalTable: "Missionen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Nachsitzen_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProtokollBeteiligte",
                columns: table => new
                {
                    ProtokollEintragId = table.Column<int>(type: "int", nullable: false),
                    SpielfigurId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtokollBeteiligte", x => new { x.ProtokollEintragId, x.SpielfigurId });
                    table.ForeignKey(
                        name: "FK_ProtokollBeteiligte_ProtokollEintraege_ProtokollEintragId",
                        column: x => x.ProtokollEintragId,
                        principalTable: "ProtokollEintraege",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProtokollBeteiligte_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Pruefungseintraege",
                columns: table => new
                {
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    Abschnitt = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Teil = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Wert = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pruefungseintraege", x => new { x.SpielfigurId, x.Abschnitt, x.Teil });
                    table.ForeignKey(
                        name: "FK_Pruefungseintraege_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchuljahrAbschnitte",
                columns: table => new
                {
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    Abschnitt = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Wert = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchuljahrAbschnitte", x => new { x.SpielfigurId, x.Abschnitt });
                    table.ForeignKey(
                        name: "FK_SchuljahrAbschnitte_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchuljahrBilanzSpielfiguren",
                columns: table => new
                {
                    SchuljahrBilanzId = table.Column<int>(type: "int", nullable: false),
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    Fachpunkte = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: true),
                    Haltungspunkte = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: true),
                    AgPunkte = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: true),
                    Abschnitt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Schnitt = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchuljahrBilanzSpielfiguren", x => new { x.SchuljahrBilanzId, x.SpielfigurId });
                    table.ForeignKey(
                        name: "FK_SchuljahrBilanzSpielfiguren_SchuljahrBilanzen_SchuljahrBilanzId",
                        column: x => x.SchuljahrBilanzId,
                        principalTable: "SchuljahrBilanzen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchuljahrBilanzSpielfiguren_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SchuljahrZeugnisnoten",
                columns: table => new
                {
                    SchuljahrBilanzId = table.Column<int>(type: "int", nullable: false),
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    FachId = table.Column<int>(type: "int", nullable: false),
                    Schnitt = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchuljahrZeugnisnoten", x => new { x.SchuljahrBilanzId, x.SpielfigurId, x.FachId });
                    table.ForeignKey(
                        name: "FK_SchuljahrZeugnisnoten_Faecher_FachId",
                        column: x => x.FachId,
                        principalSchema: "stamm",
                        principalTable: "Faecher",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SchuljahrZeugnisnoten_SchuljahrBilanzen_SchuljahrBilanzId",
                        column: x => x.SchuljahrBilanzId,
                        principalTable: "SchuljahrBilanzen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchuljahrZeugnisnoten_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Schulnoten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    FachId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schulnoten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schulnoten_Faecher_FachId",
                        column: x => x.FachId,
                        principalSchema: "stamm",
                        principalTable: "Faecher",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Schulnoten_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpielfigurFachStufen",
                columns: table => new
                {
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    FachId = table.Column<int>(type: "int", nullable: false),
                    Stufe = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpielfigurFachStufen", x => new { x.SpielfigurId, x.FachId });
                    table.ForeignKey(
                        name: "FK_SpielfigurFachStufen_Faecher_FachId",
                        column: x => x.FachId,
                        principalSchema: "stamm",
                        principalTable: "Faecher",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SpielfigurFachStufen_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StufenWahlen",
                columns: table => new
                {
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    FachId = table.Column<int>(type: "int", nullable: false),
                    Stufe = table.Column<int>(type: "int", nullable: false),
                    Wahl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StufenWahlen", x => new { x.SpielfigurId, x.FachId, x.Stufe });
                    table.ForeignKey(
                        name: "FK_StufenWahlen_Faecher_FachId",
                        column: x => x.FachId,
                        principalSchema: "stamm",
                        principalTable: "Faecher",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StufenWahlen_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TalentWerte",
                columns: table => new
                {
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    TalentCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Wert = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalentWerte", x => new { x.SpielfigurId, x.TalentCode });
                    table.ForeignKey(
                        name: "FK_TalentWerte_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TalentWerte_Talente_TalentCode",
                        column: x => x.TalentCode,
                        principalSchema: "stamm",
                        principalTable: "Talente",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "TalentZiele",
                columns: table => new
                {
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    Schluessel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TalentCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalentZiele", x => new { x.SpielfigurId, x.Schluessel });
                    table.ForeignKey(
                        name: "FK_TalentZiele_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TalentZiele_Talente_TalentCode",
                        column: x => x.TalentCode,
                        principalSchema: "stamm",
                        principalTable: "Talente",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "WettkampfTeilnehmer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Art = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Gruppe = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Typ = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SpielfigurId = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WettkampfTeilnehmer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WettkampfTeilnehmer_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WettkampfTeilnehmer_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Wirkungen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Effekt_Art = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Effekt_TalentCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Effekt_Wert = table.Column<int>(type: "int", nullable: true),
                    Effekt_VorteilAuf = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Effekt_DauerArt = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Effekt_DauerAnzahl = table.Column<int>(type: "int", nullable: true),
                    Effekt_DauerText = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Ab = table.Column<DateOnly>(type: "date", nullable: true),
                    Restanwendungen = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wirkungen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wirkungen_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wirkungen_Talente_Effekt_TalentCode",
                        column: x => x.Effekt_TalentCode,
                        principalSchema: "stamm",
                        principalTable: "Talente",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "ZirkelNotizen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Nr = table.Column<int>(type: "int", nullable: false),
                    Seite = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Typ = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SpielfigurId = table.Column<int>(type: "int", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bestaetigt = table.Column<bool>(type: "bit", nullable: false),
                    ErstelltAm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZirkelNotizen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZirkelNotizen_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ZirkelNotizen_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WettbewerbPlatzierungen",
                columns: table => new
                {
                    WettbewerbId = table.Column<int>(type: "int", nullable: false),
                    TeamCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Platz = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WettbewerbPlatzierungen", x => new { x.WettbewerbId, x.TeamCode });
                    table.ForeignKey(
                        name: "FK_WettbewerbPlatzierungen_AkademieTeams_TeamCode",
                        column: x => x.TeamCode,
                        principalSchema: "stamm",
                        principalTable: "AkademieTeams",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_WettbewerbPlatzierungen_Wettbewerbe_WettbewerbId",
                        column: x => x.WettbewerbId,
                        principalTable: "Wettbewerbe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WettkampfNpcWerte",
                columns: table => new
                {
                    WettkampfNpcId = table.Column<int>(type: "int", nullable: false),
                    Schluessel = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Wert = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WettkampfNpcWerte", x => new { x.WettkampfNpcId, x.Schluessel });
                    table.ForeignKey(
                        name: "FK_WettkampfNpcWerte_WettkampfNpcs_WettkampfNpcId",
                        column: x => x.WettkampfNpcId,
                        principalTable: "WettkampfNpcs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WettkampfZeugnisEintraege",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WettkampfZeugnisId = table.Column<int>(type: "int", nullable: false),
                    Art = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Platz = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WettkampfZeugnisEintraege", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WettkampfZeugnisEintraege_WettkampfZeugnisse_WettkampfZeugnisId",
                        column: x => x.WettkampfZeugnisId,
                        principalTable: "WettkampfZeugnisse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SonderzauberAufgedeckt",
                columns: table => new
                {
                    SonderzauberStandId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SonderzauberAufgedeckt", x => new { x.SonderzauberStandId, x.Position });
                    table.ForeignKey(
                        name: "FK_SonderzauberAufgedeckt_SonderzauberStaende_SonderzauberStandId",
                        column: x => x.SonderzauberStandId,
                        principalTable: "SonderzauberStaende",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgEintraege",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    AgCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notiz = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgEintraege", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgEintraege_Ags_AgCode",
                        column: x => x.AgCode,
                        principalSchema: "stamm",
                        principalTable: "Ags",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_AgEintraege_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpielfigurMeisterschaften",
                columns: table => new
                {
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    MeisterschaftDefinitionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpielfigurMeisterschaften", x => new { x.SpielfigurId, x.MeisterschaftDefinitionId });
                    table.ForeignKey(
                        name: "FK_SpielfigurMeisterschaften_Meisterschaften_MeisterschaftDefinitionId",
                        column: x => x.MeisterschaftDefinitionId,
                        principalSchema: "stamm",
                        principalTable: "Meisterschaften",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SpielfigurMeisterschaften_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Beziehungen",
                columns: table => new
                {
                    MitschuelerId = table.Column<int>(type: "int", nullable: false),
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    Figur_Freundschaft = table.Column<int>(type: "int", nullable: false),
                    Figur_Romantik = table.Column<int>(type: "int", nullable: false),
                    Figur_Rivalitaet = table.Column<int>(type: "int", nullable: false),
                    Figur_Abneigung = table.Column<int>(type: "int", nullable: false),
                    Figur_Respekt = table.Column<int>(type: "int", nullable: false),
                    Mitschueler_Freundschaft = table.Column<int>(type: "int", nullable: false),
                    Mitschueler_Romantik = table.Column<int>(type: "int", nullable: false),
                    Mitschueler_Rivalitaet = table.Column<int>(type: "int", nullable: false),
                    Mitschueler_Abneigung = table.Column<int>(type: "int", nullable: false),
                    Mitschueler_Respekt = table.Column<int>(type: "int", nullable: false),
                    Verborgen = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beziehungen", x => new { x.MitschuelerId, x.SpielfigurId });
                    table.ForeignKey(
                        name: "FK_Beziehungen_Mitschueler_MitschuelerId",
                        column: x => x.MitschuelerId,
                        principalTable: "Mitschueler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Beziehungen_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GlimNachwuchs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Slot = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MitschuelerId = table.Column<int>(type: "int", nullable: true),
                    Jahrgang = table.Column<int>(type: "int", nullable: true),
                    Wurf = table.Column<int>(type: "int", nullable: false),
                    Schlag = table.Column<int>(type: "int", nullable: false),
                    Tempo = table.Column<int>(type: "int", nullable: false),
                    Fang = table.Column<int>(type: "int", nullable: false),
                    Nerven = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimNachwuchs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlimNachwuchs_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GlimNachwuchs_Mitschueler_MitschuelerId",
                        column: x => x.MitschuelerId,
                        principalTable: "Mitschueler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "KlassenarbeitMitschueler",
                columns: table => new
                {
                    KlassenarbeitId = table.Column<int>(type: "int", nullable: false),
                    MitschuelerId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KlassenarbeitMitschueler", x => new { x.KlassenarbeitId, x.MitschuelerId });
                    table.ForeignKey(
                        name: "FK_KlassenarbeitMitschueler_Klassenarbeiten_KlassenarbeitId",
                        column: x => x.KlassenarbeitId,
                        principalTable: "Klassenarbeiten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KlassenarbeitMitschueler_Mitschueler_MitschuelerId",
                        column: x => x.MitschuelerId,
                        principalTable: "Mitschueler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MitschuelerAgs",
                columns: table => new
                {
                    MitschuelerId = table.Column<int>(type: "int", nullable: false),
                    AgCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MitschuelerAgs", x => new { x.MitschuelerId, x.AgCode });
                    table.ForeignKey(
                        name: "FK_MitschuelerAgs_Ags_AgCode",
                        column: x => x.AgCode,
                        principalSchema: "stamm",
                        principalTable: "Ags",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_MitschuelerAgs_Mitschueler_MitschuelerId",
                        column: x => x.MitschuelerId,
                        principalTable: "Mitschueler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MitschuelerMeinungen",
                columns: table => new
                {
                    MitschuelerId = table.Column<int>(type: "int", nullable: false),
                    Thema = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Wert = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MitschuelerMeinungen", x => new { x.MitschuelerId, x.Thema });
                    table.ForeignKey(
                        name: "FK_MitschuelerMeinungen_Mitschueler_MitschuelerId",
                        column: x => x.MitschuelerId,
                        principalTable: "Mitschueler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Protokollhefte",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KampagneId = table.Column<int>(type: "int", nullable: false),
                    Nummer = table.Column<int>(type: "int", nullable: false),
                    Entwurf = table.Column<bool>(type: "bit", nullable: false),
                    Titel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Lehrer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Ort = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Von = table.Column<DateOnly>(type: "date", nullable: true),
                    Bis = table.Column<DateOnly>(type: "date", nullable: true),
                    Abgabe = table.Column<DateOnly>(type: "date", nullable: true),
                    Auftrag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BegleiterId = table.Column<int>(type: "int", nullable: true),
                    SchreiberSpielfigurId = table.Column<int>(type: "int", nullable: true),
                    SchreiberMitschuelerId = table.Column<int>(type: "int", nullable: true),
                    AbgegebenAm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Rueckmeldung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KalenderTerminId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Protokollhefte", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Protokollhefte_KalenderTermine_KalenderTerminId",
                        column: x => x.KalenderTerminId,
                        principalTable: "KalenderTermine",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Protokollhefte_Kampagnen_KampagneId",
                        column: x => x.KampagneId,
                        principalTable: "Kampagnen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Protokollhefte_Mitschueler_BegleiterId",
                        column: x => x.BegleiterId,
                        principalTable: "Mitschueler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Protokollhefte_Mitschueler_SchreiberMitschuelerId",
                        column: x => x.SchreiberMitschuelerId,
                        principalTable: "Mitschueler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Protokollhefte_Spielfiguren_SchreiberSpielfigurId",
                        column: x => x.SchreiberSpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RatGefallen",
                columns: table => new
                {
                    RatAntragId = table.Column<int>(type: "int", nullable: false),
                    MitschuelerId = table.Column<int>(type: "int", nullable: false),
                    SpielfigurId = table.Column<int>(type: "int", nullable: true),
                    Erledigt = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatGefallen", x => new { x.RatAntragId, x.MitschuelerId });
                    table.ForeignKey(
                        name: "FK_RatGefallen_Mitschueler_MitschuelerId",
                        column: x => x.MitschuelerId,
                        principalTable: "Mitschueler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RatGefallen_RatAntraege_RatAntragId",
                        column: x => x.RatAntragId,
                        principalTable: "RatAntraege",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RatGefallen_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RatUnterschriften",
                columns: table => new
                {
                    RatAntragId = table.Column<int>(type: "int", nullable: false),
                    MitschuelerId = table.Column<int>(type: "int", nullable: false),
                    Verweigert = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatUnterschriften", x => new { x.RatAntragId, x.MitschuelerId });
                    table.ForeignKey(
                        name: "FK_RatUnterschriften_Mitschueler_MitschuelerId",
                        column: x => x.MitschuelerId,
                        principalTable: "Mitschueler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RatUnterschriften_RatAntraege_RatAntragId",
                        column: x => x.RatAntragId,
                        principalTable: "RatAntraege",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgMitglieder",
                columns: table => new
                {
                    AgEintragId = table.Column<int>(type: "int", nullable: false),
                    SpielfigurId = table.Column<int>(type: "int", nullable: false),
                    Wochentag = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgMitglieder", x => new { x.AgEintragId, x.SpielfigurId });
                    table.ForeignKey(
                        name: "FK_AgMitglieder_AgEintraege_AgEintragId",
                        column: x => x.AgEintragId,
                        principalTable: "AgEintraege",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgMitglieder_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GlimNachwuchsTraining",
                columns: table => new
                {
                    GlimNachwuchsId = table.Column<int>(type: "int", nullable: false),
                    Wert = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Steigerungen = table.Column<int>(type: "int", nullable: false),
                    Fortschritt = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlimNachwuchsTraining", x => new { x.GlimNachwuchsId, x.Wert });
                    table.ForeignKey(
                        name: "FK_GlimNachwuchsTraining_GlimNachwuchs_GlimNachwuchsId",
                        column: x => x.GlimNachwuchsId,
                        principalTable: "GlimNachwuchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeftAblauf",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProtokollheftId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateOnly>(type: "date", nullable: true),
                    Uhrzeit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Wer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Wichtig = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeftAblauf", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeftAblauf_Protokollhefte_ProtokollheftId",
                        column: x => x.ProtokollheftId,
                        principalTable: "Protokollhefte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeftAbschnitte",
                columns: table => new
                {
                    ProtokollheftId = table.Column<int>(type: "int", nullable: false),
                    Abschnitt = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeftAbschnitte", x => new { x.ProtokollheftId, x.Abschnitt });
                    table.ForeignKey(
                        name: "FK_HeftAbschnitte_Protokollhefte_ProtokollheftId",
                        column: x => x.ProtokollheftId,
                        principalTable: "Protokollhefte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeftChecks",
                columns: table => new
                {
                    ProtokollheftId = table.Column<int>(type: "int", nullable: false),
                    Schluessel = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Erledigt = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeftChecks", x => new { x.ProtokollheftId, x.Schluessel });
                    table.ForeignKey(
                        name: "FK_HeftChecks_Protokollhefte_ProtokollheftId",
                        column: x => x.ProtokollheftId,
                        principalTable: "Protokollhefte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeftFehlend",
                columns: table => new
                {
                    ProtokollheftId = table.Column<int>(type: "int", nullable: false),
                    SpielfigurId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeftFehlend", x => new { x.ProtokollheftId, x.SpielfigurId });
                    table.ForeignKey(
                        name: "FK_HeftFehlend_Protokollhefte_ProtokollheftId",
                        column: x => x.ProtokollheftId,
                        principalTable: "Protokollhefte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeftFehlend_Spielfiguren_SpielfigurId",
                        column: x => x.SpielfigurId,
                        principalTable: "Spielfiguren",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HeftFragen",
                columns: table => new
                {
                    ProtokollheftId = table.Column<int>(type: "int", nullable: false),
                    Abschnitt = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Beantwortet = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeftFragen", x => new { x.ProtokollheftId, x.Abschnitt, x.Position });
                    table.ForeignKey(
                        name: "FK_HeftFragen_Protokollhefte_ProtokollheftId",
                        column: x => x.ProtokollheftId,
                        principalTable: "Protokollhefte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeftJederEintraege",
                columns: table => new
                {
                    ProtokollheftId = table.Column<int>(type: "int", nullable: false),
                    Schluessel = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeftJederEintraege", x => new { x.ProtokollheftId, x.Schluessel });
                    table.ForeignKey(
                        name: "FK_HeftJederEintraege_Protokollhefte_ProtokollheftId",
                        column: x => x.ProtokollheftId,
                        principalTable: "Protokollhefte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeftNotizen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProtokollheftId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateOnly>(type: "date", nullable: true),
                    Uhrzeit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeftNotizen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeftNotizen_Protokollhefte_ProtokollheftId",
                        column: x => x.ProtokollheftId,
                        principalTable: "Protokollhefte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeftPersonen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProtokollheftId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Rolle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Sagt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Eindruck = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Wichtig = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeftPersonen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeftPersonen_Protokollhefte_ProtokollheftId",
                        column: x => x.ProtokollheftId,
                        principalTable: "Protokollhefte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeftSchwerpunkte",
                columns: table => new
                {
                    ProtokollheftId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeftSchwerpunkte", x => new { x.ProtokollheftId, x.Text });
                    table.ForeignKey(
                        name: "FK_HeftSchwerpunkte_Protokollhefte_ProtokollheftId",
                        column: x => x.ProtokollheftId,
                        principalTable: "Protokollhefte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeftNotizUebernahmen",
                columns: table => new
                {
                    HeftNotizId = table.Column<int>(type: "int", nullable: false),
                    Abschnitt = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeftNotizUebernahmen", x => new { x.HeftNotizId, x.Abschnitt });
                    table.ForeignKey(
                        name: "FK_HeftNotizUebernahmen_HeftNotizen_HeftNotizId",
                        column: x => x.HeftNotizId,
                        principalTable: "HeftNotizen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "stamm",
                table: "AkademieTeams",
                columns: new[] { "Code", "Eigen", "Grundwert", "Kurz", "Name", "Ort" },
                values: new object[,]
                {
                    { "aschgrund", false, 6.4m, "ASG", "Aschgrund", "Aschgrund, Bergbaustadt am Vulkanhang" },
                    { "hesperis", true, null, "HES", "Hesparis", "Hesparis, eure Akademie" },
                    { "irmin", false, 5.5m, "IRM", "St. Irmin", "St. Irmin, Klosterstadt im stillen Tal" },
                    { "norrvik", false, 6.6m, "NOR", "Norrvik", "Norrvik, an der Eisküste im hohen Norden" },
                    { "ostmark", false, 5.3m, "OST", "Ostmark", "Ostmark, Industriestadt mit Telegrafenturm" },
                    { "tellhafen", false, 5.9m, "TEL", "Tellhafen", "Tellhafen, Hafenstadt an der Westküste" }
                });

            migrationBuilder.InsertData(
                schema: "stamm",
                table: "Begabungen",
                columns: new[] { "Code", "Name" },
                values: new object[,]
                {
                    { "h", "Handeln" },
                    { "s", "Soziales" },
                    { "w", "Wissen" }
                });

            migrationBuilder.InsertData(
                schema: "stamm",
                table: "Faecher",
                columns: new[] { "Id", "Kurzname", "Name" },
                values: new object[,]
                {
                    { 0, "Pokusweben", "Pokusweben" },
                    { 1, "Resonanzkunde", "Resonanzkunde" },
                    { 2, "Elementarharmonie", "Elementarharmonie" },
                    { 3, "Astr. Orientierung", "Astronomische Orientierung" },
                    { 4, "Kartopädie", "Kartopädie" },
                    { 5, "Chronotaktik", "Chronotaktik" },
                    { 6, "Ätherfechten", "Ätherfechten" },
                    { 7, "Liminaltheorie", "Liminaltheorie" },
                    { 8, "Grenzchemie", "Grenzchemie" },
                    { 9, "Magiatur", "Magiatur" }
                });

            migrationBuilder.InsertData(
                schema: "stamm",
                table: "Gerichte",
                columns: new[] { "Code", "Art", "Lebenspunkte", "Mahlzeit", "Name", "Preis", "Wirkung" },
                values: new object[,]
                {
                    { "beerenbrei", "heal", 3, "Fruehstueck", "Haferbrei mit Beeren", 2, "+3 LP sofort" },
                    { "braten", "heal", 15, "Mittag", "Sonntagsbraten mit Knödeln", 20, "+15 LP sofort" },
                    { "bratkartoffeln", "tal", null, "Abendbrot", "Bratkartoffeln mit Spiegelei", 5, "Körperkraft +5 bis Mitternacht" },
                    { "brotzeit", "tal", null, "Abendbrot", "Brotzeit mit Kräuterquark", 4, "Wahrnehmung +5 bis Mitternacht" },
                    { "fischsuppe", "tal", null, "Mittag", "Fischsuppe mit Dill", 10, "Magisches Gespür +10 bis Abend" },
                    { "frikassee", "tal", null, "Mittag", "Hühnerfrikassee mit Reis", 10, "Körperkraft +10 bis Abend" },
                    { "gemuesesuppe", "heal", 5, "Abendbrot", "Gemüsesuppe", 4, "+5 LP sofort" },
                    { "glimmmilch", "kurios", null, "Abendbrot", "Glimm-Milch", 3, "Leuchtet 1 Stunde schwach im Dunkeln" },
                    { "griesbrei", "tal", null, "Fruehstueck", "Grießbrei mit Zimt", 3, "Beruhigen +5 bis Mittag" },
                    { "griessuppe", "heal", 5, "Mittag", "Grießklößchensuppe", 5, "+5 LP sofort" },
                    { "grillabend", "tal", null, "Abendbrot", "Grillabend im Innenhof", 10, "Beruhigen +10 bis Mitternacht" },
                    { "gulasch", "tal", null, "Mittag", "Feuerwurz-Gulasch", 12, "Kein Frieren · Reizen +5 bis Abend" },
                    { "haferbrei", "heal", 5, "Fruehstueck", "Haferbrei mit Honig", 3, "+5 LP sofort" },
                    { "hirobrot", "kurios", null, "Fruehstueck", "Hiro-Brot mit Butter", 10, "1× am Tag einen Wurf wiederholen" },
                    { "kaesebrot", "heal", 5, "Abendbrot", "Käsebrot und Apfelmost", 3, "Gut schlafen: +5 LP am Morgen" },
                    { "klosterbrot", "heal", 8, "Mittag", "Klosterbrot mit Mondmoos", 20, "+8 LP und eine leichte Wunde heilt" },
                    { "kuchen", "tal", null, "Abendbrot", "Kuchen und Tee", 6, "Nerven: Panik-Proben +10 bis Mitternacht" },
                    { "linsen", "heal", 10, "Mittag", "Linseneintopf mit Brot", 8, "+10 LP sofort" },
                    { "milchbroetchen", "tal", null, "Fruehstueck", "Milchbrötchen mit Butter", 2, "Nerven: nächste Probe +5" },
                    { "mittwochsreis", "kurios", null, "Mittag", "Mittwochsreis", 2, "Wer zweimal nimmt, schläft im Unterricht ein" },
                    { "nebelpastete", "tal", null, "Mittag", "Nebelpastete", 14, "Verschleierung +10 bis Abend" },
                    { "pfannkuchen", "tal", null, "Abendbrot", "Pfannkuchen", 4, "Erzählen +5 bis Mitternacht" },
                    { "pflaumenmus", "tal", null, "Fruehstueck", "Brötchen mit Pflaumenmus", 3, "Erzählen +5 bis Mittag" },
                    { "puffer", "tal", null, "Mittag", "Kartoffelpuffer mit Apfelmus", 6, "Akrobatik +5 bis Abend" },
                    { "ruehrei", "tal", null, "Fruehstueck", "Rührei mit Kräutern", 4, "Wahrnehmung +5 bis Mittag" },
                    { "sternensuppe", "kurios", null, "Abendbrot", "Sternensuppe", 12, "Im Dunkeln sehen bis Mitternacht" },
                    { "traumtee", "kurios", null, "Abendbrot", "Traumtee mit Honigkeks", 8, "Ein Traum mit Hinweis (Spielleitung)" },
                    { "vortag", "kurios", null, "Mittag", "Eintopf vom Vortag", 2, "Keine Wirkung, aber billig" },
                    { "wachtee", "kurios", null, "Fruehstueck", "Wachtee mit Zwieback", 4, "Kein Malus durch Müdigkeit bis Abend" },
                    { "waffeln", "tal", null, "Fruehstueck", "Waffeln mit Puderzucker", 5, "Überreden +10 bis Mittag" }
                });

            migrationBuilder.InsertData(
                schema: "stamm",
                table: "Sonderzauber",
                columns: new[] { "Code", "Person", "Titel" },
                values: new object[,]
                {
                    { "darius-schleim", "Darius", "Zusatzzauber — Wasser/Schleim" },
                    { "elma-fische", "Elma", "Zusatzzauber — Fauna/Ichtyo" },
                    { "levi-rauch", "Levi", "Zusatzzauber — Feuer/Rauch" },
                    { "rigel-kosmos", "Rigel", "Zusatzzauber — Dunkel/Kosmos" },
                    { "svea-euphorie", "Svea", "Zusatzzauber — Kunst/Euphorie" }
                });

            migrationBuilder.InsertData(
                schema: "stamm",
                table: "FachStufen",
                columns: new[] { "FachId", "Stufe", "Beschreibung" },
                values: new object[,]
                {
                    { 0, 1, "Feerum-Rüstung" },
                    { 0, 2, "Ätherblei-Rüstung" },
                    { 0, 3, "Maganbronze-Rüstung" },
                    { 0, 4, "Berylliumkupfer-Rüstung · Feuer- und Blitzschaden −1W10" },
                    { 0, 5, "Elektrum-Rüstung · 1× pro Kampf blendet sie einen Angreifer: sein nächster Angriff hat Nachteil" },
                    { 0, 6, "Tantalit-Rüstung · kein Schaden durch geworfene oder verschüttete Tränke" },
                    { 0, 7, "Vanadium-Rüstung · 1× pro Kampf einen Treffer halbieren" },
                    { 0, 8, "Aerolith-Rüstung" },
                    { 0, 9, "Osmium-Rüstung" },
                    { 0, 10, "Palladium-Rüstung" },
                    { 1, 1, "+10 AUR ODER +10 SIL" },
                    { 1, 2, "+10 AUR ODER +10 SIL" },
                    { 1, 3, "+10 AUR/SIL ODER +1 MSW" },
                    { 1, 4, "Mitschwingen: für Zauber von Verbündeten in 3 Feldern auritieren" },
                    { 1, 5, "Echo: 1× pro Kampf wirkt ein auritierter Zauber in der nächsten Runde nochmal mit halber Wirkung" },
                    { 1, 6, "+10 AUR ODER +10 SIL" },
                    { 1, 7, "Stimmgabel: 1× pro Kampf einen Zauber silizieren und seine Wirkung auf ein eigenes Ziel lenken" },
                    { 1, 8, "+10 AUR ODER +10 SIL" },
                    { 1, 9, "+10 AUR ODER +10 SIL" },
                    { 1, 10, "+20 AUR/SIL ODER +2 MSW" },
                    { 2, 1, "+10 LP ODER +10 RÜW" },
                    { 2, 2, "+10 LP ODER +10 RÜW" },
                    { 2, 3, "+1 RES" },
                    { 2, 4, "Erdung: Rast heilt +1W10 LP" },
                    { 2, 5, "Elementschild: 1× pro Kampf als Reaktion den Schaden eines Elements halbieren" },
                    { 2, 6, "+10 LP ODER +10 RÜW" },
                    { 2, 7, "Einklang: 1× pro Kampf 1 Runde kein Schaden durch ein Element, für dich und Verbündete in 2 Feldern" },
                    { 2, 8, "+15 LP ODER +15 RÜW" },
                    { 2, 9, "+15 LP ODER +15 RÜW" },
                    { 2, 10, "+2 RES" },
                    { 3, 1, "+10 MAGW ODER +10 HAPW" },
                    { 3, 2, "+10 MAGW ODER +10 HAPW" },
                    { 3, 3, "+10 MAGW/HAPW ODER +1 MSW" },
                    { 3, 4, "Sternenlicht: nachts unter freiem Himmel keine Erschwernis auf Wahrnehmung" },
                    { 3, 5, "Fixstern: 1× pro Mission einen Ort markieren, die Richtung dorthin ist immer bekannt" },
                    { 3, 6, "+10 MAGW ODER +10 HAPW" },
                    { 3, 7, "Konstellation: 1× pro Kampf die Position aller Wesen in 10 Feldern kennen, auch versteckte" },
                    { 3, 8, "+10 MAGW ODER +10 HAPW" },
                    { 3, 9, "+10 MAGW ODER +10 HAPW" },
                    { 3, 10, "+20 MAGW/HAPW ODER +2 MSW" },
                    { 4, 1, "+1 RW ODER +10 HLF" },
                    { 4, 2, "+1 RW ODER +10 HLF" },
                    { 4, 3, "+2 RW ODER +1 HLF-Aktion" },
                    { 4, 4, "Grundriss: einen Raum 1× betreten genügt, um ihn ohne Probe zu zeichnen" },
                    { 4, 5, "Abkürzung: 1× pro Kampf dich und einen Verbündeten +3 Felder bewegen" },
                    { 4, 6, "+1 RW ODER +10 HLF" },
                    { 4, 7, "Lagekarte: 1× pro Kampf stellen sich alle Verbündeten vor der Runde bis zu 2 Felder neu auf" },
                    { 4, 8, "+1 RW ODER +15 HLF" },
                    { 4, 9, "+1 RW ODER +15 HLF" },
                    { 4, 10, "+3 RW ODER +1 HLF-Aktion" },
                    { 5, 1, "+1 Krit auf alle Talente" },
                    { 5, 2, "+1 Krit auf alle Talente" },
                    { 5, 3, "+2 Krit auf alle Talente" },
                    { 5, 4, "Taktgefühl: im ersten Zug eines Kampfes +10 auf alle Proben" },
                    { 5, 5, "Zeitlupe: 1× pro Kampf einen eigenen Wurf wiederholen" },
                    { 5, 6, "+1 Krit auf alle Talente ODER Zeitlupe 2× pro Kampf" },
                    { 5, 7, "Vorgriff: 1× pro Kampf zwei Aktionen in einem Zug" },
                    { 5, 8, "+1 Krit ODER +1 Wiederholung" },
                    { 5, 9, "+1 Krit auf alle Talente" },
                    { 5, 10, "+3 Krit ODER +1 Extra-Aktion" },
                    { 6, 1, "+1W10 ODER +10 TL" },
                    { 6, 2, "+1W10 ODER +10 TL" },
                    { 6, 3, "+10 FD ODER +1 MSW" },
                    { 6, 4, "Weg der gebundenen Waffe · Schild: Standfest · Klinge: Ausfall · Pfeil: Zielen" },
                    { 6, 5, "Schild: Deckung · Klinge: Entwaffnen · Pfeil: Markieren" },
                    { 6, 6, "+10 TL ODER Schild: Langer Atem · Klinge: +1W10 · Pfeil: Weite" },
                    { 6, 7, "Schild: Spiegelschild · Klinge: Doppelschlag · Pfeil: Kettenschuss" },
                    { 6, 8, "+1W10 ODER +10 TL" },
                    { 6, 9, "+1W10 ODER +10 TL" },
                    { 6, 10, "+20 FD ODER +2 MSW" },
                    { 7, 1, "REA Wurf -1" },
                    { 7, 2, "REA = Vorteil" },
                    { 7, 3, "+1 REA" },
                    { 7, 4, "Schwellengespür: Schutzzauber und Bannkreise an Türen ohne Probe spüren" },
                    { 7, 5, "Zwischenschritt: 1× pro Kampf als Reaktion 2 Felder ausweichen, der Angriff verfehlt" },
                    { 7, 6, "+1 REA ODER REA = Vorteil gegen Magie" },
                    { 7, 7, "Halbschatten: 1× pro Kampf 1 Runde lang haben Angriffe gegen dich Nachteil" },
                    { 7, 8, "REA Wurf -2" },
                    { 7, 9, "REA auch für Verbündete" },
                    { 7, 10, "+2 REA" },
                    { 8, 1, "+10 SEP ODER +10 TL" },
                    { 8, 2, "+10 SEP ODER +10 TL" },
                    { 8, 3, "1 Trank = 2 Schluck ODER +1 MSW" },
                    { 8, 4, "Notdosis: einen Trank trinken ist eine Bonusaktion" },
                    { 8, 5, "Wurftrank: 1× pro Kampf einen Trank auf ein Ziel in 4 Feldern werfen, er wirkt dort" },
                    { 8, 6, "+10 SEP ODER +10 TL" },
                    { 8, 7, "Mischung: 1× pro Mission zwei Tränke zu einem mischen, beide wirken" },
                    { 8, 8, "+10 SEP ODER +10 TL" },
                    { 8, 9, "+10 SEP ODER +10 TL" },
                    { 8, 10, "Tränke wirken doppelt ODER +2 MSW" },
                    { 9, 1, "+1 Zauber ODER +1 Zauberslot" },
                    { 9, 2, "+1 Zauber ODER +1 Zauberslot" },
                    { 9, 3, "+2 Zauber ODER +2 Zauberslots" },
                    { 9, 4, "+1 Zauber ODER +1 Zauberslot" },
                    { 9, 5, "Schnellzauber: 1× pro Kampf einen Zauber als Bonusaktion" },
                    { 9, 6, "+1 Zauber ODER +1 Zauberslot" },
                    { 9, 7, "Doppelzauber: 1× pro Mission einen Zauber zweimal hintereinander, ohne zweiten Slot" },
                    { 9, 8, "+1 Zauber ODER +1 Zauberslot" },
                    { 9, 9, "+1 Zauber ODER +1 Zauberslot" },
                    { 9, 10, "+3 Zauber ODER +3 Zauberslots" }
                });

            migrationBuilder.InsertData(
                schema: "stamm",
                table: "SonderzauberOptionen",
                columns: new[] { "Position", "SonderzauberCode", "Anwendung", "Name", "Schaden", "Slots", "Wirkung" },
                values: new object[,]
                {
                    { 0, "darius-schleim", "Berührung", "Schleimkokon", "+2W10 Heilung", 3, "Anwender hüllt einen Verbündeten in einen weichen Schleimkokon, in dem sich Wunden rasch schließen." },
                    { 1, "darius-schleim", "Berührung", "Gallertpolster", "+10 LP bis zum Ende des Kampfes", 2, "Anwender legt einem Verbündeten eine dicke, federnde Gallertschicht an, die zusätzlich Kraft spendet." },
                    { 2, "darius-schleim", "Berührung", "Reinigungsschleim", "−1 Effekt beim Verbündeten", 2, "Anwender streicht heilenden Schleim über einen Verbündeten und löst so einen schädlichen Einfluss." },
                    { 3, "darius-schleim", "Ritual", "Rutschbahn", "Gegner im Gebiet: nächster Angriff = Nachteil", 3, "Anwender überzieht den Boden am Zielort mit glitschigem Schleim, auf dem Gegner ins Straucheln geraten." },
                    { 0, "elma-fische", "Ritual", "Panzerfisch", "+25 RÜW bis zum nächsten eigenen Zug", 3, "Anwender legt sich einen schweren Knochenpanzer an, an dem selbst harte Treffer abprallen." },
                    { 1, "elma-fische", "Blickkontakt", "Schwarmschutz", "+15 RÜW für den Verbündeten bis zu seinem nächsten Zug", 2, "Anwender lässt einen Schwarm schimmernder Fische um einen Verbündeten kreisen, der Treffer abfängt." },
                    { 2, "elma-fische", "Ritual", "Kugelfisch", "Bei gegnerischem Nahkampf-Treffer: 1W10 Schaden zurück (bis zum nächsten eigenen Zug)", 2, "Anwender bläht sich auf und stellt Stacheln auf, an denen sich Angreifer verletzen." },
                    { 3, "elma-fische", "Ritual", "Schwarmwand", "Blockt den nächsten Fernangriff auf den Anwender komplett", 3, "Anwender ruft einen dichten Schwarm Fische herbei, der sich wie eine lebende Wand vor ihn schiebt und Geschosse abfängt." },
                    { 0, "levi-rauch", "Berührung", "Rauchbund", "Kein eigener Schaden — nächster Ätherwaffen-Treffer +1W10", 2, "Anwender umhüllt die eigene Ätherwaffe mit Rauch, der sich beim nächsten Treffer entlädt." },
                    { 1, "levi-rauch", "Berührung", "Kohlefaust", "3W10", 3, "Anwender verdichtet Rauch zu glühender Kohle und schlägt mit voller Wucht zu." },
                    { 2, "levi-rauch", "Ritual", "Glutnebel", "1W10 pro Zug, 2 Züge lang (Fläche)", 3, "Anwender lässt eine schwelende Rauchwolke am Zielort entstehen, die alle darin stehenden Gegner versengt." },
                    { 3, "levi-rauch", "Sichtkontakt", "Rußwurf", "2W10 — nächster Angriff des Ziels = Nachteil", 2, "Anwender schleudert einen Klumpen heißen Rußes auf einen Gegner, der ihm beißend in die Augen fährt." },
                    { 0, "rigel-kosmos", "Ritual", "Siriusblick", "Kein eigener Schaden — nächste MAGW-Probe = Vorteil", 2, "Anwender taucht die eigenen Augen in kaltes Sternenlicht und erkennt dadurch Verborgenes." },
                    { 1, "rigel-kosmos", "Sichtkontakt", "Anziehung", "Kein eigener Schaden", 2, "Anwender lässt einen Gegenstand in Reichweite wie von einer Gravitation gezogen zu sich heranschweben." },
                    { 2, "rigel-kosmos", "Ritual", "Sternenkarte", "Kein eigener Schaden — Himmelsrichtung und Uhrzeit sind bekannt", 2, "Anwender liest den Sternenhimmel und kennt Himmelsrichtung und ungefähre Uhrzeit, auch bei bedecktem Himmel." },
                    { 3, "rigel-kosmos", "Berührung", "Sternenstaub", "Kein eigener Schaden", 2, "Anwender streut funkelnden Sternenstaub auf einen Gegenstand, der Schmutz, Staub und Flecken verschluckt." },
                    { 0, "svea-euphorie", "Ritual", "Jubelchor", "Alle Verbündeten: nächste Probe = Vorteil", 3, "Anwender stimmt eine ansteckende Hochstimmung an, die alle Verbündeten mitreißt." },
                    { 1, "svea-euphorie", "Berührung", "Überschwang", "Nächster Schadenswurf des Verbündeten +1W10", 2, "Anwender lädt einen Verbündeten mit so viel Begeisterung auf, dass dessen nächster Schlag kräftiger ausfällt." },
                    { 2, "svea-euphorie", "Blickkontakt", "Ansteckendes Lachen", "Nächste soziale Probe = Vorteil", 2, "Anwender bringt ein Gegenüber zum Lachen und öffnet es für ein freundliches Wort." },
                    { 3, "svea-euphorie", "Ritual", "Festtagsstimmung", "Alle Verbündeten: +1W10 Heilung", 3, "Anwender verbreitet ausgelassene Feierlaune, die alle Verbündeten in Sichtweite aufatmen lässt." }
                });

            migrationBuilder.InsertData(
                schema: "stamm",
                table: "Talente",
                columns: new[] { "Code", "BegabungCode", "Name", "Position" },
                values: new object[,]
                {
                    { "h0", "h", "Wahrnehmung", 0 },
                    { "h1", "h", "Magisches Gespür", 1 },
                    { "h2", "h", "Akrobatik", 2 },
                    { "h3", "h", "Motorik", 3 },
                    { "h4", "h", "Verschleierung", 4 },
                    { "h5", "h", "Körperkraft", 5 },
                    { "h6", "h", "Elementarismus", 6 },
                    { "h7", "h", "Silizieren", 7 },
                    { "h8", "h", "Auritieren", 8 },
                    { "h9", "h", "Ätherfechten", 9 },
                    { "s0", "s", "Erkennen", 0 },
                    { "s1", "s", "Überreden", 1 },
                    { "s2", "s", "Kontorieren", 2 },
                    { "s3", "s", "Täuschen", 3 },
                    { "s4", "s", "Erpressen", 4 },
                    { "s5", "s", "Beruhigen", 5 },
                    { "s6", "s", "Ablenken", 6 },
                    { "s7", "s", "Überzeugen", 7 },
                    { "s8", "s", "Reizen", 8 },
                    { "s9", "s", "Erzählen", 9 },
                    { "w0", "w", "Industriewissen", 0 },
                    { "w1", "w", "Fabriksicherheit", 1 },
                    { "w2", "w", "Metallurgie", 2 },
                    { "w3", "w", "Medizin", 3 },
                    { "w4", "w", "Telegrafie", 4 },
                    { "w5", "w", "Stadtwissen", 5 },
                    { "w6", "w", "Literatur", 6 },
                    { "w7", "w", "Etikette", 7 },
                    { "w8", "w", "Tradition", 8 },
                    { "w9", "w", "Schulwissen", 9 }
                });

            migrationBuilder.InsertData(
                schema: "stamm",
                table: "Ags",
                columns: new[] { "Code", "BonusTalentCode", "BonusWert", "Kategorie", "Leitung", "Name", "Untertitel" },
                values: new object[,]
                {
                    { "bote", "s0", 10, "Kunst", "Hr. Zielinski", "Der Hesparis-Bote", "Schülerzeitung" },
                    { "buehne", "s3", 10, "Kunst", "Hr. Sönneberg", "Bühne der Trugbilder", "Theater mit Illusionen" },
                    { "chronik", "w8", 10, "Wissen", "Sekretariat", "Chronik-AG", "Schulgeschichte" },
                    { "duell", "h9", 10, "Sport", "Hr. von Graffenried", "Duellclub", "Freies Ätherfechten" },
                    { "funkenwerk", "w2", 10, "Wissen", "Fr. Krautler", "Funkenwerk", "Junge Forschende" },
                    { "funker", "w4", 10, "Handwerk", "Hr. Haas", "Funkerclub", "Telegrafie und Morsen" },
                    { "gewaechs", "h0", 10, "Handwerk", "Fr. Boeckensteedt", "Gewächshaus-Kreis", "Kräuter und Pflanzen" },
                    { "lernzirkel", "w9", 5, "Wissen", "ältere Jahrgänge · Aufsicht der Bibliothek", "Lernzirkel", "Nachhilfe von Schülern für Schüler" },
                    { "rat", "s7", 10, "Gemeinschaft", "Schulleitung · Vorsitz: Clara Engelhardt (10a)", "Schülerrat", "Schulkomitee" },
                    { "runenlauf", "h2", 10, "Sport", "Hr. Rasmussen-Kukk", "Glimrack-Mannschaft", "Schulteam" },
                    { "sani", "w3", 10, "Gemeinschaft", "Krankenzimmer", "Schulsanitätsdienst", "Erste Hilfe" },
                    { "sterne", "h1", 10, "Wissen", "Fr. Hirschfelt", "Sternenwache", "Nachtbeobachtung" }
                });

            migrationBuilder.InsertData(
                schema: "stamm",
                table: "Meisterschaften",
                columns: new[] { "Id", "Beschreibung", "Name", "Position", "TalentCode" },
                values: new object[,]
                {
                    { 1, "Hören durch Türen und dünne Wände ohne Erschwernis. Lauschen +20.", "Lauscher", 0, "h0" },
                    { 2, "Tränke am Geruch erkennen +20. Bei Erfolg nennt die Spielleitung Name und Wirkung.", "Spürnase", 1, "h0" },
                    { 3, "In Dunkelheit oder blind keine Erschwernis auf Wahrnehmung. Wesen in 2 Feldern werden ohne Probe gespürt.", "Tastsinn", 2, "h0" },
                    { 4, "Aura eines Wesens in Sichtweite ohne Probe lesen. Die Spielleitung nennt seine Stimmung in einem Wort.", "Aurenblick", 0, "h1" },
                    { 5, "Wesen in 5 Feldern ohne Probe bemerken, auch unsichtbar oder hinter Wänden. Man erfährt Anzahl und Richtung.", "Präsenz", 1, "h1" },
                    { 6, "1× pro Sitzung eine Ja/Nein-Frage über einen bekannten Ort oder eine bekannte Person. Die Antwort ist wahr.", "Hellsicht", 2, "h1" },
                    { 7, "Fallschaden −2W10.", "Katzenlandung", 0, "h2" },
                    { 8, "1× pro Kampf als Reaktion: einen Nahkampftreffer mit Akrobatik-Probe vollständig vermeiden.", "Ausweichrolle", 1, "h2" },
                    { 9, "Sprungweite +2 Felder. Über Gegner springen löst keinen Gelegenheitsangriff aus.", "Hechtsprung", 2, "h2" },
                    { 10, "Taschendiebstahl +20.", "Flinke Finger", 0, "h3" },
                    { 11, "Schlösser öffnen +20, in halber Zeit.", "Schlossknacker", 1, "h3" },
                    { 12, "Autonome Kutschen ohne Probe fahren. In brenzligen Lagen (Verfolgung, Ausweichen) +20.", "Kutscher", 2, "h3" },
                    { 13, "Schleichen +20.", "Leisetritt", 0, "h4" },
                    { 14, "Im Halbdunkel ohne Deckung verstecken möglich. Verstecken +20.", "Schattenkleid", 1, "h4" },
                    { 15, "Die erste Aktion aus dem Verborgenen hat Vorteil.", "Überrumpeln", 2, "h4" },
                    { 16, "Traglast ×2.", "Packesel", 0, "h5" },
                    { 17, "Festhalten und Befreien mit Vorteil.", "Eisengriff", 1, "h5" },
                    { 18, "Türen und Kisten aufbrechen +30.", "Rammbock", 2, "h5" },
                    { 19, "Kein Schaden durch das eigene Element.", "Elementfreund", 0, "h6" },
                    { 20, "1× pro Kampf das Element eines eigenen Zaubers ändern.", "Wechselspiel", 1, "h6" },
                    { 21, "Kritische Elementarzauber +1W10 Schaden.", "Urgewalt", 2, "h6" },
                    { 22, "Einen verzauberten Gegenstand mit einer Silizieren-Probe 1 Stunde lang magisch stilllegen.", "Entzaubern", 0, "h7" },
                    { 23, "Silizieren ohne Geste und Wort. Andere bemerken es nur mit Magischem Gespür −20.", "Stille Hände", 1, "h7" },
                    { 24, "1× pro Kampf Bannen als Reaktion, bevor der gegnerische Zauber wirkt.", "Gegenbann", 2, "h7" },
                    { 25, "Auritierte Zauber machen +1W10 zusätzlichen Schaden oder heilen +1W10 mehr.", "Vollgas", 0, "h8" },
                    { 26, "Auritierte Zauber haben +2 Felder Reichweite.", "Weiter Hall", 1, "h8" },
                    { 27, "Auritierte Zauber mit Dauer halten 1 Runde länger.", "Lange Glut", 2, "h8" },
                    { 28, "1× pro Kampf: Schaden ×2, vor dem Schadenswurf ansagen. Mit jeder Ätherwaffe.", "Heldenhieb", 0, "h9" },
                    { 29, "1× pro Kampf: der Angriff trifft zusätzlich ein zweites Ziel, das höchstens 1 Feld vom ersten entfernt steht, mit vollem Schaden. Mit jeder Ätherwaffe.", "Zweierschlag", 1, "h9" },
                    { 30, "1× pro Kampf: ein Treffer schiebt das Ziel 2 Felder weg oder zieht es 2 Felder heran. Mit jeder Ätherwaffe.", "Rückstoß", 2, "h9" },
                    { 31, "Zweck einer unbekannten Maschine ohne Probe erkennen.", "Maschinenkenner", 0, "w0" },
                    { 32, "Angriffe gegen Maschinen und Konstrukte +2 Krit.", "Schwachstelle", 1, "w0" },
                    { 33, "1× pro Szene läuft eine kaputte Maschine für 10 Minuten.", "Notbetrieb", 2, "w0" },
                    { 34, "Fallen und Gefahrenstellen erkennen +30.", "Gefahrenblick", 0, "w1" },
                    { 35, "Eine Maschine mit 1 Aktion ohne Probe stoppen.", "Not-Aus", 1, "w1" },
                    { 36, "Die Gruppe nimmt −1W10 Schaden durch Unfälle und Umgebung.", "Absicherung", 2, "w1" },
                    { 37, "Brauproben +20.", "Sauberer Sud", 0, "w2" },
                    { 38, "Ein gelungener Brauwurf ergibt 2 Tränke statt 1.", "Doppelter Kessel", 1, "w2" },
                    { 39, "Bei einem kritischen Erfolg beim Brauen wirkt der Trank doppelt.", "Meisterbrau", 2, "w2" },
                    { 40, "Erste Hilfe als Bonusaktion.", "Feldarzt", 0, "w3" },
                    { 41, "Die Wirkung eines getrunkenen Tranks ohne Probe erkennen.", "Diagnose", 1, "w3" },
                    { 42, "Behandelte Figuren heilen bei der nächsten Rast +1W10 LP.", "Wundnaht", 2, "w3" },
                    { 43, "Morsezeichen ohne Probe verstehen.", "Morsegehör", 0, "w4" },
                    { 44, "Telegramme an einer Leitung abfangen +20.", "Leitungslauscher", 1, "w4" },
                    { 45, "Fremde Chiffren knacken +20.", "Chiffreur", 2, "w4" },
                    { 46, "In bekannten Städten halbe Wegzeit.", "Ortskenntnis", 0, "w5" },
                    { 47, "1× pro Stadt einen Kontakt benennen, den die Figur kennt.", "Kontakte", 1, "w5" },
                    { 48, "Dinge besorgen, die es in der Schule nicht gibt +20. Lieferung am nächsten Tag.", "Besorger", 2, "w5" },
                    { 49, "Ein Buch in 1 Stunde lesen.", "Schnellleser", 0, "w6" },
                    { 50, "1× pro Sitzung: die Spielleitung sagt, ob die Bibliothek ein Buch zu einem Thema hat.", "Querverweis", 1, "w6" },
                    { 51, "Alte und fremde Schriften lesen +20.", "Gelehrtenzunge", 2, "w6" },
                    { 52, "Etikette bei offiziellen Anlässen +20.", "Parkett", 0, "w7" },
                    { 53, "Rang und Herkunft einer Person ohne Probe erkennen.", "Rangkenner", 1, "w7" },
                    { 54, "1× pro Mission Zutritt zu einer geschlossenen Veranstaltung.", "Einladung", 2, "w7" },
                    { 55, "Bräuche und Feiertage ohne Probe kennen.", "Brauchtum", 0, "w8" },
                    { 56, "1× pro Mission gibt die Spielleitung einen Hinweis aus alten Geschichten.", "Ahnenwissen", 1, "w8" },
                    { 57, "Zauberproben für Zauber mit Anwendung „Ritual“ +20.", "Ritualmeister", 2, "w8" },
                    { 58, "Soziale Proben gegenüber Lehrkräften +20.", "Lehrerliebling", 0, "w9" },
                    { 59, "1× pro Sitzung eine Wissensprobe mit Schulwissen statt mit dem eigentlichen Talent würfeln.", "Spickzettel", 1, "w9" },
                    { 60, "1× pro Schuljahr eine Note um eine Stufe verbessern.", "Streber", 2, "w9" },
                    { 61, "Lügen erkennen mit Vorteil.", "Lügenblick", 0, "s0" },
                    { 62, "Grundhaltung des Gegenübers ohne Probe erkennen: freundlich, neutral oder feindlich.", "Stimmungsbild", 1, "s0" },
                    { 63, "Bei Erfolg: die nächste soziale Probe der Gruppe gegen diese Person +20.", "Wunder Punkt", 2, "s0" },
                    { 64, "Im Kampf als Bonusaktion. Bei Erfolg hat der nächste Angriff des Ziels Nachteil.", "Schnelles Mundwerk", 0, "s1" },
                    { 65, "1× pro Szene ein misslungenes Überreden wiederholen.", "Zweite Chance", 1, "s1" },
                    { 66, "Überredete Personen halten ihr Wort bis zum Ende der Szene.", "Handschlag", 2, "s1" },
                    { 67, "Preise −20 %.", "Feilscher", 0, "s2" },
                    { 68, "Den Wert einer Ware ohne Probe kennen.", "Warenkunde", 1, "s2" },
                    { 69, "1 Gegenstand pro Woche bei bekannten Händlern auf Pump.", "Kreditwürdig", 2, "s2" },
                    { 70, "Erkennen-Proben gegen dich −20.", "Pokerface", 0, "s3" },
                    { 71, "Verkleiden +20.", "Maske", 1, "s3" },
                    { 72, "1× pro Kampf: der eigene nächste Angriff hat Vorteil.", "Finte", 2, "s3" },
                    { 73, "Bei Erfolg erfährt man zusätzlich ein Geheimnis des Ziels.", "Druckmittel", 0, "s4" },
                    { 74, "Erpressen gegen Jüngere +20.", "Großer Schatten", 1, "s4" },
                    { 75, "Erpresste verraten dich nicht, solange die Erpressung läuft.", "Schweigegeld", 2, "s4" },
                    { 76, "Panik bei einem Verbündeten ohne Probe aufheben.", "Ruhepol", 0, "s5" },
                    { 77, "Tiere beruhigen +20.", "Tierflüsterer", 1, "s5" },
                    { 78, "1× pro Mission pausiert ein Kampf für 1 Runde, alle dürfen reden.", "Waffenruhe", 2, "s5" },
                    { 79, "Ablenken als Bonusaktion.", "Taschenspieler", 0, "s6" },
                    { 80, "Bei Erfolg kann sich ein Verbündeter 1 Runde lang unbemerkt bewegen.", "Lockvogel", 1, "s6" },
                    { 81, "Bei Erfolg ist die Aufsicht (Lehrkräfte, Vertrauensschüler) 10 Minuten weg.", "Fehlalarm", 2, "s6" },
                    { 82, "Eine Probe gilt für alle Zuhörer.", "Redner", 0, "s7" },
                    { 83, "Mit einem Beweis +20.", "Faktenlage", 1, "s7" },
                    { 84, "Überzeugte Verbündete: nächste Probe +10.", "Mitreißend", 2, "s7" },
                    { 85, "Provozieren: Ein gereiztes Ziel greift 1 Runde lang nur dich an.", "Weißglut", 0, "s8" },
                    { 86, "Flirten: Eine erfolgreich angeflirtete Person erfüllt dir eine kleine Bitte ohne weitere Probe.", "Charmeur", 1, "s8" },
                    { 87, "Misslingt Reizen, darfst du es sofort 1× als das andere wiederholen: aus dem Flirt wird eine Provokation oder umgekehrt.", "Doppeltes Spiel", 2, "s8" },
                    { 88, "Eine Rast mit Geschichte: die Gruppe heilt +1W10 LP.", "Lagerfeuer", 0, "s9" },
                    { 89, "Ein Gerücht ist am nächsten Tag in der ganzen Schule bekannt.", "Gerücht", 1, "s9" },
                    { 90, "Die erste soziale Probe in einer neuen Umgebung +20.", "Legende", 2, "s9" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgEintraege_AgCode",
                table: "AgEintraege",
                column: "AgCode");

            migrationBuilder.CreateIndex(
                name: "IX_AgEintraege_KampagneId_AgCode",
                table: "AgEintraege",
                columns: new[] { "KampagneId", "AgCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgMitglieder_SpielfigurId",
                table: "AgMitglieder",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_Ags_BonusTalentCode",
                schema: "stamm",
                table: "Ags",
                column: "BonusTalentCode");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivDokumente_KampagneId",
                table: "ArchivDokumente",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivDokumentKategorien_ArchivKategorieId",
                table: "ArchivDokumentKategorien",
                column: "ArchivKategorieId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivKategorien_KampagneId_Code",
                table: "ArchivKategorien",
                columns: new[] { "KampagneId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Aufgaben_KampagneId",
                table: "Aufgaben",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_Beziehungen_SpielfigurId",
                table: "Beziehungen",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_Braueintraege_KampagneId",
                table: "Braueintraege",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_Braueintraege_SpielfigurId",
                table: "Braueintraege",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_DuellClubBilanzen_KampagneId",
                table: "DuellClubBilanzen",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_DuellClubBilanzen_SpielfigurId",
                table: "DuellClubBilanzen",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_Essenskaeufe_GerichtCode",
                table: "Essenskaeufe",
                column: "GerichtCode");

            migrationBuilder.CreateIndex(
                name: "IX_Essenskaeufe_SpielfigurId_Datum_Mahlzeit",
                table: "Essenskaeufe",
                columns: new[] { "SpielfigurId", "Datum", "Mahlzeit" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Essenswirkungen_SpielfigurId",
                table: "Essenswirkungen",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_Faecher_Name",
                schema: "stamm",
                table: "Faecher",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gegenstaende_Effekt_TalentCode",
                table: "Gegenstaende",
                column: "Effekt_TalentCode");

            migrationBuilder.CreateIndex(
                name: "IX_Gegenstaende_SpielfigurId",
                table: "Gegenstaende",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_Geldbuchungen_SpielfigurId",
                table: "Geldbuchungen",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_GlimAufstellung_KampagneId_Slot",
                table: "GlimAufstellung",
                columns: new[] { "KampagneId", "Slot" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlimBank_KampagneId_Position",
                table: "GlimBank",
                columns: new[] { "KampagneId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlimCupArchiv_KampagneId",
                table: "GlimCupArchiv",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_GlimCupArchiv_MeisterTeamCode",
                table: "GlimCupArchiv",
                column: "MeisterTeamCode");

            migrationBuilder.CreateIndex(
                name: "IX_GlimKarrieren_KampagneId_Name",
                table: "GlimKarrieren",
                columns: new[] { "KampagneId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlimNachwuchs_KampagneId_Slot",
                table: "GlimNachwuchs",
                columns: new[] { "KampagneId", "Slot" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlimNachwuchs_MitschuelerId",
                table: "GlimNachwuchs",
                column: "MitschuelerId");

            migrationBuilder.CreateIndex(
                name: "IX_GlimPlayoffs_GastTeamCode",
                table: "GlimPlayoffs",
                column: "GastTeamCode");

            migrationBuilder.CreateIndex(
                name: "IX_GlimPlayoffs_HeimTeamCode",
                table: "GlimPlayoffs",
                column: "HeimTeamCode");

            migrationBuilder.CreateIndex(
                name: "IX_GlimPlayoffs_KampagneId_Runde",
                table: "GlimPlayoffs",
                columns: new[] { "KampagneId", "Runde" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlimSaisonErgebnisse_KampagneId_Fixture",
                table: "GlimSaisonErgebnisse",
                columns: new[] { "KampagneId", "Fixture" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlimSaisonStatistiken_KampagneId_Name",
                table: "GlimSaisonStatistiken",
                columns: new[] { "KampagneId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlimSpiele_GegnerTeamCode",
                table: "GlimSpiele",
                column: "GegnerTeamCode");

            migrationBuilder.CreateIndex(
                name: "IX_GlimSpiele_KampagneId",
                table: "GlimSpiele",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_GlimSpieler_KampagneId_Code",
                table: "GlimSpieler",
                columns: new[] { "KampagneId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlimSpielLog_GlimSpielId",
                table: "GlimSpielLog",
                column: "GlimSpielId");

            migrationBuilder.CreateIndex(
                name: "IX_GlimSpielStatistiken_GlimSpielId",
                table: "GlimSpielStatistiken",
                column: "GlimSpielId");

            migrationBuilder.CreateIndex(
                name: "IX_GlimTeamStaerken_KampagneId_TeamCode",
                table: "GlimTeamStaerken",
                columns: new[] { "KampagneId", "TeamCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlimTeamStaerken_TeamCode",
                table: "GlimTeamStaerken",
                column: "TeamCode");

            migrationBuilder.CreateIndex(
                name: "IX_GlimTrikotnummern_KampagneId_Mitglied",
                table: "GlimTrikotnummern",
                columns: new[] { "KampagneId", "Mitglied" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlimVerletzungen_KampagneId_Name",
                table: "GlimVerletzungen",
                columns: new[] { "KampagneId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeftAblauf_ProtokollheftId",
                table: "HeftAblauf",
                column: "ProtokollheftId");

            migrationBuilder.CreateIndex(
                name: "IX_HeftFehlend_SpielfigurId",
                table: "HeftFehlend",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_HeftNotizen_ProtokollheftId",
                table: "HeftNotizen",
                column: "ProtokollheftId");

            migrationBuilder.CreateIndex(
                name: "IX_HeftPersonen_ProtokollheftId",
                table: "HeftPersonen",
                column: "ProtokollheftId");

            migrationBuilder.CreateIndex(
                name: "IX_KalenderTermine_KampagneId_Datum",
                table: "KalenderTermine",
                columns: new[] { "KampagneId", "Datum" });

            migrationBuilder.CreateIndex(
                name: "IX_Kampagnen_GameWorldId",
                table: "Kampagnen",
                column: "GameWorldId");

            migrationBuilder.CreateIndex(
                name: "IX_KartenNotizen_KampagneId_Ort",
                table: "KartenNotizen",
                columns: new[] { "KampagneId", "Ort" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KartenZeiten_KampagneId_Schluessel",
                table: "KartenZeiten",
                columns: new[] { "KampagneId", "Schluessel" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Klassenarbeiten_FachId",
                table: "Klassenarbeiten",
                column: "FachId");

            migrationBuilder.CreateIndex(
                name: "IX_Klassenarbeiten_KampagneId",
                table: "Klassenarbeiten",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_KlassenarbeitMitschueler_MitschuelerId",
                table: "KlassenarbeitMitschueler",
                column: "MitschuelerId");

            migrationBuilder.CreateIndex(
                name: "IX_KlassenarbeitSpielfiguren_SpielfigurId",
                table: "KlassenarbeitSpielfiguren",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_KrankenGlimGemeldet_KampagneId_Name",
                table: "KrankenGlimGemeldet",
                columns: new[] { "KampagneId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Krankheitsfaelle_KampagneId",
                table: "Krankheitsfaelle",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_Krankheitsfaelle_SpielfigurId",
                table: "Krankheitsfaelle",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_LadenArtikel_Effekt_TalentCode",
                table: "LadenArtikel",
                column: "Effekt_TalentCode");

            migrationBuilder.CreateIndex(
                name: "IX_LadenArtikel_KampagneId",
                table: "LadenArtikel",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_Lehrer_KampagneId_Name",
                table: "Lehrer",
                columns: new[] { "KampagneId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LehrerFaecher_FachId",
                table: "LehrerFaecher",
                column: "FachId");

            migrationBuilder.CreateIndex(
                name: "IX_LehrerFakten_LehrerId",
                table: "LehrerFakten",
                column: "LehrerId");

            migrationBuilder.CreateIndex(
                name: "IX_Meisterschaften_TalentCode_Position",
                schema: "stamm",
                table: "Meisterschaften",
                columns: new[] { "TalentCode", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Missionen_KampagneId",
                table: "Missionen",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_MissionHinweise_MissionId",
                table: "MissionHinweise",
                column: "MissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Mitschueler_HassfachId",
                table: "Mitschueler",
                column: "HassfachId");

            migrationBuilder.CreateIndex(
                name: "IX_Mitschueler_KampagneId_Code",
                table: "Mitschueler",
                columns: new[] { "KampagneId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mitschueler_LieblingsfachId",
                table: "Mitschueler",
                column: "LieblingsfachId");

            migrationBuilder.CreateIndex(
                name: "IX_Mitschueler_SpielfigurId",
                table: "Mitschueler",
                column: "SpielfigurId",
                unique: true,
                filter: "[SpielfigurId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MitschuelerAgs_AgCode",
                table: "MitschuelerAgs",
                column: "AgCode");

            migrationBuilder.CreateIndex(
                name: "IX_Nachsitzen_KampagneId",
                table: "Nachsitzen",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_Nachsitzen_LehrerId",
                table: "Nachsitzen",
                column: "LehrerId");

            migrationBuilder.CreateIndex(
                name: "IX_Nachsitzen_MissionId",
                table: "Nachsitzen",
                column: "MissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Nachsitzen_SpielfigurId",
                table: "Nachsitzen",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_ProtokollBeteiligte_SpielfigurId",
                table: "ProtokollBeteiligte",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_ProtokollEintraege_KampagneId",
                table: "ProtokollEintraege",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_Protokollhefte_BegleiterId",
                table: "Protokollhefte",
                column: "BegleiterId");

            migrationBuilder.CreateIndex(
                name: "IX_Protokollhefte_KalenderTerminId",
                table: "Protokollhefte",
                column: "KalenderTerminId");

            migrationBuilder.CreateIndex(
                name: "IX_Protokollhefte_KampagneId",
                table: "Protokollhefte",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_Protokollhefte_SchreiberMitschuelerId",
                table: "Protokollhefte",
                column: "SchreiberMitschuelerId");

            migrationBuilder.CreateIndex(
                name: "IX_Protokollhefte_SchreiberSpielfigurId",
                table: "Protokollhefte",
                column: "SchreiberSpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_RatAntraege_KampagneId_Monat",
                table: "RatAntraege",
                columns: new[] { "KampagneId", "Monat" });

            migrationBuilder.CreateIndex(
                name: "IX_RatGefallen_MitschuelerId",
                table: "RatGefallen",
                column: "MitschuelerId");

            migrationBuilder.CreateIndex(
                name: "IX_RatGefallen_SpielfigurId",
                table: "RatGefallen",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_RatPlanungen_KampagneId_Monat_Slot",
                table: "RatPlanungen",
                columns: new[] { "KampagneId", "Monat", "Slot" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RatRegeln_KampagneId",
                table: "RatRegeln",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_RatRegeln_RatAntragId",
                table: "RatRegeln",
                column: "RatAntragId");

            migrationBuilder.CreateIndex(
                name: "IX_RatTagesordnungspunkte_KampagneId",
                table: "RatTagesordnungspunkte",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_RatUnterschriften_MitschuelerId",
                table: "RatUnterschriften",
                column: "MitschuelerId");

            migrationBuilder.CreateIndex(
                name: "IX_RatVerbrauchteAntraege_KampagneId_Schluessel",
                table: "RatVerbrauchteAntraege",
                columns: new[] { "KampagneId", "Schluessel" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rezepte_KampagneId",
                table: "Rezepte",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_RezeptZutaten_RezeptId",
                table: "RezeptZutaten",
                column: "RezeptId");

            migrationBuilder.CreateIndex(
                name: "IX_SchuljahrBilanzen_KampagneId_Jahr",
                table: "SchuljahrBilanzen",
                columns: new[] { "KampagneId", "Jahr" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchuljahrBilanzSpielfiguren_SpielfigurId",
                table: "SchuljahrBilanzSpielfiguren",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_SchuljahrZeugnisnoten_FachId",
                table: "SchuljahrZeugnisnoten",
                column: "FachId");

            migrationBuilder.CreateIndex(
                name: "IX_SchuljahrZeugnisnoten_SpielfigurId",
                table: "SchuljahrZeugnisnoten",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_Schulnoten_FachId",
                table: "Schulnoten",
                column: "FachId");

            migrationBuilder.CreateIndex(
                name: "IX_Schulnoten_SpielfigurId_FachId_Position",
                table: "Schulnoten",
                columns: new[] { "SpielfigurId", "FachId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchwarzmarktVerkaeufe_KampagneId",
                table: "SchwarzmarktVerkaeufe",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_SchwarzmarktWaren_Effekt_TalentCode",
                table: "SchwarzmarktWaren",
                column: "Effekt_TalentCode");

            migrationBuilder.CreateIndex(
                name: "IX_SchwarzmarktWaren_KampagneId",
                table: "SchwarzmarktWaren",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_SonderzauberStaende_KampagneId_SonderzauberCode",
                table: "SonderzauberStaende",
                columns: new[] { "KampagneId", "SonderzauberCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SonderzauberStaende_SonderzauberCode",
                table: "SonderzauberStaende",
                column: "SonderzauberCode");

            migrationBuilder.CreateIndex(
                name: "IX_SpeiseplanEintraege_GerichtCode",
                table: "SpeiseplanEintraege",
                column: "GerichtCode");

            migrationBuilder.CreateIndex(
                name: "IX_SpeiseplanEintraege_KampagneId_Woche_Wochentag_Mahlzeit",
                table: "SpeiseplanEintraege",
                columns: new[] { "KampagneId", "Woche", "Wochentag", "Mahlzeit" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spielfiguren_CharacterId",
                table: "Spielfiguren",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Spielfiguren_KampagneId_CharacterId",
                table: "Spielfiguren",
                columns: new[] { "KampagneId", "CharacterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spielfiguren_KampagneId_Position",
                table: "Spielfiguren",
                columns: new[] { "KampagneId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpielfigurFachStufen_FachId",
                table: "SpielfigurFachStufen",
                column: "FachId");

            migrationBuilder.CreateIndex(
                name: "IX_SpielfigurMeisterschaften_MeisterschaftDefinitionId",
                table: "SpielfigurMeisterschaften",
                column: "MeisterschaftDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_StufenWahlen_FachId",
                table: "StufenWahlen",
                column: "FachId");

            migrationBuilder.CreateIndex(
                name: "IX_Talente_BegabungCode_Position",
                schema: "stamm",
                table: "Talente",
                columns: new[] { "BegabungCode", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TalentWerte_TalentCode",
                table: "TalentWerte",
                column: "TalentCode");

            migrationBuilder.CreateIndex(
                name: "IX_TalentZiele_TalentCode",
                table: "TalentZiele",
                column: "TalentCode");

            migrationBuilder.CreateIndex(
                name: "IX_TeamNotizen_KampagneId_TeamCode",
                table: "TeamNotizen",
                columns: new[] { "KampagneId", "TeamCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamNotizen_TeamCode",
                table: "TeamNotizen",
                column: "TeamCode");

            migrationBuilder.CreateIndex(
                name: "IX_Waffenwuerfe_KampagneId",
                table: "Waffenwuerfe",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_Wettbewerbe_KampagneId",
                table: "Wettbewerbe",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_WettbewerbPlatzierungen_TeamCode",
                table: "WettbewerbPlatzierungen",
                column: "TeamCode");

            migrationBuilder.CreateIndex(
                name: "IX_WettkampfErgebnisse_KampagneId",
                table: "WettkampfErgebnisse",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_WettkampfErgebnisse_SiegerTeamCode",
                table: "WettkampfErgebnisse",
                column: "SiegerTeamCode");

            migrationBuilder.CreateIndex(
                name: "IX_WettkampfNpcs_KampagneId_Art_Code",
                table: "WettkampfNpcs",
                columns: new[] { "KampagneId", "Art", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WettkampfStaende_KampagneId_Art",
                table: "WettkampfStaende",
                columns: new[] { "KampagneId", "Art" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WettkampfTeilnehmer_KampagneId",
                table: "WettkampfTeilnehmer",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_WettkampfTeilnehmer_SpielfigurId",
                table: "WettkampfTeilnehmer",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_WettkampfZeugnisEintraege_WettkampfZeugnisId",
                table: "WettkampfZeugnisEintraege",
                column: "WettkampfZeugnisId");

            migrationBuilder.CreateIndex(
                name: "IX_WettkampfZeugnisse_KampagneId_Stichtag",
                table: "WettkampfZeugnisse",
                columns: new[] { "KampagneId", "Stichtag" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wirkungen_Effekt_TalentCode",
                table: "Wirkungen",
                column: "Effekt_TalentCode");

            migrationBuilder.CreateIndex(
                name: "IX_Wirkungen_SpielfigurId",
                table: "Wirkungen",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_ZirkelFelder_KampagneId_Nr_Seite_Feld",
                table: "ZirkelFelder",
                columns: new[] { "KampagneId", "Nr", "Seite", "Feld" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ZirkelMordfaelle_KampagneId",
                table: "ZirkelMordfaelle",
                column: "KampagneId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ZirkelNotizen_KampagneId",
                table: "ZirkelNotizen",
                column: "KampagneId");

            migrationBuilder.CreateIndex(
                name: "IX_ZirkelNotizen_SpielfigurId",
                table: "ZirkelNotizen",
                column: "SpielfigurId");

            migrationBuilder.CreateIndex(
                name: "IX_ZirkelPersonen_KampagneId_Nr_Seite",
                table: "ZirkelPersonen",
                columns: new[] { "KampagneId", "Nr", "Seite" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ZirkelVersuche_KampagneId",
                table: "ZirkelVersuche",
                column: "KampagneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgMitglieder");

            migrationBuilder.DropTable(
                name: "ArchivDokumentKategorien");

            migrationBuilder.DropTable(
                name: "Aufgaben");

            migrationBuilder.DropTable(
                name: "Beziehungen");

            migrationBuilder.DropTable(
                name: "Braueintraege");

            migrationBuilder.DropTable(
                name: "DuellClubBilanzen");

            migrationBuilder.DropTable(
                name: "Essenskaeufe");

            migrationBuilder.DropTable(
                name: "Essenswirkungen");

            migrationBuilder.DropTable(
                name: "FachStufen",
                schema: "stamm");

            migrationBuilder.DropTable(
                name: "Gegenstaende");

            migrationBuilder.DropTable(
                name: "Geldbuchungen");

            migrationBuilder.DropTable(
                name: "GlimAufstellung");

            migrationBuilder.DropTable(
                name: "GlimBank");

            migrationBuilder.DropTable(
                name: "GlimCupArchiv");

            migrationBuilder.DropTable(
                name: "GlimKarriereCups");

            migrationBuilder.DropTable(
                name: "GlimNachwuchsTraining");

            migrationBuilder.DropTable(
                name: "GlimPlayoffs");

            migrationBuilder.DropTable(
                name: "GlimSaisonErgebnisse");

            migrationBuilder.DropTable(
                name: "GlimSaisonStatistiken");

            migrationBuilder.DropTable(
                name: "GlimSpielLog");

            migrationBuilder.DropTable(
                name: "GlimSpielStatistiken");

            migrationBuilder.DropTable(
                name: "GlimTeamStaerken");

            migrationBuilder.DropTable(
                name: "GlimTraining");

            migrationBuilder.DropTable(
                name: "GlimTrikotnummern");

            migrationBuilder.DropTable(
                name: "GlimVerletzungen");

            migrationBuilder.DropTable(
                name: "HeftAblauf");

            migrationBuilder.DropTable(
                name: "HeftAbschnitte");

            migrationBuilder.DropTable(
                name: "HeftChecks");

            migrationBuilder.DropTable(
                name: "HeftFehlend");

            migrationBuilder.DropTable(
                name: "HeftFragen");

            migrationBuilder.DropTable(
                name: "HeftJederEintraege");

            migrationBuilder.DropTable(
                name: "HeftNotizUebernahmen");

            migrationBuilder.DropTable(
                name: "HeftPersonen");

            migrationBuilder.DropTable(
                name: "HeftSchwerpunkte");

            migrationBuilder.DropTable(
                name: "KartenNotizen");

            migrationBuilder.DropTable(
                name: "KartenZeiten");

            migrationBuilder.DropTable(
                name: "KlassenarbeitMitschueler");

            migrationBuilder.DropTable(
                name: "KlassenarbeitSpielfiguren");

            migrationBuilder.DropTable(
                name: "KrankenGlimGemeldet");

            migrationBuilder.DropTable(
                name: "Krankheitsfaelle");

            migrationBuilder.DropTable(
                name: "LadenArtikel");

            migrationBuilder.DropTable(
                name: "LehrerFaecher");

            migrationBuilder.DropTable(
                name: "LehrerFakten");

            migrationBuilder.DropTable(
                name: "MissionHinweise");

            migrationBuilder.DropTable(
                name: "MitschuelerAgs");

            migrationBuilder.DropTable(
                name: "MitschuelerMeinungen");

            migrationBuilder.DropTable(
                name: "Nachsitzen");

            migrationBuilder.DropTable(
                name: "ProtokollBeteiligte");

            migrationBuilder.DropTable(
                name: "Pruefungseintraege");

            migrationBuilder.DropTable(
                name: "RatGefallen");

            migrationBuilder.DropTable(
                name: "RatPlanungen");

            migrationBuilder.DropTable(
                name: "RatRegeln");

            migrationBuilder.DropTable(
                name: "RatStimmen");

            migrationBuilder.DropTable(
                name: "RatTagesordnungspunkte");

            migrationBuilder.DropTable(
                name: "RatUnterschriften");

            migrationBuilder.DropTable(
                name: "RatVerbrauchteAntraege");

            migrationBuilder.DropTable(
                name: "RezeptZutaten");

            migrationBuilder.DropTable(
                name: "SchuljahrAbschnitte");

            migrationBuilder.DropTable(
                name: "SchuljahrBilanzSpielfiguren");

            migrationBuilder.DropTable(
                name: "SchuljahrZeugnisnoten");

            migrationBuilder.DropTable(
                name: "Schulnoten");

            migrationBuilder.DropTable(
                name: "SchwarzmarktVerkaeufe");

            migrationBuilder.DropTable(
                name: "SchwarzmarktWaren");

            migrationBuilder.DropTable(
                name: "SonderzauberAufgedeckt");

            migrationBuilder.DropTable(
                name: "SonderzauberOptionen",
                schema: "stamm");

            migrationBuilder.DropTable(
                name: "SpeiseplanEintraege");

            migrationBuilder.DropTable(
                name: "SpielfigurFachStufen");

            migrationBuilder.DropTable(
                name: "SpielfigurMeisterschaften");

            migrationBuilder.DropTable(
                name: "StufenWahlen");

            migrationBuilder.DropTable(
                name: "TalentWerte");

            migrationBuilder.DropTable(
                name: "TalentZiele");

            migrationBuilder.DropTable(
                name: "TeamNotizen");

            migrationBuilder.DropTable(
                name: "Waffenwuerfe");

            migrationBuilder.DropTable(
                name: "WettbewerbPlatzierungen");

            migrationBuilder.DropTable(
                name: "WettkampfErgebnisse");

            migrationBuilder.DropTable(
                name: "WettkampfNpcWerte");

            migrationBuilder.DropTable(
                name: "WettkampfStaende");

            migrationBuilder.DropTable(
                name: "WettkampfTeilnehmer");

            migrationBuilder.DropTable(
                name: "WettkampfZeugnisEintraege");

            migrationBuilder.DropTable(
                name: "Wirkungen");

            migrationBuilder.DropTable(
                name: "ZirkelFelder");

            migrationBuilder.DropTable(
                name: "ZirkelMordfaelle");

            migrationBuilder.DropTable(
                name: "ZirkelNotizen");

            migrationBuilder.DropTable(
                name: "ZirkelPersonen");

            migrationBuilder.DropTable(
                name: "ZirkelVersuche");

            migrationBuilder.DropTable(
                name: "AgEintraege");

            migrationBuilder.DropTable(
                name: "ArchivDokumente");

            migrationBuilder.DropTable(
                name: "ArchivKategorien");

            migrationBuilder.DropTable(
                name: "GlimKarrieren");

            migrationBuilder.DropTable(
                name: "GlimNachwuchs");

            migrationBuilder.DropTable(
                name: "GlimSpiele");

            migrationBuilder.DropTable(
                name: "GlimSpieler");

            migrationBuilder.DropTable(
                name: "HeftNotizen");

            migrationBuilder.DropTable(
                name: "Klassenarbeiten");

            migrationBuilder.DropTable(
                name: "Lehrer");

            migrationBuilder.DropTable(
                name: "Missionen");

            migrationBuilder.DropTable(
                name: "ProtokollEintraege");

            migrationBuilder.DropTable(
                name: "RatAntraege");

            migrationBuilder.DropTable(
                name: "Rezepte");

            migrationBuilder.DropTable(
                name: "SchuljahrBilanzen");

            migrationBuilder.DropTable(
                name: "SonderzauberStaende");

            migrationBuilder.DropTable(
                name: "Gerichte",
                schema: "stamm");

            migrationBuilder.DropTable(
                name: "Meisterschaften",
                schema: "stamm");

            migrationBuilder.DropTable(
                name: "Wettbewerbe");

            migrationBuilder.DropTable(
                name: "WettkampfNpcs");

            migrationBuilder.DropTable(
                name: "WettkampfZeugnisse");

            migrationBuilder.DropTable(
                name: "Ags",
                schema: "stamm");

            migrationBuilder.DropTable(
                name: "AkademieTeams",
                schema: "stamm");

            migrationBuilder.DropTable(
                name: "Protokollhefte");

            migrationBuilder.DropTable(
                name: "Sonderzauber",
                schema: "stamm");

            migrationBuilder.DropTable(
                name: "Talente",
                schema: "stamm");

            migrationBuilder.DropTable(
                name: "KalenderTermine");

            migrationBuilder.DropTable(
                name: "Mitschueler");

            migrationBuilder.DropTable(
                name: "Begabungen",
                schema: "stamm");

            migrationBuilder.DropTable(
                name: "Faecher",
                schema: "stamm");

            migrationBuilder.DropTable(
                name: "Spielfiguren");

            migrationBuilder.DropTable(
                name: "Kampagnen");
        }
    }
}
