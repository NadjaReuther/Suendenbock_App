using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class deleteMagicTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "SpecialZauber");
            migrationBuilder.DropTable(name: "Grundzauber");
            migrationBuilder.DropTable(name: "CharacterMagicClasses");
            migrationBuilder.DropTable(name: "MagicClassSpecializations");
            migrationBuilder.DropTable(name: "MagicClasses");
            migrationBuilder.DropTable(name: "Obermagien");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
