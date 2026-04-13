using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class RenameChores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update existing chore names to new names
            migrationBuilder.Sql("UPDATE EventChores SET ChoreName = 'Spielmaterial' WHERE ChoreName = 'Boxen wieder einräumen'");
            migrationBuilder.Sql("UPDATE EventChores SET ChoreName = 'Müll & Leergut' WHERE ChoreName = 'Müll sammeln'");
            migrationBuilder.Sql("UPDATE EventChores SET ChoreName = 'Tisch & Raum' WHERE ChoreName = 'Alles zurück ins Lager'");
            migrationBuilder.Sql("UPDATE EventChores SET ChoreName = 'Frühdienst' WHERE ChoreName = 'Frühschicht'");
            migrationBuilder.Sql("UPDATE EventChores SET ChoreName = 'Geschirr' WHERE ChoreName = 'Geschirrdienst'");

            // Delete removed chores
            migrationBuilder.Sql("DELETE FROM EventChores WHERE ChoreName = 'Müllentsorgung'");
            migrationBuilder.Sql("DELETE FROM EventChores WHERE ChoreName = 'Tafel'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert chore names to old names
            migrationBuilder.Sql("UPDATE EventChores SET ChoreName = 'Boxen wieder einräumen' WHERE ChoreName = 'Spielmaterial'");
            migrationBuilder.Sql("UPDATE EventChores SET ChoreName = 'Müll sammeln' WHERE ChoreName = 'Müll & Leergut'");
            migrationBuilder.Sql("UPDATE EventChores SET ChoreName = 'Alles zurück ins Lager' WHERE ChoreName = 'Tisch & Raum'");
            migrationBuilder.Sql("UPDATE EventChores SET ChoreName = 'Frühschicht' WHERE ChoreName = 'Frühdienst'");
            migrationBuilder.Sql("UPDATE EventChores SET ChoreName = 'Geschirrdienst' WHERE ChoreName = 'Geschirr'");
        }
    }
}
