using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRegimentIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Foreign Key Constraint entfernen
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterAffiliations_Regiments_RegimentId",
                table: "CharacterAffiliations");

            // 2. Index entfernen
            migrationBuilder.DropIndex(
                name: "IX_CharacterAffiliations_RegimentId",
                table: "CharacterAffiliations");

            // 3. Erst jetzt die Spalte löschen
            migrationBuilder.DropColumn(
                name: "RegimentId",
                table: "CharacterAffiliations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

        // 1. Spalte wieder hinzufügen
        migrationBuilder.AddColumn<int>(
            name: "RegimentId",
            table: "CharacterAffiliations",
            type: "int",
            nullable: true);

            // 2. Index wieder erstellen
            migrationBuilder.CreateIndex(
                name: "IX_CharacterAffiliations_RegimentId",
                table: "CharacterAffiliations",
                column: "RegimentId");

            // 3. Foreign Key wieder hinzufügen
            migrationBuilder.AddForeignKey(
                name: "FK_CharacterAffiliations_Regiments_RegimentId",
                table: "CharacterAffiliations",
                column: "RegimentId",
                principalTable: "Regiments",
                principalColumn: "Id");
        }
    }
}
