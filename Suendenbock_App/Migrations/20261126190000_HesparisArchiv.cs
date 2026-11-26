using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class HesparisArchiv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Archivtür: Schlüsselwort (leer = "Sekretariat") und Zettel am Türrahmen
            migrationBuilder.AddColumn<string>(
                name: "ArchivHinweis",
                table: "Kampagnen",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchivSchluessel",
                table: "Kampagnen",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivHinweis",
                table: "Kampagnen");

            migrationBuilder.DropColumn(
                name: "ArchivSchluessel",
                table: "Kampagnen");
        }
    }
}
