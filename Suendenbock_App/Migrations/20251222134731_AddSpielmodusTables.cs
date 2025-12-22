using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class AddSpielmodusTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaseEffect",
                table: "Monsters",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsEquipped",
                table: "Monsters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SlainEffect",
                table: "Monsters",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Monsters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BaseMaxHealth",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BaseMaxPokus",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentHealth",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentPokus",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRestAt",
                table: "Characters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Acts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActNumber = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RestFoods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HealthBonus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestFoods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ActId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Maps_Acts_ActId",
                        column: x => x.ActId,
                        principalTable: "Acts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MapMarkers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MapId = table.Column<int>(type: "int", nullable: false),
                    XPercent = table.Column<double>(type: "float", nullable: false),
                    YPercent = table.Column<double>(type: "float", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapMarkers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MapMarkers_Maps_MapId",
                        column: x => x.MapId,
                        principalTable: "Maps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    MapMarkerId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quests_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Quests_MapMarkers_MapMarkerId",
                        column: x => x.MapMarkerId,
                        principalTable: "MapMarkers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Monsters_IsEquipped",
                table: "Monsters",
                column: "IsEquipped");

            migrationBuilder.CreateIndex(
                name: "IX_Monsters_Status",
                table: "Monsters",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Acts_IsActive",
                table: "Acts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MapMarkers_MapId",
                table: "MapMarkers",
                column: "MapId");

            migrationBuilder.CreateIndex(
                name: "IX_Maps_ActId",
                table: "Maps",
                column: "ActId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quests_CharacterId",
                table: "Quests",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Quests_MapMarkerId",
                table: "Quests",
                column: "MapMarkerId");

            migrationBuilder.CreateIndex(
                name: "IX_Quests_Status",
                table: "Quests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Quests_Type",
                table: "Quests",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Quests");

            migrationBuilder.DropTable(
                name: "RestFoods");

            migrationBuilder.DropTable(
                name: "MapMarkers");

            migrationBuilder.DropTable(
                name: "Maps");

            migrationBuilder.DropTable(
                name: "Acts");

            migrationBuilder.DropIndex(
                name: "IX_Monsters_IsEquipped",
                table: "Monsters");

            migrationBuilder.DropIndex(
                name: "IX_Monsters_Status",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "BaseEffect",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "IsEquipped",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "SlainEffect",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "BaseMaxHealth",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "BaseMaxPokus",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CurrentHealth",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CurrentPokus",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "LastRestAt",
                table: "Characters");
        }
    }
}
