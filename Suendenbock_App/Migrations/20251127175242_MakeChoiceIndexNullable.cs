using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class MakeChoiceIndexNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userAdventChoices_AdventDoors_AdventDoorId",
                table: "userAdventChoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userAdventChoices",
                table: "userAdventChoices");

            migrationBuilder.RenameTable(
                name: "userAdventChoices",
                newName: "UserAdventChoices");

            migrationBuilder.RenameIndex(
                name: "IX_userAdventChoices_AdventDoorId",
                table: "UserAdventChoices",
                newName: "IX_UserAdventChoices_AdventDoorId");

            migrationBuilder.AlterColumn<int>(
                name: "ChoiceIndex",
                table: "UserAdventChoices",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAdventChoices",
                table: "UserAdventChoices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAdventChoices_AdventDoors_AdventDoorId",
                table: "UserAdventChoices",
                column: "AdventDoorId",
                principalTable: "AdventDoors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAdventChoices_AdventDoors_AdventDoorId",
                table: "UserAdventChoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAdventChoices",
                table: "UserAdventChoices");

            migrationBuilder.RenameTable(
                name: "UserAdventChoices",
                newName: "userAdventChoices");

            migrationBuilder.RenameIndex(
                name: "IX_UserAdventChoices_AdventDoorId",
                table: "userAdventChoices",
                newName: "IX_userAdventChoices_AdventDoorId");

            migrationBuilder.AlterColumn<int>(
                name: "ChoiceIndex",
                table: "userAdventChoices",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_userAdventChoices",
                table: "userAdventChoices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_userAdventChoices_AdventDoors_AdventDoorId",
                table: "userAdventChoices",
                column: "AdventDoorId",
                principalTable: "AdventDoors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
