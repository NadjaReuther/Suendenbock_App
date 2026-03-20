using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGlossaryEntryFKColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlossaryEntries_Blutgruppen_BlutgruppeId",
                table: "GlossaryEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_GlossaryEntries_Haeuser_HausId",
                table: "GlossaryEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_GlossaryEntries_Herkunftslaender_HerkunftslandId",
                table: "GlossaryEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_GlossaryEntries_Infanterieraenge_InfanterierangId",
                table: "GlossaryEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_GlossaryEntries_MagicClasses_MagicClassId",
                table: "GlossaryEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_GlossaryEntries_Obermagien_ObermagieId",
                table: "GlossaryEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_GlossaryEntries_Rassen_RasseId",
                table: "GlossaryEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_GlossaryEntries_Religions_ReligionId",
                table: "GlossaryEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_GlossaryEntries_Staende_StandId",
                table: "GlossaryEntries");

            migrationBuilder.DropIndex(
                name: "IX_GlossaryEntries_BlutgruppeId",
                table: "GlossaryEntries");

            migrationBuilder.DropIndex(
                name: "IX_GlossaryEntries_HausId",
                table: "GlossaryEntries");

            migrationBuilder.DropIndex(
                name: "IX_GlossaryEntries_HerkunftslandId",
                table: "GlossaryEntries");

            migrationBuilder.DropIndex(
                name: "IX_GlossaryEntries_InfanterierangId",
                table: "GlossaryEntries");

            migrationBuilder.DropIndex(
                name: "IX_GlossaryEntries_MagicClassId",
                table: "GlossaryEntries");

            migrationBuilder.DropIndex(
                name: "IX_GlossaryEntries_ObermagieId",
                table: "GlossaryEntries");

            migrationBuilder.DropIndex(
                name: "IX_GlossaryEntries_RasseId",
                table: "GlossaryEntries");

            migrationBuilder.DropIndex(
                name: "IX_GlossaryEntries_ReligionId",
                table: "GlossaryEntries");

            migrationBuilder.DropIndex(
                name: "IX_GlossaryEntries_StandId",
                table: "GlossaryEntries");

            migrationBuilder.DropColumn(
                name: "BlutgruppeId",
                table: "GlossaryEntries");

            migrationBuilder.DropColumn(
                name: "HausId",
                table: "GlossaryEntries");

            migrationBuilder.DropColumn(
                name: "HerkunftslandId",
                table: "GlossaryEntries");

            migrationBuilder.DropColumn(
                name: "InfanterierangId",
                table: "GlossaryEntries");

            migrationBuilder.DropColumn(
                name: "MagicClassId",
                table: "GlossaryEntries");

            migrationBuilder.DropColumn(
                name: "ObermagieId",
                table: "GlossaryEntries");

            migrationBuilder.DropColumn(
                name: "RasseId",
                table: "GlossaryEntries");

            migrationBuilder.DropColumn(
                name: "ReligionId",
                table: "GlossaryEntries");

            migrationBuilder.DropColumn(
                name: "StandId",
                table: "GlossaryEntries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlutgruppeId",
                table: "GlossaryEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HausId",
                table: "GlossaryEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HerkunftslandId",
                table: "GlossaryEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InfanterierangId",
                table: "GlossaryEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MagicClassId",
                table: "GlossaryEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ObermagieId",
                table: "GlossaryEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RasseId",
                table: "GlossaryEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReligionId",
                table: "GlossaryEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StandId",
                table: "GlossaryEntries",
                type: "int",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_GlossaryEntries_Blutgruppen_BlutgruppeId",
                table: "GlossaryEntries",
                column: "BlutgruppeId",
                principalTable: "Blutgruppen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlossaryEntries_Haeuser_HausId",
                table: "GlossaryEntries",
                column: "HausId",
                principalTable: "Haeuser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlossaryEntries_Herkunftslaender_HerkunftslandId",
                table: "GlossaryEntries",
                column: "HerkunftslandId",
                principalTable: "Herkunftslaender",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlossaryEntries_Infanterieraenge_InfanterierangId",
                table: "GlossaryEntries",
                column: "InfanterierangId",
                principalTable: "Infanterieraenge",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlossaryEntries_MagicClasses_MagicClassId",
                table: "GlossaryEntries",
                column: "MagicClassId",
                principalTable: "MagicClasses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlossaryEntries_Obermagien_ObermagieId",
                table: "GlossaryEntries",
                column: "ObermagieId",
                principalTable: "Obermagien",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlossaryEntries_Rassen_RasseId",
                table: "GlossaryEntries",
                column: "RasseId",
                principalTable: "Rassen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlossaryEntries_Religions_ReligionId",
                table: "GlossaryEntries",
                column: "ReligionId",
                principalTable: "Religions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlossaryEntries_Staende_StandId",
                table: "GlossaryEntries",
                column: "StandId",
                principalTable: "Staende",
                principalColumn: "Id");
        }
    }
}
