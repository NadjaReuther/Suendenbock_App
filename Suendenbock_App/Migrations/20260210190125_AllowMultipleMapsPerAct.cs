using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class AllowMultipleMapsPerAct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Maps_ActId",
                table: "Maps");

            migrationBuilder.CreateIndex(
                name: "IX_Maps_ActId",
                table: "Maps",
                column: "ActId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Maps_ActId",
                table: "Maps");

            migrationBuilder.CreateIndex(
                name: "IX_Maps_ActId",
                table: "Maps",
                column: "ActId",
                unique: true);
        }
    }
}
