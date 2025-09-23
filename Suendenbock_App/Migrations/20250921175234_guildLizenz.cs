using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class guildLizenz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "quote",
                table: "Guilds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "urheber",
                table: "Guilds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "quote",
                table: "CharacterDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "urheber",
                table: "CharacterDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Lizenzen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lizenzen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gildenlizenz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuildId = table.Column<int>(type: "int", nullable: false),
                    LizenzenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gildenlizenz", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gildenlizenz_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Gildenlizenz_Lizenzen_LizenzenId",
                        column: x => x.LizenzenId,
                        principalTable: "Lizenzen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gildenlizenz_GuildId",
                table: "Gildenlizenz",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Gildenlizenz_LizenzenId",
                table: "Gildenlizenz",
                column: "LizenzenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gildenlizenz");

            migrationBuilder.DropTable(
                name: "Lizenzen");

            migrationBuilder.DropColumn(
                name: "quote",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "urheber",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "quote",
                table: "CharacterDetails");

            migrationBuilder.DropColumn(
                name: "urheber",
                table: "CharacterDetails");
        }
    }
}
