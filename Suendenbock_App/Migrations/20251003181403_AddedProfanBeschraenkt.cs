using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class AddedProfanBeschraenkt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Beschraenkt",
                table: "Characters",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Profan",
                table: "Characters",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Beschraenkt",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Profan",
                table: "Characters");
        }
    }
}
