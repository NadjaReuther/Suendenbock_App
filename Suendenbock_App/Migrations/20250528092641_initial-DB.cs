using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class initialDB : Migration
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
                name: "Berufe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Berufe", x => x.Id);
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
                name: "Infanterien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bezeichnung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    leader = table.Column<int>(type: "int", nullable: true),
                    vertreter = table.Column<int>(type: "int", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infanterien", x => x.Id);
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
                    CssClass = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LightCards", x => x.Id);
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
                name: "Guilds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Abenteuerrang = table.Column<int>(type: "int", nullable: false),
                    Anmeldungsstatus = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LightCardId = table.Column<int>(type: "int", nullable: false),
                    AbenteuerrangId = table.Column<int>(type: "int", nullable: false),
                    AnmeldungsstatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guilds_Abenteuerraenge_AbenteuerrangId",
                        column: x => x.AbenteuerrangId,
                        principalTable: "Abenteuerraenge",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Guilds_Anmeldungsstati_AnmeldungsstatusId",
                        column: x => x.AnmeldungsstatusId,
                        principalTable: "Anmeldungsstati",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Guilds_LightCards_LightCardId",
                        column: x => x.LightCardId,
                        principalTable: "LightCards",
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nachname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vorname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Geschlecht = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bodyheight = table.Column<int>(type: "int", nullable: false),
                    Geburtsdatum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RasseId = table.Column<int>(type: "int", nullable: true),
                    EindruckId = table.Column<int>(type: "int", nullable: true),
                    StandId = table.Column<int>(type: "int", nullable: true),
                    BerufId = table.Column<int>(type: "int", nullable: true),
                    BlutgruppeId = table.Column<int>(type: "int", nullable: true),
                    HausId = table.Column<int>(type: "int", nullable: true),
                    HerkunftslandId = table.Column<int>(type: "int", nullable: true),
                    LebensstatusId = table.Column<int>(type: "int", nullable: true),
                    VaterId = table.Column<int>(type: "int", nullable: true),
                    MutterId = table.Column<int>(type: "int", nullable: true),
                    GuildId = table.Column<int>(type: "int", nullable: true),
                    InfanterieId = table.Column<int>(type: "int", nullable: true),
                    InfanterierangId = table.Column<int>(type: "int", nullable: true),
                    ReligionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Berufe_BerufId",
                        column: x => x.BerufId,
                        principalTable: "Berufe",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Characters_Blutgruppen_BlutgruppeId",
                        column: x => x.BlutgruppeId,
                        principalTable: "Blutgruppen",
                        principalColumn: "Id");
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
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Characters_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Characters_Haeuser_HausId",
                        column: x => x.HausId,
                        principalTable: "Haeuser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Characters_Herkunftslaender_HerkunftslandId",
                        column: x => x.HerkunftslandId,
                        principalTable: "Herkunftslaender",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Characters_Infanterien_InfanterieId",
                        column: x => x.InfanterieId,
                        principalTable: "Infanterien",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Characters_Infanterieraenge_InfanterierangId",
                        column: x => x.InfanterierangId,
                        principalTable: "Infanterieraenge",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Characters_Lebensstati_LebensstatusId",
                        column: x => x.LebensstatusId,
                        principalTable: "Lebensstati",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Characters_Rassen_RasseId",
                        column: x => x.RasseId,
                        principalTable: "Rassen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Characters_Religions_ReligionId",
                        column: x => x.ReligionId,
                        principalTable: "Religions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Characters_Staende_StandId",
                        column: x => x.StandId,
                        principalTable: "Staende",
                        principalColumn: "Id");
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
                name: "CharacterMagicClasses",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    MagicClassId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    MagicClassSpecializationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterMagicClasses", x => new { x.CharacterId, x.MagicClassId });
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
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterMagicClasses_MagicClasses_MagicClassId",
                        column: x => x.MagicClassId,
                        principalTable: "MagicClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zauber",
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
                    table.PrimaryKey("PK_Zauber", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zauber_MagicClassSpecializations_MagicClassSpecializationId",
                        column: x => x.MagicClassSpecializationId,
                        principalTable: "MagicClassSpecializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Zauber_Zaubertypen_ZaubertypID",
                        column: x => x.ZaubertypID,
                        principalTable: "Zaubertypen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_CharacterMagicClasses_MagicClassId",
                table: "CharacterMagicClasses",
                column: "MagicClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterMagicClasses_MagicClassSpecializationId",
                table: "CharacterMagicClasses",
                column: "MagicClassSpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_BerufId",
                table: "Characters",
                column: "BerufId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_BlutgruppeId",
                table: "Characters",
                column: "BlutgruppeId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_EindruckId",
                table: "Characters",
                column: "EindruckId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_GuildId",
                table: "Characters",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_HausId",
                table: "Characters",
                column: "HausId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_HerkunftslandId",
                table: "Characters",
                column: "HerkunftslandId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_InfanterieId",
                table: "Characters",
                column: "InfanterieId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_InfanterierangId",
                table: "Characters",
                column: "InfanterierangId");

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
                name: "IX_Characters_ReligionId",
                table: "Characters",
                column: "ReligionId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_StandId",
                table: "Characters",
                column: "StandId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_VaterId",
                table: "Characters",
                column: "VaterId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_AbenteuerrangId",
                table: "Guilds",
                column: "AbenteuerrangId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_AnmeldungsstatusId",
                table: "Guilds",
                column: "AnmeldungsstatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_LightCardId",
                table: "Guilds",
                column: "LightCardId");

            migrationBuilder.CreateIndex(
                name: "IX_MagicClasses_ObermagieId",
                table: "MagicClasses",
                column: "ObermagieId");

            migrationBuilder.CreateIndex(
                name: "IX_MagicClassSpecializations_MagicClassId",
                table: "MagicClassSpecializations",
                column: "MagicClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Obermagien_LightCardId",
                table: "Obermagien",
                column: "LightCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Zauber_MagicClassSpecializationId",
                table: "Zauber",
                column: "MagicClassSpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Zauber_ZaubertypID",
                table: "Zauber",
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
                name: "CharacterMagicClasses");

            migrationBuilder.DropTable(
                name: "Zauber");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "MagicClassSpecializations");

            migrationBuilder.DropTable(
                name: "Zaubertypen");

            migrationBuilder.DropTable(
                name: "Berufe");

            migrationBuilder.DropTable(
                name: "Blutgruppen");

            migrationBuilder.DropTable(
                name: "Eindruecke");

            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.DropTable(
                name: "Haeuser");

            migrationBuilder.DropTable(
                name: "Herkunftslaender");

            migrationBuilder.DropTable(
                name: "Infanterien");

            migrationBuilder.DropTable(
                name: "Infanterieraenge");

            migrationBuilder.DropTable(
                name: "Lebensstati");

            migrationBuilder.DropTable(
                name: "Rassen");

            migrationBuilder.DropTable(
                name: "Religions");

            migrationBuilder.DropTable(
                name: "Staende");

            migrationBuilder.DropTable(
                name: "MagicClasses");

            migrationBuilder.DropTable(
                name: "Abenteuerraenge");

            migrationBuilder.DropTable(
                name: "Anmeldungsstati");

            migrationBuilder.DropTable(
                name: "Obermagien");

            migrationBuilder.DropTable(
                name: "LightCards");
        }
    }
}
