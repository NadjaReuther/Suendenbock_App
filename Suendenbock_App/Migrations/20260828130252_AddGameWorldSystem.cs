using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class AddGameWorldSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentCharacterId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GameWorlds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Era = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ColorCode = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameWorlds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharacterGameWorlds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    GameWorldId = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsOriginWorld = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterGameWorlds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterGameWorlds_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterGameWorlds_GameWorlds_GameWorldId",
                        column: x => x.GameWorldId,
                        principalTable: "GameWorlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CurrentCharacterId",
                table: "AspNetUsers",
                column: "CurrentCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGameWorlds_CharacterId_GameWorldId",
                table: "CharacterGameWorlds",
                columns: new[] { "CharacterId", "GameWorldId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGameWorlds_GameWorldId",
                table: "CharacterGameWorlds",
                column: "GameWorldId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Characters_CurrentCharacterId",
                table: "AspNetUsers",
                column: "CurrentCharacterId",
                principalTable: "Characters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Characters_CurrentCharacterId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "CharacterGameWorlds");

            migrationBuilder.DropTable(
                name: "GameWorlds");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CurrentCharacterId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CurrentCharacterId",
                table: "AspNetUsers");
        }
    }
}
