using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class updateRegiment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterAffiliations_Infanterien_InfanterieId",
                table: "CharacterAffiliations");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Infanterien_InfanterieId",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_CharacterAffiliations_InfanterieId",
                table: "CharacterAffiliations");

            migrationBuilder.RenameColumn(
                name: "InfanterieId",
                table: "Characters",
                newName: "RegimentId");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_InfanterieId",
                table: "Characters",
                newName: "IX_Characters_RegimentId");

            migrationBuilder.RenameColumn(
                name: "InfanterieId",
                table: "CharacterAffiliations",
                newName: "RegimentsId");

            migrationBuilder.AddColumn<string>(
                name: "Sitz",
                table: "Infanterien",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Vorname",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Nachname",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Geschlecht",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<int>(
                name: "RegimentId",
                table: "CharacterAffiliations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Regiments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Regimentsleiter = table.Column<int>(type: "int", nullable: true),
                    Adjutant = table.Column<int>(type: "int", nullable: true),
                    InfanterieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regiments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Regiments_Characters_Adjutant",
                        column: x => x.Adjutant,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Regiments_Characters_Regimentsleiter",
                        column: x => x.Regimentsleiter,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Regiments_Infanterien_InfanterieId",
                        column: x => x.InfanterieId,
                        principalTable: "Infanterien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_leader",
                table: "Guilds",
                column: "leader");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_vertreter",
                table: "Guilds",
                column: "vertreter");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAffiliations_RegimentId",
                table: "CharacterAffiliations",
                column: "RegimentId");

            migrationBuilder.CreateIndex(
                name: "IX_Regiments_Adjutant",
                table: "Regiments",
                column: "Adjutant");

            migrationBuilder.CreateIndex(
                name: "IX_Regiments_InfanterieId",
                table: "Regiments",
                column: "InfanterieId");

            migrationBuilder.CreateIndex(
                name: "IX_Regiments_Regimentsleiter",
                table: "Regiments",
                column: "Regimentsleiter");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterAffiliations_Regiments_RegimentId",
                table: "CharacterAffiliations",
                column: "RegimentId",
                principalTable: "Regiments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Regiments_RegimentId",
                table: "Characters",
                column: "RegimentId",
                principalTable: "Regiments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Characters_leader",
                table: "Guilds",
                column: "leader",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Characters_vertreter",
                table: "Guilds",
                column: "vertreter",
                principalTable: "Characters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterAffiliations_Regiments_RegimentId",
                table: "CharacterAffiliations");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Regiments_RegimentId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Characters_leader",
                table: "Guilds");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Characters_vertreter",
                table: "Guilds");

            migrationBuilder.DropTable(
                name: "Regiments");

            migrationBuilder.DropIndex(
                name: "IX_Guilds_leader",
                table: "Guilds");

            migrationBuilder.DropIndex(
                name: "IX_Guilds_vertreter",
                table: "Guilds");

            migrationBuilder.DropIndex(
                name: "IX_CharacterAffiliations_RegimentId",
                table: "CharacterAffiliations");

            migrationBuilder.DropColumn(
                name: "Sitz",
                table: "Infanterien");

            migrationBuilder.DropColumn(
                name: "RegimentId",
                table: "CharacterAffiliations");

            migrationBuilder.RenameColumn(
                name: "RegimentId",
                table: "Characters",
                newName: "InfanterieId");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_RegimentId",
                table: "Characters",
                newName: "IX_Characters_InfanterieId");

            migrationBuilder.RenameColumn(
                name: "RegimentsId",
                table: "CharacterAffiliations",
                newName: "InfanterieId");

            migrationBuilder.AlterColumn<string>(
                name: "Vorname",
                table: "Characters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nachname",
                table: "Characters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Geschlecht",
                table: "Characters",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAffiliations_InfanterieId",
                table: "CharacterAffiliations",
                column: "InfanterieId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterAffiliations_Infanterien_InfanterieId",
                table: "CharacterAffiliations",
                column: "InfanterieId",
                principalTable: "Infanterien",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Infanterien_InfanterieId",
                table: "Characters",
                column: "InfanterieId",
                principalTable: "Infanterien",
                principalColumn: "Id");
        }
    }
}
