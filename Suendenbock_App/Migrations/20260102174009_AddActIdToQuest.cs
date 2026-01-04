using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class AddActIdToQuest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Spalte zuerst als nullable hinzufügen
            migrationBuilder.AddColumn<int>(
                name: "ActId",
                table: "Quests",
                type: "int",
                nullable: true);

            // 2. Alle bestehenden Quests dem ersten/aktiven Act zuordnen
            migrationBuilder.Sql(@"
                DECLARE @FirstActId INT;

                -- Versuche zuerst, den aktiven Act zu finden
                SELECT TOP 1 @FirstActId = Id FROM Acts WHERE IsActive = 1 ORDER BY ActNumber;

                -- Falls kein aktiver Act, nimm den ersten Act
                IF @FirstActId IS NULL
                BEGIN
                    SELECT TOP 1 @FirstActId = Id FROM Acts ORDER BY ActNumber;
                END

                -- Nur updaten wenn ein Act existiert
                IF @FirstActId IS NOT NULL
                BEGIN
                    UPDATE Quests SET ActId = @FirstActId WHERE ActId IS NULL;
                END
            ");

            // 3. Spalte auf NOT NULL ändern
            migrationBuilder.AlterColumn<int>(
                name: "ActId",
                table: "Quests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 4. Index und Foreign Key erstellen
            migrationBuilder.CreateIndex(
                name: "IX_Quests_ActId",
                table: "Quests",
                column: "ActId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quests_Acts_ActId",
                table: "Quests",
                column: "ActId",
                principalTable: "Acts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quests_Acts_ActId",
                table: "Quests");

            migrationBuilder.DropIndex(
                name: "IX_Quests_ActId",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "ActId",
                table: "Quests");
        }
    }
}
