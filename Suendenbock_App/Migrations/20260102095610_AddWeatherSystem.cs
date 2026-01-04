using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class AddWeatherSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeatherOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Month = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WeatherName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherForecastDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WeatherOptionId = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Temperature = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherForecastDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeatherForecastDays_WeatherOptions_WeatherOptionId",
                        column: x => x.WeatherOptionId,
                        principalTable: "WeatherOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeatherForecastDays_WeatherOptionId",
                table: "WeatherForecastDays",
                column: "WeatherOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherOptions_Month",
                table: "WeatherOptions",
                column: "Month");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherForecastDays");

            migrationBuilder.DropTable(
                name: "WeatherOptions");
        }
    }
}
