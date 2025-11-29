using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdventDoorStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChoicesJson",
                table: "AdventDoors");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "AdventDoors");

            migrationBuilder.DropColumn(
                name: "Question",
                table: "AdventDoors");

            migrationBuilder.AddColumn<string>(
                name: "AudioPath",
                table: "AdventDoors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DoorType",
                table: "AdventDoors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EmmaAudioPath",
                table: "AdventDoors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HtmlContentPath",
                table: "AdventDoors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KasimirAudioPath",
                table: "AdventDoors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioPath",
                table: "AdventDoors");

            migrationBuilder.DropColumn(
                name: "DoorType",
                table: "AdventDoors");

            migrationBuilder.DropColumn(
                name: "EmmaAudioPath",
                table: "AdventDoors");

            migrationBuilder.DropColumn(
                name: "HtmlContentPath",
                table: "AdventDoors");

            migrationBuilder.DropColumn(
                name: "KasimirAudioPath",
                table: "AdventDoors");

            migrationBuilder.AddColumn<string>(
                name: "ChoicesJson",
                table: "AdventDoors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "AdventDoors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Question",
                table: "AdventDoors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
