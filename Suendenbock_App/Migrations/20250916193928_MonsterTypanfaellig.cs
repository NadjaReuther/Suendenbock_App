using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class MonsterTypanfaellig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Monsteranfaelligkeiten_MonsterTypes_MonstertypId",
                table: "Monsteranfaelligkeiten");

            migrationBuilder.DropIndex(
                name: "IX_Monsteranfaelligkeiten_MonstertypId",
                table: "Monsteranfaelligkeiten");

            migrationBuilder.DropColumn(
                name: "MonstertypId",
                table: "Monsteranfaelligkeiten");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonstertypId",
                table: "Monsteranfaelligkeiten",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Monsteranfaelligkeiten_MonstertypId",
                table: "Monsteranfaelligkeiten",
                column: "MonstertypId");

            migrationBuilder.AddForeignKey(
                name: "FK_Monsteranfaelligkeiten_MonsterTypes_MonstertypId",
                table: "Monsteranfaelligkeiten",
                column: "MonstertypId",
                principalTable: "MonsterTypes",
                principalColumn: "Id");
        }
    }
}
