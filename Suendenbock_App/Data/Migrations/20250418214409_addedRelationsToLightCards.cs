using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedRelationsToLightCards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MagicClasses_LightCardsId",
                table: "MagicClasses",
                column: "LightCardsId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_LightCardsId",
                table: "Guilds",
                column: "LightCardsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_LightCards_LightCardsId",
                table: "Guilds",
                column: "LightCardsId",
                principalTable: "LightCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MagicClasses_LightCards_LightCardsId",
                table: "MagicClasses",
                column: "LightCardsId",
                principalTable: "LightCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_LightCards_LightCardsId",
                table: "Guilds");

            migrationBuilder.DropForeignKey(
                name: "FK_MagicClasses_LightCards_LightCardsId",
                table: "MagicClasses");

            migrationBuilder.DropIndex(
                name: "IX_MagicClasses_LightCardsId",
                table: "MagicClasses");

            migrationBuilder.DropIndex(
                name: "IX_Guilds_LightCardsId",
                table: "Guilds");
        }
    }
}
