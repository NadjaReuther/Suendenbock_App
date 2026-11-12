using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class AddRiddleAdventCalendar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RiddleDoors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Solution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hint1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hint2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hint3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiddleDoors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRiddleAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RiddleDoorId = table.Column<int>(type: "int", nullable: false),
                    HintsUsed = table.Column<int>(type: "int", nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    IsSolved = table.Column<bool>(type: "bit", nullable: false),
                    FirstOpenedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubmittedAnswers = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRiddleAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRiddleAttempts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRiddleAttempts_RiddleDoors_RiddleDoorId",
                        column: x => x.RiddleDoorId,
                        principalTable: "RiddleDoors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRiddleAttempts_RiddleDoorId",
                table: "UserRiddleAttempts",
                column: "RiddleDoorId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRiddleAttempts_UserId",
                table: "UserRiddleAttempts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRiddleAttempts");

            migrationBuilder.DropTable(
                name: "RiddleDoors");
        }
    }
}
