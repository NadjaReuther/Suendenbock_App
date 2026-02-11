using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class AddMapRegionPolygons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MapRegions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MapId = table.Column<int>(type: "int", nullable: false),
                    LinkedMapId = table.Column<int>(type: "int", nullable: false),
                    RegionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PolygonPoints = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FillColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StrokeColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StrokeWidth = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapRegions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MapRegions_Maps_LinkedMapId",
                        column: x => x.LinkedMapId,
                        principalTable: "Maps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MapRegions_Maps_MapId",
                        column: x => x.MapId,
                        principalTable: "Maps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MapRegions_LinkedMapId",
                table: "MapRegions",
                column: "LinkedMapId");

            migrationBuilder.CreateIndex(
                name: "IX_MapRegions_MapId",
                table: "MapRegions",
                column: "MapId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MapRegions");
        }
    }
}
