using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class updateInfanterie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LeaderCharacterId",
                table: "Infanterien",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VertreterCharacterId",
                table: "Infanterien",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Infanterien_LeaderCharacterId",
                table: "Infanterien",
                column: "LeaderCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Infanterien_VertreterCharacterId",
                table: "Infanterien",
                column: "VertreterCharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Infanterien_Characters_LeaderCharacterId",
                table: "Infanterien",
                column: "LeaderCharacterId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Infanterien_Characters_VertreterCharacterId",
                table: "Infanterien",
                column: "VertreterCharacterId",
                principalTable: "Characters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Infanterien_Characters_LeaderCharacterId",
                table: "Infanterien");

            migrationBuilder.DropForeignKey(
                name: "FK_Infanterien_Characters_VertreterCharacterId",
                table: "Infanterien");

            migrationBuilder.DropIndex(
                name: "IX_Infanterien_LeaderCharacterId",
                table: "Infanterien");

            migrationBuilder.DropIndex(
                name: "IX_Infanterien_VertreterCharacterId",
                table: "Infanterien");

            migrationBuilder.DropColumn(
                name: "LeaderCharacterId",
                table: "Infanterien");

            migrationBuilder.DropColumn(
                name: "VertreterCharacterId",
                table: "Infanterien");
        }
    }
}
