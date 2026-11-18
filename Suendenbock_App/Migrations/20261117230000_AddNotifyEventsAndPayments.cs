using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class AddNotifyEventsAndPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Neue Schalter in den Benachrichtigungs-Einstellungen.
            // Standard "an", damit bestehende Spieler weiterhin alles bekommen.
            migrationBuilder.AddColumn<bool>(
                name: "NotifyEvents",
                table: "NotificationPreferences",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyPayments",
                table: "NotificationPreferences",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifyEvents",
                table: "NotificationPreferences");

            migrationBuilder.DropColumn(
                name: "NotifyPayments",
                table: "NotificationPreferences");
        }
    }
}
