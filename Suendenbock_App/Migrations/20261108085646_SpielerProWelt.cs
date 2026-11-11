using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class SpielerProWelt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CharacterGameWorlds",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGameWorlds_UserId",
                table: "CharacterGameWorlds",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterGameWorlds_AspNetUsers_UserId",
                table: "CharacterGameWorlds",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterGameWorlds_AspNetUsers_UserId",
                table: "CharacterGameWorlds");

            migrationBuilder.DropIndex(
                name: "IX_CharacterGameWorlds_UserId",
                table: "CharacterGameWorlds");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CharacterGameWorlds");
        }
    }
}
