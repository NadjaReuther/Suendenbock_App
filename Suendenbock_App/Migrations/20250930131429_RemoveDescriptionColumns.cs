using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDescriptionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessedDescription",
                table: "Regiments");

            migrationBuilder.DropColumn(
                name: "ProcessedDescription",
                table: "Infanterien");

            migrationBuilder.DropColumn(
                name: "ProcessedDescription",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "ProcessedDescription",
                table: "CharacterDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProcessedDescription",
                table: "Regiments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessedDescription",
                table: "Infanterien",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessedDescription",
                table: "Guilds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessedDescription",
                table: "CharacterDetails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
