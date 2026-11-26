using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class ZirkelGeheimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DientNr",
                table: "ZirkelPersonen",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DientSeite",
                table: "ZirkelPersonen",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeheimeNotiz",
                table: "ZirkelPersonen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ZirkelPersonen",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Treue",
                table: "ZirkelPersonen",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DientNr",
                table: "ZirkelPersonen");

            migrationBuilder.DropColumn(
                name: "DientSeite",
                table: "ZirkelPersonen");

            migrationBuilder.DropColumn(
                name: "GeheimeNotiz",
                table: "ZirkelPersonen");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ZirkelPersonen");

            migrationBuilder.DropColumn(
                name: "Treue",
                table: "ZirkelPersonen");
        }
    }
}
