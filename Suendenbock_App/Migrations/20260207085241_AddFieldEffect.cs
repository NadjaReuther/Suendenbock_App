using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldEffect : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_forumReplies_AspNetUsers_AuthorUserId1",
                table: "forumReplies");

            migrationBuilder.DropForeignKey(
                name: "FK_forumReplies_Characters_AuthorCharacterId",
                table: "forumReplies");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumThreads_AspNetUsers_AuthorUserId1",
                table: "ForumThreads");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumThreads_Characters_AuthorCharacterId",
                table: "ForumThreads");

            migrationBuilder.DropIndex(
                name: "IX_ForumThreads_AuthorUserId1",
                table: "ForumThreads");

            migrationBuilder.DropIndex(
                name: "IX_forumReplies_AuthorUserId1",
                table: "forumReplies");

            migrationBuilder.DropColumn(
                name: "AuthorUserId1",
                table: "ForumThreads");

            migrationBuilder.DropColumn(
                name: "AuthorUserId1",
                table: "forumReplies");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorUserId",
                table: "ForumThreads",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorUserId",
                table: "forumReplies",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "FeldEffekte",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Beschreibung = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LightCardId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeldEffekte", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeldEffekte_LightCards_LightCardId",
                        column: x => x.LightCardId,
                        principalTable: "LightCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForumThreads_AuthorUserId",
                table: "ForumThreads",
                column: "AuthorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_forumReplies_AuthorUserId",
                table: "forumReplies",
                column: "AuthorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FeldEffekte_LightCardId",
                table: "FeldEffekte",
                column: "LightCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_forumReplies_AspNetUsers_AuthorUserId",
                table: "forumReplies",
                column: "AuthorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_forumReplies_Characters_AuthorCharacterId",
                table: "forumReplies",
                column: "AuthorCharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ForumThreads_AspNetUsers_AuthorUserId",
                table: "ForumThreads",
                column: "AuthorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ForumThreads_Characters_AuthorCharacterId",
                table: "ForumThreads",
                column: "AuthorCharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_forumReplies_AspNetUsers_AuthorUserId",
                table: "forumReplies");

            migrationBuilder.DropForeignKey(
                name: "FK_forumReplies_Characters_AuthorCharacterId",
                table: "forumReplies");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumThreads_AspNetUsers_AuthorUserId",
                table: "ForumThreads");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumThreads_Characters_AuthorCharacterId",
                table: "ForumThreads");

            migrationBuilder.DropTable(
                name: "FeldEffekte");

            migrationBuilder.DropIndex(
                name: "IX_ForumThreads_AuthorUserId",
                table: "ForumThreads");

            migrationBuilder.DropIndex(
                name: "IX_forumReplies_AuthorUserId",
                table: "forumReplies");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorUserId",
                table: "ForumThreads",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorUserId1",
                table: "ForumThreads",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AuthorUserId",
                table: "forumReplies",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorUserId1",
                table: "forumReplies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ForumThreads_AuthorUserId1",
                table: "ForumThreads",
                column: "AuthorUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_forumReplies_AuthorUserId1",
                table: "forumReplies",
                column: "AuthorUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_forumReplies_AspNetUsers_AuthorUserId1",
                table: "forumReplies",
                column: "AuthorUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_forumReplies_Characters_AuthorCharacterId",
                table: "forumReplies",
                column: "AuthorCharacterId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumThreads_AspNetUsers_AuthorUserId1",
                table: "ForumThreads",
                column: "AuthorUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumThreads_Characters_AuthorCharacterId",
                table: "ForumThreads",
                column: "AuthorCharacterId",
                principalTable: "Characters",
                principalColumn: "Id");
        }
    }
}
