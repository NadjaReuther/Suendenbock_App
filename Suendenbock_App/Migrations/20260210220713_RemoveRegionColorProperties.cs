using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRegionColorProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FillColor",
                table: "MapRegions");

            migrationBuilder.DropColumn(
                name: "StrokeColor",
                table: "MapRegions");

            migrationBuilder.DropColumn(
                name: "StrokeWidth",
                table: "MapRegions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FillColor",
                table: "MapRegions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StrokeColor",
                table: "MapRegions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StrokeWidth",
                table: "MapRegions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
