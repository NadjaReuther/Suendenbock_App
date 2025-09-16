using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class updateRegimentCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Regiments_Infanterien_InfanterieId",
                table: "Regiments");

            migrationBuilder.AddForeignKey(
                name: "FK_Regiments_Infanterien_InfanterieId",
                table: "Regiments",
                column: "InfanterieId",
                principalTable: "Infanterien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Regiments_Infanterien_InfanterieId",
                table: "Regiments");

            migrationBuilder.AddForeignKey(
                name: "FK_Regiments_Infanterien_InfanterieId",
                table: "Regiments",
                column: "InfanterieId",
                principalTable: "Infanterien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
