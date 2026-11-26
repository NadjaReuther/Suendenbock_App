using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class HesparisQuestbrett : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Questbrett: Abschlusstag, Reihenfolge am Brett (erste aktive = Hauptquest)
            // und verfallenes Nachsitzen beim Abschließen.
            migrationBuilder.AddColumn<string>(
                name: "EndeText",
                table: "Missionen",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Missionen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VerfallenText",
                table: "Missionen",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndeText",
                table: "Missionen");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Missionen");

            migrationBuilder.DropColumn(
                name: "VerfallenText",
                table: "Missionen");
        }
    }
}
