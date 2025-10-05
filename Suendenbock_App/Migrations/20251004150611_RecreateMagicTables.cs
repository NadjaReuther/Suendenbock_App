using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class RecreateMagicTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_Grundzauber_MagicClassId",
                table: "Grundzauber",
                column: "MagicClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Grundzauber_ZaubertypID",
                table: "Grundzauber",
                column: "ZaubertypID");

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

        }
    }
}
