using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class addedPartner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PartnerId",
                table: "Characters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_PartnerId",
                table: "Characters",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Characters_PartnerId",
                table: "Characters",
                column: "PartnerId",
                principalTable: "Characters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Characters_PartnerId",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_Characters_PartnerId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "Characters");
        }
    }
}
