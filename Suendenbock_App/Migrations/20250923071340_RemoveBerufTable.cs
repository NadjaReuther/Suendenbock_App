using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBerufTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ALLE Foreign Keys zu Berufe entfernen (aus CharacterDetails)
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterDetails_Berufe_BerufId",
                table: "CharacterDetails");

            // MÖGLICHER ZWEITER Foreign Key aus Characters-Tabelle entfernen
            // (Falls Characters auch BerufId hatte)
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Berufe_BerufId",
                table: "Characters");

            // Alle Indices zu Berufe entfernen
            migrationBuilder.DropIndex(
                name: "IX_CharacterDetails_BerufId",
                table: "CharacterDetails");

            migrationBuilder.DropIndex(
                name: "IX_Characters_BerufId",
                table: "Characters");

            // Alle BerufId Spalten entfernen
            migrationBuilder.DropColumn(
                name: "BerufId",
                table: "CharacterDetails");

            migrationBuilder.DropColumn(
                name: "BerufId",
                table: "Characters");

            // JETZT kann die Tabelle gelöscht werden
            migrationBuilder.DropTable(
                name: "Berufe");

            // Neue Beruf String-Spalte hinzufügen
            migrationBuilder.AddColumn<string>(
                name: "Beruf",
                table: "CharacterDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Beruf String-Spalte entfernen
            migrationBuilder.DropColumn(
                name: "Beruf",
                table: "CharacterDetails");

            // BerufId Spalte wieder hinzufügen
            migrationBuilder.AddColumn<int>(
                name: "BerufId",
                table: "CharacterDetails",
                type: "int",
                nullable: true);

            // Berufe Tabelle wieder erstellen
            migrationBuilder.CreateTable(
                name: "Berufe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Berufe", x => x.Id);
                });

            // Index wieder erstellen
            migrationBuilder.CreateIndex(
                name: "IX_CharacterDetails_BerufId",
                table: "CharacterDetails",
                column: "BerufId");

            // Foreign Key wieder hinzufügen
            migrationBuilder.AddForeignKey(
                name: "FK_CharacterDetails_Berufe_BerufId",
                table: "CharacterDetails",
                column: "BerufId",
                principalTable: "Berufe",
                principalColumn: "Id");
        }
    }
}
