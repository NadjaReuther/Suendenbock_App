using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class AddPreviousQuestRequirement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreviousQuestRequirement",
                table: "Quests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviousQuestRequirement",
                table: "Quests");
        }
    }
}
