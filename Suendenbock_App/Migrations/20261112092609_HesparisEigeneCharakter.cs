using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class HesparisEigeneCharakter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kampagnen_GameWorlds_GameWorldId",
                table: "Kampagnen");

            migrationBuilder.DropIndex(
                name: "IX_Spielfiguren_KampagneId_CharacterId",
                table: "Spielfiguren");

            migrationBuilder.DropIndex(
                name: "IX_Kampagnen_GameWorldId",
                table: "Kampagnen");

            migrationBuilder.DropColumn(
                name: "GameWorldId",
                table: "Kampagnen");

            migrationBuilder.AlterColumn<int>(
                name: "CharacterId",
                table: "Spielfiguren",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "HesparisCharakterId",
                table: "Spielfiguren",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HesparisCharakterId",
                table: "Mitschueler",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HesparisCharakterId",
                table: "Lehrer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Charakterarten",
                schema: "stamm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Charakterarten", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HesparisCharaktere",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Vorname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nachname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Rufname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BildPfad = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    CharakterartId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HesparisCharaktere", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HesparisCharaktere_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HesparisCharaktere_Charakterarten_CharakterartId",
                        column: x => x.CharakterartId,
                        principalSchema: "stamm",
                        principalTable: "Charakterarten",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CharakterVerknuepfungen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HesparisCharakterId = table.Column<int>(type: "int", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharakterVerknuepfungen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharakterVerknuepfungen_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharakterVerknuepfungen_HesparisCharaktere_HesparisCharakterId",
                        column: x => x.HesparisCharakterId,
                        principalTable: "HesparisCharaktere",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "stamm",
                table: "Charakterarten",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Spieler" },
                    { 2, "Lehrkraft" },
                    { 3, "Mitschüler" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Spielfiguren_HesparisCharakterId",
                table: "Spielfiguren",
                column: "HesparisCharakterId",
                unique: true,
                filter: "[HesparisCharakterId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Mitschueler_HesparisCharakterId",
                table: "Mitschueler",
                column: "HesparisCharakterId");

            migrationBuilder.CreateIndex(
                name: "IX_Lehrer_HesparisCharakterId",
                table: "Lehrer",
                column: "HesparisCharakterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharakterVerknuepfungen_CharacterId",
                table: "CharakterVerknuepfungen",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharakterVerknuepfungen_HesparisCharakterId",
                table: "CharakterVerknuepfungen",
                column: "HesparisCharakterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HesparisCharaktere_CharakterartId",
                table: "HesparisCharaktere",
                column: "CharakterartId");

            migrationBuilder.CreateIndex(
                name: "IX_HesparisCharaktere_UserId",
                table: "HesparisCharaktere",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lehrer_HesparisCharaktere_HesparisCharakterId",
                table: "Lehrer",
                column: "HesparisCharakterId",
                principalTable: "HesparisCharaktere",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Mitschueler_HesparisCharaktere_HesparisCharakterId",
                table: "Mitschueler",
                column: "HesparisCharakterId",
                principalTable: "HesparisCharaktere",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Spielfiguren_HesparisCharaktere_HesparisCharakterId",
                table: "Spielfiguren",
                column: "HesparisCharakterId",
                principalTable: "HesparisCharaktere",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lehrer_HesparisCharaktere_HesparisCharakterId",
                table: "Lehrer");

            migrationBuilder.DropForeignKey(
                name: "FK_Mitschueler_HesparisCharaktere_HesparisCharakterId",
                table: "Mitschueler");

            migrationBuilder.DropForeignKey(
                name: "FK_Spielfiguren_HesparisCharaktere_HesparisCharakterId",
                table: "Spielfiguren");

            migrationBuilder.DropTable(
                name: "CharakterVerknuepfungen");

            migrationBuilder.DropTable(
                name: "HesparisCharaktere");

            migrationBuilder.DropTable(
                name: "Charakterarten",
                schema: "stamm");

            migrationBuilder.DropIndex(
                name: "IX_Spielfiguren_HesparisCharakterId",
                table: "Spielfiguren");

            migrationBuilder.DropIndex(
                name: "IX_Mitschueler_HesparisCharakterId",
                table: "Mitschueler");

            migrationBuilder.DropIndex(
                name: "IX_Lehrer_HesparisCharakterId",
                table: "Lehrer");

            migrationBuilder.DropColumn(
                name: "HesparisCharakterId",
                table: "Spielfiguren");

            migrationBuilder.DropColumn(
                name: "HesparisCharakterId",
                table: "Mitschueler");

            migrationBuilder.DropColumn(
                name: "HesparisCharakterId",
                table: "Lehrer");

            migrationBuilder.AlterColumn<int>(
                name: "CharacterId",
                table: "Spielfiguren",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameWorldId",
                table: "Kampagnen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Spielfiguren_KampagneId_CharacterId",
                table: "Spielfiguren",
                columns: new[] { "KampagneId", "CharacterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kampagnen_GameWorldId",
                table: "Kampagnen",
                column: "GameWorldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kampagnen_GameWorlds_GameWorldId",
                table: "Kampagnen",
                column: "GameWorldId",
                principalTable: "GameWorlds",
                principalColumn: "Id");
        }
    }
}
