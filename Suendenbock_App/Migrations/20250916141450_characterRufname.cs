using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class characterRufname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Rufname",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rufname",
                table: "Characters");
        }
    }
}
