using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class FixTicketReporterUserIdType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_ReporterUserId1",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_ReporterUserId1",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ReporterUserId1",
                table: "Tickets");

            migrationBuilder.AlterColumn<string>(
                name: "ReporterUserId",
                table: "Tickets",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ReporterUserId",
                table: "Tickets",
                column: "ReporterUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_ReporterUserId",
                table: "Tickets",
                column: "ReporterUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_ReporterUserId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_ReporterUserId",
                table: "Tickets");

            migrationBuilder.AlterColumn<int>(
                name: "ReporterUserId",
                table: "Tickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReporterUserId1",
                table: "Tickets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ReporterUserId1",
                table: "Tickets",
                column: "ReporterUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_ReporterUserId1",
                table: "Tickets",
                column: "ReporterUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
