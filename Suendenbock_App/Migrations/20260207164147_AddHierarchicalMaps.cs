using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class AddHierarchicalMaps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWorldMap",
                table: "Maps",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ParentMapId",
                table: "Maps",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegionName",
                table: "Maps",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LinkedMapId",
                table: "MapMarkers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Maps_ParentMapId",
                table: "Maps",
                column: "ParentMapId");

            migrationBuilder.CreateIndex(
                name: "IX_MapMarkers_LinkedMapId",
                table: "MapMarkers",
                column: "LinkedMapId");

            migrationBuilder.AddForeignKey(
                name: "FK_MapMarkers_Maps_LinkedMapId",
                table: "MapMarkers",
                column: "LinkedMapId",
                principalTable: "Maps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Maps_Maps_ParentMapId",
                table: "Maps",
                column: "ParentMapId",
                principalTable: "Maps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MapMarkers_Maps_LinkedMapId",
                table: "MapMarkers");

            migrationBuilder.DropForeignKey(
                name: "FK_Maps_Maps_ParentMapId",
                table: "Maps");

            migrationBuilder.DropIndex(
                name: "IX_Maps_ParentMapId",
                table: "Maps");

            migrationBuilder.DropIndex(
                name: "IX_MapMarkers_LinkedMapId",
                table: "MapMarkers");

            migrationBuilder.DropColumn(
                name: "IsWorldMap",
                table: "Maps");

            migrationBuilder.DropColumn(
                name: "ParentMapId",
                table: "Maps");

            migrationBuilder.DropColumn(
                name: "RegionName",
                table: "Maps");

            migrationBuilder.DropColumn(
                name: "LinkedMapId",
                table: "MapMarkers");
        }
    }
}
