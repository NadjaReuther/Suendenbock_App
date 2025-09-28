using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class NewInitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Abenteuerraenge",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abenteuerraenge", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Anmeldungsstati",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anmeldungsstati", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blutgruppen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Besonderheiten = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blutgruppen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Eindruecke",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eindruecke", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Haeuser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Haeuser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Herkunftslaender",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Herkunftslaender", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Infanterieraenge",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infanterieraenge", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lebensstati",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lebensstati", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LightCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bezeichnung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CssClass = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Farbcode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LightCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lizenzen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lizenzen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monsteranfaelligkeiten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsteranfaelligkeiten", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monstergruppen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monstergruppen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monsterimmunitaeten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsterimmunitaeten", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monsterintelligenzen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsterintelligenzen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monstervorkommen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monstervorkommen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monsterwuerfel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Wuerfel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsterwuerfel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rassen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rassen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Religions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Religions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Staende",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staende", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zaubertypen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Beschreibung = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zaubertypen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Obermagien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bezeichnung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LightCardId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obermagien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Obermagien_LightCards_LightCardId",
                        column: x => x.LightCardId,
                        principalTable: "LightCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonsterTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MonsterwuerfelId = table.Column<int>(type: "int", nullable: false),
                    MonsterintelligenzId = table.Column<int>(type: "int", nullable: false),
                    MonstergruppenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonsterTypes_Monstergruppen_MonstergruppenId",
                        column: x => x.MonstergruppenId,
                        principalTable: "Monstergruppen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MonsterTypes_Monsterintelligenzen_MonsterintelligenzId",
                        column: x => x.MonsterintelligenzId,
                        principalTable: "Monsterintelligenzen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MonsterTypes_Monsterwuerfel_MonsterwuerfelId",
                        column: x => x.MonsterwuerfelId,
                        principalTable: "Monsterwuerfel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nachname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vorname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rufname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Geschlecht = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RasseId = table.Column<int>(type: "int", nullable: false),
                    LebensstatusId = table.Column<int>(type: "int", nullable: false),
                    EindruckId = table.Column<int>(type: "int", nullable: false),
                    Geburtsdatum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VaterId = table.Column<int>(type: "int", nullable: true),
                    MutterId = table.Column<int>(type: "int", nullable: true),
                    CompletionLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Characters_MutterId",
                        column: x => x.MutterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Characters_VaterId",
                        column: x => x.VaterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Eindruecke_EindruckId",
                        column: x => x.EindruckId,
                        principalTable: "Eindruecke",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Lebensstati_LebensstatusId",
                        column: x => x.LebensstatusId,
                        principalTable: "Lebensstati",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Rassen_RasseId",
                        column: x => x.RasseId,
                        principalTable: "Rassen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MagicClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bezeichnung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObermagieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MagicClasses_Obermagien_ObermagieId",
                        column: x => x.ObermagieId,
                        principalTable: "Obermagien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Monsters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    encounter = table.Column<bool>(type: "bit", nullable: false),
                    MonstertypId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monsters_MonsterTypes_MonstertypId",
                        column: x => x.MonstertypId,
                        principalTable: "MonsterTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Monstertypanfaelligkeiten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonstertypId = table.Column<int>(type: "int", nullable: false),
                    MonsteranfaelligkeitenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monstertypanfaelligkeiten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monstertypanfaelligkeiten_MonsterTypes_MonstertypId",
                        column: x => x.MonstertypId,
                        principalTable: "MonsterTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Monstertypanfaelligkeiten_Monsteranfaelligkeiten_MonsteranfaelligkeitenId",
                        column: x => x.MonsteranfaelligkeitenId,
                        principalTable: "Monsteranfaelligkeiten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Monstertypimmunitaeten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonstertypId = table.Column<int>(type: "int", nullable: false),
                    MonsterimmunitaetenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monstertypimmunitaeten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monstertypimmunitaeten_MonsterTypes_MonstertypId",
                        column: x => x.MonstertypId,
                        principalTable: "MonsterTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Monstertypimmunitaeten_Monsterimmunitaeten_MonsterimmunitaetenId",
                        column: x => x.MonsterimmunitaetenId,
                        principalTable: "Monsterimmunitaeten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Monstertypvorkommen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonstertypId = table.Column<int>(type: "int", nullable: false),
                    MonstervorkommenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monstertypvorkommen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monstertypvorkommen_MonsterTypes_MonstertypId",
                        column: x => x.MonstertypId,
                        principalTable: "MonsterTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Monstertypvorkommen_Monstervorkommen_MonstervorkommenId",
                        column: x => x.MonstervorkommenId,
                        principalTable: "Monstervorkommen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    quote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    urheber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Beruf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyHeight = table.Column<int>(type: "int", nullable: true),
                    StandId = table.Column<int>(type: "int", nullable: true),
                    BlutgruppeId = table.Column<int>(type: "int", nullable: true),
                    HausId = table.Column<int>(type: "int", nullable: true),
                    HerkunftslandId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterDetails_Blutgruppen_BlutgruppeId",
                        column: x => x.BlutgruppeId,
                        principalTable: "Blutgruppen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterDetails_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterDetails_Haeuser_HausId",
                        column: x => x.HausId,
                        principalTable: "Haeuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterDetails_Herkunftslaender_HerkunftslandId",
                        column: x => x.HerkunftslandId,
                        principalTable: "Herkunftslaender",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterDetails_Staende_StandId",
                        column: x => x.StandId,
                        principalTable: "Staende",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    quote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    urheber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LightCardId = table.Column<int>(type: "int", nullable: false),
                    AbenteuerrangId = table.Column<int>(type: "int", nullable: false),
                    AnmeldungsstatusId = table.Column<int>(type: "int", nullable: false),
                    LeaderId = table.Column<int>(type: "int", nullable: true),
                    VertreterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guilds_Abenteuerraenge_AbenteuerrangId",
                        column: x => x.AbenteuerrangId,
                        principalTable: "Abenteuerraenge",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Guilds_Anmeldungsstati_AnmeldungsstatusId",
                        column: x => x.AnmeldungsstatusId,
                        principalTable: "Anmeldungsstati",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Guilds_Characters_LeaderId",
                        column: x => x.LeaderId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Guilds_Characters_VertreterId",
                        column: x => x.VertreterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Guilds_LightCards_LightCardId",
                        column: x => x.LightCardId,
                        principalTable: "LightCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Infanterien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bezeichnung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sitz = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LightCardId = table.Column<int>(type: "int", nullable: true),
                    LeaderId = table.Column<int>(type: "int", nullable: true),
                    VertreterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infanterien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Infanterien_Characters_LeaderId",
                        column: x => x.LeaderId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Infanterien_Characters_VertreterId",
                        column: x => x.VertreterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Grundzauber",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Spruch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Wirkung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stufe = table.Column<int>(type: "int", nullable: false),
                    Slots = table.Column<int>(type: "int", nullable: false),
                    Effekt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZaubertypID = table.Column<int>(type: "int", nullable: false),
                    MagicClassId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grundzauber", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grundzauber_MagicClasses_MagicClassId",
                        column: x => x.MagicClassId,
                        principalTable: "MagicClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Grundzauber_Zaubertypen_ZaubertypID",
                        column: x => x.ZaubertypID,
                        principalTable: "Zaubertypen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MagicClassSpecializations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MagicClassId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicClassSpecializations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MagicClassSpecializations_MagicClasses_MagicClassId",
                        column: x => x.MagicClassId,
                        principalTable: "MagicClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gildenlizenzen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuildId = table.Column<int>(type: "int", nullable: false),
                    LizenzenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gildenlizenzen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gildenlizenzen_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Gildenlizenzen_Lizenzen_LizenzenId",
                        column: x => x.LizenzenId,
                        principalTable: "Lizenzen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Regiments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegimentsleiterId = table.Column<int>(type: "int", nullable: true),
                    AdjutantId = table.Column<int>(type: "int", nullable: true),
                    InfanterieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regiments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Regiments_Characters_AdjutantId",
                        column: x => x.AdjutantId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Regiments_Characters_RegimentsleiterId",
                        column: x => x.RegimentsleiterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Regiments_Infanterien_InfanterieId",
                        column: x => x.InfanterieId,
                        principalTable: "Infanterien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterMagicClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    MagicClassId = table.Column<int>(type: "int", nullable: false),
                    MagicClassSpecializationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterMagicClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterMagicClasses_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterMagicClasses_MagicClassSpecializations_MagicClassSpecializationId",
                        column: x => x.MagicClassSpecializationId,
                        principalTable: "MagicClassSpecializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterMagicClasses_MagicClasses_MagicClassId",
                        column: x => x.MagicClassId,
                        principalTable: "MagicClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SpecialZauber",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Spruch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Wirkung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stufe = table.Column<int>(type: "int", nullable: false),
                    Slots = table.Column<int>(type: "int", nullable: false),
                    Effekt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZaubertypID = table.Column<int>(type: "int", nullable: false),
                    MagicClassSpecializationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialZauber", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecialZauber_MagicClassSpecializations_MagicClassSpecializationId",
                        column: x => x.MagicClassSpecializationId,
                        principalTable: "MagicClassSpecializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecialZauber_Zaubertypen_ZaubertypID",
                        column: x => x.ZaubertypID,
                        principalTable: "Zaubertypen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterAffiliations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    GuildId = table.Column<int>(type: "int", nullable: true),
                    RegimentId = table.Column<int>(type: "int", nullable: true),
                    InfanterierangId = table.Column<int>(type: "int", nullable: true),
                    ReligionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterAffiliations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterAffiliations_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterAffiliations_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterAffiliations_Infanterieraenge_InfanterierangId",
                        column: x => x.InfanterierangId,
                        principalTable: "Infanterieraenge",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterAffiliations_Regiments_RegimentId",
                        column: x => x.RegimentId,
                        principalTable: "Regiments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterAffiliations_Religions_ReligionId",
                        column: x => x.ReligionId,
                        principalTable: "Religions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAffiliations_CharacterId",
                table: "CharacterAffiliations",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAffiliations_GuildId",
                table: "CharacterAffiliations",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAffiliations_InfanterierangId",
                table: "CharacterAffiliations",
                column: "InfanterierangId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAffiliations_RegimentId",
                table: "CharacterAffiliations",
                column: "RegimentId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAffiliations_ReligionId",
                table: "CharacterAffiliations",
                column: "ReligionId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterDetails_BlutgruppeId",
                table: "CharacterDetails",
                column: "BlutgruppeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterDetails_CharacterId",
                table: "CharacterDetails",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterDetails_HausId",
                table: "CharacterDetails",
                column: "HausId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterDetails_HerkunftslandId",
                table: "CharacterDetails",
                column: "HerkunftslandId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterDetails_StandId",
                table: "CharacterDetails",
                column: "StandId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterMagicClasses_CharacterId",
                table: "CharacterMagicClasses",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterMagicClasses_MagicClassId",
                table: "CharacterMagicClasses",
                column: "MagicClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterMagicClasses_MagicClassSpecializationId",
                table: "CharacterMagicClasses",
                column: "MagicClassSpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_EindruckId",
                table: "Characters",
                column: "EindruckId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_LebensstatusId",
                table: "Characters",
                column: "LebensstatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_MutterId",
                table: "Characters",
                column: "MutterId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_RasseId",
                table: "Characters",
                column: "RasseId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_VaterId",
                table: "Characters",
                column: "VaterId");

            migrationBuilder.CreateIndex(
                name: "IX_Gildenlizenzen_GuildId",
                table: "Gildenlizenzen",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Gildenlizenzen_LizenzenId",
                table: "Gildenlizenzen",
                column: "LizenzenId");

            migrationBuilder.CreateIndex(
                name: "IX_Grundzauber_MagicClassId",
                table: "Grundzauber",
                column: "MagicClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Grundzauber_ZaubertypID",
                table: "Grundzauber",
                column: "ZaubertypID");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_AbenteuerrangId",
                table: "Guilds",
                column: "AbenteuerrangId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_AnmeldungsstatusId",
                table: "Guilds",
                column: "AnmeldungsstatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_LeaderId",
                table: "Guilds",
                column: "LeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_LightCardId",
                table: "Guilds",
                column: "LightCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_VertreterId",
                table: "Guilds",
                column: "VertreterId");

            migrationBuilder.CreateIndex(
                name: "IX_Infanterien_LeaderId",
                table: "Infanterien",
                column: "LeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Infanterien_VertreterId",
                table: "Infanterien",
                column: "VertreterId");

            migrationBuilder.CreateIndex(
                name: "IX_MagicClasses_ObermagieId",
                table: "MagicClasses",
                column: "ObermagieId");

            migrationBuilder.CreateIndex(
                name: "IX_MagicClassSpecializations_MagicClassId",
                table: "MagicClassSpecializations",
                column: "MagicClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Monsters_MonstertypId",
                table: "Monsters",
                column: "MonstertypId");

            migrationBuilder.CreateIndex(
                name: "IX_Monstertypanfaelligkeiten_MonsteranfaelligkeitenId",
                table: "Monstertypanfaelligkeiten",
                column: "MonsteranfaelligkeitenId");

            migrationBuilder.CreateIndex(
                name: "IX_Monstertypanfaelligkeiten_MonstertypId",
                table: "Monstertypanfaelligkeiten",
                column: "MonstertypId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterTypes_MonstergruppenId",
                table: "MonsterTypes",
                column: "MonstergruppenId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterTypes_MonsterintelligenzId",
                table: "MonsterTypes",
                column: "MonsterintelligenzId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterTypes_MonsterwuerfelId",
                table: "MonsterTypes",
                column: "MonsterwuerfelId");

            migrationBuilder.CreateIndex(
                name: "IX_Monstertypimmunitaeten_MonsterimmunitaetenId",
                table: "Monstertypimmunitaeten",
                column: "MonsterimmunitaetenId");

            migrationBuilder.CreateIndex(
                name: "IX_Monstertypimmunitaeten_MonstertypId",
                table: "Monstertypimmunitaeten",
                column: "MonstertypId");

            migrationBuilder.CreateIndex(
                name: "IX_Monstertypvorkommen_MonstertypId",
                table: "Monstertypvorkommen",
                column: "MonstertypId");

            migrationBuilder.CreateIndex(
                name: "IX_Monstertypvorkommen_MonstervorkommenId",
                table: "Monstertypvorkommen",
                column: "MonstervorkommenId");

            migrationBuilder.CreateIndex(
                name: "IX_Obermagien_LightCardId",
                table: "Obermagien",
                column: "LightCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Regiments_AdjutantId",
                table: "Regiments",
                column: "AdjutantId");

            migrationBuilder.CreateIndex(
                name: "IX_Regiments_InfanterieId",
                table: "Regiments",
                column: "InfanterieId");

            migrationBuilder.CreateIndex(
                name: "IX_Regiments_RegimentsleiterId",
                table: "Regiments",
                column: "RegimentsleiterId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialZauber_MagicClassSpecializationId",
                table: "SpecialZauber",
                column: "MagicClassSpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialZauber_ZaubertypID",
                table: "SpecialZauber",
                column: "ZaubertypID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CharacterAffiliations");

            migrationBuilder.DropTable(
                name: "CharacterDetails");

            migrationBuilder.DropTable(
                name: "CharacterMagicClasses");

            migrationBuilder.DropTable(
                name: "Gildenlizenzen");

            migrationBuilder.DropTable(
                name: "Grundzauber");

            migrationBuilder.DropTable(
                name: "Monsters");

            migrationBuilder.DropTable(
                name: "Monstertypanfaelligkeiten");

            migrationBuilder.DropTable(
                name: "Monstertypimmunitaeten");

            migrationBuilder.DropTable(
                name: "Monstertypvorkommen");

            migrationBuilder.DropTable(
                name: "SpecialZauber");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Infanterieraenge");

            migrationBuilder.DropTable(
                name: "Regiments");

            migrationBuilder.DropTable(
                name: "Religions");

            migrationBuilder.DropTable(
                name: "Blutgruppen");

            migrationBuilder.DropTable(
                name: "Haeuser");

            migrationBuilder.DropTable(
                name: "Herkunftslaender");

            migrationBuilder.DropTable(
                name: "Staende");

            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.DropTable(
                name: "Lizenzen");

            migrationBuilder.DropTable(
                name: "Monsteranfaelligkeiten");

            migrationBuilder.DropTable(
                name: "Monsterimmunitaeten");

            migrationBuilder.DropTable(
                name: "MonsterTypes");

            migrationBuilder.DropTable(
                name: "Monstervorkommen");

            migrationBuilder.DropTable(
                name: "MagicClassSpecializations");

            migrationBuilder.DropTable(
                name: "Zaubertypen");

            migrationBuilder.DropTable(
                name: "Infanterien");

            migrationBuilder.DropTable(
                name: "Abenteuerraenge");

            migrationBuilder.DropTable(
                name: "Anmeldungsstati");

            migrationBuilder.DropTable(
                name: "Monstergruppen");

            migrationBuilder.DropTable(
                name: "Monsterintelligenzen");

            migrationBuilder.DropTable(
                name: "Monsterwuerfel");

            migrationBuilder.DropTable(
                name: "MagicClasses");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Obermagien");

            migrationBuilder.DropTable(
                name: "Eindruecke");

            migrationBuilder.DropTable(
                name: "Lebensstati");

            migrationBuilder.DropTable(
                name: "Rassen");

            migrationBuilder.DropTable(
                name: "LightCards");
        }
    }
}
