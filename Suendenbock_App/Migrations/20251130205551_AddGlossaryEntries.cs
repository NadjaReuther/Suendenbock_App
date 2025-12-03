using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class AddGlossaryEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GlossaryEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RasseId = table.Column<int>(type: "int", nullable: true),
                    MagicClassId = table.Column<int>(type: "int", nullable: true),
                    ObermagieId = table.Column<int>(type: "int", nullable: true),
                    BlutgruppeId = table.Column<int>(type: "int", nullable: true),
                    HausId = table.Column<int>(type: "int", nullable: true),
                    HerkunftslandId = table.Column<int>(type: "int", nullable: true),
                    ReligionId = table.Column<int>(type: "int", nullable: true),
                    InfanterierangId = table.Column<int>(type: "int", nullable: true),
                    StandId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlossaryEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlossaryEntries_Blutgruppen_BlutgruppeId",
                        column: x => x.BlutgruppeId,
                        principalTable: "Blutgruppen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GlossaryEntries_Haeuser_HausId",
                        column: x => x.HausId,
                        principalTable: "Haeuser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GlossaryEntries_Herkunftslaender_HerkunftslandId",
                        column: x => x.HerkunftslandId,
                        principalTable: "Herkunftslaender",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GlossaryEntries_Infanterieraenge_InfanterierangId",
                        column: x => x.InfanterierangId,
                        principalTable: "Infanterieraenge",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GlossaryEntries_MagicClasses_MagicClassId",
                        column: x => x.MagicClassId,
                        principalTable: "MagicClasses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GlossaryEntries_Obermagien_ObermagieId",
                        column: x => x.ObermagieId,
                        principalTable: "Obermagien",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GlossaryEntries_Rassen_RasseId",
                        column: x => x.RasseId,
                        principalTable: "Rassen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GlossaryEntries_Religions_ReligionId",
                        column: x => x.ReligionId,
                        principalTable: "Religions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GlossaryEntries_Staende_StandId",
                        column: x => x.StandId,
                        principalTable: "Staende",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GlossaryEntries_BlutgruppeId",
                table: "GlossaryEntries",
                column: "BlutgruppeId");

            migrationBuilder.CreateIndex(
                name: "IX_GlossaryEntries_HausId",
                table: "GlossaryEntries",
                column: "HausId");

            migrationBuilder.CreateIndex(
                name: "IX_GlossaryEntries_HerkunftslandId",
                table: "GlossaryEntries",
                column: "HerkunftslandId");

            migrationBuilder.CreateIndex(
                name: "IX_GlossaryEntries_InfanterierangId",
                table: "GlossaryEntries",
                column: "InfanterierangId");

            migrationBuilder.CreateIndex(
                name: "IX_GlossaryEntries_MagicClassId",
                table: "GlossaryEntries",
                column: "MagicClassId");

            migrationBuilder.CreateIndex(
                name: "IX_GlossaryEntries_ObermagieId",
                table: "GlossaryEntries",
                column: "ObermagieId");

            migrationBuilder.CreateIndex(
                name: "IX_GlossaryEntries_RasseId",
                table: "GlossaryEntries",
                column: "RasseId");

            migrationBuilder.CreateIndex(
                name: "IX_GlossaryEntries_ReligionId",
                table: "GlossaryEntries",
                column: "ReligionId");

            migrationBuilder.CreateIndex(
                name: "IX_GlossaryEntries_StandId",
                table: "GlossaryEntries",
                column: "StandId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlossaryEntries");
        }
    }
}
