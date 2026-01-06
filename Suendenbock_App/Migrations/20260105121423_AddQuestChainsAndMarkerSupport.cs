using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestChainsAndMarkerSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PreviousQuestId",
                table: "Quests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quests_PreviousQuestId",
                table: "Quests",
                column: "PreviousQuestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quests_Quests_PreviousQuestId",
                table: "Quests",
                column: "PreviousQuestId",
                principalTable: "Quests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quests_Quests_PreviousQuestId",
                table: "Quests");

            migrationBuilder.DropIndex(
                name: "IX_Quests_PreviousQuestId",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "PreviousQuestId",
                table: "Quests");
        }
    }
}
