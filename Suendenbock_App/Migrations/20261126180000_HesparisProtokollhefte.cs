using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class HesparisProtokollhefte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Protokollhefte: Schuljahr je Heft (nur dessen Noten zählen im Zeugnis)
            migrationBuilder.AddColumn<int>(
                name: "Schuljahr",
                table: "Protokollhefte",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // Steckbriefe: "wichtig" sind 0–3 Sterne statt ja/nein (true wird zu 1 Stern)
            migrationBuilder.AlterColumn<int>(
                name: "Wichtig",
                table: "HeftPersonen",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Schuljahr",
                table: "Protokollhefte");

            migrationBuilder.AlterColumn<bool>(
                name: "Wichtig",
                table: "HeftPersonen",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
