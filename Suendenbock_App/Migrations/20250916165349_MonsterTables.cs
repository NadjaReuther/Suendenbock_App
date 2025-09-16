using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class MonsterTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Monstergruppen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monstergruppen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monsterimmunitaeten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsterimmunitaeten", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monsterintelligenzen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsterintelligenzen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monstervorkommen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monstervorkommen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monsterwuerfel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Wuerfel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsterwuerfel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonsterTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MonsterwuerfelId = table.Column<int>(type: "int", nullable: false),
                    MonsterintelligenzId = table.Column<int>(type: "int", nullable: false),
                    MonstergruppenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonsterTypes_Monstergruppen_MonstergruppenId",
                        column: x => x.MonstergruppenId,
                        principalTable: "Monstergruppen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MonsterTypes_Monsterintelligenzen_MonsterintelligenzId",
                        column: x => x.MonsterintelligenzId,
                        principalTable: "Monsterintelligenzen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MonsterTypes_Monsterwuerfel_MonsterwuerfelId",
                        column: x => x.MonsterwuerfelId,
                        principalTable: "Monsterwuerfel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Monsteranfaelligkeiten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MonstertypId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsteranfaelligkeiten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monsteranfaelligkeiten_MonsterTypes_MonstertypId",
                        column: x => x.MonstertypId,
                        principalTable: "MonsterTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Monsters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    encounter = table.Column<bool>(type: "bit", nullable: false),
                    MonstertypId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monsters_MonsterTypes_MonstertypId",
                        column: x => x.MonstertypId,
                        principalTable: "MonsterTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Monstertypimmunitaeten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonstertypId = table.Column<int>(type: "int", nullable: false),
                    MonsterimmunitaetenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monstertypimmunitaeten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monstertypimmunitaeten_MonsterTypes_MonstertypId",
                        column: x => x.MonstertypId,
                        principalTable: "MonsterTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Monstertypimmunitaeten_Monsterimmunitaeten_MonsterimmunitaetenId",
                        column: x => x.MonsterimmunitaetenId,
                        principalTable: "Monsterimmunitaeten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Monstertypvorkommen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonstertypId = table.Column<int>(type: "int", nullable: false),
                    MonstervorkommenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monstertypvorkommen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monstertypvorkommen_MonsterTypes_MonstertypId",
                        column: x => x.MonstertypId,
                        principalTable: "MonsterTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Monstertypvorkommen_Monstervorkommen_MonstervorkommenId",
                        column: x => x.MonstervorkommenId,
                        principalTable: "Monstervorkommen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Monstertypanfaelligkeiten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonstertypId = table.Column<int>(type: "int", nullable: false),
                    MonsteranfaelligkeitenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monstertypanfaelligkeiten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monstertypanfaelligkeiten_MonsterTypes_MonstertypId",
                        column: x => x.MonstertypId,
                        principalTable: "MonsterTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Monstertypanfaelligkeiten_Monsteranfaelligkeiten_MonsteranfaelligkeitenId",
                        column: x => x.MonsteranfaelligkeitenId,
                        principalTable: "Monsteranfaelligkeiten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Monsteranfaelligkeiten_MonstertypId",
                table: "Monsteranfaelligkeiten",
                column: "MonstertypId");

            migrationBuilder.CreateIndex(
                name: "IX_Monsters_MonstertypId",
                table: "Monsters",
                column: "MonstertypId");

            migrationBuilder.CreateIndex(
                name: "IX_Monstertypanfaelligkeiten_MonsteranfaelligkeitenId",
                table: "Monstertypanfaelligkeiten",
                column: "MonsteranfaelligkeitenId");

            migrationBuilder.CreateIndex(
                name: "IX_Monstertypanfaelligkeiten_MonstertypId",
                table: "Monstertypanfaelligkeiten",
                column: "MonstertypId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterTypes_MonstergruppenId",
                table: "MonsterTypes",
                column: "MonstergruppenId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterTypes_MonsterintelligenzId",
                table: "MonsterTypes",
                column: "MonsterintelligenzId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterTypes_MonsterwuerfelId",
                table: "MonsterTypes",
                column: "MonsterwuerfelId");

            migrationBuilder.CreateIndex(
                name: "IX_Monstertypimmunitaeten_MonsterimmunitaetenId",
                table: "Monstertypimmunitaeten",
                column: "MonsterimmunitaetenId");

            migrationBuilder.CreateIndex(
                name: "IX_Monstertypimmunitaeten_MonstertypId",
                table: "Monstertypimmunitaeten",
                column: "MonstertypId");

            migrationBuilder.CreateIndex(
                name: "IX_Monstertypvorkommen_MonstertypId",
                table: "Monstertypvorkommen",
                column: "MonstertypId");

            migrationBuilder.CreateIndex(
                name: "IX_Monstertypvorkommen_MonstervorkommenId",
                table: "Monstertypvorkommen",
                column: "MonstervorkommenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Monsters");

            migrationBuilder.DropTable(
                name: "Monstertypanfaelligkeiten");

            migrationBuilder.DropTable(
                name: "Monstertypimmunitaeten");

            migrationBuilder.DropTable(
                name: "Monstertypvorkommen");

            migrationBuilder.DropTable(
                name: "Monsteranfaelligkeiten");

            migrationBuilder.DropTable(
                name: "Monsterimmunitaeten");

            migrationBuilder.DropTable(
                name: "Monstervorkommen");

            migrationBuilder.DropTable(
                name: "MonsterTypes");

            migrationBuilder.DropTable(
                name: "Monstergruppen");

            migrationBuilder.DropTable(
                name: "Monsterintelligenzen");

            migrationBuilder.DropTable(
                name: "Monsterwuerfel");
        }
    }
}
